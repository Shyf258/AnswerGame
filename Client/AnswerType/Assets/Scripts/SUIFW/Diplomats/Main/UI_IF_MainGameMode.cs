//2022.8.23 管理
//主页面玩法相关初始化

using DataModule;
using DG.Tweening;
using SUIFW;
using UnityEngine;
using UnityEngine.UI;

public partial class UI_IF_Main
{

    #region 答题玩法

    //释义内容
    private Text _guessFromExplainText;
    //图
    private Image _guessFromExplainPic; 
    //古诗
    private Text _ancientPoetryCrosswordText;

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

    protected void InitGameMode()
    {

        #region 游戏玩法初始化
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
        //显示答案
        _showAnswer = UnityHelper.FindTheChildNode(_choiceGroup.gameObject, "ShowAnswer");
        _showRight = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowRight");
        _showWrong = UnityHelper.FindTheChildNode(_showAnswer.gameObject, "ShowWrong");

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


    }

    #region 答题玩法
    //刷新题目
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
        
        //刷新选项
        _btnAText.text = gameMode._levelInfo.Select1;
        _btnBText.text = gameMode._levelInfo.Select2;

        GL_VersionManager._instance.RefreshLevel(gameMode._levelInfo.ID);
        
        // GL_SceneManager._instance._levelAudio.PlayAudio(gameMode._levelInfo.ID);

        _nowAnswer.text = string.Format(_answerCount, GL_PlayerData._instance.CurLevel);
    }
    
    //刷新图
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
    
    //刷新释义
    private void RefreshExplain(TableAnswerInfoData info)
    {
        if (string.IsNullOrEmpty(info.TitleText))
            return;   
        
        _guessFromExplainText.SetActive(true);
        int index = int.Parse(info.TitleText);
        string explain = DataModuleManager._instance.TableIdiomInfoData_Dictionary[index].Explain;
        _guessFromExplainText.text = explain;
    }
    
    //刷新古诗
    private void RefreshAncientPoetry(TableAnswerInfoData info)
    {
        if (string.IsNullOrEmpty(info.TitleText) || !info.TitleText.Contains("-"))
            return;   
        
        _ancientPoetryCrosswordText.SetActive(true);
        _ancientPoetryCrosswordText.text = "";
        var chars = info.TitleText.Split('-');
        //古诗编号
        int id = int.Parse(chars[0]);
        //第几句下划线
        int spaceIndex = int.Parse(chars[1]);
        int index = id;
        string first = DataModuleManager._instance.TableAncientPoetryData_Dictionary[index].TheFirst;
        string second = DataModuleManager._instance.TableAncientPoetryData_Dictionary[index].TheSecond;
        string third = DataModuleManager._instance.TableAncientPoetryData_Dictionary[index].TheThird;
        string fourth = DataModuleManager._instance.TableAncientPoetryData_Dictionary[index].TheFourth;
        string[] contents = {first, second, third, fourth};
        //获得下划线内容长度
        int spaceStrLength = contents[spaceIndex - 1].Length;
        //获得下划线特殊符号
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
        GL_SceneManager._instance._levelAudio.StopAudio();
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


    private string _answerCount = "第<color=#f58c3e>{0}</color>关";
    private string _question = "继续答对<color=#CF0400>{0}</color>题，即可 <color=#CF0400>抽奖</color>哟";

    public void CloseBar()
    {
        // UnityHelper.FindTheChildNode(gameObject,"LeftBottom").SetActive(false);
        // _growBtn.SetActive(false);
    }
    #endregion
}