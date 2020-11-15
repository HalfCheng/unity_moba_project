using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

//实现普通的单例
public abstract class Singleton<T> where T : new()
{
    private static T _instance;
    private static object mutex = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (mutex)
                {
                    if (_instance == null)
                        _instance = new T();
                }
            }

            return _instance;
        }
    }
}

//MonoBehaviour

public class UnitySinleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = (T) obj.AddComponent(typeof(T));
                    obj.hideFlags = HideFlags.DontSave;
                }
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    protected void DestorySelf()
    {
        if (_instance != null)
        {
            GameObject.Destroy(this.gameObject);
            _instance = null;
        }
    }
}