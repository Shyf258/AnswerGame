using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_NewLogin : BaseUIForm
{
   
    #region 对象

    /// <summary>
    /// 已登录天数
    /// </summary>
    private Text _dayNumber;
    /// <summary>
    /// 登录提现提示
    /// </summary>
    private Text _loginTips;
    
    /// <summary>
    /// 提现天数要求
    /// </summary>
    private Text _textDescription;
    /// <summary>
    /// 每日提现金额
    /// </summary>
    private Text _rewardCountEvery;
    /// <summary>
    /// 每日提现按键
    /// </summary>
    private Button _btnEveryDay;
    /// <summary>
    /// 每日提现金额
    /// </summary>
    private Text _withDrawCountEveryDay;
    
    
    /// <summary>
    /// 连续登录提现金额
    /// </summary>
    private Text _rewardCountLogin;
    /// <summary>
    /// 累计登录提现按键
    /// </summary>
    private Button _btnLoginDay;
    /// <summary>
    /// 累计天差距天数
    /// </summary>
    private Text _dayGap;
    /// <summary>
    /// 累计提现金额
    /// </summary>
    private Text _withDrawCountLogin;
    
    #endregion


    #region 参数

    private List<string> _listDescription = new List<string>()
    {
        "{0}<size=72>天</size>",
        "差{0}天可额外提现",
        "每登录<color=#ff0000>5</color>天，可额外提现<color=#ff0000>{0}</color>元",
        "{0}元",
        "{0}元提现",
    };

    /// <summary>
    /// 登录配置
    /// </summary>
    private Net_CB_LoginConfig _netCbLoginConfig;
    
    /// <summary>
    /// 提现配置
    /// </summary>
    private Net_CB_WithDraw _netCbWithDraw = new Net_CB_WithDraw();

    /// <summary>
    /// 天数差距
    /// </summary>
    private int day;
    
    #endregion
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        
        RigisterButtonObjectEvent("Btn_Back", go =>
        {
            CloseUIForm();
        });

        _dayNumber = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "DayNumber");
        _loginTips = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "LoginDescription");
        Transform _top = UnityHelper.FindTheChildNode(gameObject, "EveryDay");
        _btnEveryDay = UnityHelper.GetTheChildNodeComponetScripts<Button>(_top.gameObject, "Btn_WithDraw");        
        RigisterButtonObjectEvent(_btnEveryDay, go =>
        {
            WithDrawOnClick(WithDrawType.EveryDay);
        });
        _withDrawCountEveryDay = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnEveryDay.gameObject, "Text");
        _rewardCountEvery = UnityHelper.GetTheChildNodeComponetScripts<Text>(_top.gameObject, "RewardCount");
      
        Transform _bottom = UnityHelper.FindTheChildNode(gameObject, "LoginDay");
        
      
        
        _rewardCountLogin = UnityHelper.GetTheChildNodeComponetScripts<Text>(_bottom.gameObject, "RewardCount");
        _btnLoginDay = UnityHelper.GetTheChildNodeComponetScripts<Button>(_bottom.gameObject, "Btn_WithDraw");
        RigisterButtonObjectEvent(_btnLoginDay, go =>
        {
            WithDrawOnClick(WithDrawType.Login);
        });
        _withDrawCountLogin = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnLoginDay.gameObject, "Text");
        _dayGap = UnityHelper.GetTheChildNodeComponetScripts<Text>(_bottom.gameObject, "Text_Description");
    }
    public override void Refresh(bool recall)
    { 
        RefreshPage();
        if (GL_PlayerData._instance._PlayerCostState._costState == CostState.Low)
        {
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_LoginPage);
        } 
    }

    public override void OnHide()
    {
        base.OnHide();
        GL_AD_Interface._instance.CloseBannerAd();
    }

    public override void onUpdate()
    {
       
    }

    private void RefreshPage()
    {
        _netCbLoginConfig = GL_PlayerData._instance._NetCbLoginConfig;
        
        //登录天数
        _dayNumber.text = String.Format(_listDescription[0], _netCbLoginConfig.day);
        //累计登录天数提醒
        _loginTips.text = String.Format(_listDescription[2], (_netCbLoginConfig.withDraws[1].money/100f).ToString("0.00"));
        
        //每日登录
        
        //按键文字每日提现
        if (_netCbLoginConfig.withDraws[0].withDrawLimit>0)
        {
            _withDrawCountEveryDay.text = String.Format(_listDescription[4],
                ( _netCbLoginConfig.withDraws[0].money/100f).ToString("0.00"));
        }
        else
        {
            _withDrawCountEveryDay.text = "明日再领";
        }
        //图标提现金额显示
        _rewardCountEvery.text = String.Format(_listDescription[3],
            ( _netCbLoginConfig.withDraws[0].money/100f).ToString("0.00"));
        
        // //按键可交互开关
        // _btnEveryDay.interactable =  _netCbLoginConfig.withDraws[0].withDrawLimit > 0;
        
        
        //累计登录
        
        //按键文字累计提现
        _withDrawCountLogin.text = String.Format(_listDescription[4],
            ( _netCbLoginConfig.withDraws[1].money/100f).ToString("0.00"));
        
        //图标提现金额显示
        _rewardCountLogin.text = String.Format(_listDescription[3],
            ( _netCbLoginConfig.withDraws[1].money/100f).ToString("0.00"));
        
        //累计登录天数差距提醒
        day = 5- ( _netCbLoginConfig.day%5 ) ;
        if (_netCbLoginConfig.withDraws[1].withDrawLimit<=0)
        {
            _dayGap.text =  String.Format(_listDescription[1],day);
        }
        else
        {
            _dayGap.text = "已获得额外提现机会!";
        }
        // //按键可交互开关
        // if (_netCbLoginConfig.withDraws[1].withDrawLimit<=0)
        // {
        //     _dayGap.SetActive(false);
        //     // _btnLoginDay.interactable = false;
        // }
        // else
        // {
        //     _dayGap.SetActive(true);
        //     // _btnLoginDay.interactable = true;
        // }

        if (_netCbLoginConfig.withDraws.Count<2)
        {
            Transform _bottom = UnityHelper.FindTheChildNode(gameObject, "LoginDay");
            _bottom.SetActive(false);
        }
        
    }

    private void WithDrawOnClick(WithDrawType drawType)
    {
      _netCbWithDraw = new Net_CB_WithDraw();
        switch (drawType)
        {
            case WithDrawType.EveryDay:
                _netCbWithDraw = _netCbLoginConfig.withDraws[0];
                if (_netCbWithDraw.withDrawLimit<=0)
                {
                    UI_HintMessage._.ShowMessage("请明日上线领取！");
                    return;
                }
                break;
            case WithDrawType.Login:
                _netCbWithDraw = _netCbLoginConfig.withDraws[1];

                if (_netCbWithDraw.withDrawLimit<1)
                {
                    UI_HintMessage._.ShowMessage($"再登录{day}天即可提现哦！");
                    return;
                }
                break;
            default:
                break;
        }
        
        if (GL_PlayerData._instance._NetCbLoginConfig.viewAds < _netCbWithDraw.needAd)
        {
            UI_HintMessage._.ShowMessage($"今日再浏览{_netCbWithDraw.needAd-GL_PlayerData._instance._NetCbLoginConfig.viewAds}次视频即可提现哦！");
        }
        else
        {
            CloseUIForm();
            GL_GameEvent._instance.SendEvent(EEventID.RefreshLogin);
            WithDraw();
        }
        
    }

    private void WithDraw()
    {

        
        
        if (!GL_PlayerData._instance.IsLoginWeChat())
        {
            //DDebug.Log("@@@@@@@@没有登录微信");
            //登陆微信
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin);
            return;
        }

        if (GL_PlayerData._instance._PlayerCostState._costState!= CostState.Vip)
        {
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_LoginWithDraw, (show=>
            {
                if (show)
                {
                    PlayADFinish();
                }
                else
                {
                    UI_HintMessage._.ShowMessage("广告播放失败，请重新观看！");
                }
            }));
        }
        else
        {
            PlayADFinish();
        }
        
   
    }

    private void PlayADFinish()
    {
        //提现广告播放成功
        Net_WithDraw draw = new Net_WithDraw();
        draw.withDrawId =  _netCbWithDraw.id;
        draw.type = 2;
        draw.withDrawType = (int) EWithDrawType.LoginWithDraw;
                
        GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), CB_WithDraw);

        if (draw.withDrawId == 1)
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.LoginReceive);
        }
        else
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.LoginReceiveAccumulated);
        }
    }

    private void CB_WithDraw(string param)
    {
        GL_PlayerData._instance.Net_CB_WithDrawResult(param);
        float money = _netCbWithDraw.money ;
        EWithDrawType _eWithDrawType = EWithDrawType.Normal;
        var obj = new object[]
        {
            money,
            _eWithDrawType
        };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
    }

    enum WithDrawType
    {
     EveryDay,
     Login,
    }

}
