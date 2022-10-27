using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataModule;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_PureGame : BaseUIForm
{
    // public TableAnswerInfoData _levelInfo; //关卡信息
    // public List<TableAnswerInfoData> _levelList;
    //
    // private Transform _dateGroup; //日期显示
    // private Button _setting;
    //
    // private Transform _imageMode;   //图片题目
    // private Image _imImage;
    // private Text _imText;
    //
    // private Transform _textMode;    //文字题目
    // private Text _tmText;
    //
    // private Button _btnA;
    // private Text _btnAText;
    // private Button _btnB;
    // private Text _btnBText;
    //
    // private Toggle _answerPage;
    // private Toggle _taskPage;

    private Text _level;
    private Text _textHint;
    public override void RefreshLanguage()
    {
 
    }

    public override void Refresh(bool recall)
    {
        GL_SceneManager._instance.Init();
        RefreshAnswerMode(null);
        
    }

    public override void onUpdate()
    {
      
    }

    public override void OnHide()
    {
        base.OnHide();
        GL_GameEvent._instance.UnregisterEvent(EEventID.RefreshGameMode, RefreshAnswerMode);
    }

    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.Normal;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
        GL_GameEvent._instance.RegisterEvent(EEventID.RefreshGameMode, RefreshAnswerMode);
        
        _isFullScreen = false;

        _level = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Level");
       RigisterButtonObjectEvent("Replay", go =>
       {
           OnClickReplay();
       });
       RigisterButtonObjectEvent("Hint", go =>
       {
           OnClickHint();
       });
       _textHint = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "NumHint");
    }
    



    public void FreshLevel()
    {
        _level.text = string.Format("第{0}关", GL_PlayerData._instance.CurLevel);
    }


    public void RefreshAnswerMode(EventParam param)
    {
        //GL_TileRegion._instance.Load();
        FreshLevel();
        _textHint.text = GL_CoreData._instance.TipsCount.ToString();
    }
    
    private void OnClickReplay()
    {
        GL_SceneManager._instance.LevelReplay();
    }

    private void OnClickHint()
    {
        //先检测次数, 扣除次数
        if (GL_CoreData._instance.TipsCount>0)
        {
            GL_CoreData._instance.TipsCount--;
            GL_SceneManager._instance.LevelHint();
            _textHint.text = GL_CoreData._instance.TipsCount.ToString();
        }
        else
        {
            UI_HintMessage._.ShowMessage("提示次数不足");
        }
    }

}
