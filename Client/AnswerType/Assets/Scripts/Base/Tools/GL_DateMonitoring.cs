////2021.02.01    关林
////日期监测

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GL_DateMonitoring : Singleton<GL_DateMonitoring>
{
    private SDateArchiveInfo _dateArchiveInfo;

    private bool _isNewDay = false;
    public void Init()
    {
        _dateArchiveInfo = GL_CoreData._instance._archivedData._dateArchiveInfo;
        if(_dateArchiveInfo == null)
        {
            _dateArchiveInfo = new SDateArchiveInfo();
            _dateArchiveInfo.Refresh();
            GL_CoreData._instance._archivedData._dateArchiveInfo = _dateArchiveInfo;
            GL_CoreData._instance.SaveData();
            _isNewDay = true;
        }
        else
        {
            if(CheckData())
            {
                
                _dateArchiveInfo.Refresh();
                GL_CoreData._instance._archivedData._dateArchiveInfo = _dateArchiveInfo;
                _isNewDay = true;
                GL_CoreData._instance.SaveData();
            }
            else
            {
                _dateArchiveInfo._loginTimes += 1;
                GL_CoreData._instance.SaveData();
            }
        }
    }

    public void DoUpdate()
    {
        if (CheckData())
        {
            _dateArchiveInfo.Refresh();
            GL_CoreData._instance._archivedData._dateArchiveInfo = _dateArchiveInfo;
            _isNewDay = true;
            CheckNewDay();
            GL_CoreData._instance.SaveData();
        }
    }
    private bool CheckData()
    {
        DateTime dt = DateTime.Now;
        if (_dateArchiveInfo._year < dt.Year      //超过年
            || (_dateArchiveInfo._year == dt.Year && _dateArchiveInfo._month < dt.Month)  //超过月
            || (_dateArchiveInfo._year == dt.Year && _dateArchiveInfo._month == dt.Month && _dateArchiveInfo._day < dt.Day))   //超过天
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //新的一天
    public void CheckNewDay()
    {
        if (!_isNewDay)
            return;
        _isNewDay = false;
        //通知签到 任务 刷新
        //GL_SignInSystem._instance.ParseSignInState();
        //GL_PlayerData._instance.RefreshGainCurrencyNewDay();
        //GL_TaskSystem._instance.ClearDailyData(); // 清除每日任务数据
        //ShopCtrl._instance.NewDay(); 
        //DrawCtrl._instance.ClearCoinDrawData(); // 清除金币每日抽奖次数
        //DrawCtrl._instance.ClearRotateDrawData(); // 清除转盘数据
        //GL_CoreData._instance.AddUp_Login += 1; // 统计登录数据
        //GL_PushLogic._instance.NewDay();
        GL_NewbieSign._instance.NewDay();
    }

    //获取当天登陆次数
    public int GetLoginTimes()
    {
        if (GL_CoreData._instance._archivedData._dateArchiveInfo == null)
            return 0;
        else
            return GL_CoreData._instance._archivedData._dateArchiveInfo._loginTimes;
    }
}


//日期存档
[Serializable]
public class SDateArchiveInfo
{
    public int _year;   //年
    public int _month;  //月
    public int _day;    //日

    public int _loginTimes; //登陆次数

    public int _onlineSecond; //在线时间
    
    public void Refresh()
    {
        DateTime dt = DateTime.Now;
        _year = dt.Year;
        _month = dt.Month;
        _day = dt.Day;
        _loginTimes = 1;
        _onlineSecond = 0;
        try
        {
            GL_PlayerData._instance.BankConfig.nowDay += 1;
            GL_GameEvent._instance.SendEvent(EEventID.RefreshWaitWithDraw);
        }
        catch 
        {
           
        }
    }
}

