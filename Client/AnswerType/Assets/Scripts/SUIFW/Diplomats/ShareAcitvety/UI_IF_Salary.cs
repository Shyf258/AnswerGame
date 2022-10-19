using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_Salary :BaseUIForm
{
    public GameObject _item;
    //
    // private Toggle _my;
    //
    // private Toggle _friend;

    private Transform _content;
    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        
        _content = UnityHelper.FindTheChildNode(gameObject, "Content");
        
        
        // _my = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(gameObject, "My");
        // _my.onValueChanged.AddListener(show =>
        // {
        //     if (show)
        //     {
        //         ShowMy();
        //     }
        //    
        // });
        //
        // _friend = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(gameObject, "Friend");
        // _friend.onValueChanged.AddListener(show =>
        // {
        //     if (show)
        //     {
        //         ShowFriend();
        //     }
        //    
        // });
        
        RigisterButtonObjectEvent("BtnClose", go =>
        {
            CloseUIForm();
        });
    }

    public override void Display(bool redisplay = false)
    {
        base.Display(redisplay);
       
    }

    public override void BeforeDisplay()
    {
        base.BeforeDisplay();
        // _my.isOn = true;
        // _friend.isOn = false;
       
    }

    public override void RefreshLanguage()
    {
    
    }

    public override void Refresh(bool recall)
    {
        // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_GoldenPig);
        ShowFriend();
    }

    public override void OnHide()
    {
        base.OnHide();
        // GL_AD_Interface._instance.CloseBannerAd();
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_DragRedpack);
    }

    public override void onUpdate()
    {
        
    }

    private List<GameObject> _listPayslip;
    private List<GameObject> _listRank;

    // private void ShowMy()
    // {
    //    GL_Tools.RefreshItem(GL_PlayerData._instance.NetCbWithDrawHis.withDraws,_content,_item,_listPayslip);
    // }

    private void ShowFriend()
    {
        GL_Tools.RefreshItem(GL_PlayerData._instance.NetCbProduceRanking.withDraws,_content,_item,_listRank);
    }

}
