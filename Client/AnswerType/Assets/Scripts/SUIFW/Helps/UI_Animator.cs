using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SUIFW;
using UnityEngine;

public class UI_Animator : MonoBehaviour
{
    [GL_Name("显示animation")] public string showAnimationName;
    [GL_Name("隐藏animation")] public string hideAnimationName;

    //是否播放中
    private bool _isAnimating;
    //是否是显示
    private bool _isShow;

    private float _showAniTimer = -1;

    private Coroutine _animCoroutine;   //动画的协程
    //关闭动画的时间
    public float ShowAniTimer
    {
        get
        {
            if (_showAniTimer == -1)
                _showAniTimer = GL_Tools.GetClipLength(_animator, showAnimationName);
            return _showAniTimer;
        }
    }

    private float _hideAniTimer = -1;
    //显示动画得时间
    public float HideAniTimer
    {
        get
        {
            if (_hideAniTimer == -1)
                _hideAniTimer = GL_Tools.GetClipLength(_animator, hideAnimationName);
            return _hideAniTimer;
        }
    }

    private Animator _animator;

    private Animator UiAnimator
    {
        get
        {
            if (!_animator) _animator = GetComponent<Animator>();
            if (!_animator) _animator = GetComponentInChildren<Animator>();
            return _animator;
        }
    }

    private bool? _isHasShow;
    //是否有这个动画
    private bool IsHasShow
    {
        get
        {
            if (_isHasShow == null)
            {
                _isHasShow = IsContainAnimationClip(showAnimationName);
            }

            return (bool)_isHasShow;
        }
    }

    private bool? _isHasHide;



    private bool IsHasHide
    {
        get
        {
            if (_isHasHide == null)
            {
                _isHasHide = IsContainAnimationClip(hideAnimationName);
            }

            return (bool)_isHasHide;
        }
    }

    //播放显示动画并回调
    public void DoShowPlay(Action callBack, BaseUIForm uiForm)
    {
        if (_isShow)
        {
            StopAllCoroutines();
            _isAnimating = false;
        }
        if (!UiAnimator || !IsHasShow)
        {
            callBack?.Invoke();
            return;
        }
        _isShow = true;
        MethodExeTool.StopCoroutine(_animCoroutine);
        _animCoroutine = MethodExeTool.StartCoroutine(AnimPlay(showAnimationName, callBack, uiForm, true));
    }

    //播放隐藏动画并回调
    public void DoHidePlay(Action callBack, BaseUIForm uiForm)
    {
        if (!_isShow)
        {
            StopAllCoroutines();
            _isAnimating = false;
        }

        if (!UiAnimator || !IsHasHide)
        {
            callBack?.Invoke();
            uiForm.ClearHide();
            return;
        }
        _isShow = false;
        MethodExeTool.StopCoroutine(_animCoroutine);
        _animCoroutine = MethodExeTool.StartCoroutine(AnimPlay(hideAnimationName, callBack, uiForm, false));
    }

    //播放 动画
    IEnumerator AnimPlay(string animationName, Action callBack, BaseUIForm uiForm, bool isShow)
    {
        _isAnimating = true;
        UiAnimator.Play(animationName, 0, 0);
        float timer = isShow ? ShowAniTimer : HideAniTimer;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        _isAnimating = false;
        callBack?.Invoke();
        if (isShow)
        {
        }
        else
        {
            uiForm.ClearHide();
        }
    }


    //是否存在这个动画
    private bool IsContainAnimationClip(string clipName)
    {
        if (!UiAnimator) return false;
        AnimationClip[] clips = UiAnimator.runtimeAnimatorController.animationClips;
        if (clips.Length <= 0) return false;

        foreach (var t in clips)
        {
            if (t.name == clipName)
            {
                return true;
            }
        }
        return false;
    }


    public void PlayShow()
    {
        UiAnimator.Play(showAnimationName, 0, 0);
    }

    public void Play(string name)
    {
        UiAnimator.Play(name, 0, 0);
    }
}
