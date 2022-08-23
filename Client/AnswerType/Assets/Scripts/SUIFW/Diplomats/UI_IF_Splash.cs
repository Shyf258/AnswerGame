using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using SUIFW;
using UnityEngine;

public class UI_IF_Splash : BaseUIForm
{
    private float _timer;
    private float _timerDelay = 0.2f;
    [GL_Name("splash")] public GL_SpineAnimation_UI splash;
    [GL_Name("iosLogo")] public GL_SpineAnimation_UI iosLogo;

    private bool _isOver = false;
    public override void Init()
    {
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.Normal;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;
    }

    public override void onUpdate()    {
    }
    

    public override void Refresh(bool recall)
    {
    }

    public override void RefreshLanguage()
    {
    }
    void Start()
    {
#if UNITY_IOS
        iosLogo.gameObject.SetActive(true);
        iosLogo.PlayAnimation("animation", false);
        iosLogo.SetTimeScale(0);
        _timer = iosLogo.GetDurationTime() + _timerDelay + 0.5f;
        splash.gameObject.SetActive(false);
#else
        splash.gameObject.SetActive(true);
        splash.PlayAnimation("animation",false);
        splash.SetTimeScale(0);
        _timer = splash.GetDurationTime() + _timerDelay;
        iosLogo.gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOver)
            return;
        _timerDelay -= Time.deltaTime;
        if (_timerDelay <=0 )
        {
#if UNITY_IOS
            iosLogo.SetTimeScale(1);
#else
            splash.SetTimeScale(1);
#endif
        }
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _isOver = true;
            GL_Game._instance.GameState = EGameState.Loading;
        }
    }
}

