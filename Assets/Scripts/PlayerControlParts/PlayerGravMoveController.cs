using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerGravMoveController : PlayerControllerBlueprint
{
    [Space(20)]
    [Range(1, 5)]
    [Tooltip("Скорость перемещения")]
    public float moveSpeed = 3;
    [Range(1, 5)]
    [Tooltip("Множитель при ускорении")]
    public float sprintMultiplicator = 1.5f;
    [Range(0, 100)]
    [Tooltip("Сила прыжка")]
    public float jumpForce = 40;
    [Tooltip("Чтобы работал прыжок нужно выделить игрока в отдельный слой и указать его здесь")]
    public LayerMask jumpMask;

    public SphereGravModule planet;

    public PlayerState Status => PlayerStateController.Status;
    public SaveLocationPack SaveLocationPack => _saveLocationPack;
    private SaveLocationPack _saveLocationPack;
    private bool InMenu => PlayerStateController.InMenu;

    public event Action<PhysicMaterial> FloorSoundEvent;
    public event Action<PhysicMaterial> JumpSoundEvent;
    public event Action OnGroundEvent;
    public event Action OnGravChange;


    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Transform gravObj;

    private Vector3 dir;
    private Vector3 gravVector;
    private Rigidbody gravRb;
    private Quaternion rotBufer;
    private PhysicMaterial floorMaterial;
    private float currentSprintMultiplicator;
    private float currentStepTime;
    private float stepTyme;
    private sbyte onGroundStatus;
    private int gravRotSpeed;
    private int gravDirectionMultiplicator;
    private bool rotToGrav;
    private bool opportunityToFallSound;
    private bool firstStep;

    private readonly float g = 40f;

    protected override void SetReferences(PlayerStateController playerState)
    {
        playerState.playerInputController.MoveInputEvent += PlayerMoveStandard;
        playerState.playerInputController.MoveInputEvent += PlayerMoveSphere;
        playerState.playerInputController.SprintBottonDownEvent += Sprint;
        playerState.playerInputController.JumpInputEvent += Jump;
        playerState.playerSceneManagementController.SaveLocationEvent += SaveLocation;
        playerState.playerSceneManagementController.SaveLocationWithPointEvent += SaveLocationWithPoint;
        playerState.playerSceneManagementController.OnRestartEvent += OnRestart;
        playerState.playerSceneManagementController.CheckSavePlanetEvent += CheckSavePlanet;

        _saveLocationPack = new SaveLocationPack();
        rb = GetComponent<Rigidbody>();
        currentStepTime = 0;
        stepTyme = moveSpeed / 2;
        onGroundStatus = -1;
        opportunityToFallSound = true;
    }
    private void FixedUpdate()
    {
        CheckStep();
    }
    private void LateUpdate()
    {
        if (rotToGrav)
        {
            RotateToGravSmooth();
        }
    }


    /// <summary>
    /// Перевернуть игрока
    /// </summary>
    public void RotateToGrav()
    {
        if (-Mytransform.up != Physics.gravity)
        {
            if (gravObj == null)
            {
                rotBufer = Quaternion.FromToRotation(-Mytransform.up, Physics.gravity);
                OnGravChange?.Invoke();
            }
            else
            {
                gravVector = gravDirectionMultiplicator * (gravObj.position - Mytransform.position);
                rotBufer = Quaternion.FromToRotation(-Mytransform.up, gravVector.normalized);
            }
            rotBufer = rotBufer * transform.rotation;
            gravRotSpeed = 5;
            rotToGrav = true;
        }
    }
    /// <summary>
    /// Задать центр сферической гравитации
    /// </summary>
    /// <param name="reactor"></param>
    public void SetGravObj(SphereGravModule reactor)
    {
        Physics.gravity = Vector3.zero;
        planet = reactor;
        gravObj = reactor.transform;
        transform.parent = gravObj;
        gravRb = gravObj.GetComponent<Rigidbody>();
        gravDirectionMultiplicator = reactor.planetGravityType ? 1 : -1;
    }
    /// <summary>
    /// Задать вектор гравитации
    /// </summary>
    /// <param name="vector"></param>
    public void SetGravVector(Vector3 vector)
    {
        planet = null;
        gravObj = null;
        transform.parent = null;
        Physics.gravity = vector * g;
    }

    private void PlayerMoveStandard(float horizontal, float vertical)
    {
        if (InMenu)
            horizontal = vertical = 0;

        if (horizontal != 0 || vertical != 0)
        {
            dir = Mytransform.right * horizontal + Mytransform.forward * vertical;
            dir = dir.normalized * currentSprintMultiplicator;
            if(firstStep)
            {
                firstStep = false;
                currentStepTime = 0;
            }
        }
        else
        {
            firstStep = true;
            dir = Vector3.zero;
        }
        if (dir != Vector3.zero)
        {
            Mytransform.position += (dir * moveSpeed * Time.deltaTime);
            if (onGroundStatus == 0)
            {
                currentStepTime -= dir.magnitude * moveSpeed * Time.deltaTime;
            }
        }
    }
    private void PlayerMoveSphere(float horizontal, float vertical)
    {
        if (Status == PlayerState.active && gravObj != null)
        {
            gravVector = gravDirectionMultiplicator * (gravObj.position - Mytransform.position);

            float distance = gravVector.magnitude;
            float strength = 100 * rb.mass * gravRb.mass * (planet.radius + distance) / (distance * 3);

            rb.AddForce(gravVector.normalized * strength);

            if (!rotToGrav)
            {
                rotBufer = Quaternion.FromToRotation(-Mytransform.up, gravVector.normalized);
                Mytransform.rotation = rotBufer * Mytransform.rotation;
            }

            Vector3 down = Vector3.Project(rb.velocity, Mytransform.up);
            
            if (InMenu)
            {
                horizontal = vertical = 0;
            }

            Vector3 forward = Mytransform.forward * vertical * moveSpeed * currentSprintMultiplicator; ;
            Vector3 right = Mytransform.right * horizontal * moveSpeed * currentSprintMultiplicator;

            dir = forward + right;

            if ((horizontal != 0 || vertical != 0) && onGroundStatus == 0)
            {
                currentStepTime -= dir.magnitude * moveSpeed * currentSprintMultiplicator * Time.deltaTime;
                if (firstStep)
                {
                    firstStep = false;
                    currentStepTime = 0;
                }
            }
            else
            {
                firstStep = true;
            }

            rb.velocity = down + right + forward;
        }
    }
    private void Sprint(bool useSprint)
    {
        currentSprintMultiplicator = useSprint && !InMenu ? sprintMultiplicator : 1;
    }
    private void Jump()
    {
        if (Status == PlayerState.active)
        {
            if (OnGround() && !InMenu)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void CheckSavePlanet()
    {
        if (SaveLocationPack.savePlanet != null)
        {
            planet = SaveLocationPack.savePlanet;
            SetGravObj(planet);
        }
        else
        {
            Physics.gravity = SaveLocationPack.saveGrav;
        }

    }
    private void OnRestart()
    {
        if (planet != null)
        {
            planet = SaveLocationPack.savePlanet;
            transform.parent = gravObj;
        }
        else
        {
            transform.parent = null;
            Physics.gravity = SaveLocationPack.saveGrav;
        }

        OnGravChange?.Invoke();
        Physics.gravity = SaveLocationPack.saveGrav;
        rb.velocity = Vector3.zero;
        transform.position = SaveLocationPack.savePos;
        transform.rotation = SaveLocationPack.saveRot;

    }
    private void SaveLocation()
    {
        if (planet != null)
        {
            SaveLocationPack.savePlanet = planet;
        }
        else
        {
            SaveLocationPack.saveGrav = Physics.gravity;
        }
        SaveLocationPack.savePos = Mytransform.position;
        SaveLocationPack.saveRot = Mytransform.rotation;
    }
    private void SaveLocationWithPoint(Transform point)
    {
        if (planet != null)
        {
            SaveLocationPack.savePlanet = planet;
        }
        else
        {
            SaveLocationPack.savePlanet = null;
            SaveLocationPack.saveGrav = Physics.gravity;
        }
        SaveLocationPack.savePos = point.position + transform.up * 1.5f;
        SaveLocationPack.saveRot = point.rotation;
    }
    

    private void RotateToGravSmooth()
    {
        if (Quaternion.Angle(transform.rotation, rotBufer) < 4 || OnGround())
        {
            Mytransform.rotation = rotBufer;
            rotToGrav = false;
        }
        else
        {
            Mytransform.rotation = Quaternion.Lerp(transform.rotation, rotBufer, Time.deltaTime * gravRotSpeed);
        }
    }

    public bool OnGround()
    {
        if (Physics.Raycast(transform.position + transform.up, -transform.up, out RaycastHit hit, 1.3f, ~jumpMask))
        {
            floorMaterial = hit.collider.material;
            Vector3 bufer = hit.point - transform.position;
            if (Status == PlayerState.active)
            {
                OnGroundEvent?.Invoke();
            }
            
            if(opportunityToFallSound)
            {
                if (floorMaterial != null)
                {
                   JumpSoundEvent?.Invoke(floorMaterial);
                }
                opportunityToFallSound = false;
            }

            if (onGroundStatus == -1)
            {
                currentStepTime = stepTyme;
                onGroundStatus = 0;
            }
            return true;
        }
        else
        {
            opportunityToFallSound = true;
            if (onGroundStatus == 0)
            {
                floorMaterial = null;
                onGroundStatus = -1;
            }
        }
        return false;
    }
   
    private void CheckStep()
    {
        if(OnGround())
        {
            if (currentStepTime <= 0)
            {
                if (floorMaterial != null && onGroundStatus == 0)
                {
                    FloorSoundEvent?.Invoke(floorMaterial);
                }
                currentStepTime = stepTyme;
            }
        }
    }
}

public class SaveLocationPack
{
    public Vector3 savePos;
    public Quaternion saveRot;
    public Vector3 saveGrav;
    public SphereGravModule savePlanet;
}