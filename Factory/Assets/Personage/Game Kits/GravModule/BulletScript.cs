using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BulletType
{
    Grav,
    Acid
}

public class BulletScript : MyTools
{

    #region Публичные переменные
    [Range(1, 100)]
    [Tooltip("Скорость полёта снаряда")]
    public float speed;
    [Tooltip("Здесь должно быть 2 набора частиц - первый для попадания с эффектом, второй для попадания без эффекта")]
    public GameObject[] Particles;
    [Tooltip("Здесь должно быть 2 набора частиц - первый для попадания с изменением гравитации, второй для попадания без изменения")]
    public BulletType type;
    [Tooltip("Сила притяжения гравитации (g)"), Range(0,10), SerializeField]
    private float power = 1;

    #endregion

    #region Приватные переменные
    private float lifetime;
    private LayerMask ignoreMask;
    private Vector3 lastPos;
    private RaycastHit hit;
    private GameObject decal;
    private int acidity;
    #endregion

    #region Делегаты и События
    private Action moveBullet;
    private event Action ChangeGrav;
    #endregion


    #region События Unity
    void Update()
    {
        moveBullet();
    }
    #endregion

    #region Служебные методы
    public void SetSettings(GravityThrowerScript gravityThrower)
    {
        lifetime = gravityThrower.range / speed;
        ignoreMask = gravityThrower.ignoreMask;
        lastPos = transform.position;

        if (type == BulletType.Grav)
        {
            moveBullet = MoveGravBullet;
            ChangeGrav += gravityThrower.player.RotateToGrav;
        }
        else
        {
            moveBullet = MoveAcidBullet;
            acidity = gravityThrower.acidity;
        }
        Invoke("DestroyBullet", lifetime);
    }
    private void MoveGravBullet()
    {
        lastPos = transform.position;
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Physics.Linecast(lastPos, transform.position, out hit, ~ignoreMask))
        {
            if(MyGetComponent(hit.collider.gameObject, out BulletReactor bulletReactor))
            {
                bulletReactor.Use();
            }

            if (hit.collider.tag.Equals("Grav"))
            {
                decal = Instantiate(Particles[0]);
                Physics.gravity = -hit.normal * power * 9.8f;
                ChangeGrav?.Invoke();
            }
            else
            {
                decal = Instantiate(Particles[1]);
            }
            decal.transform.forward = hit.normal;
            decal.transform.position = hit.point + hit.normal * 0.1f;
            Destroy(decal, 5);
            DestroyBullet();
        }
    }
    private void MoveAcidBullet()
    {
        lastPos = transform.position;
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Physics.Linecast(lastPos, transform.position, out hit, ~ignoreMask))
        {
            if (MyGetComponent(hit.collider.gameObject, out BulletReactor bulletReactor))
            {
                bulletReactor.Use();
            }

            if (hit.collider.tag.Equals("AcidReactor"))
            {
                decal = Instantiate(Particles[0]);
                hit.collider.GetComponent<AcidReactor>().GetDamage(acidity);
                decal.transform.position = hit.collider.transform.position +
                new Vector3(Physics.gravity.normalized.x * hit.collider.transform.lossyScale.x / 2, Physics.gravity.normalized.y * hit.collider.transform.lossyScale.y / 2,
                Physics.gravity.normalized.z * hit.collider.transform.lossyScale.z / 2);
            }
            else
            {
                decal = Instantiate(Particles[1]);
                decal.transform.position =  hit.point + hit.normal * 0.1f;
            }
            decal.transform.up = hit.normal;
            
            Destroy(decal, 6);
            DestroyBullet();
        }
    }
    private void DestroyBullet()
    {
        ChangeGrav = null;
        Destroy(gameObject);
    }
    #endregion

}
