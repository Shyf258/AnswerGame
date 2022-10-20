using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_Production : BaseUIForm
{
    
    public UI_Obj_Production _ObjProduction;
    
    private Timer _time;
    private Text _timerText;
    public override void RefreshLanguage()
    {
       
    }

    public override void Refresh(bool recall)
    {
        FreshTime();
        if (GL_PlayerData._instance._PlayerCostState._costState == CostState.Low)
        {
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_Procduction);
        } 

        //因为需要排序, 所以延迟一会检测
        Invoke(nameof(TriggerGuide), 0.05f);
    }
    private void TriggerGuide()
    {
        GL_GuideManager._instance.TriggerGuide(EGuideTriggerType.UIProduction);
    }
    public override void OnHide()
    {
        base.OnHide(); 
        GL_AD_Interface._instance.CloseBannerAd();
    }

    public override void onUpdate()
    {
     
    }

    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        
        _timerText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Time_text");
        _ObjProduction = UnityHelper.GetTheChildNodeComponetScripts<UI_Obj_Production>(gameObject, "UI_Obj_Production");
        _ObjProduction.InitObjectNode();
        _time = new Timer(this,false);
        RigisterButtonObjectEvent("ExitPage",go=>{
            CloseUIForm();
        });
    }

    public override void BeforeDisplay()
    {
        base.BeforeDisplay();
        _ObjProduction.Refresh();
    }
    
    private void FreshTime()
    {
        _time.StartCountdown(GL_PlayerData._instance.ProduceTime % 60, (GL_PlayerData._instance.ProduceTime % 3600)/60, GL_PlayerData._instance.ProduceTime/ 3600, (() =>
        { 
            _time.Stop();
            GL_PlayerData._instance.GetProduceConfig((() =>
            {
                FreshTime();
                _ObjProduction.FreshText();
                GL_PlayerData._instance.GetProduceBarrage((() =>
                {
                    _ObjProduction._index = 0;
                    // _ObjProduction._barrageMove.Stop();
                    // _ObjProduction._barrageMove.Play("barrage_Move");
                    _ObjProduction.ChangeBarrage();
                }));
            }));
            //     GL_PlayerData._instance.GetProduceConfig(() =>
            //     {
            //         _ObjProduction.FreshText();
            //     });
            //     GL_PlayerData._instance.GetProduceBarrage((() =>
            //     {
            //         _index = 0;
            //         _ObjProduction.ChangeBarrage();
            //     }));
        }),_timerText);
    }
}
