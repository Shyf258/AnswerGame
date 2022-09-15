using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SUIFW;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Logic.System.NetWork;
using Object = System.Object;

public class UI_IF_GetReward : BaseUIForm
{
    private List<string> _description = new List<string>()
    {
        "恭喜获得<size=64><color=#fb3234>{0}</color></size>元红包和<size=64><color=#fb3234>大量</color></size>金币",
        "通关成功！观看视频最多可得3万金币约0.3元",
    };
    private Text _rewardDescription;
    private Transform _group;
    private Button _btnSure;
    private Button _btnDouble;


    private Transform _groupCoin;
    private Button _btnSureCoin;
    private Button _btnDoubleCoin;
    
    
    /// <summary>
    /// 当前奖励数量
    /// </summary>
    private int _curRewardNum;

    /// <summary>
    /// 奖励回调，bool是否在游戏界面
    /// 0点击关闭 1多倍领取 2普通领取
    /// </summary>
    private Action<int> _rewardCallBack;
    //private EItemType _doubleType; //双倍按钮类型

    private Button _withdrawBtn;
    private Image _withDrawSlider;
    private string _withdrawTipsStr = "<size=64><color=#ff0000>{0}</color></size>元提现进度";
    private Text _withdrawTipsText;

    private Text _fillText;
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;
        _isOpenMainUp = true;

        RigisterButtonObjectEvent("BtnClose", (go) =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRed);
            OnClickSure();
        });
        _rewardDescription = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "RewardText");

        _group = UnityHelper.FindTheChildNode(gameObject, "Group");
        
        _btnSure = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(_group.gameObject, "BtnSure");
        RigisterButtonObjectEvent(_btnSure, (go) =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRed);
            OnClickSure();
        });
        _btnDouble = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(_group.gameObject, "BtnDouble");
        RigisterButtonObjectEvent(_btnDouble, (go) =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetCoin);
            OnBtnDouble();
        });

        _groupCoin = UnityHelper.FindTheChildNode(gameObject, "GroupCoin");
        _btnSureCoin = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(_groupCoin.gameObject, "BtnSure");
        RigisterButtonObjectEvent(_btnSureCoin, (go) =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRed);
            OnClickSure();
        });
        _btnDoubleCoin = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(_groupCoin.gameObject, "BtnDouble");
        RigisterButtonObjectEvent(_btnDoubleCoin, (go) =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetCoin);
            OnBtnDouble();
        });
        
        _rewardDescription = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "RewardText");
        
        Transform withdrawtips = UnityHelper.FindTheChildNode(gameObject, "WithDrawTips");
        _withDrawSlider = UnityHelper.GetTheChildNodeComponetScripts<Image>(withdrawtips.gameObject, "Fill");
        _fillText = UnityHelper.GetTheChildNodeComponetScripts<Text>(withdrawtips.gameObject, "FillText");
        _withdrawBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(withdrawtips.gameObject, "WithDrawBtn");
        RigisterButtonObjectEvent(_withdrawBtn, go =>
        {
            UIManager.GetInstance().GetMain()._withdrawPageToggle.isOn = true;
            OnClickSure();
        });
        _withdrawTipsText = UnityHelper.GetTheChildNodeComponetScripts<Text>(withdrawtips.gameObject, "TipsText");
    }

    public override void Display(bool redisplay = false)
    {
        base.Display(redisplay);
        gameObject.transform.SetAsLastSibling();
    }

    public override void InitData(object data)
    {
        base.InitData(data);

         ShowUi(data);
    }

   

    public override void Hiding()
    {
        base.Hiding();
        GL_AudioPlayback._instance.PlayTips(11);
        GL_AudioPlayback._instance.Play(7);
        GL_AD_Interface._instance.CloseNativeAd();
    }

    private void OnClickClose()
    {
        CloseUIForm();
        _rewardCallBack?.Invoke(2);
        _rewardCallBack = null;
    }
    
    
    private void OnClickSure()
    {

        CloseUIForm();
        GetNormal();
       
    }

    private void GetNormal()
    {
        _rewardCallBack?.Invoke(2); //2是不翻倍
        _rewardCallBack = null;
    }
    /// <summary>
    /// 翻倍领取
    /// </summary>
    private void OnBtnDouble()
    {
        CloseUIForm();
        GL_PlayerData._instance.IsPlayIdiomConjCoinRewardAD(true);
        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_GetDoubleReward, CB_AD_Double);
    }
    private void CB_AD_Double(bool set)
    {
        if(set)
        {
            _rewardCallBack?.Invoke(1);
            _rewardCallBack = null;
        }
        else
        {
            _rewardCallBack?.Invoke(2);
            _rewardCallBack = null;
        }
        
    }
    
    public override void onUpdate()
    {
    }

    public override void Refresh(bool recall)
    {
        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Native_LevelReward); 

        _withDrawSlider.fillAmount =(float) GL_PlayerData._instance.Coin / GL_PlayerData._instance._withDrawTarget[EWithDrawType.DailyWithDraw].coupon;
        _fillText.gameObject.SetActive(_withDrawSlider.fillAmount>=1);

        _withdrawTipsText.text = String.Format(_withdrawTipsStr,(GL_PlayerData._instance._withDrawTarget[EWithDrawType.DailyWithDraw].money/100f).ToString("0.00"));
    }

    private void ShowUi(object data)
    {

        var datas = data as Object[];
        if (datas == null) 
            return;
        EItemType itemType = EItemType.None;
        //奖励
        if (datas.Length > 0 && datas[0] is EItemType type)
        {
            itemType = type;
        }

        //奖励数量
        if (datas.Length > 1 && datas[1] is int value)
        {
            string num = value.ToString();

            if (itemType == EItemType.Bogus)
            {
                num =(value /100f).ToString("0.00");

                _rewardDescription.text = string.Format(_description[0],num);
                _groupCoin.SetActive(false);
                _group.SetActive(true);
            }
            else
            {
                _rewardDescription.text = _description[1];
                _groupCoin.SetActive(true);
                _group.SetActive(false);
            }
           
        }

        //回调
        if (datas.Length > 2 && datas[2] != null)
        {
            _rewardCallBack = datas[2] as Action<int>;
        }
    }
}