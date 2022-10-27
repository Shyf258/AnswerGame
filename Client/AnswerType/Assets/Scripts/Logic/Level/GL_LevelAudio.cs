//2022.4.26 关林
//问答内容 发音

using System;
using System.Collections;
using System.Collections.Generic;
using DataModule;
using UnityEngine;
using UnityEngine.Networking;

public class GL_LevelAudio
{
    private GL_Audio _audio;
    public void Init()
    {
        _audio = Resources.Load<GL_Audio>("Prefab/AudioPrefab");
        _audio = GameObject.Instantiate(_audio);
        _audio.name = "levelAudio";
        _audio.transform.SetParent(GL_AudioPlayback._instance._audioObject);

    }
    public void PlayAudio(int id)
    {
        GL_VersionManager._instance.RefreshLevel(id);
         MethodExeTool.StartCoroutine(LoadAudio(id));
    }

    public IEnumerator LoadAudio(int id)
    {
        string audio = id.ToString();
        if (DataModuleManager._instance.TableAnswerInfoData_Dictionary[id].Audio!=null)
        {
            audio =  DataModuleManager._instance.TableAnswerInfoData_Dictionary[id].Audio + audio;
        }
        // DDebug.LogError("加载语音文件："+ audio);
        string path =  DownloadConfig.downLoadPath + string.Format(GL_VersionManager.AudioUrl, audio);

        if (!GL_Tools.IsFileExist(path))
        {
            //没有音频
            yield return null;
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR || UNITY_EDITOR_OSX
        path = "file://" + path;
#endif
            //DDebug.LogError("~~~LevelAudio: 1 " + path);
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                request.downloadHandler = new DownloadHandlerAudioClip(path, AudioType.MPEG);
                //DDebug.LogError("~~~LevelAudio: 2 " + request.downloadHandler);

                yield return request.SendWebRequest();
                //播放对应音效
                //DDebug.LogError("~~~LevelAudio: 3 " + request.downloadHandler);
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                //DDebug.LogError("~~~LevelAudio: 4 " + clip);
                _audio._clip = clip;
                _audio._source = _audio.GetComponent<AudioSource>();
                _audio._source.clip = clip;
                //_audio._source.volume = 2;
                _audio.RefreshAudio();

            }
        }
        
    }

    public void StopAudio()
    {
        _audio?.StopAudio();
    }
}
