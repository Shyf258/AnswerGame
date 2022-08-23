//2018.10.31    关林
//序列帧 特效

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_Effect_Animator : GL_ObjectBase
{
    [GL_Name("生命周期")]
    public float _life = 1f;
    [GL_Name("动画名称")]
    public string _animationName;

    private float _timer;
    Transform _parent;

    private ParticleSystem[] ps;
    private Animation _animation;
    public void Awake()
    {
        ps = transform.GetComponentsInChildren<ParticleSystem>();
        _animation = GetComponentInChildren<Animation>();
        _animationName = _animation.clip.name;

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

    public void Play(Transform tf)
    {
        _animation.Stop();
        _animation.Play(_animationName);
        foreach (ParticleSystem p in ps)
        {
            p.Play();
        }
    }
    public void ResetUpdateTime()
    {
        _timer = _life;
    }

    public override bool DoUpdate(float dt)
    {
        if (null != _parent)
        {
            transform.position = _parent.position;
            transform.rotation = _parent.rotation;
        }
        if (0 < _timer)
        {
            _timer -= dt;
            if (_timer < 0)
            {
                return false;
            }
            return true;
        }

        return false;
    }
}
