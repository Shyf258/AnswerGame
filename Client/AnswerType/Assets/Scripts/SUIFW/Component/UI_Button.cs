//2021.11.22    关林
//按钮组件

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SUIFW
{
    public class UI_Button : Button
    {
        [HideInInspector] 
        public System.Action _callBack;

        private List<UI_TextChangeColor> _textComponents;
        private List<UI_ImageChangeColor> _imageComponents;
        private bool _childCompGot;

        private Animation _btnAnim;
        private bool _btnAnimGot;

        private SelectionState _lastState = SelectionState.Normal;

        private bool _isPlayIdle = true;
        protected override void Start()
        {
            if (!_btnAnimGot) GetBtnAnim();
            PlayIdle();
            //onClick.AddListener(PlayAudio);
            //SetPlayIdle(true);
        }

        public void SetPlayIdle(bool set)
        {
            _isPlayIdle = set;
        }

        private static void PlayAudio()
        {
            //GL_AudioPlayback._instance.Play(2);
        }

        private void GetChildComponents()
        {
            if (_childCompGot) return;

            _textComponents = GetComponentsInChildren<UI_TextChangeColor>().ToList();
            _imageComponents = GetComponentsInChildren<UI_ImageChangeColor>().ToList();
            _childCompGot = true;
        }

        private void GetBtnAnim()
        {
            if (_btnAnimGot) return;

            _btnAnim = GetComponent<Animation>();
            _btnAnimGot = true;
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            HandleNormalAndDisabled();

            return;
            // 状态切换的时候处理
            if (_lastState != state)
            {
                switch (state)
                {
                    case SelectionState.Normal:
                        HandleNormal();
                        break;
                    case SelectionState.Pressed:
                        HandlePressed();
                        break;
                }
            }

            _lastState = state;
        }

        private void HandleNormalAndDisabled()
        {
            if (!_childCompGot) 
                GetChildComponents();
            if (_textComponents != null && _textComponents.Count > 0) 
                _textComponents.ForEach(t => t?.SetColor(interactable));
            if (_imageComponents != null && _imageComponents.Count > 0) 
                _imageComponents.ForEach(t => t?.SetColor(interactable));
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (interactable)
                HandlePressed();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (interactable)
                HandleNormal();
        }


        private void HandleNormal()
        {
            if (!_btnAnimGot) GetBtnAnim();
            if (_btnAnim != null)
            {
                string animName = GetPointAnim();
                QuickPlayOtherAnim(_btnAnim, animName);
                float time = _btnAnim.GetClipLength(animName);
                Invoke(nameof(HandleNormalAction), time);
                Invoke(nameof(PlayIdle), time);
            }
        }

        //播放待机
        private void PlayIdle()
        {
            if(_isPlayIdle)
                QuickPlayOtherAnim(_btnAnim, GetPointAnim("idle"));
        }

        private void HandleNormalAction()
        {
            _callBack?.Invoke();
        }

        private void HandlePressed()
        {
            if (!_btnAnimGot) GetBtnAnim();
            if (_btnAnim != null)
            {
                string animName = GetPointAnim("down");
                QuickPlayOtherAnim(_btnAnim, animName);
                //float time = _btnAnim.GetClipLength(animName);
                //Invoke(nameof(PlayIdle), time);
            }
                
        }

        private static void QuickPlayOtherAnim(Animation anim, string animName)
        {
            if (anim == null)
                return;
            if (anim.GetClip(animName) == null) return;
            if (!anim.IsPlayEnd(animName)) return;

            if (!anim.enabled)
            {
                anim.enabled = true;
                var animator = anim.GetComponent<Animator>();
                animator.enabled = false;
            }

            anim.Play(animName);
        }

        private string GetPointAnim(string dir = "up")
        {
            if (_btnAnim == null)
                return string.Empty;
            foreach (AnimationState state in _btnAnim)
            {
                if (state.name.Contains(dir))
                {
                    return state.name;
                }
            }

            return string.Empty;
        }
    }
}
