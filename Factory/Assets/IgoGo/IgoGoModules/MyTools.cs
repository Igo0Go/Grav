using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#region Вспомогательные классы
public abstract class MyTools : MonoBehaviour
{
    public static bool NearColor(Color A, Color B, bool useA = false)
    {
        if(A.r - B.r > 0.1f)
        {
            return false;
        }
        if (A.g - B.g > 0.1f)
        {
            return false;
        }
        if (A.b - B.b > 0.1f)
        {
            return false;
        }
        if(useA)
        {
            if (A.a - B.a > 0.1f)
            {
                return false;
            }
        }
        return true;
    }

    public bool MyGetComponent<T>(GameObject obj, out T component) where T: Component
    {
        component = obj.GetComponent<T>();
        if (component == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool ContainsPhisicsMaterial(Collider obj, out PhysicMaterial component)
    {
        component = obj.sharedMaterial;
        if (component == null)
        {
            return false;
        }
        return true;
    }

    public void Invoke(Action action, float time)
    {
        Invoke(action.Method.Name, time);
    }
}
public static class MyTime
{
    public static float PauseTime { get; set; } = 0;
    public static bool PauseStatus => pauseKey;
    public static void Pause()
    {
        TimeScale = PauseTime;
        pauseKey = true;
    }
    public static void Start()
    {
        pauseKey = false;
        TimeScale = defaultTime;
    }
    public static float TimeScale
    {
        get
        {
            return Time.timeScale;
        }
        set
        {
            if (!pauseKey)
            {
                defaultTime = Time.timeScale;
                Time.timeScale = value;
            }
        }
    }

    private static bool pauseKey;
    private static float defaultTime;
}
public static class MyCursor
{
    public static bool OpportunityToChange { get; set; }
    public static bool Visible
    {
        get
        {
            return Cursor.visible;
        }
        set
        {
            if(OpportunityToChange)
            {
                Cursor.visible = value;
            }
        }
    }
    public static CursorLockMode LockState
    {
        get
        {
            return Cursor.lockState;
        }
        set
        {
            if(OpportunityToChange)
            {
                Cursor.lockState = value;
            }
        }
    }
}
public enum PlayerState
{
    active = 1,
    disactive = 0,
    speceUse = 2,
    dead = -1
}
public static class LoadManager
{
    public static string NameSceneForLoad = "Menu";
}
public interface IAlive
{
    float Health { get; set; }
    void GetDamage(int damage);
    void Dead();
}
#endregion
