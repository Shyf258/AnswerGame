//2021.11.23    关林
//适配text

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FItText : MonoBehaviour
{
    private int _textSizeMinWidth = 100;    //最小宽度
    private int _textSizeMaxWidth = 500;    //最大宽度

    private bool _isUpdate; //是否需要持续刷新
    private Text _text; //组件
    private string _content;    //内容
    public void Init(Text text, int minWidth, int maxWidth, bool isUpdate = false)
    {
        _text = text;
        _content = text.text;

        _textSizeMinWidth = minWidth;
        _textSizeMaxWidth = maxWidth;

        _isUpdate = isUpdate;
        SetTextSize();
    }

    public void RefreshText()
    {
        SetTextSize();
    }

    void Update()
    {
        if (!_isUpdate)
            return;
        if (_text == null || _text.text == _content)
            return;

        SetTextSize();
    }

    private void SetTextSize()
    {
        if (_text == null)
            return;

        //宽高都不缩放
        if (_text.preferredWidth <= _textSizeMinWidth)
            return;

        //宽度缩放，高度不变
        if (_text.preferredWidth <= _textSizeMaxWidth)
        {
            _text.rectTransform.sizeDelta = new Vector2(_text.preferredWidth, _text.rectTransform.sizeDelta.y);
            return;
        }

        //宽度最大，高度缩放
        //设置最大宽度
        _text.rectTransform.sizeDelta = new Vector2(_textSizeMaxWidth, _text.rectTransform.sizeDelta.y);
        //设置最优高度
        int textSizeHeight = Mathf.CeilToInt(_text.preferredHeight);
        _text.rectTransform.sizeDelta = new Vector2(_textSizeMaxWidth, textSizeHeight);
    }
}
