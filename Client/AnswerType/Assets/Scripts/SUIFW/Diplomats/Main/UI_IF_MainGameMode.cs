//2022.8.23 ����
//��ҳ���淨��س�ʼ��

using System;
using DG.Tweening;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public partial class UI_IF_Main
{

    #region �����淨

    private Transform _imageMode;   //ͼƬ��Ŀ
    private Image _imImage;
    private Text _imText;

    private Transform _textMode;    //������Ŀ
    private Text _tmText;

    /// <summary>
    /// ѡ�񰴼���
    /// </summary>
    private Transform _choiceGroup;
    private Button _btnA;
    private Text _btnAText;
    private Button _btnB;
    private Text _btnBText;

    // /// <summary>
    // /// ��ǰ��Ŀ���
    // /// </summary>
    private Text _nowAnswer;

    #endregion


    #region ��ʾ��

    private Transform _showAnswer;
    private Transform _showRight;
    private Transform _showWrong;

    #endregion

    protected void InitGameMode()
    {

        #region ��Ϸ�淨��ʼ��
        _nowAnswer = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "UserLevel");
        _imageMode = UnityHelper.FindTheChildNode(gameObject, "ImageMode");
        _imImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(_imageMode.gameObject, "IM_Image");
        _imText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_imageMode.gameObject, "IM_Text");


        _textMode = UnityHelper.FindTheChildNode(gameObject, "TextMode");
        _tmText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_textMode.gameObject, "TM_Text");


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
        
        //答案显示
        _showAnswer = UnityHelper.FindTheChildNode(_choiceGroup.gameObject, "ShowAnswer");
        _showRight = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowRight");
        _showWrong = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowWrong");
        
        #region 主页奖励玩法

        
        //财神
        _moneyPool = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "MoneyPool");
        RigisterButtonObjectEvent(_moneyPool, gp =>
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.MoneyPoolIcon);
            UI_Diplomats._instance.ShowUI(SysDefine.UI_IF_MoneyPool);
        });

        //大生产
        // _productionPageToggle = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "ProductionPageToggle");
        // RigisterButtonObjectEvent(_productionPageToggle, go =>
        // {
        //     if (!GL_PlayerData._instance.IsLoginWeChat())
        //     {
        //         //登陆微信
        //         // Action show =()=> PlayerIcon();
        //         Action show = () => { ChangeProduce(); };
        //         UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin, show);
        //         // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Setting);
        //     }
        //     else
        //     {
        //         ChangeProduce();
        //     }
        // });

        // _btnNewbieSign = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "NewbieSign");
        // _textNewbieSign = UnityHelper.GetTheChildNodeComponetScripts<Text>(_btnNewbieSign.gameObject, "Text");
        // RigisterButtonObjectEvent(_btnNewbieSign, (go => { OnClickNewbieSign(); }));
        
        #region 提现增幅

        _signDay = UnityHelper.GetTheChildNodeComponetScripts<Button>(_answerPageShow.gameObject, "SignDay");

        _day = UnityHelper.GetTheChildNodeComponetScripts<Text>(_signDay.gameObject, "Day");

        _dayGrow = UnityHelper.GetTheChildNodeComponetScripts<Text>(_signDay.gameObject, "Grow");

        RigisterButtonObjectEvent(_signDay, go =>
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_DayGrow);
        });
            
        #endregion

        #endregion
        
        
        #endregion


    }

    #region �����淨
    //ˢ����Ŀ
    public void RefreshGameMode(EventParam param)
    {

        // MoveBack();
        var info = GL_SceneManager._instance.CurGameMode._levelInfo;
        if (info == null)
            return;
        Sprite s;
        if (!string.IsNullOrEmpty(info.Picture))
        {
            //var sa = GL_LoadAssetMgr._instance.LoadAB<Texture2D>(GL_ConstData.LevelImagePath, info.Picture);
            //s = Sprite.Create(sa, new Rect(0, 0, sa.width, sa.height), Vector2.zero); 

            //s = GL_LoadAssetMgr._instance.Load<Sprite>(GL_ConstData.LevelImagePath + info.Picture);

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

                    _imImage.sprite = s;
                    _imImage.SetNativeSize();
                    _imText.text = info.TitleText;
                }
            });
        }
        else
        {
            //������Ŀ
            _imageMode.gameObject.SetActive(false);
            _textMode.gameObject.SetActive(true);

            _tmText.text = info.TitleText;
        }

        //ˢ��ѡ��
        _btnAText.text = info.Select1;
        _btnBText.text = info.Select2;

        GL_SceneManager._instance._levelAudio.PlayAudio(info.ID);

        _nowAnswer.text = string.Format(_answerCount, GL_PlayerData._instance.CurLevel);
    }
    private void OnClickChoice(int index)
    {
        GL_SceneManager._instance.CurGameMode.UI_Choice(index);
        GL_SceneManager._instance._levelAudio.StopAudio();
        // MoveChoiceGroup(false);
    }


    private Transform ChoiceBtn;
    private Transform _showResult;
    /// <summary>
    /// ���չʾ
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
    /// �ش���ȷ
    /// </summary>
    private void RightAnswer()
    {
        // // _redAward.Play("an_shake_01");
        //  ChangeRightCount();
        //   //������������
        //   GL_CoreData._instance.SaveData();
    }


    private string _answerCount = "第<color=#f58c3e>{0}</color>关";

    public void CloseBar()
    {
        // UnityHelper.FindTheChildNode(gameObject,"LeftBottom").SetActive(false);
        // _growBtn.SetActive(false);
    }
    #endregion
}