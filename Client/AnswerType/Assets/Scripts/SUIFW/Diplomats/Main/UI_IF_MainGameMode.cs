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

    /// <summary>
    /// ��Ƶģʽ
    /// </summary>
    private Transform _playVideo;

    /// <summary>
    /// ����ģʽ
    /// </summary>
    private Transform _answerMode;

    private Transform _imageMode;
    private Image _imageTopic;
    private Text _imageDescription;

    private Transform _textMode;
    private Text _answerDescription;
    
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
        VideoVolume(GL_CoreData._instance.VideoVolume);
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


        _playVideo = UnityHelper.FindTheChildNode(_answerPageShow.gameObject, "PlayVideo");
        _answerMode = UnityHelper.FindTheChildNode(_answerPageShow.gameObject, "AnswerMode");

        _imageMode = UnityHelper.FindTheChildNode(_answerMode.gameObject, "PicType");
        _imageTopic = UnityHelper.GetTheChildNodeComponetScripts<Image>(_answerMode.gameObject, "ImageTopic");
        _imageDescription = UnityHelper.GetTheChildNodeComponetScripts<Text>(_answerMode.gameObject, "ImageDescription");
        
        _textMode = UnityHelper.FindTheChildNode(_answerMode.gameObject, "AnswerType");
        _answerDescription =
            UnityHelper.GetTheChildNodeComponetScripts<Text>(_answerMode.gameObject, "AnswerDescription");

    }

    private string PATH = "https://static.ciyunjinchan.com/Unity/Video/Short/";

    private string PicPath = "https://static.ciyunjinchan.com/Unity/LifeWinnerGold/";
    #region �����淨
    //ˢ����Ŀ
    public void RefreshGameMode(EventParam param)
    {
        _choiceGroup.SetActive(false);
        _pause.SetActive(false);
        // MoveBack();
        var info = GL_SceneManager._instance.CurGameMode._levelInfo;
        if (info == null)
            return;

        if (info.Type == 1)
        {
            //ˢ����Ƶ��
            
            _answerMode.SetActive(false);
            _playVideo.SetActive(true);
            //ˢ����Ƶ
            _videoPlayer.url = PATH + info.Picture +".mp4";
            _videoPlayer.Play();

            _tmText.text = info.TitleText;
        
            //ˢ��ѡ��
            _btnAText.text = info.Select1;
            _btnBText.text = info.Select2;

            // GL_SceneManager._instance._levelAudio.PlayAudio(info.ID);

            // _nowAnswer.text = string.Format(_answerCount, GL_PlayerData._instance.CurLevel);
        }
        else
        {
            //ˢ��ͼƬ��
            _answerMode.SetActive(true);
            _playVideo.SetActive(false);


            if (string.IsNullOrEmpty(info.Picture))
            {
                _imageMode.SetActive(false);
                _textMode.SetActive(true);
                
                Sprite s;
                string path = DownloadConfig.downLoadPath + string.Format(GL_VersionManager.PictureUrl, info.Picture);
#if UNITY_ANDROID && !UNITY_EDITOR || UNITY_EDITOR_OSX
        path = "file://" + path;
#endif
                GL_ServerCommunication._instance.GetTexturePic(path, (t) =>
                {
                    if (t == null)
                    {
                        _imageMode.gameObject.SetActive(false);
                        _textMode.gameObject.SetActive(true);

                        _tmText.text = info.TitleText;
                    }
                    else
                    {
                        s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);

                        //ͼƬ��Ŀ
                        _imageMode.gameObject.SetActive(true);
                        _textMode.gameObject.SetActive(false);

                        _imageTopic.sprite = s;
                        _imageTopic.SetNativeSize();
                        _imageDescription.text = info.TitleText;
                    }
                    
                    GL_SceneManager._instance._levelAudio.PlayAudio(info.ID);
                });
            }
            else
            {
                _imageMode.SetActive(true);
                _textMode.SetActive(false);
                _answerDescription.text = info.TitleText;
            }
            
        }
     
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
             _volume.transform.DORotate(new Vector3(0, 0, -36000), 400f, RotateMode.LocalAxisAdd).SetLoops(-1,LoopType.Restart);
        }
        else
        {
            //�ر�����
            _videoPlayer.SetDirectAudioVolume(0,0);
            _volume.transform.DOPause();
        }
    }

    private void VideoFinish(VideoPlayer videoSource)
    {
        DDebug.LogError("��ɲ��š�");
        _pause.SetActive(true);
        _choiceGroup.SetActive(true);
    }
}