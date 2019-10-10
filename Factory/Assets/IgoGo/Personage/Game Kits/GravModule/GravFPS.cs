using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public delegate void RotateHandler(Quaternion rot);

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class GravFPS : MonoBehaviour
{
    #region Публичные поля (настройки игрока)
    [Tooltip("Пак для настроек управления")] public InputSettingsManager inputSettingsManager;

    [Space(20)]
    [Range(1, 5)]
    [Tooltip("Скорость перемещения")]
    public float speed;
    [Range(1, 2)]
    [Tooltip("Множитель при ускорении")]
    public float sprintMultiplicator;
    [Range(0, 100)]
    [Tooltip("Сила прыжка")]
    public float jumpForce;
    public GravFPSUI gravFPSUI;
    public SphereGravModule planet;
    public GravityThrowerScript gun;
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
    [HideInInspector]public PlayerState status;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector]public Transform gravObj;
    [HideInInspector] public bool inMenu;
    [SerializeField] private PlayerStartSceneSettingsScript playerStartSceneSettings;

    private Vector3 dir;
    private Vector3 savePos;
    private Quaternion saveRot;
    private Vector3 saveGrav;
    private Quaternion rotBufer;
    private SavePoint savePoint;
    private Vector3 gravVector;
    private Rigidbody gravRb;
    private SphereGravModule savePlanet;
    private LootPointScript currentLootPoint;
    private Collider currentMoveTransformCol;
    private int gravMultiplicator;
    private int gravRotSpeed;
    private bool onGround;
    private bool rotToGrav;
    private bool alive;
    private float currentCamAngle;
    #endregion

    #region Делегаты и События

    public event Action OnDeadEvent;
    public event Action OnRestartEvent;
    public event Action OnGroundEvent;
    public event Action OnGravChange;

    #endregion

    #region События Unity
    private void Start()
    {
        gravFPSUI.StatusPack.currentScene = SceneManager.GetActiveScene().name;
        gravFPSUI.onFinalStun += ReturnActive;

        if(playerStartSceneSettings == null)
        {
            if (planet != null)
            {
                SetGravObj(planet);
            }
            else
            {
                SetGravVector(new Vector3(0, -9.81f, 0));
            }
        }
        else
        {
            playerStartSceneSettings.SetSettings(this);
        }

        status = PlayerState.active;
        sceneManager.pack = gravFPSUI.StatusPack;
        Save();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        currentCamAngle = cam.localRotation.eulerAngles.x;
        inMenu = false;
        gravFPSUI.manager = inputSettingsManager;
    }
    void Update()
    {
        Jump();
        if (alive && status == PlayerState.active && gravObj == null)
        {
            PlayerMoveStandard();
        }
    }
    private void LateUpdate()
    {
        if(status > 0)
        {
            if (alive)
            {
                PlayerRotate();
            }
            if (rotToGrav)
            {
                RotateToGravSmooth();
            }
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
        gravFPSUI.StatusPack.acidCount = gravFPSUI.StatusPack.saveAcidCount;
        sceneManager.LoadNextScene();
    }
    public void RotateToGrav()
    {
        if (-transform.up != Physics.gravity)
        {
            if(gravObj == null)
            {
                rotBufer = Quaternion.FromToRotation(-transform.up, Physics.gravity);
                OnGravChange?.Invoke();
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
        Physics.gravity = Vector3.zero;
        planet = reactor;
        gravObj = reactor.transform;
        transform.parent = gravObj;
        gravRb = gravObj.GetComponent<Rigidbody>();
        gravMultiplicator = reactor.planetGravityType ? 1 : -1;
    }
    public void SetGravVector(Vector3 vector)
    {
        planet = null;
        gravObj = null;
        transform.parent = null;
        Physics.gravity = vector;
    }
    public void Stun()
    {
        gravFPSUI.SetStun();
        status = PlayerState.disactive;
    }
    #endregion

    #region Служебные
    private void PlayerMoveStandard()
    {
        float h, v;

        h = v = 0;

        if(!inMenu)
        {
            v = inputSettingsManager.GetAxis("Vertical");
            h = inputSettingsManager.GetAxis("Horizontal");
        }

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
        if(status == PlayerState.active)
        {
            gravVector = gravMultiplicator * (gravObj.position - transform.position);

            float distance = gravVector.magnitude;
            float strength = 100 * rb.mass * gravRb.mass * (planet.radius + distance) / (distance * 3);

            rb.AddForce(gravVector.normalized * strength);

            if (!rotToGrav)
            {
                rotBufer = Quaternion.FromToRotation(-transform.up, gravVector.normalized);
                transform.rotation = rotBufer * transform.rotation;
            }

            Vector3 down = Vector3.Project(rb.velocity, transform.up);
            float v, h;
            v = h = 0;

            if (!inMenu)
            {
                v = inputSettingsManager.GetAxis("Vertical");
                h = inputSettingsManager.GetAxis("Horizontal");
            }

            Vector3 forward = transform.forward * v * speed * Sprint();
            Vector3 right = transform.right * h * speed * Sprint(); ;

            rb.velocity = down + right + forward;
        }
    }
    private void PlayerRotate()
    {
        float mx, my;
        mx = my = 0;
        if(!inMenu)
        {
            mx = Input.GetAxis("Mouse X");
            my = Input.GetAxis("Mouse Y");
        }

        if (mx != 0 || my != 0)
        {
            transform.Rotate(Vector3.up, mx * camRotateSpeed * inputSettingsManager.inputKit.sensivityMultiplicator * Time.deltaTime);
            currentCamAngle -= my * camRotateSpeed * inputSettingsManager.inputKit.sensivityMultiplicator * Time.deltaTime;
            currentCamAngle = Mathf.Clamp(currentCamAngle, minYAngle, maxYAngle);
            cam.localRotation = Quaternion.Euler(currentCamAngle, cam.localRotation.eulerAngles.y, 0);
        }
    }
    private float Sprint()
    {
        if (Input.GetKey(inputSettingsManager.GetKey("Sprint")) && !inMenu)
        {
            return sprintMultiplicator;
        }
        return 1;
    }
    public bool OnGround()
    {
        if (Physics.Raycast(transform.position + transform.up, -transform.up, out RaycastHit hit, 1.3f, ~jumpMask))
        {
            Vector3 bufer = hit.point - transform.position;
            Debug.DrawRay(transform.position, bufer, Color.red);
            if(status == PlayerState.active)
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
        if(planet != null)
        {
            savePlanet = planet;
        }
        else
        {
            saveGrav = Physics.gravity;
        }
        savePos = transform.position;
        saveRot = transform.rotation;
        alive = true;
        gravFPSUI.deadPanel.SetActive(!alive);
    }
    private void Save(Transform point)
    {
        if (planet != null)
        {
            savePlanet = planet;
        }
        else
        {
            savePlanet = null;
            saveGrav = Physics.gravity;
        }
        savePos = point.position + transform.up * 1.5f;
        saveRot = point.rotation;
        alive = true;
        gravFPSUI.deadPanel.SetActive(!alive);
    }
    private void RestartRun()
    {
        gravFPSUI.Health = 100;

        if (planet != null)
        {
            planet = savePlanet;
        }
        else
        {
            Physics.gravity = saveGrav;
        }

        Physics.gravity = saveGrav;
        rb.velocity = Vector3.zero;
        transform.position = savePos;
        transform.rotation = saveRot;
        OnRestartEvent?.Invoke();
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
                    Cursor.visible = true;
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
        if (Input.GetKeyDown(inputSettingsManager.GetKey("Jump")))
        {
            if (OnGround() && !inMenu)
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void RestartScene()
    {
        gravFPSUI.StatusPack.money = gravFPSUI.StatusPack.saveMoney;
        sceneManager.LoadNextScene();
    }
    private void ReturnToSavePoint()
    {
        if (savePlanet != null)
        {
            planet = savePlanet;
            SetGravObj(planet);
        }
        else
        {
            Physics.gravity = saveGrav;
        }
        gravFPSUI.RemoveLifeSphere();
        OnDeadEvent?.Invoke();
        Invoke("RestartRun", 2);
        savePoint.OnRestart();
    }
    private void ReturnActive() => status = PlayerState.active;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("DeadZone"))
        {
            Death();
        }
        else if(other.tag.Equals("Loot"))
        {
            LootItem loot = other.GetComponent<LootItem>();
            if(loot.opportunityToSuffice)
            { 
                loot.SetTarget(this);
            }
        }
        else if (other.tag.Equals("CheckPoint"))
        {
            if(savePoint != other.GetComponent<SavePoint>())
            {
                savePoint = other.GetComponent<SavePoint>();
                savePoint.Save(transform);
                Save(savePoint.transform);
            }
        }
        else if(other.tag.Equals("SceneLoad"))
        {
            sceneManager.LoadNextScene();
        }
        else if (other.tag.Equals("LootPoint"))
        {
            currentLootPoint = other.GetComponent<LootPointScript>();
            gravFPSUI.SetTip(currentLootPoint, inputSettingsManager);
        }
        else if (other.tag.Equals("MoveTransform"))
        {
            currentMoveTransformCol = other;
            transform.parent = currentMoveTransformCol.transform;
        }
        else if(other.tag.Equals("EnemyView"))
        {
            other.GetComponent<ITargetTracker>().SetTarget(transform);
            return;
        }
        else if (other.tag.Equals("Dron"))
        {
            gun.SetDangerPoin(other.transform);
            return;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("LootPoint"))
        {
            if(Input.GetKeyDown(inputSettingsManager.GetKey("Using")) && currentLootPoint == other.GetComponent<LootPointScript>())
            {
                if(currentLootPoint.useble && gravFPSUI.StatusPack.money >= currentLootPoint.cost)
                {
                    gravFPSUI.SpendMoney(currentLootPoint.cost);
                    gravFPSUI.ClearTip();
                    currentLootPoint.SetPlayer(gravFPSUI);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("LootPoint") && currentLootPoint == other.GetComponent<LootPointScript>())
        {
            currentLootPoint = null;
            gravFPSUI.ClearTip();
            return;
        }
        else if (other.tag.Equals("MoveTransform") && other == currentMoveTransformCol)
        {
            currentMoveTransformCol = null;
            transform.parent = null;
            return;
        }
        else if (other.tag.Equals("EnemyView"))
        {
            other.GetComponent<ITargetTracker>().ClearTarget(transform);
            return;
        }
        else if (other.tag.Equals("Dron"))
        {
            gun.ClearDangerPoint();
            return;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Manip"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude * rb.mass > 100 - rb.mass)
            {
                GetDamage(Mathf.RoundToInt(rb.velocity.magnitude));
            }
        }
    }
}
