using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AppSettingConfig", menuName = "Create AppSetting", order = 0)]
public class AppSettingConfig : ScriptableObject
{
    [Header("")]
    [GL_Name("资源加载模式")]
    public EAppResourceLoadMode _appLoadMode;
    [GL_Name("连接服务器类型")]
    public EServerType _serverType;
    [GL_Name("是否关闭新手引导")]
    public bool _isCloseGuide;
    [GL_Name("是否开启Log")]
    public bool _isLog;
    [GL_Name("是否Unity测试")]
    public bool _isTestUnity;
    [GL_Name("游戏进入方式")]
    public EGameEnterType _enterType;
    [GL_Name("ABTest")]
    public string _abTest;
    [GL_Name("是否跳过广告")]
    public bool _isSkipAD;
    [GL_Name("包名配置")]
    public EBuildApp _buildApp;
    [GL_Name("实名认证")]
    public bool _check;
    [GL_Name("跳过微信登录")]
    public bool _skipWeChatLogin;
    [GL_Name("打包时间")]
    public double _buildTime;
    [GL_Name("防沉迷有效时间")]
    public int _buildHour;
    
}