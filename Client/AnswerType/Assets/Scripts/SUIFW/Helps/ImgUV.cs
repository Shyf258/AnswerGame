#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：ImgUV
// 创 建 者：Yangsong
// 创建时间：2022年01月24日 星期一 12:11
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Helps
{
    public class ImgUV : MonoBehaviour
    {
        private RawImage _image;
        private float _playSpeed = 0.1f;
        private float _rectX;

        private void Awake()
        {
            _image = GetComponentInChildren<RawImage>();
        }

        private void Update()
        {
            _image.uvRect = new Rect(_rectX, 0, 1, 1);
            _rectX += Time.deltaTime * _playSpeed;
            //if (_rectX >= value)
            //{
            //    _rectX -= value;
            //}
        }
    }
}