using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_ChangeSignType : BaseUIForm
{
    private string _valueContent = "<size=92>￥</size>{0}";

    private Text _getMoney;
    private Text _userName;

    private Text withDraw1up;
    private Text withDraw1down;
    private Text withDraw2up;
    private Text withDraw2down;
    private Text withDraw3up;
    private Text withDraw3down;

    private Action _callback;
    
    public override void Init()
    {
        
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        
        _getMoney = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "GetMoney");
        _userName = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "UserName");

        Button btn1 = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Btn_01");
        withDraw1up = UnityHelper.GetTheChildNodeComponetScripts<Text>(btn1.transform.parent.gameObject, "Text");
        withDraw1down = UnityHelper.GetTheChildNodeComponetScripts<Text>(btn1.transform.parent.gameObject, "Text (1)");

        Button btn2 = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Btn_02");
        withDraw2up = UnityHelper.GetTheChildNodeComponetScripts<Text>(btn2.transform.parent.gameObject, "Text");
        withDraw2down = UnityHelper.GetTheChildNodeComponetScripts<Text>(btn2.transform.parent.gameObject, "Text (1)");

        Button btn3 = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Btn_03");
        withDraw3up = UnityHelper.GetTheChildNodeComponetScripts<Text>(btn3.transform.parent.gameObject, "Text");
        withDraw3down = UnityHelper.GetTheChildNodeComponetScripts<Text>(btn3.transform.parent.gameObject, "Text (1)");
        RigisterButtonObjectEvent("ExitPage", go =>
        {
            CloseUIForm();
            _callback?.Invoke();
            _callback = null;
        });
        RigisterButtonObjectEvent(btn1, go =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Change7);
            ChangeReward(ENewbieSignState.Model1);
        });
        RigisterButtonObjectEvent(btn2, go =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Change14);
            ChangeReward(ENewbieSignState.Model7);
        });
        RigisterButtonObjectEvent(btn3, go =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Change360);
            ChangeReward(ENewbieSignState.Model360);
        });
    }
    public override void Refresh(bool recall)
    {
        _getMoney.text = string.Format(_valueContent, GL_NewbieSign._instance._gamecoreConfig.rewards[0].num/100f);
        _userName.text = GL_PlayerData._instance.WeChatName;

        var table = GameDataTable.GetTableNewbieSignData(1);
        withDraw1up.text = string.Format(table.Contents[0], table.TotalDays);
        withDraw1down.text = string.Format(table.Contents[1], table.TotalDays);

        table = GameDataTable.GetTableNewbieSignData(2);
        withDraw2up.text = string.Format(table.Contents[0], table.TotalDays);
        withDraw2down.text = string.Format(table.Contents[1], table.TotalDays);

        table = GameDataTable.GetTableNewbieSignData(3);
        withDraw3up.text = string.Format(table.Contents[0], table.TotalDays);
        withDraw3down.text = string.Format(table.Contents[1], table.TotalDays);
    }

    public override void onUpdate()
    {
        
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

    private void ChangeReward(ENewbieSignState state)
    {
        GL_NewbieSign._instance.SelectState(state);
        object[] datas = { _callback };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NewbieSign,datas);
        CloseUIForm();
    }

}
