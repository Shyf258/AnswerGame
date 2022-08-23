//2019.12.06
//Spine动画控制器    关林

using System;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Spine.Unity
{
    public class GL_SpineAnimation_UI : MonoBehaviour
    {
        //private SkeletonAnimation _skeletonGraphic;
        private SkeletonGraphic _skeletonGraphic;
        private TrackEntry trackEntry0;   //0动画通道,主通道
        private TrackEntry trackEntry1;   //1当前动画通道
        private SkeletonRenderer _renderer;

        private Action _callback;

        private void Awake()
        {
            _skeletonGraphic = gameObject.GetComponent<SkeletonGraphic>();
            //_renderer = _skeletonGraphic.gameObject.GetComponent<SkeletonRenderer>();
            //_renderer.Initialize(false);

            _skeletonGraphic.AnimationState.Start += ActionStart;// (entry) => ActionStart(entry);
            _skeletonGraphic.AnimationState.Complete += ActionComplete;// (entry) => ActionComplete(entry);
            
           

            //PlayAnimation("run", false, 0);
        }

        private void OnEnable()
        {
            if (_skeletonGraphic.startingAnimation != null)
            {
                PlayAnimation(_skeletonGraphic.startingAnimation, _skeletonGraphic.startingLoop, 0);
            }
            else
                PlayAnimation(null, true, 0);   //默认停止
        }

        private void ActionComplete(TrackEntry entry)
        {
            //ActionComplete();
            
            _callback ?.Invoke();
        }

        private void ActionStart(TrackEntry entry)
        {
            //ActionStart();
        }

        /// <summary>
        /// 获取当前播放动画的时间
        /// </summary>
        /// <returns></returns>
        public float GetFrameTime()
        {
            return trackEntry0.AnimationTime;
        }
        /// <summary>
        /// 动画持续时间
        /// </summary>
        public float GetDurationTime()
        {
            return trackEntry0.Animation.Duration;
        }
        public bool GetLoop()
        {
            return trackEntry0.Loop;
        }

        public string GetName()
        {
            return trackEntry0.Animation.Name;
        }

        #region Spine接口封装
        public void PlayAnimation(string name, bool loop = true, int trackIndex = 0, float scale = 1f, Action callback = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _callback = callback;
                if (_skeletonGraphic.AnimationState == null)
                {
                    Debug.LogError("~~~初始化动作播放异常");
                    //_skeletonGraphic.an = name;
                    trackEntry0 = _skeletonGraphic.AnimationState.GetCurrent(0);
                    return;
                }
                if (0 == trackIndex)
                {
                    //if (trackEntry0 != null && trackEntry0.animation != null && trackEntry0.animation.name == name)
                    //{
                    //    trackEntry0.trackTime = 0;
                    //    return;
                    //}

                    trackEntry0 = _skeletonGraphic.AnimationState.SetAnimation(trackIndex, name, loop);
                    trackEntry0.TimeScale = scale;
                }
                else
                {
                    trackEntry1 = _skeletonGraphic.AnimationState.SetAnimation(trackIndex, name, loop);
                    trackEntry1.TimeScale = scale;
                }
            }
            else if (trackIndex != 1)
            {
                _skeletonGraphic.AnimationState.SetEmptyAnimation(trackIndex, 0);
            }
        }

        public void AddAnimation(string name, bool loop = true, int trackIndex = 0, float scale = 1f, Action callback = null)
        {
            if (0 == trackIndex)
            {
                if (trackEntry0 != null && trackEntry0.animation.name == name)
                {
                    trackEntry0.trackTime = 0;
                    return;
                }

                trackEntry0 = _skeletonGraphic.AnimationState.AddAnimation(trackIndex, name, loop, 0);
                trackEntry0.TimeScale = scale;
            }
            else
            {
                trackEntry1 = _skeletonGraphic.AnimationState.AddAnimation(trackIndex, name, loop, 0);
                trackEntry1.TimeScale = scale;
            }
        }

        /// <summary>
        /// 动画翻转
        /// </summary>
        public void SetAnimationFlipX(bool set)
        {
            //Debug.LogError("~~~朝向设置： " + set);
            _skeletonGraphic.Skeleton.ScaleX = set ? 1 : -1;
        }
        public void SetAnimationFlipY(bool set)
        {
            _skeletonGraphic.Skeleton.ScaleY = set ? 1 : -1;
        }
        public bool GetAnimationFlipX()
        {
            return _skeletonGraphic.Skeleton.ScaleX == 1;
        }

        public void SetAnimationFlipX()
        {
            _skeletonGraphic.Skeleton.ScaleX *= -1;
        }

        public void SetTimeScale(float scale)
        {
            _skeletonGraphic.timeScale = scale;
        }

        //public Bone FindBone(string name)
        //{
        //    return _skeletonGraphic.skeleton.FindBone(name);
        //}

        //public Slot FindSlot(string name)
        //{
        //    return _skeletonGraphic.skeleton.FindSlot(name);
        //}

        //设置骨骼坐标
        public void SetPosition(Bone bone, Vector3 point)
        {
            var skeletonSpacePoint = _skeletonGraphic.transform.InverseTransformPoint(point);
            if (_skeletonGraphic.Skeleton.ScaleX == 1)
                skeletonSpacePoint.x *= -1;
            bone.SetLocalPosition(skeletonSpacePoint);
        }

        /// <summary>
        /// 设置皮肤
        /// </summary>
        public void SetSkin(string skinName)
        {
            _skeletonGraphic.Skeleton.SetSkin(skinName);
            _skeletonGraphic.Skeleton.SetSlotsToSetupPose();
        }

        #endregion




        #region 在线累计获得金币

        

        #endregion
    }

}
