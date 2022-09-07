using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataModule;
using DG.Tweening;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_IF_PureGame : BaseUIForm
{

  
    
    private Transform _pause;

    private Button _pauseBtn;
    
    private VideoPlayer _videoPlayer;
    
    private Text _tmText;   //题目
    
    /// <summary>
    /// 选择按键组
    /// </summary>
    private Transform _choiceGroup;
    private Button _btnA;
    private Image _imageA;
    private Text _btnAText;
    private Button _btnB;
    private Image _imageB;
    private Text _btnBText;
    
    /// <summary>
    /// 0.待选择 1.正确  2.错误
    /// </summary>
    public List<Sprite> _btnIcon;
    
    private Image _chanceBtn;
    
    private Button _volume;
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


        ParseTable();
        
        _choiceGroup = UnityHelper.FindTheChildNode(gameObject, "ChoiceBtnGroup");
        _tmText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_choiceGroup.gameObject, "TM_Text");
        _btnA = UnityHelper.GetTheChildNodeComponetScripts<Button>(_choiceGroup.gameObject, "ChoiceBtn1");
        _imageA = UnityHelper.GetTheChildNodeComponetScripts<Image>(_choiceGroup.gameObject, "ChoiceBtn1");
        _btnAText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnA.gameObject, "ChoiceText1");
        _btnB = UnityHelper.GetTheChildNodeComponetScripts<Button>(_choiceGroup.gameObject, "ChoiceBtn2");
        _imageB = UnityHelper.GetTheChildNodeComponetScripts<Image>(_choiceGroup.gameObject, "ChoiceBtn2");
        _btnBText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnB.gameObject, "ChoiceText2");
        
        RigisterButtonObjectEvent(_btnA, (go) =>
        {
            _chanceBtn = _imageA;
            OnClickChoice(1);
        });
        RigisterButtonObjectEvent(_btnB, (go) =>
        {
            _chanceBtn = _imageB;
            OnClickChoice(2);
        });
        
        RigisterButtonObjectEvent("Setting", go =>
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Setting);
        });
        _videoPlayer = UnityHelper.GetTheChildNodeComponetScripts<VideoPlayer>(gameObject, "Video");
        //暂停
        _pause = UnityHelper.FindTheChildNode(gameObject, "Pause");
        _pauseBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Pause");
        RigisterButtonObjectEvent(_pauseBtn, go =>
        {
            _pause.SetActive(false);
            _videoPlayer.Play();
        });
        
        _tmText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "TM_Text");
        
        Button _playBtn;
        _playBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Video");
        RigisterButtonObjectEvent(_playBtn, go =>
        {
            _pause.SetActive(true);
            _videoPlayer.Pause();
        });
        
        //暂停
        _pause = UnityHelper.FindTheChildNode(gameObject, "Pause");
        _pauseBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Pause");
        RigisterButtonObjectEvent(_pauseBtn, go =>
        {
            _pause.SetActive(false);
            _videoPlayer.Play();
        });
        _volume = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "Volume");
        RigisterButtonObjectEvent(_volume, go =>
        {
            
            GL_CoreData._instance.VideoVolume = !GL_CoreData._instance.VideoVolume;
            VideoVolume(GL_CoreData._instance.VideoVolume);
        });

        //旋转
        VideoVolume(GL_CoreData._instance.VideoVolume);
        _videoPlayer.loopPointReached += VideoFinish;

    }
    



 
    private string PATH = "https://static.ciyunjinchan.com/Unity/Video/Short/";
    private TableAnswerInfoData _info;
    public void RefreshAnswerMode(EventParam param)
    {
        CreateLevel(GL_PlayerData._instance.AnswerCount);
        if (_info == null)
            return;
        _pause.SetActive(false);
        //刷新视频
        _videoPlayer.url = PATH + _info.Picture +".mp4";
        _videoPlayer.Play();

        _tmText.text = _info.TitleText;
        
        //刷新选项
        _btnAText.text = _info.Select1;
        _btnBText.text = _info.Select2;

        // GL_SceneManager._instance._levelAudio.PlayAudio(info.ID);

       
    }
    public List<TableAnswerInfoData> _levelList;
    private void ParseTable()
    { 
        _levelList = DataModuleManager._instance.TableAnswerInfoData_Dictionary.Values.ToList();
    }
    /// <summary>
    /// 创建关卡
    /// </summary>
    /// <param name="levelIndex"></param>
    public void CreateLevel(int levelIndex) 
    {
        //DDebug.LogError("读取关卡："+GL_PlayerData._instance.AnswerCount);
        //创建新关卡
        if (levelIndex > _levelList.Count)
        {
            //1-500
            levelIndex = UnityEngine.Random.Range(0, _levelList.Count);
        }
        else
        {
         
        }
        _info = _levelList[levelIndex];
        
    }
    
    private void OnClickChoice(int index)
    {
        //DDebug.LogError("回答问题" + _levelInfo.CorrectAnswer);
        if (_info == null)
            return;
        if (_info.CorrectAnswer.Contains(index))
        {
            UI_HintMessage._.ShowMessage("回答正确");
            _chanceBtn.sprite = _btnIcon[1];
        }
        else
        {
            UI_HintMessage._.ShowMessage("回答错误");
            _chanceBtn.sprite = _btnIcon[2];
        }

        GL_PlayerData._instance.AnswerCount++;
        Invoke("FreshGame",1f);
    }
    /// <summary>
    /// 按键恢复显示
    /// </summary>
    public void MoveBack()
    {
        _chanceBtn.sprite = _btnIcon[0];
    }

    private void FreshGame()
    {
        MoveBack();
        RefreshAnswerMode(null);
    }
    
    /// <summary>
    /// 完成播放
    /// </summary>
    /// <param name="videoSource"></param>
    private void VideoFinish(VideoPlayer videoSource)
    {
        DDebug.LogError("完成播放。");
        _pause.SetActive(true);
    }
    
    /// <summary>
    /// 音频控制
    /// </summary>
    /// <param name="set"></param>
    private void VideoVolume(bool set)
    {
        if (set)
        {
            //打开声音
            _videoPlayer.SetDirectAudioVolume(0,1);
            _volume.transform.DORotate(new Vector3(0, 0, -36000), 400f, RotateMode.LocalAxisAdd).SetLoops(-1,LoopType.Restart);
        }
        else
        {
            //关闭声音
            _videoPlayer.SetDirectAudioVolume(0,0);
            _volume.transform.DOPause();
        }
    }
}
