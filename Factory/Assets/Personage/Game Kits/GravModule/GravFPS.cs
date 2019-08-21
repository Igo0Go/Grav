using System.Collections;
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
    public GravFPSSceneManager sceneManager;
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
    private SafePoint safePoint;
    private int gravRotSpeed;
    private bool onGround;
    private bool rotToGrav;
    private bool alive;
    private float currentCamAngle;

   
    #endregion

    #region Делегаты и Событие

    public event Action onDeadEvent;
    public event Action onGroundEvent;

    #endregion

    #region События Unity
    private void Start()
    {
        sceneManager.pack = gravFPSUI.StatusPack;
        gravFPSUI.StatusPack.currentScene = SceneManager.GetActiveScene().name;
        Save();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        currentCamAngle = cam.localRotation.eulerAngles.x;
    }
    void Update()
    {
        if (alive && status == 0)
        {
            PlayerMove();
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
        if(gravFPSUI.StatusPack.lifeSphereCount > 0)
        {
            gravFPSUI.RemoveLifeSphere();
            onDeadEvent?.Invoke();
            Invoke("RestartRun", 2);
            safePoint.OnRestart();
        }
        else
        {
            gravFPSUI.StatusPack.money = gravFPSUI.StatusPack.saveMoney;
            gravFPSUI.StatusPack.currentScene = gravFPSUI.StatusPack.hubScene;
            sceneManager.LoadNextScene();
        }
    }
    #endregion

    #region Служебные
    private void PlayerMove()
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
        if (OnGround())
        {
            if (Input.GetKeyDown(inputSettingsManager.GetKey("Jump")))
            {
                if (jumpForce < 0)
                {
                    Debug.LogError("Ты чё, больной?! Ты ж вниз прыгаешь...");
                }
                rb.AddForce(-Physics.gravity.normalized * jumpForce, ForceMode.Impulse);
            }
        }
        if (dir != Vector3.zero)
        {
            transform.position += (dir * speed * Time.deltaTime);
        }
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
        if (Physics.Raycast(transform.position, Physics.gravity, out RaycastHit hit, 2, ~jumpMask))
        {
            Vector3 bufer = hit.point - transform.position;
            Debug.DrawRay(transform.position, bufer, Color.red);
            if(status==0)
            {
                onGroundEvent?.Invoke();
            }
            return true;
        }
        return false;
    }
    public void RotateToGrav()
    {
        if(-transform.up != Physics.gravity)
        {
            rotToGrav = true;
            if (Vector3.Angle(transform.up, -Physics.gravity) < 1)
            {
                rotToGrav = false;
            }
            else if (Vector3.Angle(transform.up, -Physics.gravity) < 91)
            {
                rotBufer = Quaternion.LookRotation(transform.up, -Physics.gravity);
                gravRotSpeed = 5;
            }
            else
            {
                rotBufer = Quaternion.LookRotation(-transform.forward, -Physics.gravity);
                gravRotSpeed = 3;
            }
        }
    }
    public void RotateToGravSmooth()
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
            safePoint = other.GetComponent<SafePoint>();
            safePoint.Safe(transform);
            saveGrav = Physics.gravity;
            savePos = safePoint.playerPoint.position;
            saveRot = safePoint.playerPoint.rotation;
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
