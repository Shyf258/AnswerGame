//2022.8.23 ����
//��ҳ���淨��س�ʼ��

using System.Collections.Generic;
using DG.Tweening;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public partial class UI_IF_Main
{

    #region �����淨

    //private Transform _imageMode;   //ͼƬ��Ŀ
    //private Image _imImage;
    //private Text _imText;

    //private Transform _textMode;    //������Ŀ
    
    
    private Text _tmText;   //��Ŀ

    /// <summary>
    /// ѡ�񰴼���
    /// </summary>
    private Transform _choiceGroup;
    private Button _btnA;
    private Image _imageA;
    private Text _btnAText;
    private Button _btnB;
    private Image _imageB;
    private Text _btnBText;

    // /// <summary>
    // /// ��ǰ��Ŀ���
    // /// </summary>
    private Text _nowAnswer;

    #endregion


    #region ��ʾ��

    // private Transform _showAnswer;
    // private Transform _showRight;
    // private Transform _showWrong;

    /// <summary>
    /// 0.��ѡ�� 1.��ȷ  2.����
    /// </summary>
    public List<Sprite> _btnIcon;
    
    #endregion

    #region ��ͣ

    private Transform _pause;

    private Button _pauseBtn;
    
    #endregion
    
    private Button _volume;

    public Transform _rotate;
    
    private VideoPlayer _videoPlayer;
    protected void InitGameMode()
    {

        #region ��Ϸ�淨��ʼ��
        _nowAnswer = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "UserLevel");

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
        //��ʾ��
        // _showAnswer = UnityHelper.FindTheChildNode(_choiceGroup.gameObject, "ShowAnswer");
        // _showRight = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowRight");
        // _showWrong = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowWrong");

        _videoPlayer = UnityHelper.GetTheChildNodeComponetScripts<VideoPlayer>(_answerPageShow.gameObject, "Video");


        //����
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


        _volume = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "Volume");
        
        RigisterButtonObjectEvent(_volume, go =>
        {
            
            GL_CoreData._instance.VideoVolume = !GL_CoreData._instance.VideoVolume;
            VideoVolume(GL_CoreData._instance.VideoVolume);
        });

        //��ת
        _videoPlayer.loopPointReached += VideoFinish;

        Button _playBtn;
        _playBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "Video");
        RigisterButtonObjectEvent(_playBtn, go =>
        {
            _pause.SetActive(true);
            _videoPlayer.Pause();
        });
        
        //��ͣ
        _pause = UnityHelper.FindTheChildNode(_answerPageShow.gameObject, "Pause");
        _pauseBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "Pause");
        RigisterButtonObjectEvent(_pauseBtn, go =>
        {
            _pause.SetActive(false);
            _videoPlayer.Play();
        });
    }

    private string PATH = "https://static.ciyunjinchan.com/Unity/Video/Short/";
    #region �����淨
    //ˢ����Ŀ
    public void RefreshGameMode(EventParam param)
    {
        _pause.SetActive(false);
        // MoveBack();
        var info = GL_SceneManager._instance.CurGameMode._levelInfo;
        if (info == null)
            return;
        //ˢ����Ƶ
        _videoPlayer.url = PATH + info.Picture /*+".mp4"*/;
        _videoPlayer.Play();

        _tmText.text = info.TitleText;
        
        //ˢ��ѡ��
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


    /// <summary>
    /// ���չʾ
    /// </summary>
    public void ShowAnswer(bool choice)
    {
        if (choice)
        {
            _chanceBtn.sprite = _btnIcon[1];
        }
        else
        {
            _chanceBtn.sprite = _btnIcon[2];
        }
        answer = choice;
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        InvokeRepeating("GetResult", 0.3f, 0);
    }

    /// <summary>
    /// �����ָ���ʾ
    /// </summary>
    public void MoveBack()
    {
        _chanceBtn.sprite = _btnIcon[0];
    }

    private Image _chanceBtn;
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
    /// �ش���ȷ
    /// </summary>
    private void RightAnswer()
    {
        // // _redAward.Play("an_shake_01");
        //  ChangeRightCount();
        //   //������������
        //   GL_CoreData._instance.SaveData();
    }


    private string _answerCount = "��<color=#f58c3e>{0}</color>��";
    private string _question = "�������<color=#CF0400>{0}</color>�⣬���� <color=#CF0400>�齱</color>Ӵ";

    public void CloseBar()
    {
        // UnityHelper.FindTheChildNode(gameObject,"LeftBottom").SetActive(false);
        // _growBtn.SetActive(false);
    }
    #endregion
    
    //��������
    private void VideoVolume(bool set)
    {
        if (set)
        {
            //������
             _videoPlayer.SetDirectAudioVolume(0,1);
        }
        else
        {
            //�ر�����
            _videoPlayer.SetDirectAudioVolume(0,0);
        }
    }

    private void VideoFinish(VideoPlayer videoSource)
    {
        DDebug.LogError("��ɲ��š�");
        _pause.SetActive(true);
    }
}