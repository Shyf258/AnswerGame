//UGUI Text组件 文字竖版

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class VirticalText : MonoBehaviour
{
    //[GL_Name("字体尺寸")]
    private int _textSize; //字体尺寸

    private VerticalLayoutGroup _layoutGroup;   //排序
    private List<Text> _textList = new List<Text>();    //所有文字列表

    private float _sizePre = 1.15f; //文字尺寸和UI显示的比例
    public void Init()
    {
        _layoutGroup = GetComponent<VerticalLayoutGroup>();
        _textList = GetComponentsInChildren<Text>().ToList();
        if(_textList.Count == 0)
        {
            //一个也没有, 则创建
            _textList.Add(CreateText(null));
        }
        _textSize = _textList[0].fontSize;

        foreach (var text in _textList)
        {
            text.GetComponent<RectTransform>().sizeDelta = Vector2.one * _textSize * _sizePre;
        }
    }

    //根据文字长度, 设置竖版
    public void RefreshText(string str)
    {
        int strLength = str.Length;
        
        if(strLength > _textList.Count)
        {
            //需要补
            int i = _textList.Count;
            for (; i < strLength; i++)
            {
                _textList.Add(CreateText(_textList[0]));
            }
        }
        else
        {
            //需要隐藏多余的
            for (int i = strLength; i < _textList.Count; i++)
            {
                _textList[i].gameObject.SetActive(false);
            }
        }

        //刷新文字
        for (int i = 0; i < _textList.Count; i++)
        {
            _textList[i].text = str[i].ToString();
        }
        
    }


    private Text CreateText(Text text)
    {
        if(text == null)
        {
            text = new GameObject("text").AddComponent<Text>();
            text.transform.parent = transform;
            return text;
        }
        else
        {
            return GameObject.Instantiate(text, transform);
        }


    }

}