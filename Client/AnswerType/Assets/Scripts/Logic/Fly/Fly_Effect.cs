using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Fly_Effect : Fly_ItemBase
{
    private float _timer;

    private Vector3 _startPos, _endPos;

    private Vector3 _startRot, _endRot;


    [GL_Name("x轴位移曲线")]
    public AnimationCurve moveXCurve;
    [GL_Name("y轴位移曲线")]
    public AnimationCurve moveYCurve;

    [GL_Name("y轴偏移高度")]
    public float offsetYValue;
    [GL_Name("y轴偏移曲线(曲线首尾为0)")]
    public AnimationCurve offsetYCure;

    [GL_Name("icon")]
    public SpriteRenderer icon;

    [GL_Name("时间曲线")]
    public AnimationCurve timeCurve;

    [GL_Name("飞行时间")]
    public float flyTimer = 2f;
    [GL_Name("延时")] public float delay = 0;
    private bool _isDelay;

    [GL_Name("是否随机")]
    public bool isRandom = false;
    public List<AnimationCurve> randomsCurves;

    [GL_Name("是否旋转")]
    public bool isRotate = false;
    

    private AnimationCurve _curCurveX;

    public override void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }


    public override void SetPos(Vector3 pos)
    {
        transform.position = pos;
        _curCurveX = (isRandom && randomsCurves.Count > 0) ? randomsCurves[Random.Range(0, randomsCurves.Count)] : moveXCurve;
    }

    public override void SetEndPos(Vector3 pos)
    {
        _endPos = pos;
    }

    public override void ToPos(Vector3 pos, Action onEnd)
    {
        base.ToPos(pos, onEnd);
        _timer = flyTimer;
        _isDelay = true;
        _timer = delay;

        _endPos = pos;
    }

    public override void SetRote(Vector3 rot)
    {
        base.SetRote(rot);
        _endRot = rot;
    }

    public override bool DoUpdate(float dt)
    {
        if (_isDelay)
        {
            _timer -= dt;
            if (_timer <= 0)
            {
                _startPos = transform.position;
                _startRot = transform.localEulerAngles;

                _timer = flyTimer;
                _isDelay = false;
            }
        }
        else if (_timer > 0)
        {
            _timer -= dt;
            var t = 1 - _timer / flyTimer;
            t = timeCurve.Evaluate(t);
            var curveX = _curCurveX?.Evaluate(t) ?? moveXCurve.Evaluate(t);
            var curveY = moveYCurve.Evaluate(t);
            
            Vector3 dir = _endPos - _startPos;
            //Debug.Log(transform.position);
            transform.position = _startPos + new Vector3(dir.x * curveX, dir.y * curveY + offsetYCure.Evaluate(t) * offsetYValue);
            if (isRotate)
            {
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(_startRot), Quaternion.Euler(_endRot), t);
            }
            if (_timer <= 0)
            {
                transform.position = _endPos;
                OnEnd?.Invoke();
                return false;
            }
        }
        return true;
    }

}
