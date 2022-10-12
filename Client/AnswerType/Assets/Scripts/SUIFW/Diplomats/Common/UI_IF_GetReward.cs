using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SUIFW;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Logic.System.NetWork;
using SUIFW.Diplomats.Common;
using Object = System.Object;

public class UI_IF_GetReward : BaseUIForm
{
    private string _description =
        "恭喜获得<size=64><color=#fb3234>{0}</color></size>元红包和<size=64><color=#fb3234>大量</color></size>金币";
    private Text _rewardDescription;
    private Button _btnSure;
    private Button _btnDouble;

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
    private Image _btnImage;
    private Image _withDrawSlider;
    private string _withdrawTipsStr = "<size=64><color=#ff0000>{0}</color></size>元提现进度";
    private Text _withdrawTipsText;

    private Text _fillText;

    public List<Sprite> _ListSprite;
    
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

        _btnSure = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "BtnSure");
        RigisterButtonObjectEvent(_btnSure, (go) =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetRed);
            OnClickSure();
        });

        _btnDouble = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "BtnDouble");
        RigisterButtonObjectEvent(_btnDouble, (go) =>
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
            if (_withDrawSlider.fillAmount>=1)
            {
                UIManager.GetInstance().GetMain()._withdrawPageToggle.isOn = true;
                OnClickSure();
            }
            else
            {
                UI_HintMessage._.ShowMessage("金币不足，请继续努力!");
            }
           
        });
        _btnImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(withdrawtips.gameObject, "WithDrawBtn");
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
        if (_withDrawSlider.fillAmount>=1)
        {
            _btnImage.sprite = _ListSprite[0];
            _fillText.text = "Max";
        }
        else
        {
            _btnImage.sprite = _ListSprite[1];
            _fillText.text = (_withDrawSlider.fillAmount * 100).ToString("0") + "%";
        }
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
                num =(value /100f).ToString("0.00");

            _rewardDescription.text = string.Format(_description,num);
        }

        //回调
        if (datas.Length > 2 && datas[2] != null)
        {
            _rewardCallBack = datas[2] as Action<int>;
        }
    }
}