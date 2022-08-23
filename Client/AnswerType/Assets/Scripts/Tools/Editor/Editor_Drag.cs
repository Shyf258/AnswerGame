using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// 从Project窗口拖动图片到Canvas下任意层级，自动生成对应Image
/// 会覆盖原生成Sprite操作
/// </summary>
public class Editor_Drag
{
    /// <summary>
    /// 总开关
    /// </summary>
    public static bool Switch = true;

    private static bool isTrigger = false;

    [InitializeOnLoadMethod]
    private static void Init()
    {
        if (!Switch)
        {
            return;
        }
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGui;
        //检测hierarchy窗口下 gameobject拖拽变化
        EditorApplication.hierarchyChanged += HierarchyWindowChanged;

    }

    #region 拖拽到canvas下 SpriteRenderer自动变成image

    private static void ProjectWindowItemOnGui(string guid, Rect selectionRect)
    {
        if (!Switch)
        {
            return;
        }
        // 拖动图片出Project窗口时
        if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited)
        {
            isTrigger = true;
        }

    }

    private static void HierarchyWindowChanged()
    {
        if (!Switch)
        {
            return;
        }
        if (!isTrigger)
        {
            return;
        }
        // 此时Unity会默认创建Sprite并定位到该GameObject上
        GameObject go = Selection.activeGameObject;
        if (go == null)
            return;
        SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            return;

        //检测是否有canvas
        Canvas canvas = go.GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        //go.name = "Image";
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;
        Image image = go.AddComponent<Image>();
        image.raycastTarget = false;
        image.sprite = spriteRenderer.sprite;
        Object.DestroyImmediate(spriteRenderer);
        image.SetNativeSize();

        isTrigger = false;

    }
    #endregion



}