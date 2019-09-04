﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class GravFPS : MonoBehaviour
{
    #region Публичные поля (настройки игрока)
    [Tooltip("Пак для настроек управления")] public InputSettingsManager inputSettingsManager;

    [Space(20)]
    [Range(1, 10)]
    [Tooltip("Скорость перемещения")]
    public float speed;
    [Range(1, 5)]
    [Tooltip("Множитель при ускорении")]
    public float sprintMultiplicator;
    [Range(0, 100)]
    [Tooltip("Сила прыжка")]
    public float jumpForce;
    public GravFPSUI gravFPSUI;
    public SphereGravModule startPlanet;
    public GravFPSSceneManager sceneManager;
    public bool inHub;
    #endregion
    #region Публичные поля (служебное)
    [Space(20)]
    [Tooltip("Transform камеры игрока")]
    public Transform cam;
    [Tooltip("Чтобы работал прыжок нужно выделить игрока в отдельный слой и указать его здесь")]
    public LayerMask jumpMask;
    #endregion
    #region Публичные поля (настройки камеры)
    [Space(10)]
    [Range(1, 360)]
    [Tooltip("Скорость вращения камеры")]
    public float camRotateSpeed;
    [Range(0, 90)]
    [Tooltip("Ограничение камеры по вертикальному углу сверху")]
    public float maxYAngle;
    [Range(-90, 0)]
    [Tooltip("Ограничение камеры по вертикальному углу снизу")]
    public float minYAngle;
    #endregion

    #region Служебные
    [HideInInspector]public int status;
    [HideInInspector] public Rigidbody rb;

    private Vector3 dir;
    private Vector3 savePos;
    private Quaternion saveRot;
    private Vector3 saveGrav;
    private Quaternion rotBufer;
    private SavePoint savePoint;
    private Vector3 gravVector;
    public Transform gravObj;
    private Rigidbody gravRb;
    private int gravMultiplicator;
    private int gravRotSpeed;
    private bool onGround;
    private bool rotToGrav;
    private bool alive;
    private float currentCamAngle;
    #endregion

    #region Делегаты и Событие

    public event Action OnDeadEvent;
    public event Action OnGroundEvent;

    #endregion

    #region События Unity
    private void Start()
    {
        gravObj = null;
        gravVector = Physics.gravity;
        sceneManager.pack = gravFPSUI.StatusPack;
        gravFPSUI.StatusPack.currentScene = SceneManager.GetActiveScene().name;
        Save();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        currentCamAngle = cam.localRotation.eulerAngles.x;
        if(startPlanet != null)
        {
            SetGravObj(startPlanet);
        }
    }
    void Update()
    {
        Jump();
        if (alive && status == 0 && gravObj == null)
        {
            PlayerMoveStandard();
        }
    }
    private void LateUpdate()
    {
        if(alive)
        {
            PlayerRotate();
        }
        if (rotToGrav)
        {
            RotateToGravSmooth();
        }
    }
    private void FixedUpdate()
    {
        if(gravObj != null)
        {
            PlayerMoveSphere();
        }
    }

    #endregion

    #region Публичные методы

    public void GetDamage(int damage)
    {
        if(alive)
        {
            if (gravFPSUI.Health - damage > 0)
            {
                gravFPSUI.Health -= damage;
            }
            else
            {
                gravFPSUI.Health = 0;
                Death();
            }
        }
    }
    public void Death()
    {
        alive = false;
        gravFPSUI.deadPanel.SetActive(!alive);
        CheckLoad();
    }
    public void RestartSceneWithLoadSphere()
    {
        gravFPSUI.RemoveLifeSphere();
        RestartScene();
    }
    public void LoadHubScene()
    {
        gravFPSUI.CheckSpheres();
        gravFPSUI.StatusPack.money = gravFPSUI.StatusPack.saveMoney;
        gravFPSUI.StatusPack.currentScene = gravFPSUI.StatusPack.hubScene;
        sceneManager.LoadNextScene();
    }
    public void RotateToGrav()
    {
        if (-transform.up != Physics.gravity)
        {
            if(gravObj == null)
            {
                rotBufer = Quaternion.FromToRotation(-transform.up, Physics.gravity);
            }
            else
            {
                gravVector = gravMultiplicator * (gravObj.position - transform.position);
                rotBufer = Quaternion.FromToRotation(-transform.up, gravVector.normalized);
            }
            rotBufer = rotBufer * transform.rotation;
            gravRotSpeed = 5;
            rotToGrav = true;
        }
    }
    public void SetGravObj(SphereGravModule reactor)
    {
        startPlanet = reactor;
        gravObj = reactor.transform;
        transform.parent = gravObj;
        gravRb = gravObj.GetComponent<Rigidbody>();
        gravMultiplicator = reactor.planetGravityType ? 1 : -1;
    }
    #endregion

    #region Служебные
    private void PlayerMoveStandard()
    {
        float h, v;

        v = inputSettingsManager.GetAxis("Vertical");
        h = inputSettingsManager.GetAxis("Horizontal");

        Vector3 camForward = transform.forward;

        if (h != 0 || v != 0)
        {
            dir = transform.right * h + camForward * v;
            dir *= Sprint();
        }
        else
        {
            dir = Vector3.zero;
        }
        if (dir != Vector3.zero)
        {
            transform.position += (dir * speed * Time.deltaTime);
        }
    }
    private void PlayerMoveSphere()
    {
        gravVector = gravMultiplicator * (gravObj.position - transform.position);

        float distance = gravVector.magnitude;
        float strength = rb.mass * gravRb.mass * distance / startPlanet.radius;
        rb.AddForce(gravVector.normalized * strength);

        if(!rotToGrav)
        {
            rotBufer = Quaternion.FromToRotation(-transform.up, gravVector.normalized);
            transform.rotation = rotBufer * transform.rotation;
        }

        Vector3 down = Vector3.Project(rb.velocity, transform.up);
        Vector3 forward = transform.forward * inputSettingsManager.GetAxis("Vertical") * speed * Sprint();
        Vector3 right = transform.right * inputSettingsManager.GetAxis("Horizontal") * speed * Sprint(); ;

        rb.velocity = down + right + forward;
    }
    private void PlayerRotate()
    {
        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        if (mx != 0 || my != 0)
        {
            if (!rotToGrav)
            {
                transform.Rotate(Vector3.up, mx * camRotateSpeed * Time.deltaTime);
                currentCamAngle -= my * camRotateSpeed * Time.deltaTime;
                currentCamAngle = Mathf.Clamp(currentCamAngle, minYAngle, maxYAngle);
                cam.localRotation = Quaternion.Euler(currentCamAngle, cam.localRotation.eulerAngles.y, 0);
            }
            else
            {
                //currentCamAngle -= my * camRotateSpeed * Time.deltaTime;
                //currentCamAngle = Mathf.Clamp(currentCamAngle, minYAngle, maxYAngle);
                //cam.localRotation = Quaternion.Euler(currentCamAngle, cam.localRotation.eulerAngles.y + mx * camRotateSpeed * Time.deltaTime, 0);
            }
           
        }
    }
    private float Sprint()
    {
        if (Input.GetKey(inputSettingsManager.GetKey("Sprint")))
        {
            return sprintMultiplicator;
        }
        return 1;
    }
    public bool OnGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 2, ~jumpMask))
        {
            Vector3 bufer = hit.point - transform.position;
            Debug.DrawRay(transform.position, bufer, Color.red);
            if(status==0)
            {
                OnGroundEvent?.Invoke();
            }
            return true;
        }
        return false;
    }
    private void RotateToGravSmooth()
    {
        if (Quaternion.Angle(transform.rotation, rotBufer) < 4)
        {
            transform.rotation = rotBufer;
            rotToGrav = false;
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotBufer, Time.deltaTime * gravRotSpeed);
        }
    }
    private void Save()
    {
        saveGrav = Physics.gravity;
        savePos = transform.position;
        saveRot = transform.rotation;
        alive = true;
        gravFPSUI.deadPanel.SetActive(!alive);
    }
    private void RestartRun()
    {
        gravFPSUI.Health = 100;
        Physics.gravity = saveGrav;
        rb.velocity = Vector3.zero;
        transform.position = savePos;
        transform.rotation = saveRot;
        alive = true;
        gravFPSUI.deadPanel.SetActive(!alive);
    }
    private void CheckLoad()
    {
        if(inHub)
        {
            if (savePoint != null)
            {
                if (gravFPSUI.StatusPack.lifeSphereCount > 0)
                {
                    ReturnToSavePoint();
                }
                else
                {
                    LoadHubScene();
                }
            }
            else
            {
                if (gravFPSUI.StatusPack.lifeSphereCount > 0)
                {
                    gravFPSUI.RemoveLifeSphere();
                }
                LoadHubScene();
            }
        }
        else
        {
            if (savePoint != null)
            {
                if (gravFPSUI.StatusPack.lifeSphereCount > 0)
                {
                    ReturnToSavePoint();
                }
                else
                {
                    LoadHubScene();
                }
            }
            else
            {
                if (gravFPSUI.StatusPack.lifeSphereCount > 0)
                {
                    Cursor.lockState = CursorLockMode.None;
                    gravFPSUI.loadPanel.SetActive(true);
                    gravFPSUI.panels[1].anim.SetBool("Visible", true);
                }
                else
                {
                    LoadHubScene();
                }
            }
        }
    }
    private void Jump()
    {
        if (OnGround())
        {
            if (Input.GetKeyDown(inputSettingsManager.GetKey("Jump")))
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void RestartScene()
    {
        gravFPSUI.StatusPack.money = gravFPSUI.StatusPack.saveMoney;
        sceneManager.LoadNextScene();
    }
    private void ReturnToSavePoint()
    {
        gravFPSUI.RemoveLifeSphere();
        OnDeadEvent?.Invoke();
        Invoke("RestartRun", 2);
        savePoint.OnRestart();
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("DeadZone"))
        {
            Death();
        }
        else if(other.tag.Equals("Loot"))
        {
            other.GetComponent<LootItem>().SetTarget(this);
        }
        else if (other.tag.Equals("CheckPoint"))
        {
            if(savePoint != other.GetComponent<SavePoint>())
            {
                savePoint = other.GetComponent<SavePoint>();
                savePoint.Save(transform);
                saveGrav = Physics.gravity;
                savePos = savePoint.playerPoint.position;
                saveRot = savePoint.playerPoint.rotation;
            }
        }
        else if(other.tag.Equals("SceneLoad"))
        {
            gravFPSUI.StatusPack.saveMoney = gravFPSUI.StatusPack.money;
            sceneManager.LoadNextScene();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Manip"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude * rb.mass > 70 - rb.mass)
            {
                Death();
            }
        }
    }
}
