using System;
using System.Collections;
using System.Collections.Generic;
using DataModule;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_ShareResult : BaseUIForm
{

    private string countStr = "本轮已介绍：{0}人";
    
    private Button _shareBtn;

    private InputField _textInput;

    private Button _btnInput;

    private Text _friendCount;

    private Text _friendId;

    private Transform _friend;

    private Transform _input;

    private Text _id;
    private string _idStr = "我的邀请码：{0}";
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

        Transform group = UnityHelper.FindTheChildNode(gameObject, "MessageGroup");

        _friend = UnityHelper.FindTheChildNode(group.gameObject, "Friend");

        _input = UnityHelper.FindTheChildNode(group.gameObject, "InputID");
        
        _friendId = UnityHelper.GetTheChildNodeComponetScripts<Text>(group.gameObject, "IdText");

        _friendCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(group.gameObject, "Count");

        _textInput = UnityHelper.GetTheChildNodeComponetScripts<InputField>(group.gameObject, "InputField");
        
        _btnInput = UnityHelper.GetTheChildNodeComponetScripts<Button>(group.gameObject, "GetFriend");

        _id = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "InviteID");
        RigisterButtonObjectEvent(_btnInput, go =>
        {
            GetReward();
        });
        
        _shareBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(group.gameObject, "Share");

        RigisterButtonObjectEvent(_shareBtn, go =>
        {
            //判断是否微信登陆.
            if (!GL_PlayerData._instance.IsLoginWeChat())
            {
                //登陆微信
                // Action show =()=> PlayerIcon();
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin);
                // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Setting);
            }
            else
            {
                WeChatShare();
            }
        });
        
        RigisterButtonObjectEvent("ExitPage", go =>
        {
            CloseUIForm();
        });
    }
    public override void RefreshLanguage()
    {
        
    }

    public override void Refresh(bool recall)
    {
        ShowPage();
        try
        {
            _id.text = string.Format(_idStr, GL_PlayerData._instance.InvitationCode);
        }
        catch 
        {
            _id.gameObject.SetActive(false);
        }
        // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_GoldenPig);
    }

    public override void onUpdate()
    {
      
    }

    public override void OnHide()
    {
        base.OnHide();
        // GL_AD_Interface._instance.CloseBannerAd();
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_DragRedpack);

    }

    public override void Display(bool redisplay = false)
    {
        base.Display(redisplay);
        
    }

    /// <summary>
    /// 领取分享奖励
    /// </summary>
    private void GetReward()
    {
        if (_textInput.text.Length <8|| _textInput.text.Length >14)
        {
           UI_HintMessage._.ShowMessage("请输入正确的邀请码");
           return;
        }

        if (_textInput.text == GL_PlayerData._instance.InvitationCode)
        {
            UI_HintMessage._.ShowMessage("请不要尝试邀请自己");
            return;
        }
        
        //上报服务器领取奖励
        RepeatInvoke();
    }

    private void RepeatInvoke()
    {
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        Net_Rq_InputInvoke req = new Net_Rq_InputInvoke();
        req.inviteCode = Convert.ToInt32(_textInput.text);
        GL_ServerCommunication._instance.Send(Cmd.InviteInput, JsonHelper.ToJson(req),(delegate(string s)
        {
            UI_HintMessage._.ShowMessage("绑定邀请人信息成功");
            CloseUIForm();
            GL_PlayerData._instance.GetProduceConfig((() =>
            {
             UI_IF_Production production = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Production) as UI_IF_Production;
             production._ObjProduction.FreshText();
                UIManager.GetInstance().CloseUIForms(SysDefine.UI_Path_NetLoading);
            }));
        }));
       
    }

    private void ShowPage()
    {
       
        //是否已完成绑定好友
        bool show ;
        try
        {
            show = GL_PlayerData._instance.InviteConfig.parentUser.invitationCode != null;
            // DDebug.LogError("***** 我的邀请人："+GL_PlayerData._instance.InviteConfig.parentUser.invitationCode);
        }
        catch
        {
            show = false;
        }
        //是否显示输入ID
        _input.gameObject.SetActive(!show);
        _friend.gameObject.SetActive(show);

        if (show)
        {
            _friendId.text = GL_PlayerData._instance.InviteConfig.parentUser.invitationCode.ToString();
        }
        _friendCount.text = String.Format(countStr,GL_PlayerData._instance.NetCbProduceConfig.inviteNum);
        
    }

    /// <summary>
    /// 显示玩家微信头像
    /// </summary>
    private void PlayerIcon()
    {
        // UI_IF_MainUp _uiIfMain = UIManager.GetInstance().GetUI(SysDefine.UI_Path_MainUp) as UI_IF_MainUp;
        // _uiIfMain.RefreshPlayInfoIcon();
    }
    #region Share

  
    /// <summary>
    /// 截图保存分享
    /// </summary>

    private void GetPNG()
    {
        // GL_Tools.SaveRenderTextureToPNG(GL_Tools.Screenshot(0, 0, Screen.width,Screen.height), "Art", "TTTpic");
        StartCoroutine(Screenshot());
    }

    private Texture2D _screenshotTexture;
    private IEnumerator Screenshot()
    {
        yield return new WaitForEndOfFrame();
        _screenshotTexture = GL_Tools.Screenshot(Screen.width, Screen.height, 0, 0);
        GL_Tools.SaveTexture2DToPNG(_screenshotTexture, GL_ConstData.ScreenshotTexture, "ScreenShot");
        
        //跳转微信分享
        WeChatShare();
    }

    /// <summary>
    /// 微信分享
    /// </summary>
    private void WeChatShare()
    {
        //分享
        GL_SDK._instance.GameShareBGSK();
    }

    #endregion
    
    
}
