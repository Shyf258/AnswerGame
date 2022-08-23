//2018.10.10    关林
//资源池

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataModule;

public class GL_ResourcePool : Mono_Singleton_DontDestroyOnLoad<GL_ResourcePool>
{

    #region 模块池管理
    //资源池
    private Dictionary<int, List<GL_ObjectBase>> _objectPool = new Dictionary<int, List<GL_ObjectBase>>();

    //地址资源池
    //private Dictionary<string, List<GL_ObjectBase>> _pathPool = new Dictionary<string, List<GL_ObjectBase>>();
    //存
    public void Clear()
    {
        foreach (var list in _objectPool.Values)
        {
            foreach (var obj in list)
            {
                if(obj && obj.gameObject)
                    Destroy(obj.gameObject);
            }
        }
        _objectPool.Clear();
    }
    public void SaveObject(GL_ObjectBase obj)
    {
        int key = obj._hashCode;
        if (!obj._isActivate)
            return;
        obj._isActivate = false;
        if (_objectPool.ContainsKey(key))
        {
            //有， 则存
            if (null == _objectPool[key])
                _objectPool[key] = new List<GL_ObjectBase>();
            _objectPool[key].Add(obj);
        }
        else
        {
            //没有, 则创建 存
            List<GL_ObjectBase> objs = new List<GL_ObjectBase>();
            objs.Add(obj);
            _objectPool.Add(key, objs);
        }
        //TODO:GL
        //隐藏对象
        obj.Resource_Hide();
        //obj.transform.parent = transform;
        //obj.transform.SetParent(transform);
    }

    //public void SaveObject(string path)
    //{
    //    int key = obj._hashCode;
    //    if (!obj._isActivate)
    //        return;
    //    obj._isActivate = false;
    //    if (_objectPool.ContainsKey(key))
    //    {
    //        //有， 则存
    //        if (null == _objectPool[key])
    //            _objectPool[key] = new List<GL_ObjectBase>();
    //        _objectPool[key].Add(obj);
    //    }
    //    else
    //    {
    //        //没有, 则创建 存
    //        List<GL_ObjectBase> objs = new List<GL_ObjectBase>();
    //        objs.Add(obj);
    //        _objectPool.Add(key, objs);
    //    }
    //    //TODO:GL
    //    //隐藏对象
    //    obj.Resource_Hide();
    //    //obj.transform.parent = transform;
    //    obj.transform.SetParent(transform);
    //}
    //取
    public T TakeObject<T>(GL_ObjectBase obj,System.Action<T> preactive=null) where T : GL_ObjectBase
    {
        GL_ObjectBase result = null;
        //池有没有
        int key = obj.GetHashCode();
        if (_objectPool.ContainsKey(key))
        {
            //有 则取
            if (_objectPool[key] != null && _objectPool[key].Count > 0)
            {
                result = _objectPool[key][0];
                _objectPool[key].RemoveAt(0);
                if (result._isActivate)
                {
                    DDebug.LogError("~~~资源获取失败");
                }
                result._isActivate = true;
            }
        }
        if (result == null)
        {
            //没有 则创建
            result = GameObject.Instantiate(obj.gameObject).GetComponent<GL_ObjectBase>();
            result._hashCode = key;
            result._isActivate = true;
            result.Resource_Awake();
        }
        preactive?.Invoke(result as T);
        result.Resource_Activate();
        
        return result as T;
    }


    public T TakeObject<T>(int tableID, EResourceType type) where T : GL_ObjectBase
    {
        GL_ObjectBase result = null;
        //生成唯一标示
        int key = (int)type * 10000000 + tableID;
        //池有没有

        if (_objectPool.ContainsKey(key))
        {
            //有 则取
            if (_objectPool[key] != null && _objectPool[key].Count > 0)
            {
                result = _objectPool[key][0];
                _objectPool[key].RemoveAt(0);
                if (result._isActivate)
                {
                    DDebug.LogError("~~~资源获取失败");
                }
                result._isActivate = true;
            }
        }
        if (result == null)
        {
            //没有 则创建
            result = GetObject(tableID, type);
            if (result == null)
                return null;

            result._hashCode = key;
            result._isActivate = true;
            result.Resource_Awake();
        }

        result.Resource_Activate();

        return result as T;
    }
    #endregion

    private GL_ObjectBase GetObject(int tableID, EResourceType type)
    {
        if (type == EResourceType.Audio)
        {
            TableAudioData ad = GameDataTable.GetTableAudioData(tableID);
            if (ad == null) return null;

            //加载音效片段
            AudioClip clip = GL_LoadAssetMgr._instance.Load(GL_ConstData.AuidoClipPath + ad.Path) as AudioClip;
            if (clip == null) return null;

            //加载音效prefab
            GL_Audio prefab = Resources.Load<GL_Audio>("Prefab/AudioPrefab");
            if (prefab == null) return null;
            prefab = GameObject.Instantiate(prefab);
            prefab.name = "Audio_" + clip.name;
            prefab._clip = clip;
            prefab._isLoop = ad.Loop;
            prefab._volume = ad.Volume;
            prefab._source = prefab.GetComponent<AudioSource>();
            return prefab;
        }
        else if(type == EResourceType.Effect)
        {
            TableEffectData data = GameDataTable.GetTableEffectData(tableID);
            //字段里没有预制,需要重新创建
            GameObject obj = Resources.Load(GL_ConstData.EffectPrefabPath + data.Path) as GameObject;
            if (obj != null)
            {
                GL_Effect effect = obj.GetComponent<GL_Effect>();
                if (effect == null)
                {
                    GL_Tools.TransformMakeZero(obj.transform, null);
                    effect = obj.AddComponent<GL_Effect>();
                }
                effect.Init(data);
                return effect;
            }
        }
        return null;
    }


    //Loading界面可以预加载 资源
    //模块. 道具 障碍物
    public void PreloadInit()
    {

    }
    public void PreLoad<T>(T obj,int size) where T : GL_ObjectBase
    {
        if (obj != null)
        {
            for (int i = 0; i < size; ++i)
            {
                T g = TakeObject<T>(obj);
                SaveObject(g);
              
            }
        }
    }
}
