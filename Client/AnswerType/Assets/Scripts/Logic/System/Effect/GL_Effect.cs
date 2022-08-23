//2018.07.18    关林
//特效


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataModule;

public class GL_Effect : GL_ObjectBase
{
    [GL_Name("真实生命周期")]
    public float _realLife = 0;
    #region private
    //粒子系统  
    private ParticleSystem[] ps;
    //播放时间  
    private float playTime;
    //跟随物体，如果此物体不为空，特效会跟随父物体移动，直到父物体变为null  
    private Transform parent;

    private float _life = -1;    //生命周期
    private float _timer;

    private Color _curColor;    //当前颜色
    private Material _material;
    private bool _isRotate = true;
    #endregion

    public void Init(TableEffectData data)
    {
        _realLife = data.Life;
        if (_realLife < 0)
        {
            ps = transform.GetComponentsInChildren<ParticleSystem>();
            if (_realLife <= 0)
            {
                foreach (ParticleSystem p in ps)
                {
                    p.loop = true;
                }
            }
        }
    }

    public void Awake()
    {
        ps = transform.GetComponentsInChildren<ParticleSystem>();
        if(_realLife <= 0)
        {
            foreach (ParticleSystem p in ps)
            {
                if (p.main.loop)
                {
                    //没有生命周期, 还是循环的,则Life不计算
                    //特效消失时间由,外部决定
                    break;
                }
                if (_life < p.main.duration)
                    _life = p.main.duration;
            }
        }
        else
        {
            _life = _realLife;
        }
        if(ps.Length > 0)
        {
            var rendererModule = ps[0].GetComponent<ParticleSystemRenderer>();
            _material = rendererModule.sharedMaterial;
        }
    }

    public override void Resource_Activate()
    {
        _timer = _life;
        gameObject.SetActive(true);
    }

    public override void Resource_Hide()
    {
        _timer = 0;
        gameObject.SetActive(false);
    }

    public void Play(Transform tf , bool isRotate = true)
    {
        //设置位置
        parent = tf;
        SetFreezeRotate(isRotate);
        foreach (ParticleSystem p in ps)
        {
            p.Play();
        }
    }

    public void SetFreezeRotate(bool isRotate)
    {
        _isRotate = isRotate;
    }

    public void SetScale(Transform t, float scale)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            SetScale(t.GetChild(i), scale);
        }
        t.localScale = new Vector3(scale, scale, scale);
    }

    public void SetLoop(bool isLoop)
    {
        foreach (ParticleSystem p in ps)
        {
            p.loop = isLoop;
        }
    }


    //刷新特效颜色
    public void RefreshColor(Color color)
    {
        //if (_curColor == color)
        //    return;
        _curColor = color;
        //foreach (var p in ps)
        //{
        //    p.startColor = color;
        //}
        _material.SetColor("_TintColor", color);
    }

    public override bool DoUpdate(float dt)
    {
        if (null != parent)
        {
            transform.position = parent.position;
            if(_isRotate)
                transform.rotation = parent.rotation;
        }
        if (_life == -1)
            return true;

        if(0 < _timer)
        {
            _timer -= dt;
            if(_timer < 0)
            {
                return false;
            }
            return true;
        }

        return false;
    }
}
