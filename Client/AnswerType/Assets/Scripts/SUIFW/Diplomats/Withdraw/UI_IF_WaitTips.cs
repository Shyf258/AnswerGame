using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class UI_IF_WaitTips : BaseUIForm
{

    private Text _descriptionText;

    private string _description = "再玩{0}天即可翻{1}倍，届时可提{2}元！您是否愿意损失即将获得的金额？";

    private Action<int> _action;
    
    
    
    
    public override void Refresh(bool recall)
    {
       
    }

    public override void onUpdate()
    {
       
    }

    public override void Init()
    {
        
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;
        _isOpenMainUp = false;

        
        
        _descriptionText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Description");
        
        RigisterButtonObjectEvent("BtnClose", go =>
        {
            CloseUIForm();
        });
        RigisterButtonObjectEvent("BtnGiveUp", go =>
        {
            CloseUIForm();
        });
        RigisterButtonObjectEvent("BtnSure", go =>
        {
            CloseUIForm();
            _action?.Invoke(2);
            _action = null;
        });
    }


    public override void InitData(object data)
    {
        base.InitData(data);
        
        var datas = data as Object[];
        if (datas == null) 
            return;
        
        //奖励
        if (datas.Length > 0 && datas[0] is WithDrawWaitConfig config)
        {
            int day = config.targetDayCount - GL_PlayerData._instance.BankConfig.nowDay;
            day = day > 0 ? day : 0;
            _descriptionText.text = string.Format(_description,
                day, config.multiple,
                (config.multiple * GL_PlayerData._instance.BankConfig.nowMoney));
           
        }

        if (datas.Length > 1 && datas[1] is Action<int> action)
        {
            _action = action;
        }
        
    }
}
