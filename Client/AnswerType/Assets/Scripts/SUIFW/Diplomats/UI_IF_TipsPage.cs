using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class UI_IF_TipsPage : BaseUIForm
{

    private bool _exit;
    
    private int _times;
    private Text _description;
    private bool _close;
    private List<string> _listString = new List<string>()
    {
        "[防沉迷系统]您今日的游戏时间已经登录{0}分钟，请合理安排游戏时间，劳逸结合。",
        "[防沉迷系统]您今日的游戏时间已经超过{0}分钟，根据防沉迷系统规则，您今日将无法登录游戏，请合理安排游戏时间。"
    };
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;

        _description = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Description");
        
        RigisterButtonObjectEvent("Btn", go =>
        {
            ClosePage();
        });
    }
    
    public override void RefreshLanguage()
    {
      
    }

    public override void Refresh(bool recall)
    {
      
    }

    public override void onUpdate()
    {
       
    }


    
    public override void InitData(object data)
    {
        base.InitData(data);
        var datas = data as Object[];
        if (datas == null) 
            return;

        if (datas.Length>0 && datas[0] is int value)
        {
            _times = value;
        }
        if (datas.Length>1 && datas[1] is bool exit)
        {
            _exit = exit;
        }
        if (_exit)
        {
            _close = true;
            _description.text = string.Format(_listString[0], _times);
        }
        else
        {
            _close = false;
            _description.text = string.Format(_listString[1], 90);
        }
    }

    private void ClosePage()
    {
        if (_close)
        {
            CloseUIForm();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
    }


}
