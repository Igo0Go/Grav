using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class GravGanMode
{
    [Tooltip("Включен ли режим в пушке")] public bool active;
    [Tooltip("Цвет для слайдера")] public Color modeColorForSlider;
    [Tooltip("Материал для индикатора пушки")] public Material modeMaterialForGun;
    [Tooltip("Снаряд")] public GameObject Bulleet;
}

public delegate void ISeeDronModuleHandler(FriendModulePoint point);

[RequireComponent(typeof(LineRenderer))]
public class GravityThrowerScript : PlayerControllerBlueprint
{
    #region Публичные переменные (ссылки)
    [Tooltip("В маске указать, на ккие слои выстрел не должен реагировать")]
    public LayerMask ignoreMask;
    [Tooltip("Transform камеры игрок. Требуется для прицеливания")]
    public Transform cam;
    [Tooltip("Transform пустого объекта, который используется в качестве пустого объекта при прицеливании")]
    public Transform lookPoint;
    [Tooltip("Animator пушки")]
    public Animator anim;
    [Tooltip("Отрисовывать луч при манипуляции")]
    public bool drawManipLine;
    #endregion

    #region Публичные переменные (Пушка)
    [Tooltip("Слйдер заряда")]
    public Slider powerSlider;
    [Tooltip("Часть слайдера - Fill")]
    public Image powerFillArea;
    [Tooltip("Показатель, что можно отправить дрона")]
    public GameObject dronLight;
    [Tooltip("Показатель, что есть опасность")]
    public GameObject dangerLight;
    [Tooltip("Поместить объект, который должен менять цвет в зависимости от режима пушки")]
    public MeshRenderer modeIndicator;
    [Tooltip("Режимы для пушки. 0 - кислота, 1 - притяжение, 2 - манипуляция")]
    public GravGanMode[] modes;
    [Tooltip("Частицы будут запускаться при выстреле")]
    public ParticleSystem shootParticles;
    [Tooltip("Точка, из которой будут лететь снаряды")]
    public Transform ShootPoint;
    [Tooltip("Звук счётчика опасности")]
    public AudioClip dangerClip;
    [Tooltip("Источник звука - счтчик опасности")]
    public AudioSource dangerSoundSource;
    [Tooltip("Источник звука - выстрел")]
    public AudioSource shootSource;
    [Tooltip("0 - грав, 1 - манипулятор, 2 - кислота, 3 - смена")]
    public List<AudioClip> shootSounds;
    #endregion

    #region Свойства
    private StatusPack StatusPack => PlayerStateController.statusPack; 
    private Vector3 ManipPosBufer => cam.transform.position + cam.forward + cam.forward * currentManipObj.transform.lossyScale.magnitude;
    private float ManipDistance => Vector3.Distance(ManipPosBufer, currentManipObj.transform.position);
    private float DistanceToDanger => Vector3.Distance(dangerPoint.position, transform.position);
    private bool InMenu => PlayerStateController.InMenu;
    #endregion

    #region Настройки пушки
    [Range(5, 500)] [Tooltip("Дальность стрельбы. После снаряд уничтожается")]
    public float range;
    [Tooltip("Дальность стрельбы. После снаряд уничтожается"), SerializeField, Range(5,30)]
    private float manipulationRange = 5;
    [Tooltip("Параметр кислотности - сколько связей структуры будет нарушено вокруг точки попадания.")]
    public int acidity = 3;
    [Tooltip("количество точек, между соседними А и Б (чем больше, тем сильнее сглаживание и выше нагрузка на систему)"), Range(6, 100)] public int segmentCount = 25;
    [Tooltip("Сила броска"), Range(1, 100)] public float manipDropForce = 10;
    #endregion

    #region Приватные переменные
    private GameObject currentBullet;
    private GameObject currentManipObj;
    private Rigidbody currentRB;
    private MeshRenderer manipRenderer;
    private Color materialColor;
    private Vector3[] bezierPath;
    private Vector3 nearPoint = Vector3.zero;
    private Vector3 farPoint = Vector3.zero;
    private LineRenderer line;
    private Transform dangerPoint;
    private Collider currentFriendPoint;
    private bool delay;
    private bool manipKey;
    private bool opportunityToPlayDangerSound;
    private int toggle;
    #endregion

    #region Делегаты и События

    public event Action AcidShootEvent;
    public event ISeeDronModuleHandler ISeeDronPointEvent;
    private Action shoot;

    #endregion

    #region Публичные методы
    public void SetDangerPoin(Transform point)
    {
        dangerPoint = point;
        opportunityToPlayDangerSound = true;
    }
    public void ClearDangerPoint()
    {
        dangerPoint = null;
    }
    #endregion

    #region Основное
    protected override void SetReferences(PlayerStateController playerState)
    {
        playerState.playerInputController.ChangeGunTypeInputEvent += Toggle;
        playerState.playerInputController.ShootInputEvent += OnShootInput;
        playerState.playerInputController.UsingInputEvent += ThrowManipObject;
        playerState.playerInputController.DronInputEvent += UseDron;

        dangerLight.SetActive(false);
        delay = false;
        if(modes[0].active)
        {
            toggle = 0;
        }
        else if(modes[1].active)
        {
            toggle = 1;
        }
        else if(modes[2].active)
        {
            toggle = -1;
        }
        Toggle();
        line = GetComponent<LineRenderer>();
        playerState.playerUIController.OnGetLoot += CheckSlider;
        anim.SetBool("Ready", true);
    }
    void Update()
    {
        TargetLook();
        CheckDanger();
        ManipUpdate();
    }
    #endregion

    #region Служебные методы
    private void OnShootInput()
    {
        if(!InMenu)
            shoot();
    }
    /// <summary>
    /// Используется с помощью Invoke("methodName")
    /// </summary>
    private void ReturnDelay()
    {
        delay = false;
    }
    private void GravShoot()
    {
        if(!delay)
        {

            BulletScript bullet = Instantiate(currentBullet, ShootPoint.position, ShootPoint.rotation).GetComponent<BulletScript>();
            bullet.SetSettings(this);
            delay = true;
            anim.SetTrigger("Shoot");
            shootSource.PlayOneShot(shootSounds[0]);
            shootParticles.Play();
            Invoke("ReturnDelay", 1f);
        }
    }
    private void ManipulationShoot()
    {
        if(manipKey)
        {
            ReturnManip();
        }
        else
        {
            Vector3 dir = lookPoint.position - cam.position;
            Debug.DrawRay(cam.position, dir, Color.red, 3);
            if (Physics.Raycast(cam.position, dir, out RaycastHit hit, manipulationRange, ~ignoreMask))
            {
                if (hit.collider.CompareTag("Manip"))
                {
                    currentManipObj = hit.collider.gameObject;
                    manipRenderer = currentManipObj.GetComponent<MeshRenderer>();
                    materialColor = manipRenderer.material.color;
                    currentRB = currentManipObj.GetComponent<Rigidbody>();
                    if (currentRB.mass <= 30)
                    {
                        currentRB.useGravity = false;
                        currentRB.isKinematic = false;
                        manipKey = true;
                        shootSource.clip = shootSounds[2];
                        shootSource.loop = true;
                        shootSource.playOnAwake = true;
                        shootSource.Play();
                        ManipItem item = currentManipObj.GetComponent<ManipItem>();
                        if (item != null)
                        {
                            item.OnSocketPos += ReturnManip;
                        }
                    }
                }
                else if (hit.collider.CompareTag("ManipForEnemy"))
                {
                    currentManipObj = hit.collider.transform.parent.parent.gameObject;
                    manipRenderer = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    if (manipRenderer != null)
                    {
                        materialColor = manipRenderer.material.color;
                    }
                    currentRB = currentManipObj.GetComponent<Rigidbody>();
                    Pawn pawn = currentManipObj.GetComponent<Pawn>();
                    if (pawn != null)
                    {
                        pawn.ToManipState();
                        manipKey = true;
                    }
                }
            }
        }
    }
    private void AcidShoot()
    {
        if (!delay && StatusPack.currentAcidCount > 0)
        {
            BulletScript bullet = Instantiate(currentBullet, ShootPoint.position, ShootPoint.rotation).GetComponent<BulletScript>();
            bullet.SetSettings(this);
            delay = true;
            anim.SetTrigger("Shoot");
            shootSource.PlayOneShot(shootSounds[1]);
            shootParticles.Play();
            AcidShootEvent?.Invoke();
            powerSlider.value = StatusPack.currentAcidCount;
            Invoke("ReturnDelay", 1);
        }
    }
    private void ManipUpdate()
    {
        if (manipKey)
        {
            if (ManipDistance > 0.1f)
            {
                if (Vector3.Angle(ManipPosBufer - currentManipObj.transform.position, currentRB.velocity) > 30)
                {
                    currentRB.velocity = Vector3.zero;
                }
                if (Vector3.Angle(-cam.forward, currentManipObj.transform.position - cam.position) < 60 && Vector3.Angle(cam.forward, currentRB.velocity) < 30)
                {
                    currentRB.AddForce(cam.up * 5 * currentRB.mass, ForceMode.Impulse);
                }
                currentRB.AddForce((ManipPosBufer - currentManipObj.transform.position) * (2 - (currentRB.mass / 30)), ForceMode.Impulse);
            }
            else
            {
                currentRB.velocity = Vector3.zero;
            }

            if(drawManipLine)
            {
                nearPoint = ShootPoint.position + ShootPoint.forward * Vector3.Distance(ShootPoint.position, currentManipObj.transform.position) / 2;
                farPoint = Vector3.Lerp(currentManipObj.transform.position, nearPoint, Vector3.Distance(nearPoint, currentManipObj.transform.position) / 2);
            }


            if (manipRenderer != null)
            {
                if (materialColor.a > 0.4f)
                {
                    materialColor.a -= Time.deltaTime;
                    manipRenderer.material.color = materialColor;
                }
                
                if(drawManipLine) DrawCurves();
            }
        }
    }
    private void ThrowManipObject()
    {
        if (manipKey)
        {
            ManipItem item = currentManipObj.GetComponent<ManipItem>();
            if(item != null)
                item.damaged = true;
            ReturnManip();
            currentRB.AddForce((transform.forward) * manipDropForce, ForceMode.Impulse);
        }
    }
    private void UseDron()
    {
        if (currentFriendPoint != null)
            ISeeDronPointEvent?.Invoke(currentFriendPoint.GetComponent<FriendModulePoint>());
    }
    private void TargetLook()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 500, ~ignoreMask))
        {
            lookPoint.position = hit.point;
            if(hit.collider.CompareTag("DronModule"))
            {
                dronLight.SetActive(true);
                if (hit.collider != currentFriendPoint)
                {
                    currentFriendPoint = hit.collider;
                }
            }
            else
            {
                currentFriendPoint = null;
                dronLight.SetActive(false);
            }
        }
        else
        {
            currentFriendPoint = null;
            lookPoint.position = cam.position + cam.forward * range;
            dronLight.SetActive(false);
        }
        ShootPoint.LookAt(lookPoint);
    }
    /// <summary>
    /// Переключение режима с учётом подписок на события и активной манипуляции объектом
    /// </summary>
    private void Toggle()
    {
        if (!delay && PlayerStateController.Status > 0 && !PlayerStateController.InMenu)
        {
            PlayerStateController.playerReactionsController.DeathEvent -= ReturnManip;
            int modeNumber = 0;
            switch (toggle)
            {
                case -1:
                    if (modes[0].active)
                    {
                        modeNumber = 0;
                        shoot = AcidShoot;
                        toggle = 0;
                    }
                    else if (modes[1].active)
                    {
                        modeNumber = 1;
                        shoot = GravShoot;
                        toggle = 1;
                    }
                    else
                    {
                        modeNumber = 2;
                        PlayerStateController.playerReactionsController.DeathEvent += ReturnManip;
                        shoot = ManipulationShoot;
                        toggle = -1;
                    }
                    break;
                case 0:
                    if (modes[1].active)
                    {
                        modeNumber = 1;
                        shoot = GravShoot;
                        toggle = 1;
                    }
                    else if (modes[2].active)
                    {
                        modeNumber = 2;
                        shoot = ManipulationShoot;
                        PlayerStateController.playerReactionsController.DeathEvent += ReturnManip;
                        toggle = -1;
                    }
                    else
                    {
                        modeNumber = 0;
                        shoot = AcidShoot;
                        toggle = 0;
                    }
                    break;
                case 1:
                    if (modes[2].active)
                    {
                        modeNumber = 2;
                        shoot = ManipulationShoot;
                        PlayerStateController.playerReactionsController.DeathEvent += ReturnManip;
                        toggle = -1;
                    }
                    else if (modes[0].active)
                    {
                        modeNumber = 0;
                        shoot = AcidShoot;
                        toggle = 0;
                    }
                    else
                    {
                        modeNumber = 1;
                        shoot = GravShoot;
                        toggle = 1;
                    }
                    break;
            }
            if (toggle != -1 && manipKey)
            {
                ReturnManip();
            }
            shootSource.PlayOneShot(shootSounds[3]);
            CheckSlider();
            modeIndicator.material = modes[modeNumber].modeMaterialForGun;
            powerFillArea.color = modes[modeNumber].modeColorForSlider;
            currentBullet = modes[modeNumber].Bulleet;
            ReturnDelay();
        }
    }
    private void CheckSlider()
    {
        if (toggle == 0)
        {
            powerSlider.maxValue = StatusPack.maxAcidCount;
            powerSlider.value = StatusPack.currentAcidCount;
        }
        else
        {
            powerSlider.maxValue = powerSlider.value = 0;
        }
    }
    private void ReturnManip()
    {
        materialColor.a = 1;
        if (currentManipObj != null)
        {
            Pawn pawn = currentManipObj.GetComponent<Pawn>();
            if (pawn != null)
            {
                pawn.ToDefaultState();
            }
            ManipItem item = currentManipObj.GetComponent < ManipItem> ();
            if (item != null)
            {
                item.OnSocketPos -= ReturnManip;
            }
            manipRenderer.material.color = materialColor;
            currentRB.useGravity = true;
        }
        manipKey = false;
        shootSource.loop = false;
        shootSource.Stop();

        if(drawManipLine)
            line.positionCount = 0;
    }
    private void CheckDanger()
    {
        if(dangerPoint != null)
        {
            if(opportunityToPlayDangerSound)
            {
                PlayDangerSound();
                Invoke("ReturnDangerLight", 0.3f);
                Invoke("ReturnDangerSound", (DistanceToDanger - 4) / 15);
            }
        }
    }
    private void PlayDangerSound()
    {
        dangerSoundSource.PlayOneShot(dangerClip);
        
        opportunityToPlayDangerSound = false;
        dangerLight.SetActive(true);
    }
    private void ReturnDangerSound() => opportunityToPlayDangerSound = true;
    private void ReturnDangerLight()
    {
        if (!opportunityToPlayDangerSound)
            dangerLight.SetActive(false);
    }
    /// <summary>
    /// Cоздание кривой и визуализация
    /// </summary>
    public void DrawCurves()
    {
        List<Vector3> l = new List<Vector3>();

        for (int j = 0; j <= segmentCount; j++)
        {
            float t = (float)j / segmentCount;
            Vector3 pxl = CalculateBezierPoint(t, ShootPoint.position, nearPoint, farPoint, currentManipObj.transform.position);
            l.Add(pxl);
        }

        bezierPath = new Vector3[] { };
        bezierPath = l.ToArray();
        line.positionCount = bezierPath.Length;
        line.SetPositions(bezierPath);
    }
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;
        return p;
    }
    #endregion
}
