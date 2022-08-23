//2019.08.08    关林
//推送封装

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
#if UNITY_IPHONE
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif


public class GL_PushLogic : Mono_Singleton<GL_PushLogic>
{
    private bool _isInit = false;
    private List<SPushInfo> _pushInfo; //推送内容
    private SPushArchiveInfo _pushArchiveInfo;    //推送存档
    #region 初始化
    public void Init()
    {
        _isInit = true;
        //_pushArchiveInfo = GL_CoreData._instance._archivedData._pushArchiveInfo;
        //if(_pushArchiveInfo == null)
        //{
        //    _pushArchiveInfo = GL_CoreData._instance._archivedData._pushArchiveInfo = new SPushArchiveInfo();
        //    GL_CoreData._instance.SaveData();
        //}
        ParseTable();
#if UNITY_IPHONE
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(
            NotificationType.Alert |
            NotificationType.Badge |
            NotificationType.Sound);
#endif
        //第一次进入游戏的时候清空，有可能用户自己把游戏冲后台杀死，这里强制清空
        //CleanNotification();
        NotificationLogic();
    }
    private void ParseTable()
    {
        //_pushInfo = new List<SPushInfo>();
        //var list = DataModule.DataModuleManager._instance.TableLocalNotificationData_Dictionary;
        //foreach (var value in list.Values)
        //{
        //    SPushInfo info = new SPushInfo();
        //    info._id = value.ID;
        //    info._isLoop = value.IsLoop;
        //    info._titles = new List<string>();
        //    info._message = new List<string>();
        //    foreach (var id in value.Details)
        //    {
        //        string str = LanguageMgr.GetInstance().ShowText(id);
        //        info._titles.Add(str);
        //    }
        //    foreach(var id in value.Contents)
        //    {
        //        string str = LanguageMgr.GetInstance().ShowText(id);
        //        info._message.Add(str);
        //    }

        //    _pushInfo.Add(info);
        //}
    }
    #endregion

    //唤醒api
    void OnApplicationPause(bool paused)
    {
        //DDebug.LogError("~~~OnApplicationPause: " + paused);
        if (!_isInit)
            return;
        //程序进入后台时
        if (paused)
        {
            //10秒后发送
            NotificationLogic();

            //测试
            //NotificationMessage(_pushInfo[0], DateTime.Now.AddSeconds(10));
        }
        else
        {
            //程序从后台进入前台时
            CleanNotification();
        }
    }

    private void NotificationLogic()
    {
        CleanNotification();
        try
        {
            //四个推送
            //1。转盘推送
            //if(DrawCtrl._instance.IsUnlockRotateDraw())
            //{
            //    var time = DrawCtrl._instance.GetRotateDrawCd();
            //    if(time != TimeSpan.Zero 
            //        && GL_CoreData._globalData.RotateDrawMaxCount !=  GL_CoreData._instance._archivedData.rotateDrawArchiveInfo.rotateCount
            //       && time.TotalMilliseconds>0)
            //    {
            //        //当前CD中
            //        DateTime dt = DateTime.Now.AddMilliseconds(time.TotalMilliseconds);
            //        //DDebug.LogError("转盘推送："+ time.TotalMilliseconds + "豪秒后");
            //        NotificationMessage(_pushInfo[0], dt);
            //    }
            //}
            ////2. 体力推送
            //if(!GL_PlayerData._instance.IsMaxEnergy())
            //{
            //    bool isPush = false;
            //    if(_pushArchiveInfo._energyPushNumber < 1)
            //    {
            //        if(_pushArchiveInfo._energypushTime == 0)
            //        {
            //            isPush = true;

            //        }
            //        else
            //        {
            //            //检测是否过期
            //            if(GL_Time._instance.CalculateSeconds() > _pushArchiveInfo._energypushTime)
            //            {
            //                _pushArchiveInfo._energyPushNumber = 1;
            //            }
            //            else
            //            {
            //                isPush = true;
            //            }

            //        }
            //    }

            //    if(isPush)
            //    {
            //        //需要推送

            //        double value = GL_PlayerData._instance.HasConsumedEnergy() * GL_CoreData._globalData.EnergyDuration;

            //        DateTime dt = DateTime.Now.AddSeconds(value);
            //        //DDebug.LogError("体力推送：" + value + "秒后");
            //        NotificationMessage(_pushInfo[1], dt);

            //        //存档
            //        value += GL_Time._instance.CalculateSeconds();
            //        _pushArchiveInfo._energypushTime = value;
            //    }
            //}

            ////3. 签到推送
            //bool isCanSign =  GL_SignInSystem._instance.CheckCanSign(); //是否可以签到
            //bool isExpired = DateTime.Now.Hour >= 15;             //是否过了15点
            //if(isCanSign && !isExpired)
            //{
            //    //当天推送
            //    DateTime dt = CalculateDateTime(15);

            //    //DDebug.LogError("签到推送：当天15点");
            //    NotificationMessage(_pushInfo[2], dt);
            //}
            //else
            //{
            //    //第二天开始推送
            //    DateTime dt = CalculateDateTime(15);
            //    dt.AddDays(1);
            //    //DDebug.LogError("签到推送：名天15点");
            //    NotificationMessage(_pushInfo[2], dt);
            //}

            //4.多日未登陆推送
            //DDebug.LogError("登录推送：名天这个时间");
            NotificationMessage(_pushInfo[3], DateTime.Now.AddDays(1));
        }
        catch (System.Exception)
        {
            DDebug.LogError(" catch (System.Exception)");
            throw;
        }


    }

    public void NewDay()
    {
        _pushArchiveInfo._energyPushNumber = 0;
        _pushArchiveInfo._energypushTime = 0;
    }


    #region 接口

    private DateTime CalculateDateTime(int hour)
    {
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        return new DateTime(year, month, day, hour, 0, 0);
    }

    //本地推送 你可以传入一个固定的推送时间
    void NotificationMessage(SPushInfo info, DateTime newDate)
    {
        if (newDate < System.DateTime.Now)
        {
            newDate = newDate.AddDays(1);
        }
        string title = GL_Tools.RandomList(info._titles);
        title = Regex.Unescape(title);
        //根据title位置找到content
        string content = "";
        if (!string.IsNullOrEmpty(title))
        {
            for (int i = 0; i < info._titles.Count; i++)
            {
                if (title.Equals(Regex.Unescape(info._titles[i])))
                {
                    content = info._message[i];
                    content = Regex.Unescape(content);
                }
            }
        }

       

#if UNITY_IPHONE
        //推送时间需要大于当前时间
        if (newDate > System.DateTime.Now)
		{
			UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
			localNotification.fireDate =newDate;	
			localNotification.alertBody = string.Empty;
			localNotification.applicationIconBadgeNumber = 1;
			localNotification.hasAction = true;
			localNotification.alertAction = title;
            localNotification.alertTitle = title;
            localNotification.alertBody = content;
			if(info._isLoop)
			{
				//是否每天定期循环
				localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;
				localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
			}
			localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
		}
#endif
#if UNITY_ANDROID
        if (newDate > System.DateTime.Now)
        {
            var time = newDate - System.DateTime.Now;
            if (info._isLoop)
            {
                //Debug.LogError("~~~发送定时: " + index + " 时间为: " + delay);
                LocalNotification.SendRepeatingNotification(info._id, (long)time.TotalMilliseconds, 24 * 60 * 60 * 1000, title, content, new Color32(0xff, 0x44, 0x44, 255));
            }
            else
            {
                LocalNotification.SendNotification(info._id, (long)time.TotalMilliseconds, title,content, new Color32(0xff, 0x44, 0x44, 255));
            }
        }
#endif
    }

    //清空所有本地消息
    void CleanNotification()
    {
        //DDebug.LogError("CleanNotification");
#if UNITY_IPHONE
        UnityEngine.iOS.LocalNotification l = new UnityEngine.iOS.LocalNotification();
        l.applicationIconBadgeNumber = -1;
        UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow(l);
        UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#endif
#if UNITY_ANDROID
        foreach (var info in _pushInfo)
        {
            LocalNotification.CancelNotification(info._id);
        }
#endif
    }
    #endregion
}


public class SPushInfo
{
    public int _id;
    public bool _isLoop;    //是否循环
    public List<string> _titles;  //标题
    public List<string> _message;    //内容
}


//推送存档
[Serializable]
public class SPushArchiveInfo
{
    public int _energyPushNumber;   //体力推送次数
    public double _energypushTime;  //体力推送时间
}