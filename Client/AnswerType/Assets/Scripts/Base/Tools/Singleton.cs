//2018.03.22
//单例模式  关林

using UnityEngine;
using System.Collections;

//Mono的单例
public class Mono_Singleton_DontDestroyOnLoad<T> : MonoBehaviour where T : Mono_Singleton_DontDestroyOnLoad<T>
{
    private static T _inst;
    public static T _instance
    {
        private set
        {
            _inst = value;
        }

        get
        {
            #if GuanGuan_Test
            if (typeof(T) == typeof(GL_GameEvent))
            {
                //DDebug.Log("1"); // 监听一下，可能还有别的地方在Destroy的时候注销事件
            }
            #endif
            
            if (_inst == null)
            {
                _inst = GameObject.FindObjectOfType<T>();
                if (_inst != null)
                    DontDestroyOnLoad(_inst.gameObject);
            }

            if (_inst == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                _inst = obj.AddComponent<T>();
                DontDestroyOnLoad(_inst.gameObject);
            }

            return _inst;
        }
    }

    [System.NonSerialized]
    public static int InstanceCount;

    [System.NonSerialized]
    public T PrevInstance;

    [System.NonSerialized]
    public T NextInstance;

    protected virtual void OnEnable()
    {
        var t = (T)this;

        if (_instance != null)
        {
            _instance.PrevInstance = t;

            PrevInstance = null;
            NextInstance = _instance;
        }
        else
        {
            PrevInstance = null;
            NextInstance = null;
        }

        _instance = t;
        InstanceCount += 1;
    }

    protected virtual void OnDisable()
    {
        if (_instance == this)
        {
            _instance = NextInstance;

            if (NextInstance != null)
            {
                NextInstance.PrevInstance = null;
            }
        }
        else
        {
            if (NextInstance != null)
            {
                NextInstance.PrevInstance = PrevInstance;
            }

            PrevInstance.NextInstance = NextInstance;
        }

        InstanceCount -= 1;
    }
    //public static void LoadInstance(string res)
    //{
    //    T prefab = XD.Game.ResourceAdapter.LoadPrefab<T>(res);
    //    T obj = GameObject.Instantiate<T>(prefab);
    //    obj.name = typeof(T).ToString();
      
    //    DontDestroyOnLoad(obj.gameObject);
    //    _inst = obj;
       
    //}
}

public class Mono_Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _inst;
    public static T _instance
    {
        private set
        {
            _inst = value;
        }

        get
        {
            if (null == _inst)
                _inst = GameObject.FindObjectOfType<T>();
            if (null == _inst)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                _inst = obj.AddComponent<T>();
            }
            return _inst;
        }
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}

//单例
public class Singleton<T> where T : new()
{
    private static T _inst;
    public static T _instance
    {
        private set
        {
            _inst = value;
        }

        get
        {
            if (null == _inst)
                _inst = new T();
            return _inst;
        }
    }
}

