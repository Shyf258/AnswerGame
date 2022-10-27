//2021.12.1 关林
//计时器

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer
{


    /// <summary>
    /// 当前显示的秒
    /// </summary>
    public int _showSecond;
    /// <summary>
    /// 当前显示的分
    /// </summary>
    public int _showMinute;
    /// <summary>
    /// 当前显示的小时
    /// </summary>
    public int _showHour;
    #region 回调
    /// <summary> 开始计时回调 </summary>
    public Action OnCountdownStart { get; set; }
    /// <summary> 结束计时回调 </summary>
    public Action OnCountdownEnd { get; set; }
    /// <summary> 暂停计时回调 </summary>
    public Action OnCountdownPause { get; set; }
    /// <summary> 分别代表小时数和分钟数,计时过程回调 </summary>
    public Action<int, int, int> OnCountdowning { get; set; }
    /// <summary> 是否正在计时 </summary>
    public bool IsCountdowning { get; set; }
    private bool _pause;
    /// <summary> 每秒做回调 </summary>
    public Action OnEverySecond { get; set; }
    #endregion

    public Timer(MonoBehaviour mono)
    {
        _mono = mono;
    }
    /// <summary>
    /// 是否是全局倒计时
    /// </summary>
    /// <param name="mono">启动协程的对象</param>
    /// /// <param name="isGlobal">是否是全局倒计时</param>
    public Timer(MonoBehaviour mono, bool isGlobal = false)
    {
        _mono = mono;
        _isGlobal = isGlobal;
    }

    //存本地mono的目的是 为了方便停止计时器
    private MonoBehaviour _mono;
    private Text _uiText;    //携带UI组件

    private bool _isCountdown;   //是否是倒计时
    private bool _isGlobal;
    private DateTime _startDateTime; //启动时间
    private Coroutine _coroutine; //当前协程


    //总秒数
    private int _totalSecond;
    //计时器
    private float _timer;    //计时器中剩余的秒数

    //显示几个, 0 时分秒 有值则显示, 1只显示秒 2显示分秒 3显示小时分秒
    private int _showNumber;
    public void SetShowNumber(int num)
    {
        _showNumber = num;
    }
    /// <summary>
    /// 开始倒计时
    /// </summary>
    public void StartCountdown(int second, int minute = 0, int hour = 0, Action overCallBack = null, Text uiText = null)
    {
        _isCountdown = true;
        SetTime(hour, minute, second);
        DoStart(overCallBack, uiText);
    }

    //targetDateTime 目标时间
    public void StartCountdown(DateTime targetDateTime, Action overCallBack = null, Text uiText = null)
    {
        _isCountdown = true;
        SetTime(targetDateTime);
        DoStart(overCallBack, uiText);
    }


    public void StartTime(int second, int minute = 0, int hour = 0, Action overCallBack = null, Text uiText = null)
    {
        _isCountdown = false;
        SetTime(hour, minute, second);
        DoStart(overCallBack, uiText);
    }

    //计时器停止
    public void Stop()
    {
        if (_coroutine == null)
            return;

        IsCountdowning = false;
        
        if(_mono != null)
        {
            _mono.StopCoroutine(_coroutine);
        }
        else
        {
            MethodExeTool.StopCoroutine(_coroutine);
        }
    }


    private void DoStart(Action overCallBack = null, Text uiText = null)
    {
        OnCountdownEnd = overCallBack;
        _uiText = uiText;
        OnCountdownStart?.Invoke();
        if (_isGlobal)
        {
            if (_mono != null)
                _coroutine = _mono.StartCoroutine(Timer__Global());
            else
                _coroutine = MethodExeTool.StartCoroutine(Timer__Global());
        }
        else
        {
            if (_mono != null)
                _coroutine = _mono.StartCoroutine(Timer_Local());
            else
                _coroutine = MethodExeTool.StartCoroutine(Timer_Local());
        }
    }
    //全局计时器
    private IEnumerator Timer__Global()
    {
        DateTime target = _startDateTime.AddSeconds(_totalSecond);
        do
        {
            TimeSpan time = target - DateTime.Now;
            _timer = (float)time.TotalSeconds;
            RefreshData();
            yield return null;
        } while (_timer >= 0);
        //计时器结束
        OnCountdownEnd?.Invoke();
        IsCountdowning = false;
    }

    //本地计时器
    private IEnumerator Timer_Local()
    {
        do
        {
            _timer -= Time.deltaTime;
            RefreshData();
            yield return null;
        } while (_timer > 0);
        //计时器结束
        OnCountdownEnd?.Invoke();
        IsCountdowning = false;
    }

    //刷新数据
    private void RefreshData()
    {
        float value = _timer;
        if (!_isCountdown)
        {
            //正计时
            value = _totalSecond - value;
        }

        //倒计时
        _showHour = (int)(value / 3600);
        value = value % 3600;
        _showMinute = (int)(value / 60);
        _showSecond = (int)(value % 60);
        
        OnCountdowning?.Invoke(_showHour, _showMinute, _showSecond);
        IsCountdowning = true;

        //刷新UI
        if (_uiText != null && _uiText.gameObject.activeInHierarchy)
        {
            _uiText.text = GL_Tools.GetFormatCd(_showHour, _showMinute, _showSecond, _showNumber);
        }
    }

    //设置时间
    private void SetTime(int hour, int minute, int second)
    {

        _totalSecond = hour * 3600 + minute * 60 + second;
        _timer = _totalSecond;

        if (_isGlobal)
            _startDateTime = DateTime.Now;
    }

    //设置时间
    private void SetTime(DateTime targetDateTime)
    {
        _startDateTime = DateTime.Now;
        _totalSecond = (int)(targetDateTime - _startDateTime).TotalSeconds;
        _timer = _totalSecond;
    }
}