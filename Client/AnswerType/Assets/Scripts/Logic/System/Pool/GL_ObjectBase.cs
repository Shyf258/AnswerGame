//2018.10.11    关林
//单位基类

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GL_ObjectBase : MonoBehaviour
{
    [HideInInspector]
    public int _hashCode;       //预制的HashCode

    [HideInInspector]
    public bool _isActivate;    //是否激活

    public virtual bool DoUpdate(float dt) { return true; }


    public virtual void Resource_Awake() { }
    /// <summary>
    /// 资源池, 激活
    /// </summary>
    public virtual void Resource_Activate()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 资源池,销毁
    /// </summary>
    public virtual void Resource_Destory()
    {
        //Debug.LogError("~~~name: " + this.name);
        GL_ResourcePool._instance.SaveObject(this);
    }

    /// <summary>
    /// 资源池,隐藏
    /// </summary>
    public virtual void Resource_Hide()
    {
        //transform.position = Vector3.one * 1000f;
        gameObject.SetActive(false);
    }
}
