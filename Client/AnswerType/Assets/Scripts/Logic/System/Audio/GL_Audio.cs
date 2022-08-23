//2018.09.11    关林
//音效

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_Audio : GL_ObjectBase
{
    [GL_Name("音效片段")]
    public AudioClip _clip;

    [GL_Name("循环")]
    public bool _isLoop;

    [Range(0, 1)]
    [GL_Name("音量")]
    public float _volume = 1;

    public AudioSource _source;

    private bool _isLive;   //是否存活
    private float _liveTime;    //生命周期
    public bool _stopBgm = false;
    public override void Resource_Activate()
    {
        gameObject.SetActive(true);
        _isLive = true;
        if (!_isLoop)
            _liveTime = _clip.length;

    }
    public override void Resource_Hide()
    {
        if (_stopBgm)
        {
            GL_AudioPlayback._instance.BeginBGM();
        }
        gameObject.SetActive(false);
    }

    public override bool DoUpdate(float dt)
    {
        return true;
    }

    public void Update()
    {
        if (!_isLoop && _isLive)
        {
            if (_liveTime < 0)
                GL_ResourcePool._instance.SaveObject(this);

            _liveTime -= Time.deltaTime;
        }
        
    }

    public void RefreshAudio()
    {
        _source.Play();
    }

    public void StopAudio()
    {
        _source.Stop();
      
    }

    public void PauseAudio()
    {
        _source.Pause();
    }
}
