using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataModule;

public class Gl_EffectManager : Mono_Singleton_DontDestroyOnLoad<Gl_EffectManager>
{

    private List<GL_ObjectBase> _lifeEffectList = new List<GL_ObjectBase>();

    public GL_Effect PlayEffect(GL_Effect prefab, Transform tf, Vector3 pos,bool isRotate = true)
    {
        if (prefab == null)
            return null;

        GL_Effect effect = GL_ResourcePool._instance.TakeObject<GL_Effect>(prefab);
        effect.transform.parent = transform;
        effect.transform.position = pos;
        effect.Play(tf,isRotate);
        _lifeEffectList.Add(effect);
        return effect;
    }

    public GL_Effect PlayEffect(GL_Effect prefab, Vector3 pos, Quaternion q)
    {
        if (prefab == null)
            return null;

        GL_Effect effect = GL_ResourcePool._instance.TakeObject<GL_Effect>(prefab);
        effect.transform.SetParent(transform);
        effect.transform.position = pos;
        effect.transform.rotation = q;
        effect.Play(null);
        _lifeEffectList.Add(effect);
        return effect;
    }

    public GL_Effect_Animator PlayEffectAnimator(GL_Effect_Animator prefab, Transform tf, Vector3 pos)
    {
        GL_Effect_Animator effect = GL_ResourcePool._instance.TakeObject<GL_Effect_Animator>(prefab);
        effect.transform.parent = transform;
        effect.transform.position = pos;
        effect.transform.localScale = Vector3.one;
        effect.Play(tf);
        _lifeEffectList.Add(effect);
        return effect;
    }

    public GL_Effect PlayEffectP(GL_Effect prefab, Transform tf,Vector3 pos)
    {
        if (prefab == null)
            return null;

        GL_Effect effect = GL_ResourcePool._instance.TakeObject<GL_Effect>(prefab);
        effect.transform.SetParent(tf);
        effect.transform.position = pos;
        effect.Play(tf);
        _lifeEffectList.Add(effect);
        return effect;
    }
    public void DoUpdate(float dt)
    {
        if (_lifeEffectList.Count == 0)
            return;
        for(int i = _lifeEffectList.Count - 1; i >=0; --i)
        { 
            if(!_lifeEffectList[i].DoUpdate(dt))
            {
                //消亡
                _lifeEffectList[i].Resource_Destory();
                _lifeEffectList.RemoveAt(i);
            }
        }
    }

    public void RemoveEffect(GL_ObjectBase effect)
    {
        if (effect == null)
            return;
        effect.Resource_Destory();
        if (_lifeEffectList != null && _lifeEffectList.Count != 0)
            _lifeEffectList.Remove(effect);
        
    }

    public void RemoveAllEffect()
    {
        for (int i = _lifeEffectList.Count - 1; i >= 0; --i)
        {

            //消亡
            _lifeEffectList[i].Resource_Destory();
            _lifeEffectList.RemoveAt(i);
        }

    }


    #region 根据ID播放特效

    private GL_Effect GetEffectPrefab(int tableID)
    {
        return GL_ResourcePool._instance.TakeObject<GL_Effect>(tableID, EResourceType.Effect); 
    }

    public GL_Effect PlayEffect(int effectID, Transform tf, Vector3 pos)
    {
        return PlayEffect(effectID, tf, pos, true);
    }



    public GL_Effect PlayEffect(int effectID, Transform tf, Vector3 pos ,bool isRotate = true)
    {
        GL_Effect prefab = GetEffectPrefab(effectID);

        return PlayEffect(prefab, tf, pos,isRotate);
    }

    public GL_Effect PlayEffect(int effectID, Vector3 pos, Quaternion q)
    {
        GL_Effect prefab = GetEffectPrefab(effectID);

        return PlayEffect(prefab, pos, q);
    }

    public GL_Effect PlayEffectP(int effectID, Transform tf, Vector3 pos)
    {
        GL_Effect prefab = GetEffectPrefab(effectID);

        return PlayEffectP(prefab, tf, pos);
    }

    //预加载, 暂时无用
    //private bool loaded = false;
    //internal void CreatePrefabs()
    //{       
    //    if(loaded)
    //    {
    //        return;
    //    }
    //    loaded = true;
    //    var dic = TableEffectData.dic;
    //    foreach (var d in dic.Values)
    //    {
    //        var prefab = GetEffectPrefabByID(d);
           
    //         GL_ResourcePool._instance.PreLoad<GL_Effect>(prefab, 2);
    //    }
    //}


    #endregion
}
