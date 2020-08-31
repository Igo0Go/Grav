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
    [Range(1, 300)]
    [Tooltip("Скорость полёта снаряда")]
    public float speed;
    [Tooltip("Здесь должно быть 2 набора частиц - первый для попадания с эффектом, второй для попадания без эффекта")]
    public GameObject[] Particles;
    [Tooltip("Здесь должно быть 2 набора частиц - первый для попадания с изменением гравитации, второй для попадания без изменения")]
    public BulletType type;
    [Tooltip("На какие слои не реагировать"), SerializeField]
    private LayerMask ignoreMask;
    private PlayerStateController player;
    #endregion

    #region Приватные переменные
    private float lifetime;
    private Vector3 lastPos;
    private RaycastHit hit;
    private GameObject decal;
    private int acidity;
    #endregion

    #region Делегаты и События
    private Action moveBullet;
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
            player = gravityThrower.PlayerStateController;
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
            if (MyGetComponent(hit.collider.gameObject, out SphereGravItem item))
            {
                item.InvokeOnBullet();
            }

            if (hit.collider.CompareTag("Grav"))
            {
                decal = Instantiate(Particles[0]);
                if (MyGetComponent(hit.collider.gameObject, out SphereGravModule gravReactor))
                {
                    player.playerGravMoveController.SetGravObj(gravReactor);
                }
                else
                {
                    player.playerGravMoveController.SetGravVector(-hit.normal);
                    player.transform.localScale = Vector3.one;
                }
                player.playerGravMoveController.RotateToGrav();
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                decal = Instantiate(Particles[0]);
                if (MyGetComponent(hit.transform.parent.gameObject, out TurretScript turret))
                {
                    turret.Disactive();
                }
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

            if (hit.collider.CompareTag("AcidReactor"))
            {
                
                if(MyGetComponent(hit.collider.gameObject, out AcidReactor acidReactor))
                {
                    decal = Instantiate(Particles[0]);
                    acidReactor.source.Play();
                    acidReactor.GetDamage(acidity);
                    decal.transform.position = hit.collider.transform.position +
                    new Vector3(Physics.gravity.normalized.x * hit.collider.transform.lossyScale.x / 2, Physics.gravity.normalized.y * hit.collider.transform.lossyScale.y / 2,
                    Physics.gravity.normalized.z * hit.collider.transform.lossyScale.z / 2);
                }
                else
                {
                    decal = Instantiate(Particles[1]);
                    decal.transform.position = hit.point + hit.normal * 0.1f;
                }
                
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
        Destroy(gameObject);
    }
    #endregion
}
