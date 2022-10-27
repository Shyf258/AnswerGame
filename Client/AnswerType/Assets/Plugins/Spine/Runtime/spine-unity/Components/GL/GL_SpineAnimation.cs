//2019.12.06
//Spine动画控制器    关林

using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_SpineAnimation : MonoBehaviour
{
    private SkeletonAnimation _skeletonAnimation;
    private TrackEntry trackEntry0;   //0动画通道,主通道
    private TrackEntry trackEntry1;   //1当前动画通道
    //private SkeletonRenderer _renderer;
    private MeshRenderer _renderer;

    public void Init()
    {
        _skeletonAnimation = gameObject.GetComponentInChildren<SkeletonAnimation>();
        _renderer = _skeletonAnimation.GetComponent<MeshRenderer>();
        _skeletonAnimation.AnimationState.Start += ActionStart;// (entry) => ActionStart(entry);
        _skeletonAnimation.AnimationState.Complete += ActionComplete;// (entry) => ActionComplete(entry);
        PlayAnimation(null, true, 0);   //默认停止
    }
    

    private void ActionComplete(TrackEntry entry)
    {
        //ActionComplete();
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
    public void PlayAnimation(string name, bool loop = true, int trackIndex = 0, float scale = 1f, float trackTime = 0)
    {
        if (!string.IsNullOrEmpty(name))
        {
            if (_skeletonAnimation.AnimationState == null)
            {
                Debug.LogError("~~~初始化动作播放异常");
                _skeletonAnimation.name = name;
                trackEntry0 = _skeletonAnimation.AnimationState.GetCurrent(0);
                return;
            }
            if (0 == trackIndex)
            {
                trackEntry0 = _skeletonAnimation.AnimationState.SetAnimation(trackIndex, name, loop);
                trackEntry0.TimeScale = scale;
                trackEntry0.TrackTime = trackTime;
            }
            else
            {
                trackEntry1 = _skeletonAnimation.AnimationState.SetAnimation(trackIndex, name, loop);
                trackEntry1.TimeScale = scale;
                trackEntry1.TrackTime = trackTime;
            }
        }
        else if(trackIndex != 1)
        {
            _skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndex, 0);
        }
        
    }

    //停止动画
    public void StopAnimation(float minDuration)
    {
        _skeletonAnimation.AnimationState.SetEmptyAnimation(0, minDuration);
    }

    /// <summary>
    /// 动画翻转
    /// </summary>
    public void SetAnimationFlipX(bool set)
    {
        //Debug.LogError("~~~朝向设置： " + set);
        _skeletonAnimation.Skeleton.ScaleX = set ? 1 : -1;
    }
    public void SetAnimationFlipY(bool set)
    {
        _skeletonAnimation.Skeleton.ScaleY = set ? 1 : -1;
    }
    public bool GetAnimationFlipX()
    {
        return _skeletonAnimation.Skeleton.ScaleX == 1;
    }

    public void SetAnimationFlipX()
    {
        _skeletonAnimation.Skeleton.ScaleX *= -1;
    }

    public void SetTimeScale(float scale)
    {
        _skeletonAnimation.timeScale = scale;
    }

    public void SetOrder(int order)
    {
        if(!_renderer) return;
        _renderer.sortingOrder = order;
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
        var skeletonSpacePoint = _skeletonAnimation.transform.InverseTransformPoint(point);
        if (_skeletonAnimation.Skeleton.ScaleX == 1) 
            skeletonSpacePoint.x *= -1;
        bone.SetLocalPosition(skeletonSpacePoint);
    }
    
    
    public void ClearLastAnimation() 
    {
        // 清除动画插值
        _skeletonAnimation.Skeleton.SetToSetupPose();
        _skeletonAnimation.AnimationState.ClearTracks();
    }
    #endregion
}
