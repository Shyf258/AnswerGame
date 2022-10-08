//2022.8.23 ����
//��ҳ���淨��س�ʼ��

using DataModule;
using DG.Tweening;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public partial class UI_IF_Main
{

    #region �����淨

    //��������
    private Text _guessFromExplainText;
    //ͼ
    private Image _guessFromExplainPic; 
    //��ʫ
    private Text _ancientPoetryCrosswordText;

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


    #region 对象

    private Transform _showAnswer;
    private Transform _showRight;
    private Transform _showWrong;

    #endregion

    protected void InitGameMode()
    {

        #region 主页玩法
        _nowAnswer = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "UserLevel");
        var gameArea = UnityHelper.FindTheChildNode(gameObject, "GameArea");
        var guessFromExplainRoot = UnityHelper.FindTheChildNode(gameArea.gameObject, "GuessFromExplainRoot");
        _guessFromExplainText = UnityHelper.GetTheChildNodeComponetScripts<Text>(guessFromExplainRoot.gameObject, "Text");
        _guessFromExplainPic = UnityHelper.GetTheChildNodeComponetScripts<Image>(guessFromExplainRoot.gameObject, "Pic");
        var ancientPoetryCrosswordRoot = UnityHelper.FindTheChildNode(gameArea.gameObject, "AncientPoetryCrosswordRoot");
        _ancientPoetryCrosswordText = UnityHelper.GetTheChildNodeComponetScripts<Text>(ancientPoetryCrosswordRoot.gameObject, "Text");


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
        //答题按键
        _showAnswer = UnityHelper.FindTheChildNode(_choiceGroup.gameObject, "ShowAnswer");
        _showRight = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowRight");
        _showWrong = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowWrong");

      

        #endregion


    }

    #region �����淨
    //ˢ����Ŀ
    public void RefreshGameMode(EventParam param)
    {
        var gameMode = GL_SceneManager._instance.CurGameMode;
        _guessFromExplainPic.SetActive(false);
        _guessFromExplainText.SetActive(false);
        _ancientPoetryCrosswordText.SetActive(false);
        switch ((EGameUIType)gameMode._levelInfo.Type)
        {
            case EGameUIType.GuessFromPictures:
                RefreshPicture(gameMode._levelInfo);
                break;
            case EGameUIType.GuessFromExplain:
                RefreshExplain(gameMode._levelInfo);
                break;
            case EGameUIType.AncientPoetryCrossword:
                RefreshAncientPoetry(gameMode._levelInfo);
                break;
        }
        
        //ˢ��ѡ��
        _btnAText.text = gameMode._levelInfo.Select1;
        _btnBText.text = gameMode._levelInfo.Select2;

        GL_VersionManager._instance.RefreshLevel(gameMode._levelInfo.ID);
        
        // GL_SceneManager._instance._levelAudio.PlayAudio(gameMode._levelInfo.ID);

        _nowAnswer.text = string.Format(_answerCount, GL_PlayerData._instance.CurLevel);
    }
    
    //ˢ��ͼ
    private void RefreshPicture(TableAnswerInfoData info)
    {
        if (string.IsNullOrEmpty(info.Picture))
            return;
        
        string path = DownloadConfig.downLoadPath + string.Format(GL_VersionManager.PictureUrl, info.Picture);
#if UNITY_ANDROID && !UNITY_EDITOR || UNITY_EDITOR_OSX
        path = "file://" + path;
#endif
        GL_ServerCommunication._instance.GetTexturePic(path, (t) =>
        {
            if (t != null)
            {
                Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                _guessFromExplainPic.SetActive(true);
                _guessFromExplainPic.sprite = s;
            }
        });
    }
    
    //ˢ������
    private void RefreshExplain(TableAnswerInfoData info)
    {
        if (string.IsNullOrEmpty(info.TitleText))
            return;   
        
        _guessFromExplainText.SetActive(true);
        int index = int.Parse(info.TitleText);
        string explain = DataModuleManager._instance.TableIdiomInfoData_Dictionary[index].Explain;
        _guessFromExplainText.text = explain;
    }
    
    //ˢ�¹�ʫ
    private void RefreshAncientPoetry(TableAnswerInfoData info)
    {
        if (string.IsNullOrEmpty(info.TitleText) || !info.TitleText.Contains("-"))
            return;   
        
        _ancientPoetryCrosswordText.SetActive(true);
        _ancientPoetryCrosswordText.text = "";
        var chars = info.TitleText.Split('-');
        //��ʫ���
        int id = int.Parse(chars[0]);
        //�ڼ����»���
        int spaceIndex = int.Parse(chars[1]);
        int index = id;
        string first = DataModuleManager._instance.TableAncientPoetryData_Dictionary[index].TheFirst;
        string second = DataModuleManager._instance.TableAncientPoetryData_Dictionary[index].TheSecond;
        string third = DataModuleManager._instance.TableAncientPoetryData_Dictionary[index].TheThird;
        string fourth = DataModuleManager._instance.TableAncientPoetryData_Dictionary[index].TheFourth;
        string[] contents = {first, second, third, fourth};
        //����»������ݳ���
        int spaceStrLength = contents[spaceIndex - 1].Length;
        //����»����������
        string spaceSymbol = contents[spaceIndex - 1][spaceStrLength - 1].ToString();
        string newSpaceStr = "";
        for (int i = 0; i < spaceStrLength - 1; i++)
        {
            newSpaceStr += "__";
        }
        newSpaceStr = $"<color=#b50005>{newSpaceStr}</color>";
        contents[spaceIndex - 1] = newSpaceStr + spaceSymbol;
        for (var i = 0; i < contents.Length; i++)
        {
            _ancientPoetryCrosswordText.text += contents[i];
            if (i < contents.Length - 1)
                _ancientPoetryCrosswordText.text += "\n";
        }
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


    private string _answerCount = "第<color=#f58c3e>{0}</color>题";
   

    public void CloseBar()
    {
        // UnityHelper.FindTheChildNode(gameObject,"LeftBottom").SetActive(false);
        // _growBtn.SetActive(false);
    }
    #endregion
}