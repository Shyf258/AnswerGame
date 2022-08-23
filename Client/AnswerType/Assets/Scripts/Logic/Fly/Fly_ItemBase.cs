using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum EFlyPathType
{
    Line,
    Bezier,
}

public class Fly_ItemBase : GL_ObjectBase
{
    
    
    protected Action OnEnd;

    
    public virtual void FinishTargetCount()
    {
        
    }

    public virtual void SetIcon(Sprite icon)
    {
    }

    public virtual void SetPos(Vector3 pos)
    {
    }

    public virtual void SetEndPos(Vector3 pos)
    {
    }

    public virtual void SetRote(Vector3 rot)
    {
    }

    public virtual void ToPos(Vector3 pos, Action onEnd)
    {
        OnEnd = onEnd;
    }
    
    public virtual void ToPos(Vector3 pos, Action onEnd,EFlyPathType pathType = EFlyPathType.Line)
    {
        OnEnd = onEnd;
    }

    public virtual bool OnFixedUpdate()
    {
        return true;
    }

    public virtual void SetFlyTimer(float time)
    {
        
    }
    
    public virtual void SetDelayTimer(float time)
    {
        
    }

    public override void Resource_Activate()
    {
        OnEnd -= OnEnd;
        base.Resource_Activate();
    }

    public override void Resource_Destory()
    {
        OnEnd -= OnEnd;
        base.Resource_Destory();
    }
}



[Serializable]
public class Fly_Logic
{
    private float _timer;

    [GL_Name("x轴位移曲线")]
    public AnimationCurve moveXCurve;
    [GL_Name("y轴位移曲线")]
    public AnimationCurve moveYCurve;

    [GL_Name("延时")] public float delay;
    [GL_Name("时间曲线")] public AnimationCurve timeCurve;
    [GL_Name("飞行时间")]
    public float flyTimer = 2f;
    [GL_Name("偏移")] public Vector3 offset;
    [Space]
    [Space]
    [Space]
    [GL_Name("moveBody")] public RectTransform moveBody;
    private Vector3 _startPos, _endPos;
    private bool _isDelay;
    private bool _isFinish;
    
    public void ToPos(Vector3 endPos)
    {
        moveBody.anchoredPosition  = moveBody.anchoredPosition + (Vector2)offset;
        _startPos = moveBody.anchoredPosition;
        _endPos = endPos ;
        _isDelay = true;
        _timer = delay;
        _isFinish = false;
    }
    
    public bool DoUpdate(float dt)
    {
        if (_isFinish) return false;
        
        if (_isDelay)
        {
            _timer -= dt;
            if (_timer<=0)
            {
                _timer = flyTimer;
                _isDelay = false;
            }
        }
        else if (_timer>=0)
        {
            _timer -= dt;
            var t = 1 - _timer / flyTimer;
            t = timeCurve.Evaluate(t);
            var curveX = moveXCurve.Evaluate(t);
            var curveY = moveYCurve.Evaluate(t);
            Vector3 dir = _endPos - _startPos;
            moveBody.anchoredPosition = _startPos + new Vector3(dir.x * curveX, dir.y * curveY);
            if (_timer<=0)
            {
                moveBody.anchoredPosition = _endPos;
                _isFinish = true;
                return false;
            }
        }
        return true;
    }

}
