using SUIFW;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataModule;
using System.Linq;
using UnityEngine.Video;
using UnityEngine.UI;

public class UI_IF_GuideTip : BaseUIForm
{
    public Button _btnClose;

    //视频组件
    private VideoPlayer _videoPlayer;
    private RenderTexture _renderTexture;
    //private RawImage _rawImage;

    private float _timer;
    //private Coroutine _onDialogCoroutine;
    public override void Init()
    {
       
        this.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        this.CurrentUIType.UIForms_Type = UIFormType.PopUp;
        this.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

        _btnClose = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "");

        _videoPlayer = UnityHelper.GetTheChildNodeComponetScripts<VideoPlayer>(gameObject, "Video");
        //_rawImage = _videoPlayer.GetComponent<RawImage>();

        RigisterButtonObjectEvent("BtnClose", (go =>
        {
            if(Time.time > _timer + 2)
            {
                //2秒后, 可关闭界面
                CloseUIForm();
            }
        }));
    }

    public override void InitData(object data)
    {
        //_tableData = data as TableGuideData;

    }

    public override void onUpdate()
    {

    }

    public override void Refresh(bool recall)
    {
        _timer = Time.time;
        //_renderTexture = new RenderTexture(564, 1006, 24);

        //VideoClip vc = GL_LoadAssetMgr._instance.Load<VideoClip>("Video/Test");

        //_videoPlayer.clip = vc;
        //_videoPlayer.targetTexture = _renderTexture;
        //_rawImage.texture = _renderTexture;
        _videoPlayer.Play();

        //if (_onDialogCoroutine != null)
        //    StopCoroutine(_onDialogCoroutine);
        //_onDialogCoroutine = StartCoroutine(OnDialogShow(_guideData.dialogDurtion, _guideData.dialogList));

    }

    public override void OnHide()
    {
        base.OnHide();

        //释放RenderTexture
        //_renderTexture.Release();
        //Destroy(_renderTexture);
        //_renderTexture = null;
    }
    //private IEnumerator OnDialogShow()
    //{
    //    VideoClip vc = GL_LoadAssetMgr._instance.Load<VideoClip>("Video/Test");

    //    _videoPlayer.clip = vc;
    //    _videoPlayer.targetTexture = _renderTexture;
    //    _rawImage.texture = _renderTexture;
    //    _videoPlayer.Play();
    //    _videoPlayer.playbackSpeed = 0f;
    //    var isSpecialBall = (EGuideTriggerUseType)_guideData.UseType == EGuideTriggerUseType.ComboSpecialBall;
    //    _guideAnimator.Play(isSpecialBall ? "UI_IF_GuideTip_Delay" : "UI_IF_GuideTip_Immed", 0, 0);
    //    _guideTimer = GL_Tools.GetClipLength(_guideAnimator,
    //        isSpecialBall ? "UI_IF_GuideTip_Delay" : "UI_IF_GuideTip_Immed");

    //    float firstDuration = (float)_videoPlayer.clip.length;
    //    float secondDuration = (float)_videoPlayer.clip.length - timer;
    //    float addDuration = 0;
    //    int dialogIndex = 0;
    //    _dialog.text = LanguageMgr.GetInstance().ShowText(dialogList[0]);
    //    yield return new WaitUntil(() => _videoPlayer.playbackSpeed == 1);
    //    while (true)
    //    {
    //        var curIndex = dialogIndex % dialogList.Count;
    //        _dialog.text = LanguageMgr.GetInstance().ShowText(dialogList[curIndex]);
    //        var curTimer = curIndex == 0 ? firstDuration : secondDuration;
    //        dialogIndex++;
    //        yield return new WaitForSeconds(curTimer);
    //        addDuration += curTimer;
    //        _continueRoot.gameObject.SetActive(addDuration >= (float)_videoPlayer.clip.length);

    //    }
    //    if (dialogList.Count > 0) _dialog.text = LanguageMgr.GetInstance().ShowText(dialogList[0]);
    //    if (dialogList.Count > 1)
    //    {
    //        var timerSingle = timer / dialogList.Count;
    //        for (int i = 1; i < dialogList.Count; i++)
    //        {
    //            yield return new WaitForSeconds(timerSingle);
    //            _dialog.text = LanguageMgr.GetInstance().ShowText(dialogList[i]);
    //        }
    //        yield return new WaitForSeconds(timerSingle);
    //    }
    //    else
    //    {
    //        yield return new WaitForSeconds(timer);
    //    }


    //    _continueRoot.gameObject.SetActive(true);
    //}

    public override void RefreshLanguage()
    {
    }
}
