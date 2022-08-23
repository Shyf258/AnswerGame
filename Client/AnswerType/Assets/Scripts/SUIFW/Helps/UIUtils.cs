using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SUIFW;
using UnityEngine.UI;
/// <summary>
/// ui工具类
/// </summary>
public static class UIUtils
{
    

    /// <summary>
    /// 删除所有子对象
    /// </summary>
    /// <param name="go"></param>
    public static void removeChildren(GameObject go)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            GameObject.Destroy(child);
        }
    }
    /// <summary>
    /// 删除所有子对象 除了XXX
    /// </summary>
    /// <param name="go"></param>
    /// <param name="one">xxx</param>
    public static void removeChildrenWithoutOne(GameObject go, string one)
    {
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            if (child.name == one)
            {
                continue;
            }
            GameObject.Destroy(child);
        }
    }
    /// <summary>
    /// 添加子节点
    /// </summary>
    /// <param name="kid"></param>
    /// <param name="parent"></param>
    public static void setParent(GameObject kid, GameObject parent)
    {
        if (kid != null && parent != null)
        {
            kid.transform.parent = parent.transform;
            kid.transform.localPosition = Vector3.zero;
            kid.transform.localRotation = Quaternion.identity;
            kid.transform.localScale = Vector3.one;
        }
    }
    /// <summary>
    /// 时钟式倒计时
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetSecondString(int second)
    {
        if (second / 3600 > 0)
        {
            return string.Format("{0:D2}", second / 3600) + ":" + string.Format("{0:D2}", second % 3600 / 60) + ":" + string.Format("{0:D2}", second % 60);
        }
        else
        {
            return string.Format("{0:D2}", second % 3600 / 60) + ":" + string.Format("{0:D2}", second % 60);
        }
    }
    /// <summary>
    /// 时钟式倒计时 仅秒
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetSecondString_OnlySec(int second)
    {
        return string.Format("{0:D2}", second % 60);
    }

    /// <summary>
    /// 时钟式倒计时
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetSecondString_HMS(int second)
    {
        int h = second / 3600;
        int m = second % 3600 / 60;
        int s = second % 60;

        string strH = h == 0 ? "" : h + "h";
        string strM = m == 0 ? "" : m + "m";
        string strS = /*s == 0 ? "" : */s + "s";

        return strH + strM + strS;

        //if (second / 3600 > 0)
        //{
        //    return second / 3600 + "h" + second % 3600 / 60 + "m" + second % 60 + "s";
        //}
        //else
        //{
        //    return second % 3600 / 60 + "m" +  second % 60 + "s";
        //}
    }
    /// <summary>
    /// 设置child颜色 graphic?
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="childname"></param>
    /// <param name="color"></param>
    public static void SetChildColor(Transform tr, string childname, Color color)
    {
        if (tr == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(childname))
        {
            return;
        }
        var child = tr.Find(@childname);
        if (child != null)
        {
            var Graphic = child.GetComponent<UnityEngine.UI.Graphic>();
            if (Graphic != null)
            {
                Graphic.color = color;
            }
        }
    }
    /// <summary>
    /// 创建sprite图片
    /// </summary>
    /// <param name="sprite"></param>
    private static void CreatImage(Sprite sprite)
    {
        GameObject go = new GameObject(sprite.name);
        go.layer = LayerMask.NameToLayer("UI");
        //go.transform.parent = transform;
        go.transform.localScale = Vector3.one;
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
    }
    /// <summary>
    /// 检查一个点是不是在相机内
    /// </summary>
    /// <param name="cam"></param>
    /// <param name="wordPos"></param>
    /// <param name="viewPort"></param>
    /// <returns></returns>
    static public bool IsAPointInACamera(Camera cam, Vector3 wordPos, ref Vector3 viewPort)
    {
        // 是否在视野内
        bool result1 = false;
        Vector3 posViewport = cam.WorldToViewportPoint(wordPos);
        viewPort = posViewport;///这个只是为了减少计算量///
        Rect rect = new Rect(0, 0, 1, 1);
        result1 = rect.Contains(posViewport);
        // 是否在远近平面内
        bool result2 = false;
        if (posViewport.z >= cam.nearClipPlane && posViewport.z <= cam.farClipPlane)
        {
            result2 = true;
        }
        // 综合判断
        bool result = result1 && result2;
        return result;
    }
    /// <summary>
    /// 检查一个点是不是在相机内
    /// </summary>
    /// <param name="cam"></param>
    /// <param name="wordPos"></param>
    /// <param name="viewPort"></param>
    /// <returns></returns>
    static public bool IsAPointInACamera2(Camera cam, Vector3 wordPos, ref Vector3 viewPort)
    {
        // 是否在视野内
        bool result1 = false;
        Vector3 posViewport = cam.WorldToViewportPoint(wordPos);
        viewPort = posViewport;///这个只是为了减少计算量///
        Rect rect = new Rect(-0.1f, -0.1f, 1.1f, 1.1f);
        result1 = rect.Contains(posViewport);
        // 是否在远近平面内
        bool result2 = false;
        if (posViewport.z >= cam.nearClipPlane && posViewport.z <= cam.farClipPlane)
        {
            result2 = true;
        }
        // 综合判断
        bool result = result1 && result2;
        return result;
    }
    /// <summary>
    /// 判断一个物体是不是在相机前方的一个角度内
    /// </summary>
    /// <param name="camTr">相机的Transform</param>
    /// <param name="targetPos">目标点</param>
    /// <param name="angle">限定角度</param>
    /// <returns>前方角度内</returns>
    static public bool InAngle(Transform camTr, Vector3 targetPos, float angle, ref float dot)
    {
        Vector3 forward = camTr.forward;
        Vector3 offset = targetPos - camTr.position;
        var tmpangle = Vector3.Angle(forward, offset);
        dot = Vector3.Dot(forward, offset);
        return angle > tmpangle && dot > 0;
    }
    /// <summary>
    /// 添加或者获取一个组建
    /// </summary>
    static public T GAComp<T>(this GameObject go) where T : Component
    {
        var tmp = go.GetComponent<T>();
        if (tmp == null)
        {
            tmp = go.AddComponent<T>();
        }
        return tmp;
    }
    /// <summary>
    /// 获取鼠标经过的目标(UI)
    /// </summary>
    /// <returns></returns>
    public static GameObject GetPointerOverGameObject()
    {
        var tmp = UnityEngine.EventSystems.EventSystem.current;
        if (tmp == null)
        {
            return null;
        }

        var m_CurrentInputModule = tmp.currentInputModule;
        if (m_CurrentInputModule == null)
        {
            return null;
        }
        var ty = typeof(UnityEngine.EventSystems.PointerInputModule);
        var pointerID = UnityEngine.EventSystems.PointerInputModule.kMouseLeftId;

        var mi = ty.GetMethod("GetLastPointerEventData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
        var result =
            mi.Invoke(
            m_CurrentInputModule as UnityEngine.EventSystems.PointerInputModule,
            new System.Object[] { pointerID });
        var pointerData = result as UnityEngine.EventSystems.PointerEventData;
        if (pointerData == null)
        {
            return null;
        }
        var enterObj = pointerData.pointerEnter;
        //if (enterObj != null)
        //    Debug.Log("PointerIn:  "+enterObj.name);
        return enterObj;
    }
    /// <summary>
    /// 迭代寻找父物体的脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T getCompInParent<T>(this GameObject go) where T : Component
    {
        var parent = go.transform.parent;
        if (parent == null)
        {
            return null;
        }
        var comp = parent.GetComponent<T>();
        if (comp == null)
        {
            return getCompInParent<T>(parent.gameObject);
        }
        return comp;
    }
    /*
    /// <summary>
    /// 设置canvas的层级
    /// </summary>
    /// <param name="ui"></param>
    public static void SetCanvasOrder(UGUI_Window_Base ui, int order)
    {
        var c = ui.GetComponent<Canvas>();
        if (c != null)
        {
            c.sortingOrder = order;
        }
    }
    /// <summary>
    /// 给每个UI都添加Canvas
    /// </summary>
    public static void addCanvas(UGUI_Window_Base win)
    {
        //添加Canvas用作排序用
        if (!win.GetComponent<GraphicRaycaster>())
        {
            GraphicRaycaster gaphicRaycaster = win.gameObject.AddComponent<GraphicRaycaster>();
        }
        Canvas canvas = win.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = win.gameObject.AddComponent<Canvas>();
        }
        canvas.overrideSorting = true;
        //canvas.sortingOrder = (int)show_order;
    }
    */
    public static void WorldPosToUIAnchoredPosition(Vector3 worldPos, RectTransform uiRT)
    {
        var gameCam = UIUtils.GetGameCam();
        Vector2 pos;
        var canvas = UIUtils.GetUGUIRoot().GetComponent<Canvas>();
        if (canvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, gameCam.WorldToScreenPoint(worldPos), canvas.worldCamera, out pos);
            uiRT.anchoredPosition = pos;
        }
    }

    #region UIUlit
    private static Camera _uicam;
    public static GameObject _uiroot;

    public static Vector2? _uiRootCanvasSize;
    public static RectTransform _uiRootCanvasRect;
    public static Camera GetUGUICam()
    {
        if (_uicam == null)
        {
            _uicam = UIManager.GetInstance().UiCamera;
        }
        return _uicam;
    }
    public static Camera GetGameCam()
    {
        var gameCam = Camera.main;
        return gameCam;
    }
    public static GameObject GetUGUIRoot()
    {
        if (_uiroot == null)
        {
            _uiroot = GameObject.Find("Canvas");
        }
        return _uiroot;
    }

    //得到UI层在Z轴的绝对位置
    public static float GetPosZInUGUICamera()
    {
        return -(GetUGUIRoot().transform.localScale.x + GetUGUICam().transform.position.z);
    }
    //通过UI上两个点，在UI上用sprite画一条线
    public static void drawUILineBySprite(Vector3 src, Vector3 des, Image sprite)
    {
        sprite.rectTransform.anchoredPosition = src;

        float distance = Vector3.Distance(src, des);
        if (distance > 2000)
        {
            distance = 2000;
        }
        sprite.rectTransform.sizeDelta = new Vector2(distance, sprite.rectTransform.sizeDelta.y);
        //Debug.Log(distance);
        float angle = Vector2.Angle(Vector2.right, des - src);
        //Debug.Log(angle);
        sprite.rectTransform.eulerAngles = new Vector3(0, 0, des.y > src.y ? angle : -angle);
    }
    //根据Viewport获取屏幕实际位置，并根据锚点做了一次坐标转换
    public static Vector3 GetPosByViewport(Vector3 pos)
    {
        return UIUtils.GetUGUICam().ViewportToWorldPoint(pos);
    }
    public static Vector2 GetUGUIRootSize()
    {
        return GetUGUIRootRectTransform().sizeDelta;
    }
    
    public static RectTransform GetUGUIRootRectTransform()
    {
        if(_uiRootCanvasRect == null)
            _uiRootCanvasRect = GetUGUIRoot().GetComponent<RectTransform>();
        return _uiRootCanvasRect;
    }

    public static void PlayBtnSound()
    {

    }

    #endregion

    public static IEnumerator MoveObject_Time(GameObject root, Vector3 startPos, Vector3 endPos, float time)
    {
        float dur = 0.0f;
        while (dur <= time)
        {
            dur += Time.deltaTime;
            root.transform.localPosition = Vector3.Lerp(startPos, endPos, dur / time);
            yield return null;
        }
    }

    public static IEnumerator MoveObject_Speed(GameObject root, Vector3 startPos, Vector3 endPos, float speed)
    {
        float startTime = Time.time;
        float length = Vector3.Distance(startPos, endPos);
        float frac = 0f;
        while (frac <= 1.0f)
        {
            float dist = (Time.time - startTime) * speed;
            frac = dist / length;
            root.transform.localPosition = Vector3.Lerp(startPos, endPos, frac);
            yield return null;
        }
    }
    public static IEnumerator TextAlpha_Change(Text root, float start, float end, float time)
    {
        float dur = 0.0f;
        while(dur <= time)
        {
            dur += Time.deltaTime;
            float value = Mathf.Lerp(start, end, dur / time);
            if(root!=null)
            {
                root.color = new Color(root.color.r, root.color.g, root.color.b, value);
            }
            yield return null;
        }
        yield return null;
    }

}
