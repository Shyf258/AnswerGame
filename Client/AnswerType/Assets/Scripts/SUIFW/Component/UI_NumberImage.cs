//2022.1.26 关林
//图片字

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NumberImage : MonoBehaviour
{
    public List<Sprite> _sprites;

    [GL_Name("小数点")]
    public Sprite _pointSprite;
    [GL_Name("货币单位")]
    public Sprite _monetarySprite;

    private bool _isUnit;   //是否创建单位
    private List<Image> _images = new List<Image>();
    private Image _number;
    private float _curNumber = 99999999;

    private RectTransform _root;

    public void Init(bool isUnit = false)
    {
        _isUnit = isUnit;
        if (_root == null)
        {
            _root = transform.GetChild(0) as RectTransform;
        }
    }
    public void SetNumber(float number)
    {
        if (_curNumber == number)
            return;

        if (_images == null)
            _images = new List<Image>();

        for (int i = 0; i < _images.Count; i++)
        {
            GameObject.DestroyImmediate(_images[i].gameObject);
        }
        _images.Clear();

        _curNumber = number;
        string num = number.ToString();
        int value;
        foreach (var s in num)
        {
            if(int.TryParse(s.ToString(), out value))
            {
                //创建图片
                _images.Add(CreateImage(_sprites[value]));
            }
            else if (s.ToString() == ".")
            {
                _images.Add(CreateImage(_pointSprite));
            }
        }
        //创建单位
        if(_isUnit)
            _images.Add(CreateImage(_monetarySprite));

        LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
    }

    private Image CreateImage(Sprite sprite)
    {
        GameObject obj = new GameObject();
        Image image = obj.AddComponent<Image>();

        image.sprite = sprite;
        GL_Tools.TransformMakeZero(obj.transform, _root);
        image.SetNativeSize();
        image.transform.localScale = new Vector3(1.5f,1.5f);
        return image;
    }
}