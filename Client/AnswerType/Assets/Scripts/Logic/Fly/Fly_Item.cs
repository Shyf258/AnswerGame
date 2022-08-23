using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Fly_Item : Fly_ItemBase
{
    private float _timer;

    private Vector3 _startPos, _endPos;
    private Vector3 _startRot, _endRot;

    private RectTransform _transform;
    [GL_Name("x轴位移曲线")]
    public AnimationCurve moveXCurve;
    [GL_Name("y轴位移曲线")]
    public AnimationCurve moveYCurve;

    [GL_Name("icon")] public Image iconSprite;

    [GL_Name("时间曲线")] public AnimationCurve timeCurve;

    [GL_Name("是否旋转")]
    public bool isRotate = false;
    
    #region 飞行时间
    [GL_Name("飞行时间")]
    public float flyTimer = 2f;

    [HideInInspector] public float bezierTimer;
    #endregion
   
    [GL_Name("延时")] public float delay = 0;
    private bool _isDelay;
    [GL_Name("结束后等待时间")] public float endDelay = 0;
    private bool _isEndDelay;

    #region 随机

    [GL_Name("是否随机")] public bool isRandom = false;
    public List<AnimationCurve> randomsCurves;

    #endregion
    

    private AnimationCurve _curCurveX;
    private Image _followImg;
    private bool _isFollow;
    private bool _isFlying;

    private EFlyPathType _pathType;
    private RectTransform moveBody
    {
        get
        {
            if (!_transform)
            {
                _transform = GetComponent<RectTransform>();
            }
            return _transform;
        }
    }


    public override void FinishTargetCount()
    {
        
    }

    public override void SetIcon(Sprite icon)
    {
        if(iconSprite)
            iconSprite.sprite = icon;
    }

    public override void SetFlyTimer(float time)
    {
        base.SetFlyTimer(time);
        flyTimer = time;
    }
    
    public override void SetDelayTimer(float time)
    {
        base.SetDelayTimer(time);
        delay = time;
    }

    public void SetFollow(Image tf,Action followCall)
    {
        _followImg = tf;
        _isFollow = true;
        _isFlying = false;
        if (iconSprite && _followImg)
        {
            iconSprite.sprite = _followImg.sprite;
            iconSprite.color = _followImg.color;
            iconSprite.transform.position = _followImg.transform.position;
            iconSprite.transform.localScale = _followImg.transform.localScale;
        }
    }

    public override void SetPos(Vector3 pos)
    {
        SetMoveBodyPos(pos);
        _curCurveX = (isRandom && randomsCurves.Count > 0) ? randomsCurves[Random.Range(0, randomsCurves.Count)] : moveXCurve;

    }

    public override void ToPos(Vector3 pos,Action onEnd )
    {
        base.ToPos(pos, onEnd);
        _timer = flyTimer;
        _isDelay = true;
        _timer = delay;
        _isEndDelay = false;
        _endPos = pos;
    } 
    public override void ToPos(Vector3 pos,Action onEnd , EFlyPathType pathType = EFlyPathType.Line)
    {
        base.ToPos(pos, onEnd,pathType);
        _pathType = pathType;
        _timer = flyTimer;
        _isDelay = delay > 0;
        _timer = _isDelay ? delay : flyTimer;
        _isEndDelay = false;
        _endPos = pos;
        if(!_isDelay && pathType == EFlyPathType.Bezier)
            OnBezierEnter(transform.position, _endPos);
    }
    
    public override void SetRote(Vector3 rot)
    {
        base.SetRote(rot);
        _endRot = rot;
    }

    
    
    public override bool DoUpdate(float dt)
    {
        if (_pathType != EFlyPathType.Line)
        {
            return true;
        }
        if (_isFollow )
        {
            if (_followImg && iconSprite)
            {
                iconSprite.sprite = _followImg.sprite;
                iconSprite.color = _followImg.color;
                if (!_isFlying)
                    iconSprite.transform.position = _followImg.transform.position;
                iconSprite.transform.localScale = _followImg.transform.localScale;
            }
        }
        if (_isDelay)
        {
            _timer -= dt;
            if (_timer<=0)
            {
                
                if (moveBody)
                {
                    _startPos = moveBody.anchoredPosition;
                }
                else
                {
                    _startPos = transform.localPosition;
                }
                _timer = flyTimer;
                _isDelay = false;
            }
        }
        else if (_timer>0 && !_isEndDelay)
        {
            _isFlying = true;
            _timer -= dt;
            var t = 1 - _timer / flyTimer;
            t = timeCurve.Evaluate(t);
            var curveX = _curCurveX?.Evaluate(t) ?? moveXCurve.Evaluate(t);
            var curveY = moveYCurve.Evaluate(t);
            Vector3 dir = _endPos - _startPos;
            SetMoveBodyPos(_startPos + new Vector3(dir.x * curveX, dir.y * curveY));
            if (isRotate)
            {
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(_startRot), Quaternion.Euler(_endRot), t);
            }
            if (_timer<=0)
            {
                SetMoveBodyPos(_endPos);
                OnEnd?.Invoke();
                _isEndDelay = true;
                _timer = endDelay;
            }
        }

        if (_isEndDelay)
        {
            _timer -= dt;
            if (_timer<=0)
            {
                _isEndDelay = false;
                return false;
            }
        }
        return true;
    }
    

    private void SetMoveBodyPos(Vector3 pos)
    {
        if (moveBody)
        {
            moveBody.anchoredPosition = pos;
        }
        else
        {
            transform.localPosition = pos;
        }
    }


    #region 曲线位移

    private List<Vector3> _pathVector3s;
    private int _curMoveIndex;
    private float _bezierMoveTimer,_bezierStaticTime;
    private float _bezierSpeed;

    public override bool OnFixedUpdate()
    {
        if (_pathType != EFlyPathType.Bezier)
        {
            return true;
        }
        if (_isDelay)
        {
            _timer -= Time.fixedDeltaTime;
            if (_timer<=0)
            {
                
                OnBezierEnter(transform.position, _endPos);
                _isDelay = false;
            }
        }
        else if (_timer>0)
        {
            return OnUpdateFly();
        }

        return true;
    }

    private void OnBezierEnter(Vector3 startPos, Vector3 endPos)
    {
        _bezierSpeed = (endPos - startPos).magnitude / flyTimer;
        
        float flyT = 0;
        _pathVector3s =  GL_Tools.GetPathPoints(startPos, endPos,_bezierSpeed,ref flyT);
        _timer = flyT;
        var ani = GetComponentInChildren<Animator>();
        if (ani) ani.speed = flyTimer / flyT;
        bezierTimer = flyT;
        CalculateNextPos();
    }
    
    private void CalculateNextPos()
    {
        float distance = 0;
        _startPos = transform.position;
        if(isRotate)
            _startRot = transform.up;
        if (_curMoveIndex < _pathVector3s.Count - 1)
        {
            distance = (_pathVector3s[_curMoveIndex + 1] - _pathVector3s[_curMoveIndex]).magnitude;
            _endPos = _pathVector3s[_curMoveIndex + 1];
            if(isRotate)
                _endRot = _pathVector3s[_curMoveIndex + 1] - _pathVector3s[_curMoveIndex];
        }
       
        _bezierMoveTimer = distance / _bezierSpeed;

        _bezierStaticTime = _bezierMoveTimer;
    }

    public bool OnUpdateFly()
    {
        if (_curMoveIndex >= _pathVector3s.Count - 1 )
        {
            OnEnd?.Invoke();
            return false;
        }

        _bezierMoveTimer -= Time.fixedDeltaTime;
        var t = 1 - _bezierMoveTimer / _bezierStaticTime;
        transform.position = Vector3.Lerp(_startPos, _endPos, t);
        if (isRotate)
            transform.up = Vector3.Lerp(_startRot, _endRot, t);
        if (_bezierMoveTimer <=0)
        {
            _curMoveIndex++;
            transform.position = _endPos;
            if (isRotate)
                transform.up = _endRot;
            CalculateNextPos();
        }
        return true;
    }
    #endregion

}
