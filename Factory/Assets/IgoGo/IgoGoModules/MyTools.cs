using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#region Вспомогательные классы
public abstract class MyTools : MonoBehaviour
{
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
    public static float PauseTime { get; set; }
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
public enum PlayerState
{
    active,
    disactive,
    dead,
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
