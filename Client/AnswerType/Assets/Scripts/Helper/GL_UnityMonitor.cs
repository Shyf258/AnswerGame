//2021.2.6  关林
//unity后台事件监听

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_UnityMonitor : MonoBehaviour
{
    //唤醒api
    void OnApplicationPause(bool paused)
    {
        if (!GL_Game._instance._isInitialize)
            return;
        //DDebug.LogError("~~~OnApplicationPause: " + paused);
        if (paused)
        {
            //程序进入后台时
            DDebug.LogError("~~~程序进入后台时");
        }
        else
        {
            //程序从后台进入前台时
            DDebug.LogError("~~~程序从后台进入前台时");

            if (!GL_GuideManager._instance._isGuideing && !GL_AD_Logic._instance._isShowAD)
                GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Splash_Splash);
        }
    }

    //游戏失去焦点也就是进入后台时 focus为false 切换回前台时 focus为true
    //void OnApplicationFocus(bool focus)
    //{
    //    DDebug.LogError("~~~OnApplicationFocus: " + focus);
    //    if (focus)
    //    {
    //        //切换到前台时执行，游戏启动时执行一次
    //    }
    //    else
    //    {
    //        //切换到后台时执行
    //    }
    //}
}
