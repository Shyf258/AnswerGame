using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SUIFW;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DataModule;
using SUIFW.Diplomats.Common;
using UnityEditor;

public class UI_IF_Setting : BaseUIForm
{
    
    public List<Sprite> _icon;
    
    private Transform _audioBtn;

    private Transform _aboutUs;
    private Button _exit;
    
    
    private Image _playerIcon;
    private Text _playerName;
    private Text _playerId;
    
    private Text _logIn;

    private Text _productName;

    private Text _version;

    private Image _logo;

    private Text _description;
    
    #region 声音开关

    private Toggle _audioControl;

    #endregion
    
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

        FreshProduction();
        
        _aboutUs = UnityHelper.FindTheChildNode(gameObject, "AboutUS");
        _exit = UnityHelper.GetTheChildNodeComponetScripts<Button>(_aboutUs.gameObject, "ExitPage");
        RigisterButtonObjectEvent(_exit, go =>
        {
            OnClickClose();
            _aboutUs.SetActive(false);
        });
        _audioBtn = UnityHelper.FindTheChildNode(gameObject, "AudioBtn");
        RigisterButtonObjectEvent("BtnClose", (go) =>
        {
            OnClickClose();
        });
        RigisterButtonObjectEvent("BtnPrivacyPolicy", (go) =>
        {
            // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetReward);
            // CloseUIForm();
            GL_AD_Logic._instance.Open_Privacy();
        });
        // RigisterButtonObjectEvent("PlayerReport", (go) =>
        // {
        //     UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_FeedBack);
        // });
        
        RigisterButtonObjectEvent("BtnTermOfUse", (go) =>
        {
            GL_AD_Logic._instance.Open_TermsOfUse();
        });
        // RigisterButtonObjectEvent("AudioBtn", (go) =>
        // {
        //     AudioControll();
        // });
        // RigisterButtonObjectEvent("PlayerReport", (go) =>
        // {
        //     PlayerReport();
        // });
        RigisterButtonObjectEvent("Version", (go) =>
        {
            //查看版本信息
            _aboutUs.SetActive(true);
        });
        Transform playermessage = UnityHelper.FindTheChildNode(gameObject, "PlayerMessage");
        _playerIcon = UnityHelper.GetTheChildNodeComponetScripts<Image>(playermessage.gameObject, "UserIcon");
        _playerName = UnityHelper.GetTheChildNodeComponetScripts<Text>(playermessage.gameObject, "UserName");
        _playerId = UnityHelper.GetTheChildNodeComponetScripts<Text>(playermessage.gameObject, "UserID");
        
        _logIn = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "LogInText");
        RigisterButtonObjectEvent("BtnLogOut", go => {LogIn(); });
        RigisterButtonObjectEvent("BtnExit", go => {LogOut(); });
        //声音控制
        _audioControl = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(gameObject, "AudioControl");
        
        ShowAudio();
        
        _audioControl.onValueChanged.AddListener((delegate(bool set)
        {
            SetAudio(!set); 
        }));


        _logo = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "icon");
        
        _productName = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "ProductName");

        _version = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "VersionText");

        _description = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Description");
        
        // if (GL_CoreData._instance._archivedData._versionIndex ==1)
        // {
        //     Transform btnLogin = UnityHelper.FindTheChildNode(gameObject, "BtnLogOut");
        //     btnLogin.SetActive(false);
        // }
        
        // ShowAudioButton();
        
#if PureVersion
        // Transform btn = UnityHelper.FindTheChildNode(gameObject, "BtnLogOut");
        // btn.gameObject.SetActive(false);
        // Transform message = UnityHelper.FindTheChildNode(gameObject, "PlayerMessage");
        // message.gameObject.SetActive(false);
        // Text title = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "Title");
        // title.text = "设置";
#endif
        
        Transform logTool = UnityHelper.FindTheChildNode(gameObject, "LogTool");
        logTool.GetComponent<UI_Obj_LogTool>().InitObjectNode();
    }


    public override void Display(bool redisplay = false)
    {
        base.Display(redisplay);
        gameObject.transform.SetAsLastSibling();
    }

    public override void OnHide()
    {
        base.OnHide();
        // GL_AD_Interface._instance.CloseBannerAd();
    }

    /// <summary>
    /// 声音控制
    /// </summary>
    private void AudioControll()
    {
        GL_CoreData._instance.AudioOn = !GL_CoreData._instance.AudioOn;
        GL_CoreData._instance.BGMAudioOn = GL_CoreData._instance.AudioOn;
        ShowAudioButton();
    }

    private void ShowAudioButton()
    {
        if (GL_CoreData._instance.AudioOn)
        {
            //切换图片显示
            // _audioBtn.GetComponent<Image>().sprite = _audioIcon[1];
            //更改文字描述
            // _audioBtn.Find("AudioState").GetComponent<Text>().text = "<color=#000000>声音：开</color>";
        }
        else
        {
            //切换图片显示
            // _audioBtn.GetComponent<Image>().sprite = _audioIcon[0];
            //更改文字描述
            // _audioBtn.Find("AudioState").GetComponent<Text>().text = "<color=#5b9bc2>声音：关</color>";
        }
    }

    private void PlayerReport()
    {
        
    }

    // /// <summary>
    // /// 退出微信登录
    // /// </summary>
    // private void LogOut()
    // {
    //     DDebug.LogError("退出登录按钮：点击 ");
    // }

    private void OnClickClose()
    {
        CloseUIForm();
    }

    public override void onUpdate()
    {
    }

    public override void Refresh(bool recall)
    {
        ShowPlayerMessage();
        // if (GL_CoreData._instance._archivedData._versionIndex !=1)
        // {
        //     // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_PlayerMessageBar);
        // }
        ShowGameName();
    }

    #region 声音控制

    private void ShowAudio()
    {
        _audioControl.isOn = GL_CoreData._instance.AudioOn;
    }

    private void SetAudio(bool set)
    {
        GL_CoreData._instance.AudioOn = set;
        GL_CoreData._instance.BGMAudioOn = set;
    }

    #endregion
    
    public override void RefreshLanguage()
    {
    }

    private void ShowPlayerMessage()
    {
        //显示游戏图标
        switch (AppSetting.BuildApp)
        {
            case EBuildApp.RSDYJ:
                _playerIcon.sprite = _icon[0];
                _logo.sprite = _icon[0];
                break;
            case EBuildApp.ZYXLZ:
                _playerIcon.sprite = _icon[1];
                _logo.sprite = _icon[1];
                break;
            case EBuildApp.NJZW:
                _playerIcon.sprite = _icon[2];
                _logo.sprite = _icon[2];
                break;
        }
        
       
      
        if (GL_PlayerData._instance.IsLoginWeChat())
        {
            GL_PlayerData._instance.GetWeChatIcon((t) =>
            {
                _playerIcon.sprite = t;
            });
            _playerName.text = GL_PlayerData._instance.WeChatName;
            _playerId.text = GL_PlayerData._instance.InvitationCode;
            _logIn.text = "退出游戏";
        }
        else
        {
            _playerName.text = "游客";
            _logIn.text = "登录微信";
            _playerId.text = "11962344";
        }
        if (GL_Game._instance._sceneSwitch._enterType == EGameEnterType.PureVersion)
        {
            _logIn.text = "退出游戏";
        }
    }

    private TableBuildAppData _tableBuildAppData;
    private void FreshProduction()
    {
        int appId = (int)AppSetting.BuildApp;
        _tableBuildAppData = DataModuleManager._instance.TableBuildAppData_Dictionary[appId];
    }

    private void ShowGameName()
    {
        

        _productName.text = _tableBuildAppData.ProductName;

         _version.text = $"当前版本{GL_SDK._instance.GetAppVersion()}";

         _description.text = $"欢迎使用{_tableBuildAppData.ProductName}APP，为了更好的给用户提供服务，在用户使用{_tableBuildAppData.ProductName}服务前，请用户仔细阅读用户协议。";
    }



    /// <summary>
    /// 注销
    /// </summary>
    private void LogOut()
    {
        
        if (GL_Game._instance._sceneSwitch._enterType == EGameEnterType.PureVersion)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
           Application.Quit();
#endif
           return;
        }
        
        
        
        GL_PlayerPrefs.SetInt(EPrefsKey.IsAgreeGDPR, 0);

        if (GL_PlayerData._instance.AppConfig.isLogout == 1 )
        {
            if (GL_PlayerData._instance.IsLoginWeChat())
            {
                Net_RequesetCommon com = new Net_RequesetCommon();
                GL_ServerCommunication._instance.Send(Cmd.LogOutWeChat, JsonUtility.ToJson(com), (string msg) =>
                {
                    // Flogin();
                    GL_SDK._instance.GameClear();
                    GL_CoreData._instance.ClearData_Character();
                    GL_CoreData._instance.RealSaveData();
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
           Application.Quit();
#endif
                });
                GL_CoreData._instance._archivedData._weChatInfo = null;
            }
            ShowPlayerMessage();
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

    private void Flogin()
    {
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        InvokeRepeating("CloseLoading",5f,1);
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.LoginGuest, JsonHelper.ToJson(com), CB_LoginGuest);
    }

    private void CloseLoading()
    {
        UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NetLoading);
        CancelInvoke();
    }

    private void CB_LoginGuest(string json)
    {
        
        Net_CB_LoginGuest msg = JsonUtility.FromJson<Net_CB_LoginGuest>(json);
        if (msg == null)
            return;
        GL_PlayerData._instance.UserSecret = msg.usersecret;
        GL_SDK._instance.SetUserSecret(msg.usersecret);
        
        //刷新显示内容
        GetPlayerMessage();
    }

    private void GetPlayerMessage()
    {
        GL_PlayerData._instance.GetSystemConfig(() =>
        {
            UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NetLoading);
            GL_GameEvent._instance.SendEvent(EEventID.RefreshGameMode);
            GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
        });
    }
    
    /// <summary>
    /// 登录微信
    /// </summary>
    private void LogIn()
    {
        if (GL_Game._instance._sceneSwitch._enterType == EGameEnterType.PureVersion)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        //判断是否微信登陆
        if(GL_PlayerData._instance.IsLoginWeChat())
        {
            // //退出登录
            // GL_PlayerData._instance.LogOut();
            // ShowPlayerMessage();
            // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.QuitLogin);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            //登陆微信弹窗
            Action cb = () => ShowPlayerMessage();
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin, cb);
        }
    }

    
}
