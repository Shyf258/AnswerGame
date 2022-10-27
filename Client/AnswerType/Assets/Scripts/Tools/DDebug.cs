//2020.04.02    关林
//Log封装

using System.Text;
using UnityEngine;

public class DDebug
{
    private static string _errorColor = "yellow";

    private static string _warningColor = "yellow";

    private static string _logColor = "white";

    public static void Log(string info)
    {
        if(AppSetting.IsLog)
        {
            info = string.Concat("<color=", _logColor, ">", info, "</color>");
            Debug.Log(info);
        }
        
    }

    public static void LogError(string info)
    {
        if (AppSetting.IsLog)
        {
            info = string.Concat("<color=", _errorColor, ">", info, "</color>");
            Debug.LogError(info);
        }
    } 

    //格式 ~~~值: {0}  , 值1: {1} 
    public static void LogError(string info, object arg0)
    {
        if (AppSetting.IsLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(info, arg0);
            LogError(sb.ToString());
        }
    }
    public static void LogError(string info, object arg0, object arg1)
    {
        if (AppSetting.IsLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(info, arg0, arg1);
            LogError(sb.ToString());
        }
    }
    public static void LogError(string info, object arg0, object arg1, object arg2)
    {
        if (AppSetting.IsLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(info, arg0, arg1, arg2);
            LogError(sb.ToString());
        }
    }
    public static void LogError(string info, params object[] args)
    {
        if (AppSetting.IsLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(info, args);
            LogError(sb.ToString());
        }
    }

    public static void LogWarning(string info)
    {
        if (AppSetting.IsLog)
        {
            info = string.Concat("<color=", _warningColor, ">", info, "</color>");
            Debug.LogWarning(info);
        }
    }

    public static void LogColorRed(string info)
    {
        if (AppSetting.IsLog)
        {
            info = string.Concat("<color=", _errorColor, ">", info, "</color>");
            Debug.Log(info);
        }
    }
}