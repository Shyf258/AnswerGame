using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_NewbieSign : BaseUIForm
{

    public GameObject _signToggleItem;
    public GameObject _signMessageItem;

    private Transform _toggleContent;
    private Transform _messageContent;
    
    private Button _btnGet;
    private Text _btnText;
    private Image _playAd;
    
    private Image _lastFinish;
    private Text _lasText;
    private Toggle _lastToggle;
    
    private Transform _modeSign;
    private Transform _modeGetReward;

    private FF_SignMessage _ffSignMessage;
    private FF_SignToggle _ffSignToggle;
    private ESignDayState _eSignDayState;
    
    private Timer _timer;
    private Action _callback;


    private Text _title;
    
    private List<string> _btnStr = new  List<string>()
    {
        "点击到账",
        "已完成签到",
        "快速签到",
    };
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        _timer = new Timer(null,true);
        _btnGet = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Get");
        _playAd = UnityHelper.GetTheChildNodeComponetScripts<Image>(_btnGet.gameObject, "PlayAd");
        RigisterButtonObjectEvent(_btnGet, go =>
        {
            ClickBtn();
        });
        _btnText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnGet.gameObject, "Text");
        RigisterButtonObjectEvent("ExitPage", go =>
        {
            CloseUIForm();
            _callback?.Invoke();
            _callback = null;
        });

        RigisterButtonObjectEvent("NextDay", go =>
        {
            MoveMessage(true);
        });
        
        RigisterButtonObjectEvent("LastDay", go =>
        {
            MoveMessage(false);
        });
        
        Transform last = UnityHelper.FindTheChildNode(gameObject, "LastToggle");
        _lastFinish = UnityHelper.GetTheChildNodeComponetScripts<Image>(last.gameObject, "Finish");
        _lasText = UnityHelper.GetTheChildNodeComponetScripts<Text>(last.gameObject, "Text");
        _lastToggle = last.GetComponent<Toggle>();
        _lastToggle.onValueChanged.AddListener(
            (delegate(bool show)
                {
                    if (show)
                    {
                        DayCount = GL_NewbieSign._instance._newbieSignDayInfoList.Count-1;
                        MovePage( DayCount);
                    }
                }
                ));
        _toggleContent = UnityHelper.FindTheChildNode(gameObject, "ToggleContent");
        _messageContent = UnityHelper.FindTheChildNode(gameObject, "MessageContent");

        _modeSign = UnityHelper.FindTheChildNode(gameObject, "ModeSign");
        _modeGetReward = UnityHelper.FindTheChildNode(gameObject, "ModeGetReward");

        _title = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TitleText");

    }
    public override void InitData(object data)
    {
        base.InitData(data);
        var datas = data as object[];
        if (datas == null)
            return;

        //1.回调
        if (datas.Length > 0 && datas[0] is Action action)
        {
            _callback = action;
        }
    }

    private List<GameObject> _toggleList = new List<GameObject>();
    private List<GameObject> _messageList = new List<GameObject>();
    
    public override void Refresh(bool recall)
    {
        DayCount = GL_NewbieSign._instance._curDate;
        switch (GL_NewbieSign._instance.NewbieSignState)
        {
            case ENewbieSignState.WaitingGet:
                ChangeMode(true);
                break;
            case ENewbieSignState.Model1:
            case ENewbieSignState.Model7:
            case ENewbieSignState.Model360:
                ChangeMode(false);
                break;
            case ENewbieSignState.Complete:
                FinishSign();
                ChangeMode(false);
                break;
        }

       
    }

    public override void onUpdate()
    {
        
    }

    private int dayCount;

    public int DayCount
    {
        get => dayCount;
        set => dayCount = value;
    }

    private void GetSign()
    {
        if ( GL_NewbieSign._instance.NewbieSignState == ENewbieSignState.WaitingGet)
        {
            GL_NewbieSign._instance.SelectState(ENewbieSignState.WaitSelect);
            object[] datas = { _callback };
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_ChangeSignType,datas);
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.NewPlayReceive);
            CloseUIForm();
        }
        else if (GL_NewbieSign._instance.NewbieSignState == ENewbieSignState.Complete)
        {
            //完成签到
          
        }
        else
        {
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_NewbieSign,(success =>
            {
                if (success)
                { 
                    //签到
                    GL_NewbieSign._instance.DoSign(DayCount);
                    //切换显示状态 
                    _ffSignMessage.FreshMessage(
                        GL_NewbieSign._instance._newbieSignDayInfoList[dayCount]._index,
                        GL_NewbieSign._instance._newbieSignDayInfoList[dayCount]._total);

                    _ffSignToggle.ChangeState(GL_NewbieSign._instance._newbieSignDayInfoList[dayCount]._index >=
                                              GL_NewbieSign._instance._newbieSignDayInfoList[dayCount]._total);
                    
                    switch (GL_NewbieSign._instance._newbieSignDayInfoList.Count)
                    {
                        case 7:
                            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRewardFinishAd7);
                            break;
                        case 14:
                            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRewardFinishAd14);
                            break;
                        case 360:
                            // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetReward360);
                            break;
                    }
                }
            }));
        }
      
    }
    
    private DateTime _signTime;
    private bool _canGet = true;
    
    private void ClickBtn()
    {
        if ( GL_NewbieSign._instance.NewbieSignState == ENewbieSignState.WaitingGet)
        {
            GetSign();
            return;
        }

        if (_eSignDayState == ESignDayState.Complete)
        {
            UI_HintMessage._.ShowMessage("不可重复签到！");
            return;
        }
        if (_canGet)
        {
            _canGet = false;
            GetSign();
            if (GL_NewbieSign._instance._newbieSignDayInfoList[dayCount]._state == ESignDayState.Complete)
            {
                _btnGet.SetActive(false);
                return;
            }
            double now = GL_Time._instance.CalculateSeconds();

            if (now < GL_NewbieSign._instance._curCooldown)
            {
                var offset = GL_NewbieSign._instance._curCooldown - now;
                _signTime = DateTime.Now;
                _signTime = _signTime.AddSeconds(offset);
                if (_timer == null)
                {
                    _timer = new Timer(null, true);
                }
                _timer.StartCountdown(_signTime,(() =>
                {
                    _canGet = true;
                    _timer?.Stop();
                    ShowBtnState(DayCount);
                }),_btnText);
            }
        }
        else
        {
            UI_HintMessage._.ShowMessage("冷却中，请稍等");
            double now = GL_Time._instance.CalculateSeconds();

            if (now > GL_NewbieSign._instance._curCooldown)
            {
                _canGet = true;
                _timer?.Stop();
                ShowBtnState(DayCount);
            }
        }
    }

    private void ChangeMode(bool get)
    {
        if (get)
        {
            _modeSign.SetActive(false);
            _modeGetReward.SetActive(true);
        }
        else
        {
            _modeSign.SetActive(true);
            _modeGetReward.SetActive(false);
            GL_Tools.RefreshItem(GL_NewbieSign._instance._newbieSignDayInfoList,_toggleContent,_signToggleItem,_toggleList);
            GL_Tools.RefreshItem(GL_NewbieSign._instance._newbieSignDayInfoList,_messageContent,_signMessageItem,_messageList);
            _ffSignToggle =  _toggleList[DayCount].GetComponent<FF_SignToggle>();
            _ffSignMessage = _messageList[DayCount].GetComponent<FF_SignMessage>();
            _lasText.text = $"{ GL_NewbieSign._instance._newbieSignDayInfoList.Count }天";
            _title.text = $"签到{GL_NewbieSign._instance._newbieSignDayInfoList.Count}天自动到账";
            _ffSignToggle.GetComponent<Toggle>().isOn = true;
            MovePage(DayCount);
        }
        ShowBtnState(DayCount);
      
    }
    
    public void MovePage(int index)
    {
        Vector3 position =  new Vector3( ((-(540 + 30)) * index)-270, 0,0);
        _messageContent.DOLocalMove(position, 0.5f, false);
        //DDebug.LogError("***** 移动位置："+ position);
        ShowBtnState(index);
    }


    private void MoveMessage(bool next)
    {
        int count = next ? 1 : -1;
        
        if (DayCount == 0 && count ==- 1  || DayCount== GL_NewbieSign._instance._newbieSignDayInfoList.Count && count == 1)
        {
            return;
        }

        DayCount += count;
        MovePage(DayCount);
    }

    private void ShowBtnState(int day)
    {
        _playAd.SetActive(false);

        if (GL_NewbieSign._instance.NewbieSignState == ENewbieSignState.Complete || GL_NewbieSign._instance.NewbieSignState == ENewbieSignState.WaitingGet)
        {
            _btnText.text = _btnStr[0];
            return;
        }
        
        _eSignDayState =  GL_NewbieSign._instance._newbieSignDayInfoList[day]._state;
        switch (_eSignDayState)
        {
            case ESignDayState.Can :
                _btnGet.SetActive(true);
                _btnText.text = _btnStr[2];
                _playAd.SetActive(true);
                break;
            case ESignDayState.Cannot :
               _btnGet.SetActive(false);
                break;
            case ESignDayState.Complete :
                _btnGet.SetActive(true);
                _btnText.text = _btnStr[1];
                break;
        }
    }

    /// <summary>
    /// 完成签到
    /// </summary>
    private void FinishSign()
    {
        _lastFinish.SetActive(true);
    }

}
public enum SignMessageState
{
    Finish,
    UnFinish,
}