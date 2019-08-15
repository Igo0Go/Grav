using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReactor : UsingOrigin, IAlive
{
    [Range(0,300)]
    [Tooltip("Количетсо Hit Point у данной точки")] public float health;
    [Range(0, 10)]
    [Tooltip("Время удаления (используется, чтобы успел проиграться взрыв или что-то ещё)")] public float removeTime;
    [Tooltip("Сюда можно вставить взрыв или другие частицы")] public ParticleSystem particle;
    [Tooltip("Можно поместить объект, который останется после уничтожения точки")] public GameObject ofterDeadPrefab;
    [Tooltip("Удаляется сразу")] public GameObject model;
    [Space(10)]
    [Tooltip("Звук проигрывается при уничтожении (Audiosource должен быть на этом же объекте)")]
    public AudioClip clip;

    private AudioSource source;

    public float Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
            if (health <= 0)
            {
                Dead();
            }
        }
    }

    private void Start()
    {
        MyGetComponent(gameObject, out source);
    }

    public void Dead()
    {
        Use();
    }

    private void UseAl()
    {
        for (int i = 0; i < actionObjects.Length; i++)
        {
            if (actionObjects[i] != null)
            {
                actionObjects[i].Use();
            }
            else
            {
                Debug.LogError("Элемент " + i + " равен null. Вероятно, была утеряна ссылка. Источник :" + gameObject.name);
            }
        }
    }
    private void Remove()
    {
        if(ofterDeadPrefab != null)
        {
            Instantiate(ofterDeadPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    public void GetDamage(int damage)
    {
        if (Health > 0)
        {
            Health -= damage;
        }
    }
    public override void Use()
    {
        if(model != null)
        {
            Destroy(model);
        }
        if (particle != null)
        {
            particle.Play();
        }
        if(source != null && clip != null)
        {
            source.PlayOneShot(clip);
        }
        UseAl();
        Invoke("Remove", removeTime);
    }
   
}
