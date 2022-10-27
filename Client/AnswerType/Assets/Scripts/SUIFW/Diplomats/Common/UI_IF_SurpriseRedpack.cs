#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_OpenRedPack
// 创 建 者：Yangsong
// 创建时间：2021年12月01日 星期三 11:10
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using SUIFW;
using SUIFW.Helps;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_SurpriseRedpack : BaseUIForm
{
    public Sprite[] Sprites;

    private string[] _explainContents = new[] { "", "恭喜发财大吉大利" };
    private string[] _tipsContents = new[] { "无套路 秒到账", "微信提现秒到账" };

    private Action<bool> _rewardCallBack;   //回调
    private bool _isIgnoreAD = false;   //是否跳过广告
    private bool _isShowClose = true;   //是否显示关闭按钮
    private ERewardSource _rewardSource;
    private List<Rewards> _rewards;
    //private SRewardData _rewardData;    //奖励
    //private int _rewardNumber;  //奖励数量

    private Button _btnClose;

    private Text _explain1Text;      //解释第一条
    private Text _explain3Text;      //解释第三条
    private Button _btnPlayAD;      //播放视频按钮
    private Transform _tips1;
    private Text _tips2;
    private Image _btnImage;

    //private Image _openImg;
    //private Text _openContent;
    //public Sprite[] Sprites;

    

    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        _isFullScreen = false;

        Transform explain = UnityHelper.FindTheChildNode(gameObject, "Explain");
        _explain1Text = UnityHelper.GetTheChildNodeComponetScripts<Text>(explain.gameObject, "Text1");
        _explain3Text = UnityHelper.GetTheChildNodeComponetScripts<Text>(explain.gameObject, "Text3");

        var _btnOpenRedPack = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "_btnOpenRedPack");
        _btnImage = _btnOpenRedPack.GetComponent<Image>();
        _btnClose = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "_btnClose");

        Transform tips = UnityHelper.FindTheChildNode(gameObject, "Tips");
        _tips1 = UnityHelper.FindTheChildNode(tips.gameObject, "RedPackIcon_01");
        _tips2 = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "RewardDescription");


        RigisterButtonObjectEvent(_btnOpenRedPack, (go => { OnBtnOpenRedPack(); }));
        RigisterButtonObjectEvent(_btnClose, (go => { OnBtnClose(); }));

        _explain1Text.text = string.Format("[{0}]", GL_SDK._instance.GetProductName());
    }

    public override void InitData(object data)
    {
        base.InitData(data);

        _isIgnoreAD = false;
        _isShowClose = true;

        var datas = data as object[];
        if (datas == null)
            return;

        //1.回调
        if (datas.Length > 0 && datas[0] is Action<bool> action)
        {
            _rewardCallBack = action;
        }
        //2.显示关闭
        if (datas.Length > 1 && datas[1] is bool show)
        {
            _isShowClose = show;
        }
        //跳过广告
        if (datas.Length > 2 && datas[2] is bool set)
        {
            _isIgnoreAD = set;
        }
        //来源
        if (datas.Length > 3 && datas[3] is ERewardSource source)
        {
            _rewardSource = source;
        }
        //奖励数量
        if (datas.Length > 4 && datas[4] is List<Rewards> rewards)
        {
            _rewards = rewards;
        }

        Show();
    }

    private void Show()
    {
        _btnClose.gameObject.SetActive(_isShowClose);
        switch (_rewardSource)
        {
            case  ERewardSource.Guide:
                //int value = _rewards.Count > 0 ? _rewards[0].num : 1000;
                //UI_AnimHelper.AddVauleAnim(EItemType.Coin, _explain3Text, value, 2f, 0, null, true);
                _explain3Text.text = _rewards[0].num / 100 + "元";
                _explain3Text.fontSize = 100;
                _tips1.SetActive(false);
                _tips2.text = _tipsContents[0];
                break;
            default:
                _explain3Text.fontSize = 55;
                _explain3Text.text = _explainContents[1];
                break;
        }
        if(!_isIgnoreAD)
            _btnImage.sprite = Sprites[0];
        else
            _btnImage.sprite = Sprites[1];
    }

    public override void PoppedRefresh()
    {
        base.PoppedRefresh();
        _animator.Play("UI_IF_PopUp_Shake");
    }
    public override void OnHide()
    {
        base.OnHide();

        _rewardCallBack = null;
    }


    public override void Refresh(bool recall)
    {
        //_btnClose.gameObject.SetActive(_isShowClose);
        //if (GL_Game._instance.GameState == EGameState.GameMain
        //    && GL_PlayerData._instance.CurLevel > 1)
        //    // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_GoldenPig);
        //    GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Native_LevelReward);
        //_isPlayVideo = false;
    }
    public override void Hiding()
    {
        base.Hiding();
        //if (GL_Game._instance.GameState == EGameState.GameMain)
        //    // GL_AD_Interface._instance.CloseBannerAd();
        //    GL_AD_Interface._instance.CloseNativeAd();
    }

    public override void onUpdate()
    {

    }

    private float _nextGetOpenRed;
    /// <summary>
    /// 打开红包
    /// </summary>
    private void OnBtnOpenRedPack()
    {
        //----------------------- 按键间隔

        if (_nextGetOpenRed <= 0)
        {
            _nextGetOpenRed = Time.time;
        }
        else
        {
            if (GL_PlayerData._instance.ClickBtn(_nextGetOpenRed, GL_PlayerData._instance.CbPlayAd.waitTime))
            {
                _nextGetOpenRed = Time.time;
                // DDebug.LogError("***** 当前按键生效！");
            }
            else
            {
                SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage(
                    "冷却中，请稍等"
                );
                // DDebug.LogError("***** 当前按键冷却中...");
                return;
            }
        }

        //播放打开音效
        GL_AudioPlayback._instance.Play(8);

        if (_isIgnoreAD)
        {
            //DDebug.LogError("~~~CloseUIForm");
            ADCallback(true);
        }
        else
        {
            if (_rewardSource == ERewardSource.Guide)
            {
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.NewPlaySign);
                GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_OpenRedPack, (set) =>
                {
                    ADCallback(true);
                });
            }
            else if (_rewardSource == ERewardSource.OpenRed)
            {
                GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_OpenRedPack, (set) =>
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType. ActivityAnswerRedGetSuccess);
                    ADCallback(true);
                });
            }
        }
    }

    private void ADCallback(bool isSuccess)
    {
        CloseUIForm();
        if (isSuccess)
            _rewardCallBack?.Invoke(true);
        _rewardCallBack = null;
    }

    /// <summary>
    /// 关闭
    /// </summary>
    private void OnBtnClose()
    {
        // if (IsPlayAd(false))
        // {
        //     GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_Reright, (set) =>
        //     {
        //         DoClose();
        //         if (set)
        //             IsPlayAd(true);
        //     });
        // }
        // else
        // {
        //     DoClose();
        // }


        // //需求只在主页才弹出插屏
        // if (_sdkAD != null && !_isPlayVideo && GL_Game._instance.GameState == EGameState.Playing)
        //     _sdkAD.PlayAD(GL_AD_Interface.AD_Interstitial_OpenRedBack,180);

        
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActivityAnswerRedCloseGet);
        
        CloseUIForm();
    }

    private void DoClose()
    {
        CloseUIForm();
        _rewardCallBack?.Invoke(false);
    }



}
