//2022.8.23 管理
//主页面玩法相关初始化

using DG.Tweening;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public partial class UI_IF_Main
{

    #region 答题玩法

    //private Transform _imageMode;   //图片题目
    //private Image _imImage;
    //private Text _imText;

    //private Transform _textMode;    //文字题目
    //private Text _tmText;

    /// <summary>
    /// 选择按键组
    /// </summary>
    private Transform _choiceGroup;
    private Button _btnA;
    private Text _btnAText;
    private Button _btnB;
    private Text _btnBText;

    // /// <summary>
    // /// 当前题目序号
    // /// </summary>
    private Text _nowAnswer;

    #endregion


    #region 显示答案

    private Transform _showAnswer;
    private Transform _showRight;
    private Transform _showWrong;

    #endregion

    private Toggle _volume;
    
    private VideoPlayer _videoPlayer;
    protected void InitGameMode()
    {

        #region 游戏玩法初始化
        _nowAnswer = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "UserLevel");

        _choiceGroup = UnityHelper.FindTheChildNode(gameObject, "ChoiceBtnGroup");
        _btnA = UnityHelper.GetTheChildNodeComponetScripts<Button>(_choiceGroup.gameObject, "ChoiceBtn1");
        _btnAText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnA.gameObject, "ChoiceText1");
        _btnB = UnityHelper.GetTheChildNodeComponetScripts<Button>(_choiceGroup.gameObject, "ChoiceBtn2");
        _btnBText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnB.gameObject, "ChoiceText2");

        RigisterButtonObjectEvent(_btnA, (go) =>
        {
            ChoiceBtn = _btnA.transform;
            OnClickChoice(1);

        });
        RigisterButtonObjectEvent(_btnB, (go) =>
        {
            ChoiceBtn = _btnB.transform;
            OnClickChoice(2);

        });
        //显示答案
        _showAnswer = UnityHelper.FindTheChildNode(_choiceGroup.gameObject, "ShowAnswer");
        _showRight = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowRight");
        _showWrong = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowWrong");

        _videoPlayer = UnityHelper.GetTheChildNodeComponetScripts<VideoPlayer>(_answerPageShow.gameObject, "Video");


        //财神
        _moneyPool = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "MoneyPool");
        RigisterButtonObjectEvent(_moneyPool, gp =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.MoneyPoolIcon);
            UI_Diplomats._instance.ShowUI(SysDefine.UI_IF_MoneyPool);
        });


        _btnNewbieSign = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "NewbieSign");
        _textNewbieSign = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnNewbieSign.gameObject, "Text");
        RigisterButtonObjectEvent(_btnNewbieSign, (go => { OnClickNewbieSign(); }));

        #endregion


        _volume = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(_answerPageShow.gameObject, "Volume");
        _volume.onValueChanged.AddListener(set =>
        {
            GL_CoreData._instance.VideoVolume = set;
           VideoVolume(set);
        });

        _volume.isOn = GL_CoreData._instance.VideoVolume;
        _videoPlayer.loopPointReached += VideoFinish;

    }

    private string PATH = "https://static.ciyunjinchan.com/Unity/Video/Short/";
    #region 答题玩法
    //刷新题目
    public void RefreshGameMode(EventParam param)
    {

        // MoveBack();
        var info = GL_SceneManager._instance.CurGameMode._levelInfo;
        if (info == null)
            return;
        //刷新视频
        _videoPlayer.url = PATH + info.Picture;
        _videoPlayer.Play();

        //刷新选项
        _btnAText.text = info.Select1;
        _btnBText.text = info.Select2;

        // GL_SceneManager._instance._levelAudio.PlayAudio(info.ID);

        _nowAnswer.text = string.Format(_answerCount, GL_PlayerData._instance.CurLevel);
    }
    private void OnClickChoice(int index)
    {
        GL_SceneManager._instance.CurGameMode.UI_Choice(index);
        // GL_SceneManager._instance._levelAudio.StopAudio();
        // MoveChoiceGroup(false);
    }


    private Transform ChoiceBtn;
    private Transform _showResult;
    /// <summary>
    /// 结果展示
    /// </summary>
    public void ShowAnswer(bool choice)
    {
        if (choice)
        {
            _showResult = _showRight;
            // _showRight.SetParent(ChoiceBtn);
            // _showRight.localPosition=Vector3.zero;
            answer = true;
        }
        else
        {
            _showResult = _showWrong;
            answer = false;
        }
        _showResult.SetParent(ChoiceBtn);
        _showResult.localPosition = Vector3.zero;
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        InvokeRepeating("GetResult", 0.3f, 0);
    }

    public void MoveBack()
    {
        try
        {
            _showResult.SetParent(_showAnswer);
            _showResult.localPosition = Vector3.zero;
            _showResult.localScale = Vector3.one;
        }
        catch
        {

        }

    }

    private bool answer;
    private void GetResult()
    {
        UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
        if (answer)
        {
            GL_SceneManager._instance.CurGameMode.LevelState = GL_GameMode.ELevelState.SettleWait;
        }
        else
        {
            GL_SceneManager._instance.CurGameMode.LevelState = GL_GameMode.ELevelState.Fail;
        }

        CancelInvoke("GetResult");
    }

    public void MoveChoiceGroup(bool show)
    {
        _choiceGroup.localPosition = new Vector3(3 * Screen.width, -450, 0);
        _choiceGroup.DOLocalMove(new Vector3(0, -450, 0), 1, false);
    }



    /// <summary>
    /// 回答正确
    /// </summary>
    private void RightAnswer()
    {
        // // _redAward.Play("an_shake_01");
        //  ChangeRightCount();
        //   //发送升级请求
        //   GL_CoreData._instance.SaveData();
    }


    private string _answerCount = "第<color=#f58c3e>{0}</color>题";
    private string _question = "继续答对<color=#CF0400>{0}</color>题，即可 <color=#CF0400>抽奖</color>哟";

    public void CloseBar()
    {
        // UnityHelper.FindTheChildNode(gameObject,"LeftBottom").SetActive(false);
        // _growBtn.SetActive(false);
    }
    #endregion
    
    //声音开关
    private void VideoVolume(bool set)
    {
        if (set)
        {
             _videoPlayer.SetDirectAudioVolume(0,1);
        }
        else
        {
            _videoPlayer.SetDirectAudioVolume(0,0);
        }
    }

    private void VideoFinish(VideoPlayer videoSource)
    {
        DDebug.LogError("完成播放。");
    }
}