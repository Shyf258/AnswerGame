using System.Collections;
using System.Collections.Generic;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_DayGrow : BaseUIForm
{
    /// <summary>
    /// 奖池累计现金
    /// </summary>
    private Text _moneyPoolCount;
    /// <summary>
    /// 奖池刷新倒计时
    /// </summary>
    private Text _time;

    /// <summary>
    /// 提现增幅比例
    /// </summary>
    private Text _increase;
    /// <summary>
    /// 登录天数
    /// </summary>
    private Text _dayCount;
    /// <summary>
    /// 登录天数增幅
    /// </summary>
    private Text _dayPercent;
    /// <summary>
    /// 观看视频次数
    /// </summary>
    private Text _videoCount;
    /// <summary>
    /// 视频数增幅
    /// </summary>
    private Text _videoPercent;


    private Timer _timer;
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        
        RigisterButtonObjectEvent("ClosePage", go =>
        {
            CloseUIForm();
        });

        Transform Topframe = UnityHelper.FindTheChildNode(gameObject, "TopFrame");
        _moneyPoolCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(Topframe.gameObject, "MoneyPoolCount");
        _time = UnityHelper.GetTheChildNodeComponetScripts<Text>(Topframe.gameObject, "Time");
        
        Transform DescriptionFrame = UnityHelper.FindTheChildNode(gameObject, "DescriptionFrame");
        _increase = UnityHelper.GetTheChildNodeComponetScripts<Text>(DescriptionFrame.gameObject, "Increase");
        _dayCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(DescriptionFrame.gameObject, "DayCount");
        _dayPercent = UnityHelper.GetTheChildNodeComponetScripts<Text>(DescriptionFrame.gameObject, "PercentDay");
        _videoCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(DescriptionFrame.gameObject, "VideoCount");
        _videoPercent = UnityHelper.GetTheChildNodeComponetScripts<Text>(DescriptionFrame.gameObject, "PercentVideo");
        _timer = new Timer(this,true);

    }
    
    public override void Refresh(bool recall)
    {
        RefreshPage();
    }

    public override void onUpdate()
    {
        
    }


    private void ShowMessage()
    {
        _moneyPoolCount.text =
            $"<size=90><color=#fc0000>{(GL_PlayerData._instance._WithDrawGrowConfig.money / 100f).ToString("0")}</color></size>元";

        _increase.text = $"{GL_PlayerData._instance._WithDrawGrowConfig.growth}%";

        _dayCount.text = $"{GL_PlayerData._instance._WithDrawGrowConfig.day}天";

        _dayPercent.text = $"+{GL_PlayerData._instance._WithDrawGrowConfig.loginGrowth}%";

        _videoCount.text = $"{GL_PlayerData._instance._WithDrawGrowConfig.view}";
        
        _videoPercent.text = $"+{GL_PlayerData._instance._WithDrawGrowConfig.videoGrowth}%";

        _timer.StartCountdown(GL_PlayerData._instance._WithDrawGrowConfig.countDown, 0, 0, () =>
        {
           _timer.Stop();
            RefreshPage();
        },_time);
        
        if (GL_PlayerData._instance._WithDrawGrowConfig.money  == 0)
        {
            UI_HintMessage._.ShowMessage("距离下次红包额度刷新还剩下\n"+_time.text );
        }
    }

    private void RefreshPage()
    {
        GL_PlayerData._instance.GetWithDrawGrowConfig(()=>
        {
            ShowMessage();
        });
    }

}
