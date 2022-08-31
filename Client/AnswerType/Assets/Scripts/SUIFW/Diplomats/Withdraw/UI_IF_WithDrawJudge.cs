using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class UI_IF_WithDrawJudge : BaseUIForm
{
    private Color _colorGray;
    private Color _colorGreen;
    private Color _colorOrange;

    public List<Sprite> _icon;
     
    private Image _turnIcon;

    private Text _targetMoney;

    private Image _turnJudge;

    private Text _textJudge;
    
    private Text _growVideo;
    
    private int _videoCount;
    private EWithDrawType _drawType;
    private MyWithdrawData _withDraw;
    private Text _userName;
    private Text _enough;
    private Text _growMoney;

    private Action _action;
    private Timer _timer;
    private DateTime _growTime;
    private Text _btnText;
    
    private List<string> _growStr = new List<string>()
    {
        "还差{0}元",
        "还差{0}金币"
    };
    
    
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
         // _isOpenMainUp = true;
         
        
        Transform frame = UnityHelper.FindTheChildNode(gameObject, "TC_join");
        _turnIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(frame.gameObject, "TurnIcon");
        _growMoney = UnityHelper.GetTheChildNodeComponetScripts<Text>(frame.gameObject, "GrowMoney");
        _enough = UnityHelper.GetTheChildNodeComponetScripts<Text>(frame.gameObject, "Enough");

        _turnJudge = UnityHelper.GetTheChildNodeComponetScripts<Image>(frame.gameObject, "TurnJudge");
        _textJudge = UnityHelper.GetTheChildNodeComponetScripts<Text>(frame.gameObject, "TextJudge");
        _growVideo = UnityHelper.GetTheChildNodeComponetScripts<Text>(frame.gameObject, "GrowVideo");
        _userName = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "PLayName");
        _targetMoney = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TargetMoney");

        _btnText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "BtnText");
        RigisterButtonObjectEvent("ExitPage", go =>
        {
            CloseUIForm();
        });
        
        RigisterButtonObjectEvent("GetReward", go =>
        {
            GetReward();
        });
      
        _timer = new Timer(null,true);
        _state = State.Get;

        ColorUtility.TryParseHtmlString("#949494", out _colorGray);
        ColorUtility.TryParseHtmlString("#FFA132", out _colorOrange);
        ColorUtility.TryParseHtmlString("#309C0F", out _colorGreen);
    }

    public override void InitData(object data)
    {
        base.InitData(data);

        if (data == null)
        {
            return;
        }
        var datas = data as System.Object[];
        if (datas == null) 
            return;
        if ( datas.Length > 0 && datas[0] is MyWithdrawData withDraw )
        {
            _withDraw = withDraw;
            _videoCount = _withDraw.ViewNum;
        }

        if (datas.Length > 1 && datas[1] is EWithDrawType type)
        {
            _drawType = type;
        }

        if (datas.Length>2 && datas[2] is Action action)
        {
            _action = action;
        }
        
        
        
        FreshMessage();
        
    }

    public override void Refresh(bool recall)
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GrowMoneyPlan);
        // UIManager.GetInstance().GetMainUp().SetActive(true);
        _turnIcon.sprite = _icon[0];
        _turnIcon.transform.rotation = Quaternion.identity;
        
        _enough.color = _colorGray;
        _textJudge.color = _colorGray;
        if (CheckMoney())
        {
            _enough.text = $"余额达到{((float)_withDraw.WithDraw.money / 100)}元";
        }
        else
        {
            _enough.text = "余额不足";
        }
        TurnStart();
     
        _turnJudge.SetActive(false);
        _growVideo.SetActive(false);
        _textJudge.SetActive(false);
    }

    public override void onUpdate()
    {
       
    }

    public override void OnHide()
    {
        base.OnHide();
        // UIManager.GetInstance().GetMainUp().SetActive(false);
    }

    private void FreshMessage()
    {
        _growMoney.SetActive(true);
        _userName.text = GL_PlayerData._instance.WeChatName;
        _targetMoney.text = String.Format("<size=92>￥</size>{0}",_withDraw.WithDraw.money/100f);
       float needMoney = 0;
        switch (_drawType)
        {
            case EWithDrawType.CashWithDraw: //红包提现
                needMoney = _withDraw.WithDraw.money - GL_PlayerData._instance.Bogus;
                _growMoney.text = string.Format(_growStr[0], needMoney/100f);
                break;
            case EWithDrawType.DailyWithDraw: //金币提现
                needMoney = _withDraw.WithDraw.coupon - GL_PlayerData._instance.Coin;
                if (needMoney>0)
                {
                    _growMoney.text = string.Format(_growStr[1], needMoney);
                }
                else
                {
                    _growMoney.text = "";
                }
                break;
        }

        // int count = _withDraw.WithDraw.viewAdTimes - _videoCount;
      
      
        
    }

    private void ChangeIcon()
    {
        _turnIcon.sprite = _icon[1];
        
        //_enough.color = newColor;
        _enough.DOColor(_colorOrange, 0.6f);
    }

    private void TurnStart()
    {
        _turnIcon.transform.DORotate(new Vector3(0, 0, -720), 1f, RotateMode.FastBeyond360)
            .SetAs(TweenParams.Params.OnComplete(() =>
            {
                if (CheckMoney())
                {
                    TurnJudge();
                    _growMoney.SetActive(false);
                    _turnIcon.sprite = _icon[2];
                    // _enough.text = $"余额达到{((float)_withDraw.WithDraw.money / 100)}元";
                    
                    _enough.DOColor(_colorGreen, 0.6f);
                    //_enough.color = newColor;
                }
                else
                {
                    ChangeIcon();
                }
                //DDebug.LogError("旋转完成");
            }));
    }

    private void TurnJudge()
    {
        _turnJudge.SetActive(true);
        _growVideo.SetActive(true);
        _textJudge.SetActive(true);
        
        int count = _withDraw.WithDraw.viewAdTimes- _videoCount ;
        if (count>0)
        {
            _textJudge.text = $"观看{(_withDraw.WithDraw.viewAdTimes)}次广告";
            _growVideo.text = $"还差{(_withDraw.WithDraw.viewAdTimes-_videoCount  )}次广告";
            _textJudge.DOColor(_colorOrange, 0.6f);
        }
        // else if( GL_CoreData._instance._archivedData._dateArchiveInfo._loginDays <  _withDraw.WithDraw.needDay)
        // {
        //     _textJudge.text = $"登录{(_withDraw.WithDraw.needDay)}天";
        //     _growVideo.text = $"还差{(_withDraw.WithDraw.needDay-GL_CoreData._instance._archivedData._dateArchiveInfo._loginDays)}天";
        //     _textJudge.DOColor(_colorOrange, 0.6f);
        // }
        else
        {
            _textJudge.text = $"可以提现";
            _growVideo.text = "";
            _textJudge.DOColor(_colorGreen, 0.6f);
        }
        _turnJudge.transform.DORotate(new Vector3(0, 0, -7200), 10f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart);
    }
    

    private  bool _enoughMoney = false;
    private bool CheckMoney()
    {
       
        switch (_drawType)
        {
            case  EWithDrawType.CashWithDraw:
                _enoughMoney = GL_PlayerData._instance.Bogus >= _withDraw.WithDraw.money;
                break;
            
            case EWithDrawType.DailyWithDraw:
                _enoughMoney = GL_PlayerData._instance.Coin >= _withDraw.WithDraw.coupon;
                break;
        
        }

        return _enoughMoney;
    }


    /// <summary>
    /// 获取奖励
    /// </summary>
    private void GetReward()
    {
        if (_state== State.Wait)
        {
            return;
        }
        else
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GrowMoneyPlanClick);

            if (_enoughMoney)
            {
                Action<bool> cb_playad = delegate(bool show)
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GrowMoneyPlanFinish);
                    CoolDown();
                    _action?.Invoke();
                    //获取奖励
                    CloseUIForm();
                    if (show)
                    {
                        if (_videoCount>= _withDraw.WithDraw.viewAdTimes)
                        {
                            UI_HintMessage._.ShowMessage("提现已满足，快去提现吧！");
                            //WithDraw();
                        }
                    }
                };
                switch (_drawType)
                {
                    case  EWithDrawType.CashWithDraw:
                        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_MoreRedChance, cb_playad);
                        break;
                    
                    case EWithDrawType.DailyWithDraw:
                        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_MoreCoinChance, cb_playad);
                        break;
                }
            }
            else
            {
                GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_MoneyShort, (set) =>
                {
                    if(set)
                    {
                        GL_PlayerData._instance.SendGetVideoRedpack(EVideoRedpackType.WithDrawGrow, true, (msg) =>
                        {
                            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GrowMoneyPlanFinish);
                            Action action = () =>
                            {
                                // GL_PlayerData._instance.GetTaskConfig();
                                CoolDown();
                                _action?.Invoke();
                                _action = null;
                            };
                            object[] datas = { msg.rewards, action ,true };
                            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetResult, datas);
                     
                        });
                    
                   
                    }
                    //获取奖励
                    CloseUIForm();
                });
            }
           
        }
    }
    
    /// <summary>
    /// 冷却时间
    /// </summary>
    private void CoolDown()
    {
        double now = GL_Time._instance.CalculateSeconds();
    
        var intervalTime = GL_PlayerData._instance.GetVideoRedpackCD(EVideoRedpackType.WithDrawGrow);
        
        if (now < intervalTime)
        {
            _state = State.Wait;
            var offset = intervalTime - now;
            _growTime = DateTime.Now;
            _growTime = _growTime.AddSeconds(offset);
            if (_timer == null)
            {
                _timer = new Timer(null, true);
            }
            _timer.StartCountdown(_growTime,(() =>
            {
                _state = State.Get;
                _timer?.Stop();
                _btnText.text = "加速提现";
            }),_btnText);
        }
    }

    private void WithDraw()
    {
        //提现广告播放成功
        Net_WithDraw draw = new Net_WithDraw();
        draw.withDrawId = _withDraw.WithDraw.id;
        draw.type = 2;
        draw.withDrawType = (int) _drawType;
        GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_WithDraw);
    }

    //提现回调
    private void CB_WithDraw(string param)
    {
        float money = _withDraw.WithDraw.money /100f;
        EWithDrawType _eWithDrawType = EWithDrawType.Normal;
        var obj = new object[]
        {
            money,
            _eWithDrawType
        };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
        GL_PlayerData._instance.Coin -= _withDraw.WithDraw.coupon;
        //GL_PlayerData._instance._showWithDraw = true;
        // _withDrawData.withDrawLimit -- ;
        // with?.RefreshData();
        GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
        
        if (_drawType == EWithDrawType.DailyWithDraw)
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.CoinDailySuccess);
        }
        _action?.Invoke();
        _action = null;
    }
    
    private State _state;
    
    enum State
    {
        Wait,
        Get
    }
    

}
