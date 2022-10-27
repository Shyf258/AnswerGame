//2018.12.07    关林
//添加水印

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GL_Watermark : Singleton<GL_Watermark>
{

    public string _textPath = "Image/UI/icon_back_to_home";
    /// <summary>
        /// 添加水印
        /// </summary>
        /// <param name="background">背景图</param>
        /// <param name="watermark">水印</param>
        /// <param name="foffsetX">x偏移量</param>
        /// <param name="offsetY">y偏移量</param>
        /// <returns>结果图</returns>
    public Texture2D AddWatermark(Texture2D background, Texture2D watermark, int foffsetX, int offsetY)
    {
        int startX = background.width - watermark.width - foffsetX;
        int endX = startX + watermark.width;
        int startY = offsetY;
        int endY = startY + watermark.height;

        
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Color bgColor = background.GetPixel(x, y);
                Color wmColor = watermark.GetPixel(x - startX, y - startY);
                Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);
                background.SetPixel(x, y, final_color);
            }
        }
        background.Apply();
        return background;
    }


    /// <summary>
    /// 文字转图片
    /// </summary>
    /// <param name="text">文字内容</param>
    /// <param name="fontSize">字体大小</param>
    /// <param name="paddingLeft">左边距</param>
    /// <param name="paddingTop">上边距</param>
    /// <param name="textColor">字体颜色</param>
    //public Texture2D TextToPicture(string text, float fontSize, int paddingLeft, int paddingTop, System.Drawing.Color textColor)
    //{
    //    System.Drawing.Font textFont = new System.Drawing.Font("宋体", fontSize);
    //    System.Drawing.Bitmap bm = new System.Drawing.Bitmap((int)(text.Length * (fontSize + 40)), (int)(fontSize + 40));
    //    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);
    //    System.Drawing.Brush b = new System.Drawing.SolidBrush(textColor);
    //    g.DrawString(text, textFont, b, paddingLeft, paddingTop);
    //    bm.Save(Application.persistentDataPath + "/UserName.png", System.Drawing.Imaging.ImageFormat.Png);
    //    Texture2D tex = GetPicture(Application.persistentDataPath + "/UserName.png");
    //    //File.Delete(Application.persistentDataPath + "/UserName.png");
    //    g.Dispose();
    //    bm.Dispose();
    //    return tex;
    //}


    /// <summary>
    /// 文件流加载图片
    /// </summary>
    /// <param name="path">路径</param>
    private Texture2D GetPicture(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileStream fs = new FileStream(path, FileMode.Open);
        byte[] buffer = new byte[fs.Length];
        fs.Read(buffer, 0, buffer.Length);
        fs.Close();
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(buffer);
        tex.Apply();
        return tex;
    }

    public void WriteFileByLine(string file_path,string file_name,string str_info)
    {
        StreamWriter sw;
        if (!File.Exists(file_path + "//" + file_name))
        {
            sw = File.CreateText(file_path + "//" + file_name);//创建一个用于写入 UTF-8 编码的文本  
            Debug.Log("文件创建成功！");
        }
        else
        {
            sw = File.AppendText(file_path + "//" + file_name);//打开现有 UTF-8 编码文本文件以进行读取  
        }
        sw.WriteLine(str_info);//以行为单位写入字符串  
        sw.Close();
        sw.Dispose();//文件流释放  
    }

}
