//2020.05.20    关林
//版本信息获取

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_SystemInfo : Singleton<GL_SystemInfo>
{
    //是否降低配置
    //=1. 安卓9.0 稍微降低一些配置
    //=2. 安卓8.0及以下  大量降低配置
    private int _isReduce = 0;
    public void Init()
    {
        string operatingSystem = SystemInfo.operatingSystem;
        //-1 是满配
        //IsReduce = -1;
#if UNITY_EDITOR
        
#elif UNITY_ANDROID
        operatingSystem = operatingSystem.Substring(11, 2);
        int version;
        if(!int.TryParse(operatingSystem, out version))
            version = int.Parse(operatingSystem.Substring(0, 1));
#if GuanGuan_Test
        IsReduce = -1;
#endif
        if (version <= 8)
        {
            IsReduce = 2;
        }
        else if (version == 9)
        {
            IsReduce = 1;
        }
#elif UNITY_IOS
#endif

    }

    public int IsReduce
    {
        get
        {
            return _isReduce;
        }
        set
        {
            _isReduce = value;
            if(value > 1)
            {
                //关闭阴影
                //QualitySettings.shadows = ShadowQuality.Disable;
                //骨骼顶点权重
                QualitySettings.skinWeights = SkinWeights.TwoBones;
                //切割次数减少
            }

            if(value == 2)
            {


            }
        }
    }


    //应用版本信息
    private string _appVersion;
    public string AppVersion
    { 
        get
        {
            if(string.IsNullOrEmpty(_appVersion))
                _appVersion = GL_SDK._instance.GetAppVersion();

            return _appVersion;
        }
    }
}
