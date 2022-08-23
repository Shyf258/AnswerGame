//2018.09.05    关林
//时间计算, 用于离线收益

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_Time : Singleton<GL_Time>
{
    //原始时间
    private DateTime _initialTime;

    /// <summary>
    /// 同步时时间
    /// </summary>
    private DateTime serverSynchronizationTime;

    /// <summary>
    /// 同步时秒数
    /// </summary>
    private double serverSynchronizationTimer;

    private bool isServerInit = false;

    public void Init()
    {
        _initialTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    }

    /// <summary>
    /// 同步服务器时间
    /// </summary>
    public void InitServerTime(DateTime serverTime)
    {
        Debug.Log("Init " + serverTime);
        serverSynchronizationTime = serverTime;
        serverSynchronizationTimer = Time.realtimeSinceStartup;
        isServerInit = true;

    }

    /// <summary>
    /// 获取服务器时间
    /// </summary>
    /// <returns></returns>
    public DateTime GetServerTime()
    {
        // 服务器没返时间 使用本地时间
        if (!isServerInit)
        {
            return System.DateTime.Now;
        }
        DateTime serverDateTime = serverSynchronizationTime;
        serverDateTime = serverDateTime.AddSeconds(Time.realtimeSinceStartup - serverSynchronizationTimer);
        return serverDateTime;
    }

    /// <summary>
    /// 时间戳转日期
    /// </summary>
    public DateTime TimestampToDateTime(double time)
    {
        return _initialTime.AddSeconds(time);
    }

    //计算分钟
    public double CalculateMinute()
    {
        //首先检查网络
        //TODO:GL 以后添加

        //读取本地时间
        DateTime dt = DateTime.Now;
        TimeSpan ts = dt - _initialTime;
        return ts.TotalMinutes;
    }

    //计算秒
    public double CalculateSeconds(DateTimeKind kind = DateTimeKind.Local)
    {
        DateTime dt = DateTime.Now;
        if (kind == DateTimeKind.Utc)
            dt = DateTime.UtcNow;
        TimeSpan ts = dt - _initialTime;
        return ts.TotalSeconds;
    }

    public double CalculateSeconds(DateTime time)
    {
        TimeSpan ts = time - _initialTime;
        return ts.TotalSeconds;
    }
    //今天的时间戳
    public double CalculateSecondsToday()
    {
        DateTime dt = DateTime.Today;
        TimeSpan ts = dt - _initialTime;
        return ts.TotalSeconds;
    }


    /// <summary>
    /// 获得时间戳(13位
    /// </summary>
    public long GetTimestamp()
    {
        //DateTime dt = DateTime.Now;
        DateTime ttt = new DateTime(1970, 2, 1, 0, 0, 0, 0);
        TimeSpan ts = ttt - _initialTime;
        return (long)ts.TotalMilliseconds;
    }



    /// <summary>
    /// 获得时间戳(10位
    /// </summary>
    public long GetTimestamp_10()
    {
        DateTime dt = DateTime.UtcNow;
        TimeSpan ts = dt - _initialTime;
        return (long)ts.TotalSeconds;
    }

    public int GetIntervalDay(double time)
    {
        time = time / 60 / 60 / 24;
        return (int)time;
    }

    public double GetDaySeconds(int day)
    {
        double result = 24 * 60 * 60 * day;
        return result;
    }

    #region 获取腾讯时间
    //public int year, mouth, day, hour, min, sec;

    //public string timeURL = "http://cgi.im.qq.com/cgi-bin/cgi_svrtime";

    //void Start()
    //{
    //    StartCoroutine(GetTime());
    //}

    //IEnumerator GetTime()
    //{
    //    WWW www = new WWW(timeURL);
    //    while (!www.isDone)
    //    {
    //        yield return www;
    //    }
    //    SplitTime(www.text);
    //}

    //void SplitTime(string dateTime)
    //{
    //    dateTime = dateTime.Replace("-", "|");
    //    dateTime = dateTime.Replace(" ", "|");
    //    dateTime = dateTime.Replace(":", "|");
    //    string[] Times = dateTime.Split('|');
    //    year = int.Parse(Times[0]);
    //    mouth = int.Parse(Times[1]);
    //    day = int.Parse(Times[2]);
    //    hour = int.Parse(Times[3]);
    //    min = int.Parse(Times[4]);
    //    sec = int.Parse(Times[5]);
    //}
    #endregion
}
