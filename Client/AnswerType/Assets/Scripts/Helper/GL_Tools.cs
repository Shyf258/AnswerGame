//2018.09.25    关林
//工具类

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

public class GL_Tools
{
    //计算带正负值的角度
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Transform 归零
    /// </summary>
    public static void TransformMakeZero(Transform tf, Transform parent = null)
    {
        if (tf == null) return;
        if (parent != null)
            tf.transform.SetParent(parent);
        tf.localPosition = Vector3.zero;
        tf.localEulerAngles = Vector3.zero;
        tf.localScale = Vector3.one;
    }

    //查找Transform 子集的对象
    public static Transform FindTransform(Transform tf, string name)
    {
        Transform result = tf.Find(name);
        if (result == null)
        {
            for (int i = 0; i < tf.childCount; i++)
            {
                result = FindTransform(tf.GetChild(i), name);
                if (result != null)
                    break;
            }
        }

        return result;
    }


    //获取长度方法
    public static int GetStrLength(string str)
    {
        if (string.IsNullOrEmpty(str)) return 0;
        ASCIIEncoding ascii = new ASCIIEncoding();
        int tempLen = 0;
        byte[] s = ascii.GetBytes(str);
        for (int i = 0; i < s.Length; i++)
        {
            if ((int)s[i] == 63)
            {
                tempLen += 2;
            }
            else
            {
                tempLen += 1;
            }
        }

        return tempLen;
    }

    //设置所有子集 层级
    public static void SetLayer(GameObject obj, string layer)
    {
        SetLayer(obj, LayerMask.NameToLayer(layer));
    }

    public static void SetLayer(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform tf in obj.transform)
        {
            if (tf == obj.transform)
                continue;
            SetLayer(tf.gameObject, layer);
        }
    }

    public static SortingLayer GetSortingLayer(string name)
    {
        foreach (var layer in SortingLayer.layers)
        {
            if (layer.name == name)
                return layer;
        }

        return SortingLayer.layers[0];
    }

    /// <summary>
    /// 平滑方法
    /// </summary>
    public static float SmoothApproach(float curValue, float targetValue, float speed, float deltaTime)
    {
        float t = deltaTime * speed;
        //float v = (targetValue - curValue) / t; 
        //float f = curValue - targetValue + v;
        return targetValue - (targetValue - curValue) * Mathf.Exp(-t);
    }

    public static Vector3 SmoothApproach(Vector3 curValue, Vector3 targetValue, float speed, float deltaTime)
    {
        float t = deltaTime * speed;
        //float v = (targetValue - curValue) / t; 
        //float f = curValue - targetValue + v;
        return targetValue - (targetValue - curValue) * Mathf.Exp(-t);
    }

    public static float DampenFactor(float dampening, float elapsed)
    {
        if (dampening < 0.0f)
        {
            return 1.0f;
        }
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            return 1.0f;
        }
#endif
        return 1.0f - Mathf.Pow((float)System.Math.E, -dampening * elapsed);
    }

    public static void AddInt(ref List<int> list, int a, int b, int c)
    {
        list.Add(a);
        list.Add(b);
        list.Add(c);
    }

    //GetComponent, 没有则添加
    public static T GetComponent<T>(GameObject obj) where T : Component
    {
        T t = obj.GetComponent<T>();
        if (t == null)
            t = obj.AddComponent<T>();
        return t;
    }

    static List<string> patten = new List<string>()
    {
        @"\p{Cs}",
        @"\p{Co}",
        @"\p{Cn}",
        @"[\u2700-\u27BF]",
        @"[\u2600-\u26FF]",

    };
    /// <summary>
    /// Emoji过滤
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string FilterEmoji(string str)
    {
        for (int i = 0; i < patten.Count; i++)
        {
            str = Regex.Replace(str, patten[i], "");//屏蔽emoji   
        }
        return str;
    }



    /// <summary>
    /// Emoji过滤
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsHaveEmoji(string str)
    {
        if (patten.Count > 0)
        {
            bool bEmoji = false;
            for (int i = 0; i < patten.Count; ++i)
            {
                bEmoji = Regex.IsMatch(str, patten[i]);
                if (bEmoji)
                {
                    break;
                }
            }

            return bEmoji;
        }
        else
        {
            return false;
        }
    }

    //string转 vector3
    public static Vector3 GetVectorByString(string str, char c)
    {
        Vector3 reslut = Vector3.zero;
        string[] token = str.Split(c);

        try
        {
            reslut = new Vector3(int.Parse(token[0]), int.Parse(token[1]), int.Parse(token[2]));
        }
        catch (Exception)
        {
        }

        return reslut;
    }

    //通过随机权重, 计算随机结果
    public static int CalculateRandomWeight(List<int> weightList)
    {
        if (weightList == null || weightList.Count == 0)
            return 0;

        int result = 0;
        int randomWeight = 0;
        foreach (int value in weightList)
        {
            randomWeight += value;
        }

        randomWeight = UnityEngine.Random.Range(0, randomWeight);


        for (int i = 0; i < weightList.Count; ++i)
        {
            if (randomWeight < weightList[i])
            {
                //随机成功
                result = i;
                break;
            }
            else
                randomWeight -= weightList[i];
        }

        return result;
    }

    /// <summary>
    /// 获取文字坐标
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="text"></param>
    /// <param name="strFragment"></param>
    /// <returns></returns>
    public static Vector3 GetPosAtText(Canvas canvas, Text text, string strFragment)
    {
        int strFragmentIndex = text.text.IndexOf(strFragment);//-1表示不包含strFragment
        Vector3 stringPos = Vector3.zero;
        if (strFragmentIndex > -1)
        {
            Vector3 firstPos = GetPosAtText(canvas, text, strFragmentIndex + 1);
            Vector3 lastPos = GetPosAtText(canvas, text, strFragmentIndex + strFragment.Length);
            stringPos = (firstPos + lastPos) * 0.5f;
        }
        else
        {
            stringPos = GetPosAtText(canvas, text, strFragmentIndex);
        }
        return stringPos;
    }

    /// <summary>
    /// 得到Text中字符的位置；canvas:所在的Canvas，text:需要定位的Text,charIndex:Text中的字符位置
    /// </summary>
    public static Vector3 GetPosAtText(Canvas canvas, Text text, int charIndex)
    {
        string textStr = text.text;
        Vector3 charPos = Vector3.zero;
        if (charIndex <= textStr.Length && charIndex > 0)
        {
            TextGenerator textGen = new TextGenerator(textStr.Length);
            Vector2 extents = text.gameObject.GetComponent<RectTransform>().rect.size;
            textGen.Populate(textStr, text.GetGenerationSettings(extents));

            int newLine = textStr.Substring(0, charIndex).Split('\n').Length - 1;
            int whiteSpace = textStr.Substring(0, charIndex).Split(' ').Length - 1;
            int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - 4;
            if (indexOfTextQuad < textGen.vertexCount)
            {
                charPos = (textGen.verts[indexOfTextQuad].position +
                    textGen.verts[indexOfTextQuad + 1].position +
                    textGen.verts[indexOfTextQuad + 2].position +
                    textGen.verts[indexOfTextQuad + 3].position) / 4f;


            }
        }
        charPos /= canvas.scaleFactor;//适应不同分辨率的屏幕
        charPos = text.transform.TransformPoint(charPos);//转换为世界坐标
        return charPos;
    }

    /// <summary>
    /// 点击屏幕坐标
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static GameObject GetFirstPickGameObject(Vector2 position)
    {
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = position;
        //射线检测ui
        List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, uiRaycastResultCache);
        if (uiRaycastResultCache.Count > 0)
            return uiRaycastResultCache[0].gameObject;
        return null;
    }


    //通过随机权重, 计算随机结果(可能没有随机到
    //0-1的随机权重
    public static int CalculateRandomWeight1(List<float> weightList)
    {
        int result = -1;
        //float randomWeight = 0;
        //foreach (float value in weightList)
        //{
        //    randomWeight += value;
        //}

        float randomResult = UnityEngine.Random.Range(0, 1f);


        for (int i = 0; i < weightList.Count; ++i)
        {
            if (randomResult < weightList[i])
            {
                //随机成功
                result = i;
                break;
            }
            else
                randomResult -= weightList[i];
        }

        //result = -1则说明没有随机到
        return result;
    }

    //随机抽出 数值的值
    public static T RandomList<T>(List<T> list)
    {
        int count = list.Count;
        count = Random.Range(0, count);
        return list[count];
    }

    /// <summary>
    /// 数组乱序排列
    /// </summary>
    public static List<T> ListRandom<T>(List<T> tempList)
    {
        if (tempList == null) return new List<T>();
        System.Random rand = new System.Random();
        int k = 0;
        T tmp;
        for (int i = 0; i < tempList.Count; i++)
        {
            k = rand.Next(0, tempList.Count);
            if (k != i) //换位置
            {
                tmp = tempList[i];
                tempList[i] = tempList[k];
                tempList[k] = tmp;
            }
        }

        return tempList;
    }

    /// <summary>
    /// 是否有网
    /// </summary>
    /// <returns></returns>
    public static bool IsInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return false;
        return true;
    }

    //长数组, 随机成短数组
    public static List<T> ListRandom<T>(List<T> tempList, int number)
    {
        if (tempList.Count < number)
            return tempList;
        if (tempList.Count == number)
            return ListRandom(tempList);

        List<T> result = new List<T>();
        do
        {
            int i = Random.Range(0, tempList.Count);
            result.Add(tempList[i]);
            tempList.RemoveAt(i);
        } while (result.Count < number);

        return result;
    }

    public static void Swap<T>(List<T> list, int index1, int index2)
    {
        var temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }

    public static void SetLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                GL_CoreData._instance.Language = ELanguage.CN;
                break;
            case SystemLanguage.ChineseTraditional:
                GL_CoreData._instance.Language = ELanguage.HK;
                break;
            case SystemLanguage.French:
                GL_CoreData._instance.Language = ELanguage.FR;
                break;
            case SystemLanguage.German:
                GL_CoreData._instance.Language = ELanguage.DE;
                break;
            case SystemLanguage.Italian:
                GL_CoreData._instance.Language = ELanguage.IT;
                break;
            case SystemLanguage.Japanese:
                GL_CoreData._instance.Language = ELanguage.JA;
                break;
            case SystemLanguage.Portuguese:
                GL_CoreData._instance.Language = ELanguage.PT;
                break;
            case SystemLanguage.Spanish:
                GL_CoreData._instance.Language = ELanguage.ES;
                break;
            case SystemLanguage.Korean:
                GL_CoreData._instance.Language = ELanguage.KO;
                break;
            case SystemLanguage.English:
            default:
                GL_CoreData._instance.Language = ELanguage.EN;
                break;
        }
    }

    //获取一组指定综合的随机数
    public static int[] GetRandomAndTotalEq(int count, int total, int min, int[] result)
    {
        int random = 0;
        if (count > 1)
        {
            int useTotal = total - (count - 1) * min;
            random = (int)(Random.Range(0f, 1f) * (useTotal - 1) + 1);
        }
        else
        {
            random = total;
        }

        result[count - 1] = random;
        int surplusTotal = total - random;
        count--;
        if (count > 0)
        {
            GetRandomAndTotalEq(count, surplusTotal, min, result);
        }

        return result;
    }

    #region 图片转换
    public static bool SaveTexture2DToPNG(Texture2D tt, string contents, string pngName)
    {
        byte[] bytes = tt.EncodeToPNG();

        if (!Directory.Exists(Application.persistentDataPath + "/" + contents))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + contents);
        FileStream file = File.Open(Application.persistentDataPath + "/" + contents+"/" + pngName, FileMode.OpenOrCreate);
        //FileStream file = File.Open(Application.persistentDataPath + "/" + contents + pngName + ".png", FileMode.OpenOrCreate);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();

        return true;
    }
    public static bool SaveRenderTextureToPNG(RenderTexture rt, string contents, string pngName)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToPNG();

        if (!Directory.Exists(Application.persistentDataPath + "/" + contents))
            Directory.CreateDirectory(Application.persistentDataPath + "/" + contents);
        FileStream file = File.Open(Application.persistentDataPath + "/" + contents + pngName, FileMode.OpenOrCreate);
        //FileStream file = File.Open(Application.persistentDataPath + "/" + contents + pngName + ".png", FileMode.OpenOrCreate);

        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
        Texture2D.DestroyImmediate(png);
        png = null;
        RenderTexture.active = prev;


        return true;
    }
    public static byte[] TextureEncodeToPNG(RenderTexture rt)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToPNG();
        Texture2D.DestroyImmediate(png);
        RenderTexture.active = prev;
        return bytes;
    }
    //屏幕截图
    public static Texture2D Screenshot(int width, int height, int offsetX = 0, int offsetY = 0)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.ReadPixels(
            new Rect((Screen.width - width) / 2 + offsetX, (Screen.height - height) / 2 + offsetY, width, height), 0,
            0);
        tex.Apply();
        return tex;
    }
    public static Texture2D LoadPNGTORenderTexture(string filePath, string pngName)
    {
        if (string.IsNullOrEmpty(pngName))
            return null;
        string totalPath = Application.persistentDataPath + "/" + filePath + pngName;// + ".png";
        //DirectoryInfo mydir = new DirectoryInfo(totalPath);
        if (!System.IO.File.Exists(totalPath))
        {
            return null;
        }

        FileStream fileStream = new FileStream(totalPath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        int width = 512;
        int height = 512;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        return texture;
    }
    public static Texture2D RenderTexture2Texture2D(RenderTexture rt)
    {
        RenderTexture preRT = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = preRT;
        return tex;
    }
    //压缩图片
    public static Texture2D ReSetTextureSize(Texture2D tex, int scale)
    {
        Texture2D texture = new Texture2D(tex.width / scale, tex.height / scale, TextureFormat.RGBA32, true);

        for (int i = 0; i < texture.height; i++) //压缩图片
        {
            for (int j = 0; j < texture.width; j++)
            {
                Color color = tex.GetPixel(j * scale, i * scale);
                texture.SetPixel(j, i, color);
            }
        }

        texture.Apply();
        return texture;
    }
    
    
    /// <summary>
    /// 文件写入
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static bool WriteUpdateFileToDisk(string path, byte[] content)
    {
        try
        {
            string dir = path.Substring(0, path.LastIndexOf("/"));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Write(content, 0, content.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            return true;
        }
        catch (Exception e)
        {
            DDebug.Log("write download file to disk exception " + e.ToString());
        }
        return false;
    }

    #endregion

    //public static GL_Effect PlayEffectWithColor(GL_Effect prefab, Transform tf, Vector3 pos, Color color)
    //{
    //    GL_Effect effect = Gl_EffectManager._instance.PlayEffect(prefab, tf, pos);
    //    Transform b = effect.transform.Find("M_AS_B");
    //    if (b != null)
    //    {
    //        for (int i = 0; i < b.childCount; i++)
    //        {
    //            ParticleSystem eff = b.GetChild(i).GetComponent<ParticleSystem>();
    //            if (eff != null)
    //            {
    //                //ParticleSystem.MinMaxGradient startColor = eff.main.startColor;
    //                //startColor.color = color;
    //                //System.Type type = eff.main.GetType();
    //                //PropertyInfo property = type.GetProperty("startColor");
    //                //property.SetValue(eff.main, startColor, null);
    //                eff.startColor = color;
    //            }
    //        }
    //    }

    //    return effect;
    //}

    #region UI 空间转换

    public static Vector2 MouseToUGUIPosition(Vector3 mouse, RectTransform parent)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,
            mouse, UIUtils.GetUGUICam(), out localPoint);
        return localPoint;
    }

    public static Vector2 WorldToUGUIPosition(Vector3 worldPosition, Camera camera)
    {
        Vector3 screenPoint = camera.WorldToScreenPoint(worldPosition);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIUtils.GetUGUIRoot().GetComponent<RectTransform>(),
            screenPoint, UIUtils.GetUGUICam(), out localPoint);
        return localPoint;
    }

    // public static Vector2 WorldToUGUIPosition(Vector3 worldPosition, RectTransform parent)
    // {
    //     Vector3 screenPoint = GL_Game._instance.CurActiveCamera._camera.WorldToScreenPoint(worldPosition);
    //     Vector2 localPoint;
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, UIUtils.GetUGUICam(),
    //         out localPoint);
    //     return localPoint;
    // }

    public static Vector2 UGUiToUGUIPosition(Vector3 worldPosition, RectTransform parent)
    {
        Vector3 screenPoint = UIUtils.GetUGUICam().WorldToScreenPoint(worldPosition);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, UIUtils.GetUGUICam(),
            out localPoint);
        return localPoint;
    }

    public static Vector2 UGUiScreenToUGUIPosition(Vector3 screenPoint, RectTransform parent)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, UIUtils.GetUGUICam(),
            out localPoint);
        return localPoint;
    }
    #endregion


    /// <summary>
    /// String转Color
    /// </summary>
    /// <param name="value">FFFFFF</param>
    /// <returns></returns>
    public static Color GetColorByString(string value)
    {
        Color result;
        int v = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
        result = new Color(
            //int>>移位 去低位
            //&按位与 去高位
            ((float)(((v >> 16) & 255))) / 255,
            ((float)((v >> 8) & 255)) / 255,
            ((float)((v >> 0) & 255)) / 255
        );

        return result;
    }

    public static D MapperToModel<D>(D d, D s)
    {
        try
        {
            var Typed = typeof(D);
            foreach (PropertyInfo dp in Typed.GetProperties())
            {
                dp.SetValue(d, dp.GetValue(s, null), null);//获得s对象属性的值复制给d对象的属性 
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return d;
    }
    public static D MapperToModel<D, S>(D d, S s)
    {
        try
        {
            var Types = s.GetType();//获得类型  
            var Typed = typeof(D);
            foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段  
            {
                foreach (PropertyInfo dp in Typed.GetProperties())
                {
                    if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")//判断属性名是否相同  
                    {
                        dp.SetValue(d, sp.GetValue(s, null), null);//获得s对象属性的值复制给d对象的属性  
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return d;
    }

    #region 曲线路径

    //获取贝塞尔上得点
    public static Vector3[] GetBezierPoints(Vector3 startPos, Vector3 endPos)
    {
        AnimationCurve curve = new AnimationCurve();
        Vector3[] points = new Vector3[7];
        //var curve = GL_GameData._instance.planeBezierCurve;
        var dir = endPos - startPos;
        var distance = dir.magnitude;
        Vector3 newEnd = new Vector3((dir.x > 0 ? 1 : -1) * distance, 0);
        var isRight = dir.x < 0 ? -1 : 1;
        //计算点
        points[0] = Vector3.zero;
        var point1Param = Random.Range(-0.2f, -0.1f);
        points[1] = distance * new Vector3(0, curve.Evaluate(point1Param));
        var point2Param = Random.Range(0.1f, 0.3f);
        points[2] = distance * new Vector3(point2Param * isRight, curve.Evaluate(point2Param));
        var point3Param = Random.Range(0.4f, 0.6f);
        points[3] = distance * new Vector3(point3Param * isRight, curve.Evaluate(point3Param));
        var point4Param = Random.Range(0.7f, 0.9f);
        points[4] = distance * new Vector3(point4Param * isRight, curve.Evaluate(point4Param));
        var point5Param = Random.Range(1.1f, 1.2f);
        points[5] = distance * new Vector3(1 * isRight, curve.Evaluate(point5Param));
        points[6] = newEnd;
        //计算各个点旋转
        var angle = getAngleV2(dir, newEnd);
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = startPos + Quaternion.AngleAxis(angle, Vector3.forward) * points[i];
        }
        return points;
    }

    private static float getAngleV2(Vector2 v1, Vector2 v2)
    {
        Vector3 cross = Vector3.Cross(v1, v2);
        float angle = Vector2.Angle(v1, v2);
        angle = cross.z > 0 ? -angle : angle;
        return angle;
    }


    //计算出指定个点,将他们练成一条直线,使其开起来像是曲线
    public static List<Vector3> GetPathPoints(Vector3 startPos, Vector3 endPos, float flySpeed, ref float flyTime)
    {

        Vector3[] temp_1 = GetBezierPoints(startPos, endPos);
        var point = new List<Vector3>();//最终贝塞尔曲线上点的链表集合
        float pointNumber = 50;//贝塞尔曲线上点的数量
        Vector3[] temp_2;
        Vector3[] temp_3;

        float tempTime = 0;
        for (int i = 0; i <= (int)pointNumber; i++)
        {
            temp_3 = temp_1;
            for (int j = temp_3.Length - 1; j > 0; j--)
            {
                temp_2 = new Vector3[j];
                for (int k = 0; k < j; k++)
                {
                    temp_2[k] = Vector3.Lerp(temp_3[k], temp_3[k + 1], i / pointNumber);
                }
                temp_3 = temp_2;
            }
            Vector3 find = temp_3[0];
            //计算路程
            if (i != 0)
            {
                var t = (find - point[point.Count - 1]).magnitude / flySpeed;
                t = ((int)(t / Time.fixedDeltaTime) + (t == Time.fixedDeltaTime ? 0 : 1)) *
                    Time.fixedDeltaTime;
                tempTime += t;
            }

            flyTime = tempTime;

            point.Add(find);
        }

        return point;
    }


    #endregion

    #region json 与 数组转化

    public static string ToJson<T>(T obj)
    {
        if (obj == null)
            return "null";

        if (typeof(T).GetInterface("IList") != null)
        {
            Pack<T> pack = new Pack<T>();
            pack.data = obj;
            string json = JsonUtility.ToJson(pack);
            return json.Substring(8, json.Length - 9);
        }

        return JsonUtility.ToJson(obj);
    }

    /// <summary> 解析Json </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="json">Json字符串</param>
    public static T FromJson<T>(string json)
    {
        if (json == "null" && typeof(T).IsClass)
            return default(T);

        if (typeof(T).GetInterface("IList") != null)
        {
            json = "{\"data\":{data}}".Replace("{data}", json);
            Pack<T> Pack = JsonUtility.FromJson<Pack<T>>(json);
            return Pack.data;
        }

        return JsonUtility.FromJson<T>(json);
    }

    /// <summary> 内部包装类 </summary>
    private class Pack<T>
    {
        public T data;
    }

    #endregion

    #region Time
    
    /// <summary>
    /// 时间戳反转为时间，有很多中翻转方法，但是，请不要使用过字符串（string）进行操作，大家都知道字符串会很慢！
    /// </summary>
    /// <param name="timeStamp">时间戳</param>
    /// <param name="accurateToMilliseconds">是否精确到毫秒</param>
    /// <returns>返回一个日期时间</returns>
    public static DateTime GetTime(long timeStamp, bool accurateToMilliseconds = false)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
        if (accurateToMilliseconds)
        {
            return startTime.AddTicks(timeStamp * 10000);
        }
        else
        {
            return startTime.AddTicks(timeStamp * 10000000);
        }
    }

    public static System.DateTime ConvertLongToDateTime(long timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        System.TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }

    public static long ConvertDateTimeToLong(DateTime time)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return (long)(time - startTime).TotalSeconds;
    }

    public static string SecondDateToStringDate(long second)
    {
        long h = second / 3600;
        long m = second / 60 % 60;
        long s = second % 60;
        string hStr = h / 10 > 0 ? h + "" : "0" + h;
        string mStr = m / 10 > 0 ? m + "" : "0" + m;
        string sStr = s / 10 > 0 ? s + "" : "0" + s;
        string result = "";
        if (hStr == "00")
        {
            result = mStr + ":" + sStr;
        }
        else
        {
            result = hStr + ":" + mStr + ":" + sStr;
        }

        return result;
    }

    public static string SecondDateToStringDate(TimeSpan time)
    {
        long h = time.Hours;
        long m = time.Minutes;
        long s = time.Seconds;
        string hStr = h / 10 > 0 ? h + "" : "0" + h;
        string mStr = m / 10 > 0 ? m + "" : "0" + m;
        string sStr = s / 10 > 0 ? s + "" : "0" + s;
        string result = "";
        if (hStr == "00")
        {
            result = mStr + ":" + sStr;
        }
        else
        {
            result = hStr + ":" + mStr + ":" + sStr;
        }

        return result;
    }

    public static string SecondDateToStringDateHM(long second)
    {
        long h = second / 3600;
        long m = second / 60 % 60;
        string hStr = h / 10 > 0 ? h + "" : "0" + h;
        string mStr = m / 10 > 0 ? m + "" : "0" + m;
        return hStr + "h:" + mStr + "m";
    }

    #endregion

    #region Tag计算

    public const string BoomTag = "BoomTag";
    public const string PenetrateTag = "PenetrateTag";
    public const string PenetrateAndBoom = "PenetrateAndBoom";
    public const string Untagged = "Untagged";
    public static bool IsBoomTag(string tag)
    {
        return tag == BoomTag || tag == PenetrateAndBoom;
    }

    public static bool IsPenetrateTag(string tag)
    {
        return tag == PenetrateTag || tag == PenetrateAndBoom;
    }

    #endregion

    #region 动画字计算

    public static string GetNumber001Text(string value)
    {
        var result = "";
        foreach (var charT in value)
        {
            switch (charT)
            {
                case '+':
                    result += string.Format("<sprite=10>");
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    result += string.Format("<sprite={0}>", charT);
                    break;
                default:
                    result += "";
                    break;
            }
        }
        return result;
    }


    #endregion

    #region Animation
    public static bool IsPlayEnd(Animation anim, string name)
    {
        if (anim == null || anim.GetClip(name) == null)
            return true;

        return !anim.IsPlaying(name) || anim[name].normalizedTime >= 1f || anim[name].normalizedTime <= 0f;
    }

    public static float GetClipLength(Animator anim, string name)
    {
        if (anim == null || string.IsNullOrEmpty(name) || anim.runtimeAnimatorController == null)
            return 0;

        // 获取所有的clips	
        var clips = anim.runtimeAnimatorController.animationClips;
        if (null == clips || clips.Length <= 0)
            return 0;

        foreach (var clip in clips)
        {
            if (clip != null && clip.name == name)
                return clip.length;
        }

        return 0;
    }

    public static float GetClipLength(Animation anim, string name)
    {
        if (anim == null)
            return 0;
        if (string.IsNullOrEmpty(name))
            return 0;

        var clip = anim.GetClip(name);
        if (clip == null)
            return 0;
        return clip.length;
    }
    #endregion

    public static string FormatTime(int num)
    {
        if (num < 10)
        {
            return $"0{num}";
        }

        return num.ToString();
    }

    public static string GetFormatCdBySecond(int second)
    {
        var hour = second / 3600;
        second -= hour * 3600;
        var minute = second / 60;
        second -= minute * 60;

        return $"{FormatTime(hour)}:{FormatTime(minute)}:{FormatTime(second)}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <param name="showNum">显示几个, 0 时分秒 有值则显示, 1只显示秒 2显示分秒 3显示小时分秒</param>
    /// <returns></returns>
    public static string GetFormatCd(int hour, int minute, int second, int showNum = 0)
    {
        if (showNum == 3 || hour > 0)
        {
            return $"{FormatTime(hour)}:{FormatTime(minute)}:{FormatTime(second)}";
        }
        else if (showNum == 2 || minute > 0)
        {
            return $"{FormatTime(minute)}:{FormatTime(second)}";
        }
        else
        {
            return $"00:{FormatTime(second)}";
        }


    }

    ///获取动画状态机animator的动画clip的播放持续时长
    public static AnimationClip GetClip(Animator animator, string clipName)
    {
        if (null == animator ||
            string.IsNullOrEmpty(clipName) ||
            null == animator.runtimeAnimatorController)
            return null;
        // 获取所有的clips	
        var clips = animator.runtimeAnimatorController.animationClips;
        if (null == clips || clips.Length <= 0) return null;
        AnimationClip clip;
        for (int i = 0, len = clips.Length; i < len; ++i)
        {
            clip = clips[i];
            if (null != clip && clip.name == clipName)
                return clip;
        }

        return null;
    }



    // UI 刷新子项数据
    public static List<H> RefreshItem<T, H>(List<T> data, Transform content, GameObject prefab, List<GameObject> outList = default) where H : UI_BaseItem
    {
        if (data == null)
            data = new List<T>();


        // 数据多于子项
        if (data.Count > content.childCount)
        {
            for (var i = content.childCount; i < data.Count; i++)
            {
                var go = GameObject.Instantiate(prefab, content);
                go.transform.localScale = Vector3.one;
                go.SetActive(true);

                var item = go.GetComponent<UI_BaseItem>();
                item.Init();
            }
        }

        if (content.childCount > data.Count)
        {
            for (var i = data.Count; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
        }

        outList?.Clear();
        List<H> result = new List<H>();

        for (var i = 0; i < data.Count; i++)
        {
            var go = content.GetChild(i).gameObject;
            go.SetActive(true);
            var item = go.GetComponent<UI_BaseItem>();
            if (item)
            {
                item.Refresh(data[i], i);
                outList?.Add(go);
                result.Add((H)item);
            }
        }

        return result;
    }
    public static void RefreshItem<T>(List<T> data, Transform content, GameObject prefab, List<GameObject> outList = default)
    {
        if (data == null)
            data = new List<T>();


        // 数据多于子项
        if (data.Count > content.childCount)
        {
            for (var i = content.childCount; i < data.Count; i++)
            {
                var go = GameObject.Instantiate(prefab, content);
                go.transform.localScale = Vector3.one;
                go.SetActive(true);

                var item = go.GetComponent<UI_BaseItem>();
                item.Init();
            }
        }

        if (content.childCount > data.Count)
        {
            for (var i = data.Count; i < content.childCount; i++)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
        }

        outList?.Clear();

        for (var i = 0; i < data.Count; i++)
        {
            var go = content.GetChild(i).gameObject;
            go.SetActive(true);
            var item = go.GetComponent<UI_BaseItem>();
            if (item)
            {
                item.Refresh(data[i], i);
                outList?.Add(go);
            }
        }
    }
    /// <summary>
    /// 字符长度
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int TextLenth(string str)
    {
        int length = 0;
        ASCIIEncoding ascii = new ASCIIEncoding();
        byte[] s = ascii.GetBytes(str);
        for (int i = 0; i < s.Length; i++)
        {
            if ((int)s[i] == 63)
            {
                length += 2;
            }
            else
            {
                length += 1;
            }
        }
        return length;
    }


    public static int GetRandom4List(List<float> weightList)
    {
        var random = UnityEngine.Random.Range(0f, weightList.Sum());
        var sum = 0f;
        var randomIndex = 0;

        for (var i = 0; i < weightList.Count; i++)
        {
            sum += weightList[i];

            if (sum > random)
            {
                randomIndex = i;
                break;
            }
        }

        randomIndex = Mathf.Clamp(randomIndex, 0, weightList.Count - 1);
        return randomIndex;
    }

    public static int GetRandom4List(List<int> weightList)
    {
        var random = UnityEngine.Random.Range(0, weightList.Sum());
        var sum = 0f;
        var randomIndex = 0;

        for (var i = 0; i < weightList.Count; i++)
        {
            sum += weightList[i];

            if (sum > random)
            {
                randomIndex = i;
                break;
            }
        }

        randomIndex = Mathf.Clamp(randomIndex, 0, weightList.Count - 1);
        return randomIndex;
    }

    //刷新金币
    public static string UpdateCoinText(int coin)
    {
        if (coin / 100000000 < 1) return coin + "";
        string hpText = "";
        int hpInt = (int)coin;
        int m = hpInt / 1000000;
        int k = 0;
        int b = 0;
        if (m <= 0)
        {
            k = hpInt / 1000;
            hpInt %= 1000;
            b = hpInt / 100;
        }

        hpText += m > 0 ? m + "M" : (k > 0 ? (k + "." + b + "K") : hpInt + "");
        return hpText + "";
    }

    /// <summary>
    /// 金币 三位加空格
    /// </summary>
    /// <param name="coin"></param>
    /// <returns></returns>
    public static string CoinTextWithSpace(int coin)
    {
        string coinSpace = coin.ToString();
        if (coin >= 1000)
        {
            int insertPos = coinSpace.Length - 3;
            while (insertPos > 0)
            {
                coinSpace = coinSpace.Insert(insertPos, " ");
                insertPos = insertPos - 3;
            }

        }


        return coinSpace;
    }

    //获取带逗号的 字符串
    public static string GetNumCommaStr(int num)
    {
        string result = "";
        while (num >= 1000)
        {
            int temp = num % 1000;
            string tempStr = "" + temp;
            if (temp < 10)
            {
                tempStr = "0" + temp;
            }

            if (temp < 100)
            {
                tempStr = "0" + tempStr;
            }

            tempStr = "," + tempStr;
            result = tempStr + result;
            num /= 1000;
        }

        result = num + result;
        return result;
    }



    public static string SecondToTime(double a)
    {
        int time = (int)a;
        int h, m, s;
        h = time / 3600;
        time %= 3600;
        m = time / 60;
        time %= 60;
        s = time;
        string hStr = h + "";
        string mStr = m + "";
        string sStr = s + "";
        if (h / 10 == 0) hStr = "0" + hStr;
        if (m / 10 == 0) mStr = "0" + mStr;
        if (s / 10 == 0) sStr = "0" + sStr;
        return hStr + ":" + mStr + ":" + sStr;
    }

    /// <summary>
    /// 本地视频文件是否存在
    /// </summary>
    public static bool IsFileExist(string path)
    {
        if (File.Exists(path))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 获取文件md5
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetMd5Val(string path)
    {
        FileStream file = new FileStream(path, FileMode.Open);
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(file);
        file.Close();
        StringBuilder sb = new StringBuilder();
        for (int j = 0; j < retVal.Length; j++)
        {
            sb.Append(retVal[j].ToString("x2"));
        }
        return sb.ToString();
    }

    public static string GetMd5String(string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
        //System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
        //StringBuilder sb = new StringBuilder();
        //for (int j = 0; j < retVal.Length; j++)
        //{
        //    sb.Append(retVal[j].ToString("x2"));
        //}
        //return sb.ToString();

        char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] md = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
        // 把密文转换成十六进制的字符串形式
        int j = md.Length;
        char[] str1 = new char[j * 2];
        int k = 0;
        for (int i = 0; i < j; i++)
        {
            byte byte0 = md[i];
            str1[k++] = hexDigits[byte0 >> 4 & 0xf];
            str1[k++] = hexDigits[byte0 & 0xf];
        }
        return new String(str1);
    }

    #region Rsa

    #endregion



    #region 加密
    /// <summary>
    /// 签名
    /// </summary>
    /// <param name="convertKey"></param>
    /// <returns></returns>
    //public static Dictionary<string, string> SignKey(Cmd type, string secret)
    //{
    //    //时间戳
    //    long timestamp = GL_Time._instance.GetTimestamp_10();
    //    //md5(timestamp拼接user_id拼接http_path), md5(1629702742-1-api/v1/orders),  如: 4de8114a4c3317be5a9faad2e8456018
    //    DataModule.TableNetworkRequestData requestData = GameDataTable.GetTableNetworkRequestData((int)type);
    //    string nonce = timestamp + "-" + GL_PlayerData._instance.GetServerPlayerInfo.id + "-" + requestData.JointURL;
    //    //DDebug.LogError("~~~nonce" + nonce);
    //    nonce = GetMd5String(nonce);
    //    string stringToSign = "http_method=" + requestData.Method + "&http_path=" + requestData.JointURL + "&nonce=" + nonce + "&timestamp=" + timestamp;
    //    //DDebug.LogError("~~~stringToSign: " + stringToSign);
    //    byte[] signData = HmacSHA256(encoding.GetBytes(secret), encoding.GetBytes(stringToSign));
    //    string sign = BitConverter.ToString(signData).Replace("-", "").ToLower();

    //    return new Dictionary<string, string>
    //    {
    //        { "timestamp",timestamp.ToString() },
    //        { "nonce",nonce },
    //        { "sign",sign }
    //    };
    //}

    /// <summary>
    /// HmacSHA256算法,返回的结果始终是32位
    /// </summary>
    /// <param name="key">加密的键，可以是任何数据</param>
    /// <param name="content">待加密的内容</param>
    /// <returns></returns>
    public static byte[] HmacSHA256(byte[] key, byte[] content)
    {
        using (var hmacsha256 = new HMACSHA256(key))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(content);
            return hashmessage;
        }
    }


    private static readonly Encoding encoding = Encoding.UTF8;
    //加密
    public static string Encrypt(string plainText, string key)
    {
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            aes.Key = Convert.FromBase64String(key);
            aes.GenerateIV();

            ICryptoTransform AESEncrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] buffer = encoding.GetBytes(plainText);

            string encryptedText = Convert.ToBase64String(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));

            string mac = BitConverter.ToString(HmacSHA256(aes.Key, encoding.GetBytes(Convert.ToBase64String(aes.IV) + encryptedText))).Replace("-", "").ToLower();

            SCipherInfo data = new SCipherInfo();
            data.iv = Convert.ToBase64String(aes.IV);
            data.value = encryptedText;
            data.mac = mac;
            data.tag = "";
            //DDebug.LogError("~~~iv: " + data.iv);
            //DDebug.LogError("~~~value: " + data.value);
            //DDebug.LogError("~~~mac: " + data.mac);
            return Convert.ToBase64String(encoding.GetBytes(JsonUtility.ToJson(data)));
        }

    }

    //解密
    public static string Decrypt(string plainText, string key)
    {
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = Convert.FromBase64String(key);

            // Base 64 decode
            byte[] base64Decoded = Convert.FromBase64String(plainText);
            string base64DecodedStr = encoding.GetString(base64Decoded);

            // JSON Decode base64Str
            SCipherInfo data = JsonUtility.FromJson<SCipherInfo>(base64DecodedStr);
            aes.IV = Convert.FromBase64String(data.iv);

            ICryptoTransform AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] buffer = Convert.FromBase64String(data.value);
            return encoding.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }
    }

    [System.Serializable]
    public class SCipherInfo
    {
        public string iv;
        public string value;
        public string mac;
        public string tag;
    }

    #endregion

    #region 生成指定长度的随机字符串
    /// <summary>
    /// 生成指定长度的随机字符串
    /// </summary>
    /// <param name="intLength">随机字符串长度</param>
    /// <param name="booNumber">生成的字符串中是否包含数字</param>
    /// <param name="booSign">生成的字符串中是否包含符号</param>
    /// <param name="booSmallword">生成的字符串中是否包含小写字母</param>
    /// <param name="booBigword">生成的字符串中是否包含大写字母</param>
    /// <returns></returns>
    public static string GetRandomStr(int intLength, bool booNumber, bool booSign, bool booSmallword, bool booBigword)
    {
        //定义
        System.Random ranA = new System.Random();
        int intResultRound = 0;
        int intA = 0;
        string strB = "";

        while (intResultRound < intLength)
        {
            //生成随机数A，表示生成类型
            //1=数字，2=符号，3=小写字母，4=大写字母

            intA = ranA.Next(1, 5);

            //如果随机数A=1，则运行生成数字
            //生成随机数A，范围在0-10
            //把随机数A，转成字符
            //生成完，位数+1，字符串累加，结束本次循环

            if (intA == 1 && booNumber)
            {
                intA = ranA.Next(0, 10);
                strB = intA.ToString() + strB;
                intResultRound = intResultRound + 1;
                continue;
            }

            //如果随机数A=2，则运行生成符号
            //生成随机数A，表示生成值域
            //1：33-47值域，2：58-64值域，3：91-96值域，4：123-126值域

            if (intA == 2 && booSign)
            {
                intA = ranA.Next(1, 5);

                //如果A=1
                //生成随机数A，33-47的Ascii码
                //把随机数A，转成字符
                //生成完，位数+1，字符串累加，结束本次循环

                if (intA == 1)
                {
                    intA = ranA.Next(33, 48);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

                //如果A=2
                //生成随机数A，58-64的Ascii码
                //把随机数A，转成字符
                //生成完，位数+1，字符串累加，结束本次循环

                if (intA == 2)
                {
                    intA = ranA.Next(58, 65);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

                //如果A=3
                //生成随机数A，91-96的Ascii码
                //把随机数A，转成字符
                //生成完，位数+1，字符串累加，结束本次循环

                if (intA == 3)
                {
                    intA = ranA.Next(91, 97);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

                //如果A=4
                //生成随机数A，123-126的Ascii码
                //把随机数A，转成字符
                //生成完，位数+1，字符串累加，结束本次循环

                if (intA == 4)
                {
                    intA = ranA.Next(123, 127);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

            }

            //如果随机数A=3，则运行生成小写字母
            //生成随机数A，范围在97-122
            //把随机数A，转成字符
            //生成完，位数+1，字符串累加，结束本次循环

            if (intA == 3 && booSmallword)
            {
                intA = ranA.Next(97, 123);
                strB = ((char)intA).ToString() + strB;
                intResultRound = intResultRound + 1;
                continue;
            }

            //如果随机数A=4，则运行生成大写字母
            //生成随机数A，范围在65-90
            //把随机数A，转成字符
            //生成完，位数+1，字符串累加，结束本次循环

            if (intA == 4 && booBigword)
            {
                intA = ranA.Next(65, 89);
                strB = ((char)intA).ToString() + strB;
                intResultRound = intResultRound + 1;
                continue;
            }
        }
        return strB;

    }
    #endregion
}

#region 扩展
public static class Extension
{

    public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
        dict.TryGetValue(key, out var value);
        return value;
    }

    public static AnimationClip[] GetClips(this Animation anim)
    {
        return (from AnimationState o in anim select anim.GetClip(o.name)).ToArray();
    }

    public static void SetX(this Vector2 vec, float x)
    {
        vec.Set(x, vec.y);
    }

    public static void SetY(this Vector2 vec, float y)
    {
        vec.Set(vec.x, y);
    }

    public static void SetZ(this Vector3 vec, float value)
    {
        vec.Set(vec.x, vec.y, value);
    }

    public static void SetOrderSort(this ParticleSystem p, int layerId, int order, bool recurse)
    {
        var prs = new List<ParticleSystemRenderer>();
        if (!recurse) prs.Add(p.GetComponent<ParticleSystemRenderer>());
        else prs.AddRange(p.GetComponentsInChildren<ParticleSystemRenderer>());

        prs.ForEach(pr =>
        {
            if (null != pr)
            {
                pr.sortingLayerID = layerId;
                pr.sortingOrder = order;
            }
        });
    }

    public static Vector2 Vector2Abs(this Vector2 value)
    {
        return new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
    }

    public static void ReplaceValueList<T>(this List<T> optionList, int insertIndex, T value)
    {
        if (insertIndex >= optionList.Count)
        {
            T[] arr = new T[insertIndex + 1 - optionList.Count];
            optionList.AddRange(arr);
        }
        optionList[insertIndex] = value;
    }

    public static void SetActive(this Component c, bool value)
    {
        c.gameObject.SetActive(value);
    }

    public static bool IsPlayEnd([NotNull] this Animation anim, string name)
    {
        if (anim.GetClip(name) == null) return true;

        return !anim.IsPlaying(name) || anim[name].normalizedTime >= 1f || anim[name].normalizedTime <= 0f;
    }

    public static void Play([NotNull] this Animator anim, string name, Action action)
    {
        anim.Play(name);
        float time = GetClipLength(anim, name); 
        MethodExeTool.Loop((() => action?.Invoke()), time);
    }

    public static float GetClipLength([NotNull] this Animator anim, string name)
    {
        if (string.IsNullOrEmpty(name) || anim.runtimeAnimatorController == null)
            return 0f;

        // 获取所有的clips	
        var clips = anim.runtimeAnimatorController.animationClips;
        if (null == clips || clips.Length <= 0)
            return 0f;

        foreach (var clip in clips)
        {
            if (clip != null && clip.name == name)
                return clip.length;
        }


        return 0f;
    }
    public static void Play([NotNull] this Animator anim, string name,int layer,float normalizedTime, float intervalTime, Action _action)
    {
        anim.Play(name,layer,normalizedTime);
        // time = GetClipLength(anim, name);
        MethodExeTool.InvokeDT(_action,intervalTime);
    }
    public static float GetClipLength([NotNull] this Animation anim, string name)
    {
        if (string.IsNullOrEmpty(name))
            return 0f;

        var clip = anim.GetClip(name);
        if (clip == null)
            return 0f;
        return clip.length;
    }

    public static Vector2 ToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector2 Abs(this Vector2 vec)
    {
        return new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
    }

    public static Vector3 Abs(this Vector3 vec)
    {
        return new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
    }

    public static void ToScrollChildPos(this ScrollRect scrollRect, int siblingIndex)
    {
        int childCount = scrollRect.content.transform.childCount;
        RectTransform contentGrid = scrollRect.content;
        var rect = scrollRect.GetComponent<RectTransform>();
        float height = contentGrid.rect.height * (siblingIndex / (float)childCount) - rect.rect.height * 0.5f;
        scrollRect.StartCoroutine(OnScrollRect(contentGrid, height));
    }

    static IEnumerator OnScrollRect(RectTransform content, float height)
    {
        float time = 0.5f;
        float timer = time;
        Vector2 startPos = content.anchoredPosition;
        Vector2 endPos = new Vector2(0, height);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            var t = 1 - timer / time;
            content.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        content.anchoredPosition = endPos;
    }
}
#endregion