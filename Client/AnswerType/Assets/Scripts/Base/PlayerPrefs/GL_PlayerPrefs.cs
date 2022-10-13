﻿//2022.6.24 关林
//PlayerPrefs数据存储

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EPrefsKey
{
    IsAgreeGDPR = 0, //隐私协议
    IsFCM, //防沉迷
    IsWeChatLogIn = 0, //微信登录 
    IsOpenLogToTool,  //工具log开关
    CoreData,       //核心数据类
    IsReceiveNewPlayer,//领取新手红包
}

public class GL_PlayerPrefs : MonoBehaviour
{

    #region 存
    public static void SetInt(EPrefsKey key, int value)
    {
        PlayerPrefs.SetInt(key.ToString(), value);
    }
    public static void SetFloat(EPrefsKey key, float value)
    {
        PlayerPrefs.SetFloat(key.ToString(), value);
    }
    public static void SetString(EPrefsKey key, string value)
    {
        PlayerPrefs.SetString(key.ToString(), value);
    }
    
    public static void SetBool(EPrefsKey key, bool value)
    {
        int result = value ? 1 : 0;
        PlayerPrefs.SetInt(key.ToString(), result);
    }
    
    #endregion

    #region 取
    public static int GetInt(EPrefsKey key)
    {
        return PlayerPrefs.GetInt(key.ToString());
    }
    public static float GetFloat(EPrefsKey key)
    {
        return PlayerPrefs.GetFloat(key.ToString());
    }

    public static string GetString(EPrefsKey key)
    {
        return PlayerPrefs.GetString(key.ToString());
    }
    
    public static bool GetBool(EPrefsKey key)
    {
        return PlayerPrefs.GetInt(key.ToString()) == 1;
    }

    #endregion

    #region 处理对象
    public static void SaveObject<T>(T obj, EPrefsKey key)
    {
        string json = JsonUtility.ToJson(obj);
        SetString(EPrefsKey.CoreData, json);
    }

    public static T GetObject<T>(EPrefsKey key)
    {
        string json = GetString(key);
        T t = JsonUtility.FromJson<T>(json);
        return t;
    }
    #endregion

    public static bool HasKey(EPrefsKey key)
    {
        return PlayerPrefs.HasKey(key.ToString());
    }
    public static void DeleteKey(EPrefsKey key)
    {
        PlayerPrefs.DeleteKey(key.ToString());
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }
}