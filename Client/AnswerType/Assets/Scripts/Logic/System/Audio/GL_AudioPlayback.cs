//2018.09.11    关林
//音效播放

using System;
using UnityEngine;
using System.Collections;

public class GL_AudioPlayback : Mono_Singleton_DontDestroyOnLoad<GL_AudioPlayback>
{
    public GL_Audio _bgmAudio;  //背景音乐
    public GL_Audio _gameBGMAudio; //游戏内音乐
    [HideInInspector]
    public Transform _audioObject;  //音效
    public void Init()
    {
        _audioObject = new GameObject().transform;
        _audioObject.parent = transform;
        _audioObject.name = "AudioObject";

        PlayBGM(false);

        //PlayGameBGM(false);
    }
    #region 接口


    //切换背景音
    public void SwitchBGM(EGameState gameState = EGameState.None)
    {
        if (!GL_CoreData._instance.BGMAudioOn)// || GL_Game._instance.GameState == EGameState.Loading)
            return;

        if (gameState == EGameState.None)
            gameState = GL_Game._instance.GameState;

        if (gameState == EGameState.GameMain)
        {
            // PlayGameBGM(false);
            PlayBGM(true);
        }
        //else if (gameState == EGameState.Playing)
        //{
        //    PlayGameBGM(true);
        //    PlayBGM(false);
        //}
    }

    public void StopBGM()
    {
        // PlayGameBGM(false);
        PlayBGM(false);
    }

    public void BeginBGM()
    {
        // PlayGameBGM(false);
        PlayBGM(true);
    }
    #endregion

    #region 逻辑
    //刷新背景音
    private void PlayBGM(bool set)
    {
        // if (_bgmAudio == null)
        // {
        //     _bgmAudio = PlayAudio(1);
        //     if(_bgmAudio != null)
        //         _bgmAudio.transform.parent = transform;
        // }
        // if (set)
        // {
        //     _bgmAudio?.RefreshAudio();
        // }
        // else
        // {
        //     _bgmAudio?.StopAudio();
        // }
    }
    // private void PlayGameBGM(bool set)
    // {
    //     if (_gameBGMAudio == null)
    //     {
    //         _gameBGMAudio = PlayAudio(3);
    //         _gameBGMAudio.transform.parent = transform;
    //     }
    //     if (set)
    //         _gameBGMAudio.RefreshAudio();
    //     else
    //         _gameBGMAudio.StopAudio();
    // }
    #endregion

    //音效开关
    public void AudioOnOff(bool set)
    {
        if (_audioObject != null)
            _audioObject.gameObject.SetActive(set);
    }

    public void BGMOnOff(bool set)
    {
        if (set)
        {
            SwitchBGM();
        }
        else
        {
            PlayBGM(false);
            // PlayGameBGM(false);
        }
    }

    public GL_Audio Play(int tableID)
    {
        //DDebug.LogError("~~~播放音效:  " + tableID);
        GL_Audio result = null;
        if (GL_CoreData._instance.AudioOn)
        {
            //普通音效
            result = PlayAudio(tableID);
            if (result)
                result.transform.parent = _audioObject;
        }

        return result;
    }

    private GL_Audio PlayAudio(int tableID)
    {
        GL_Audio clip;
        clip = GL_ResourcePool._instance.TakeObject<GL_Audio>(tableID, EResourceType.Audio);
        if (clip == null)
        {
            DDebug.Log("该音效为空+" + tableID);
            return null;
        }

        clip._source.clip = clip._clip;
        clip._source.volume = clip._volume;
        clip._source.loop = clip._isLoop;
        clip._source.Play();

        return clip;
    }

   
    public GL_Audio PlayTips(int tableID)
    {
       // StopBGM();
        //DDebug.LogError("~~~播放音效:  " + tableID);
        GL_Audio result = null;
        if (GL_CoreData._instance.AudioOn)
        {
            //普通音效
            result = PlayTipsAudio(tableID);
            
            if (result)
                result.transform.parent = _audioObject;
        }
        return result;
    }
    private GL_Audio PlayTipsAudio(int tableID)
    {
        GL_Audio clip;
        clip = GL_ResourcePool._instance.TakeObject<GL_Audio>(tableID, EResourceType.Audio);
        if (clip == null)
        {
            DDebug.Log("该音效为空+" + tableID);
            return null;
        }

        clip._source.clip = clip._clip;
        clip._source.volume = clip._volume;
        clip._source.loop = clip._isLoop;
        clip._stopBgm = true;
        clip._source.Play();
        

        return clip;
    }

}
