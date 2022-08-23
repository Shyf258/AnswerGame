using SUIFW;
using System;
using Logic.System.NetWork;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_IF_WeChatLogin : BaseUIForm
{

    private Action _callback;
    // /// <summary>
    // /// 同意协议
    // /// </summary>
    // private Toggle _agreeToggle;
    //
    // private Transform _agree;
    public override void Init()
    {
        CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        CurrentUIType.UIForms_Type = UIFormType.PopUp;
        CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;

        // _agreeToggle = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(gameObject, "AgreeToggle");  
        // _agree = UnityHelper.FindTheChildNode(gameObject, "Agreement");
  
        
      

        RigisterButtonObjectEvent("BtnSure", (go => 
        {
            GL_SDK._instance.Login("wechat", CB_Login);
            CloseUIForm();
            // if (GL_Game._instance._sceneSwitch._enterType == EGameEnterType.OfficialVersion)
            // {
            //     GL_SDK._instance.Login("wechat", CB_Login);
            //     CloseUIForm();
            //     return;
            // }
            // if (_agreeToggle.isOn)
            // {
            //     GL_SDK._instance.Login("wechat", CB_Login);
            //     CloseUIForm();
            //     // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Guide_WechatLogin);
            // }
            // else
            // {
            //     UI_HintMessage._.ShowMessage(/*this.transform,*/"请先同意用户协议和隐私协议");
            // }
          
        }));

        RigisterButtonObjectEvent("BtnClose", (go => { CloseUIForm(); }));
        
        RigisterButtonObjectEvent("UserAgree", go =>
        {
            GL_AD_Logic._instance.Open_TermsOfUse();
        });
        RigisterButtonObjectEvent("PrivacyAgree", go =>
        {
            GL_AD_Logic._instance.Open_Privacy();
        });
    }

    private void CB_Login(string param)
    {
#if GuanGuan_Test
        if(!string.IsNullOrEmpty(param))
        {
            SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage(transform.parent, "登陆成功");
            GL_PlayerData._instance.CB_WeChatLoginSuccess(param);
            _callback?.Invoke();
        }
        else
        {
            SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage(transform.parent, "登陆失败");
        }
        return;
#endif
        DDebug.LogError("~~~SDK登陆回调: " + param);
        Net_WeChatLogin com = new Net_WeChatLogin();
        com.code = param;
        GL_ServerCommunication._instance.Send(Cmd.LoginWeChat, JsonUtility.ToJson(com), (string msg) =>
        {
            if (!string.IsNullOrEmpty(msg))
            {
                SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage(/*transform.parent, */"登陆成功");
                GL_PlayerData._instance.CB_WeChatLoginSuccess(msg);
                //YS_NetLogic._instance.InviteConfig();
                _callback?.Invoke();
            }
            else
            {
                SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage(/*transform.parent,*/ "登陆失败");
            }
         
        });

     
    }

    public override void InitData(object data)
    {
        if(data != null)
            _callback = data as Action;

       
    }
    public override void onUpdate()
    {
    }

    public override void Refresh(bool recall)
    {
        int isLog = GL_PlayerPrefs.GetInt(EPrefsKey.IsWeChatLogIn);
        //1.隐私协议
        // if (isLog != 0)
        // {
        //     Transform agreeMent = UnityHelper.FindTheChildNode(gameObject, "Agreement");
        //     agreeMent.SetActive(false);
        // }
    }

    public override void RefreshLanguage()
    {
    }
}