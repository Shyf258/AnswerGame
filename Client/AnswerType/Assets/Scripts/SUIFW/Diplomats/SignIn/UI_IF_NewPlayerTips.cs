using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Fly;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class UI_IF_NewPlayerTips : BaseUIForm
{
    private string _description = "恭喜成功获得新人奖金{0}金币,\n可以立即提现{1}元";

    private Text _descriptionText;
    
    /// <summary>
    /// 新人奖励配置
    /// </summary>
    private Net_CB_GamecoreAccept _gamecoreAccept = new Net_CB_GamecoreAccept();
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        
        _descriptionText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "NewPLayerTips");
        RigisterButtonObjectEvent("Btn_Sign",go=>{
            CloseUIForm();
        });
        
        RigisterButtonObjectEvent("Btn_Back", go =>
        {
            CloseUIForm();
        });
    }

    public override void InitData(object data)
    {
        base.InitData(data);

        var datas = data as Object[];
        if (datas == null) 
            return;
        if (datas.Length>0 && datas[0] is Net_CB_GamecoreAccept accept )
        {
            _gamecoreAccept = new Net_CB_GamecoreAccept();
            _gamecoreAccept = accept;
            _descriptionText.text = String.Format(_description, _gamecoreAccept.rewards[0].num,( _gamecoreAccept.rewards[0].num/100000f).ToString("0.00"));
        }
        
    }

    public override void Refresh(bool recall)
    {
        
    }

    public override void onUpdate()
    {
       
    }

    public override void OnHide()
    {
        base.OnHide();
        
        GL_PlayerData._instance.Coin += _gamecoreAccept.rewards[0].num;
        GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Coin));
        UI_HintMessage._.ShowMessage($"恭喜！获得{_gamecoreAccept.rewards[0].num}金币");
        
    }
}
