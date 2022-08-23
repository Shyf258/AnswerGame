using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using SUIFW;
using SUIFW.Diplomats.Common;
using System;

public class UI_IF_Loading : BaseUIForm
{
    private Image _slider;
    private Text _fillText;
    private Transform _ball;
    private string _loadingStr;

    public List<Sprite> _bg;

    private Image _bgImage;
    /// <summary>
    /// 微信登录
    /// </summary>
    private Transform _weChatLogin;
    
    /// <summary>
    /// 进度条显示
    /// </summary>
    private Transform _sliderObj;
    /// <summary>
    /// 同意协议
    /// </summary>
     private Toggle _agreeToggle;
    
    /// <summary>
    /// 登录按钮
    /// </summary>
    private Button _loginBtn;

    private Text _loginText;

    private Transform _agreement;//隐私协议选勾界面
    /// <summary>
    /// 是否显示进度条
    /// </summary>
    [HideInInspector]
    public bool _showSlider =true;
    public override void Init()
    {
        _isFullScreen = false;
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.Normal;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;

        //sceneLoading = UnityHelper.GetTheChildNodeComponetScripts<GL_SceneLoading>(this.gameObject, "Root");
        _slider = UnityHelper.GetTheChildNodeComponetScripts<Image>(this.gameObject, "Fill");
        _sliderObj = UnityHelper.FindTheChildNode(gameObject, "Slider");
        _fillText = UnityHelper.GetTheChildNodeComponetScripts<Text>(this.gameObject, "FillText");
        _ball = UnityHelper.FindTheChildNode(gameObject, "SnowBall");
        //_loadingStr = LanguageMgr.GetInstance().ShowText("loading");

        _bgImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "BG");
        
        _weChatLogin = UnityHelper.FindTheChildNode(gameObject, "WeChatLogin");
        _agreement = UnityHelper.FindTheChildNode(_weChatLogin.gameObject, "Agreement");
        _agreeToggle = UnityHelper.GetTheChildNodeComponetScripts<Toggle>(_agreement.gameObject, "AgreeToggle");
        _loginBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(_weChatLogin.gameObject, "LoginBtn");
        _loginText = UnityHelper.GetTheChildNodeComponetScripts<Text>(_loginBtn.gameObject, "Text");
        RigisterButtonObjectEvent(_loginBtn, go =>
        {
           
            if(_agreeToggle.isOn)
            {
                GL_Game._instance._sceneSwitch.UI_OnClickWeChat();
            }
            else
            {
                UI_HintMessage._.ShowMessage("请阅读并点击同意下方\n隐私政策及用户协议");
            }
            //int isAgreeGDPR = GL_PlayerPrefs.GetInt(EPrefsKey.IsAgreeGDPR);
            //if (isAgreeGDPR == 0 || _agreeToggle.isOn
            //||(GL_PlayerData._instance.AppConfig != null && GL_PlayerData._instance.AppConfig.isNotice == 2))
            //{
            //    //点击微信登陆
            //    GL_Game._instance._sceneSwitch.UI_OnClickWeChat();

            //}
            //else
            //{
            //    UI_HintMessage._.ShowMessage(/*this.transform,*/"请先同意用户协议和隐私协议");
            //}



        });
        
        RigisterButtonObjectEvent("UserAgree", go =>
        {
            GL_AD_Logic._instance.Open_TermsOfUse();
        });
        RigisterButtonObjectEvent("PrivacyAgree", go =>
        {
            GL_AD_Logic._instance.Open_Privacy();
        });
        _loadingStr = LanguageMgr.GetInstance().ShowText("loading");

        ShowSlider(false, false, "");
        //判断是否显示微信登陆
        //if(!GL_PlayerData._instance.IsLoginWeChat())
        //{
        //    ShowSlider(false);
        //}
        //else
        //{
        //    ShowSlider(true);
        //}
    }

    public override void InitData(object data)
    {
        base.InitData(data);
    }

    public override void onUpdate()
    {
    }

    public override void Refresh(bool recall)
    {
        SetPackage();
    }

    public override void RefreshLanguage()
    {
    }

    //是否勾选
    public bool IsAgreeToggle()
    {
        return _agreeToggle.isOn;
    }
    void Update()
    {
        float value = GL_Game._instance._sceneSwitch._uiProgress;
        _slider.fillAmount = value / 100f;
        _ball.GetComponent<RectTransform>().anchoredPosition = new Vector3((_slider.fillAmount *  (float)_slider.GetComponent<RectTransform>().sizeDelta.x)-50,0,0);
        
        // DDebug.LogError(_ball.GetComponent<RectTransform>().anchoredPosition.ToString());
        //_fillText.text = string.Format(_loadingStr, value.ToString("F2"));
        //_fillText.text = value.ToString("F2");
        _fillText.text = value.ToString("F0") + "%";
    }
    
    /// <summary>
    /// 进度条显示
    /// </summary>
    /// <param name="show">是否显示</param>
    public void ShowSlider(bool show)
    {
        _showSlider = show;
        _sliderObj.gameObject.SetActive(show);
        _weChatLogin.gameObject.SetActive(!show);
        
        //弹出隐私协议关闭toggle
        // if (!show && GL_PlayerData._instance.AppConfig.isNotice == 1 && GL_CoreData._instance.Notice)
        //if (!show && GL_PlayerData._instance.AppConfig.isNotice == 1)
        //{
        //    // GL_CoreData._instance.Notice = false;
        //    // _agreeToggle.isOn = false;
        //}
    }
    public void ShowSlider(bool isSlider, bool isLogin, string loginStr)
    {
        _sliderObj.gameObject.SetActive(isSlider);
        _weChatLogin.gameObject.SetActive(isLogin);

        if(!string.IsNullOrEmpty(loginStr))
        {
            _loginText.text = loginStr;
            if (loginStr.Equals("微信登录"))
            {
                _agreeToggle.isOn = true;
                _agreement.SetActive(false);
            }
                
        }
    }

    private void SetPackage()
    {
        switch (AppSetting.BuildApp)
        {
            case EBuildApp.RSDYJ:
                _bgImage.sprite = _bg[0];
                Transform rsdyj = UnityHelper.FindTheChildNode(gameObject, "RSDYJ");
                rsdyj.SetActive(true);
                break;
            case EBuildApp.ZYXLZ:
                _bgImage.sprite = _bg[1];
                Transform zyxlz = UnityHelper.FindTheChildNode(gameObject, "ZYXLZ");
                zyxlz.SetActive(true);
                break;
            case EBuildApp.CYZDD:
                _bgImage.sprite = _bg[2];
                Transform cyzdd = UnityHelper.FindTheChildNode(gameObject, "CYZDD");
                cyzdd.SetActive(true);
                break;
        }

    }
}
