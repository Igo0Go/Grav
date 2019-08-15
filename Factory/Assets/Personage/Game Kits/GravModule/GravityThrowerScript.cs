using System.Collections;
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

public class GravityThrowerScript : MonoBehaviour
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
    [Tooltip("Ссылка на владельца пушки (GravFPS)")]
    public GravFPS player;
    #endregion

    #region Публичные переменные (Пушка)
    //[Tooltip("Слйдер накопления заряда")]
    //public Slider powerSlider;
    [Tooltip("Часть слайдера - Fill")]
    public Image powerFillArea;
    [Tooltip("Поместить объект, который должен менять цвет в зависимости от режима пушки")]
    public MeshRenderer modeIndicator;
    [Tooltip("Режимы для пушки. 0 - нулевая гравитация, 1 - притяжение, 2 - отталкивание")]
    public GravGanMode[] modes;
    [Tooltip("Частицы будут запускаться при выстреле")]
    public ParticleSystem shootParticles;
    [Tooltip("Точка, из которой будут лететь снаряды")]
    public Transform ShootPoint;
    #endregion

    #region Свойства

    private Vector3 ManipPosBufer => cam.transform.position + cam.forward + cam.forward * currentManipObj.transform.lossyScale.magnitude;
    private float ManipDistance => Vector3.Distance(ManipPosBufer, currentManipObj.transform.position);

    #endregion

    #region Настройки пушки
    [Range(5, 500)] [Tooltip("Дальность стрельбы. После снаряд уничтожается")]
    public float range;
    [Tooltip("Дальность стрельбы. После снаряд уничтожается"), SerializeField, Range(5,30)]
    private float manipulationRange = 5;
    [Tooltip("Параметр кислотности - сколько связей структуры будет нарушено вокруг точки попадания.")]
    public int acidity = 3;
    #endregion

    #region Приватные переменные
    private GameObject currentBullet;
    private GameObject currentManipObj;
    private Rigidbody currentRB;
    private MeshRenderer manipRenderer;
    private Color materialColor;
    private bool delay;
    private bool manipKey;
    private int toggle;
    #endregion

    #region Делегаты и События

    private Action shoot;

    #endregion

    #region События Unity
    void Start()
    {
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
    }
    void Update()
    {
        TargetLook();

        if (!delay)
        {
            shoot();
        }

    }
    #endregion

    #region Служебные методы
    private void ReturnDelay()
    {
        delay = false;
    }
    private void GravShoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BulletScript bullet = Instantiate(currentBullet, ShootPoint.position, ShootPoint.rotation).GetComponent<BulletScript>();
            bullet.SetSettings(this);
            delay = true;
            anim.SetTrigger("Shoot");
            shootParticles.Play();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Toggle();
        }
    }
    private void ManipulationShoot()
    {
        if(!manipKey)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 dir = lookPoint.position - cam.position;
                Debug.DrawRay(cam.position, dir, Color.red, 3);
                if (Physics.Raycast(cam.position, dir, out RaycastHit hit, manipulationRange, ~ignoreMask))
                {
                    if (hit.collider.tag.Equals("Manip"))
                    {
                        currentManipObj = hit.collider.gameObject;
                        manipRenderer = currentManipObj.GetComponent<MeshRenderer>();
                        materialColor = manipRenderer.material.color;
                        currentRB = currentManipObj.GetComponent<Rigidbody>();
                        if(currentRB.mass <= 30)
                        {
                            currentRB.useGravity = false;
                            manipKey = true;
                        }
                    }
                }
            }
        }
        else
        {
            if(ManipDistance > 0.1f)
            {
                if(Vector3.Angle(ManipPosBufer - currentManipObj.transform.position, currentRB.velocity) > 30)
                {
                    currentRB.velocity = Vector3.zero;
                }
                if(Vector3.Angle(-cam.forward, currentManipObj.transform.position - cam.position) < 60 && Vector3.Angle(cam.forward, currentRB.velocity) < 30)
                {
                    currentRB.AddForce(cam.up * 5 * currentRB.mass, ForceMode.Impulse);
                }
                currentRB.AddForce((ManipPosBufer - currentManipObj.transform.position) * (2 - (currentRB.mass/30)) , ForceMode.Impulse);
            }
            else
            {
                currentRB.velocity = Vector3.zero;
            }

            if(Input.GetMouseButtonDown(0))
            {
                ReturnManip();
            }

        }

        if(manipKey)
        {
            if(materialColor.a > 0.4f)
            {
                materialColor.a -= Time.deltaTime;
                manipRenderer.material.color = materialColor;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Toggle();
        }
    }
    private void AcidShoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BulletScript bullet = Instantiate(currentBullet, ShootPoint.position, ShootPoint.rotation).GetComponent<BulletScript>();
            bullet.SetSettings(this);
            delay = true;
            anim.SetTrigger("Shoot");
            shootParticles.Play();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Toggle();
        }
    }
    private void TargetLook()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 500, ~ignoreMask))
        {
            lookPoint.position = hit.point;
        }
        else
        {
            lookPoint.position = cam.position + cam.forward * range;
        }
        ShootPoint.LookAt(lookPoint);
    }
    private void Toggle()
    {
        player.onDeadEvent -= ReturnManip;
        int modeNumber = 0;
        switch (toggle)
        {
            case -1:
                if(modes[0].active)
                {
                    modeNumber = 0;
                    shoot = AcidShoot;
                    toggle = 0;
                }
                else if(modes[1].active)
                {
                    modeNumber = 1;
                    shoot = GravShoot;
                    toggle = 1;
                }
                else
                {
                    modeNumber = 2;
                    player.onDeadEvent += ReturnManip;
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
                    player.onDeadEvent += ReturnManip;
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
                    player.onDeadEvent += ReturnManip;
                    toggle = -1;
                }
                else if(modes[0].active)
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
        modeIndicator.material = modes[modeNumber].modeMaterialForGun;
        powerFillArea.color = modes[modeNumber].modeColorForSlider;
        currentBullet = modes[modeNumber].Bulleet;
    }
    private void ReturnManip()
    {
        materialColor.a = 1;
        manipRenderer.material.color = materialColor;
        currentRB.useGravity = true;
        manipKey = false;
    }
    #endregion

}
