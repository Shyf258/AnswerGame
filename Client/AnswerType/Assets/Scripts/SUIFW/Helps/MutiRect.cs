using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MutiRect : MaskableGraphic, ICanvasRaycastFilter
{
    private List<Rect> m_RectList;
    private List<Vector3> m_CircleList;

    protected MutiRect()
    {
        m_RectList = new List<Rect>();
        m_CircleList = new List<Vector3>();
    }

    public void Clear()
    {
        m_RectList.Clear();
        m_CircleList.Clear();
    }

    public void AddRect(Rect rect)
    {
        m_RectList.Add(rect);
    }

    public void AddCircle(Vector2 center, float radius)
    {
        Vector3 circle = new Vector3(center.x, center.y, radius);
        m_CircleList.Add(circle);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        var r = GetPixelAdjustedRect();
        int offset = 0;
        foreach (var rect in m_RectList)
        {
            var color32 = color;
            float left = r.x + r.width * rect.x;
            float right = r.x + r.width * (rect.x + rect.width);
            float bottom = r.y + r.height * rect.y;
            float top = r.y + r.height * (rect.y + rect.height);

            vh.AddVert(new Vector3(left, bottom), color32, new Vector2(0, 0));
            vh.AddVert(new Vector3(left, top), color32, new Vector2(0, 1));
            vh.AddVert(new Vector3(right, top), color32, new Vector2(1, 1));
            vh.AddVert(new Vector3(right, bottom), color32, new Vector2(1, 0));

            vh.AddTriangle(offset + 0, offset + 1, offset + 2);
            vh.AddTriangle(offset + 2, offset + 3, offset + 0);

            offset += 4;
        }
        foreach (var circle in m_CircleList)
        {
            var color32 = color;
            Vector3 center = new Vector3(r.x + r.width * circle.x, r.y + r.height * circle.y, 0);
            float radius = circle.z * r.height;
            vh.AddVert(center, color32, Vector2.zero);

            int seg = 36;

            for (int i = 0; i < seg; i++)
            {
                float angle = Mathf.Deg2Rad * i * 360.0f / seg;
                Vector3 pos = center + new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0);
                vh.AddVert(pos, color32, Vector2.zero);
            }

            for (int i = 0; i < seg; i++)
            {
                if (i < seg - 1)
                {
                    vh.AddTriangle(offset + 0, offset + i + 2, offset + i + 1);
                }
                else
                {
                    vh.AddTriangle(offset + 0, offset + 1, offset + i + 1);
                }
            }

            offset += seg + 1;
        }
    }

    public static Rect WorldSpaceRectToMaskRelateveSpace(Rect worldSpaceRect)
    {
        Rect maskSpaceRect = new Rect();
        float viewportHeight = Camera.main.orthographicSize * 2;
        float viewportWidht = viewportHeight * Screen.width / Screen.height;
        float viewportX = Camera.main.transform.position.x - viewportWidht * 0.5f;
        float viewportY = Camera.main.transform.position.y - viewportHeight * 0.5f;
        maskSpaceRect.x = (worldSpaceRect.x - viewportX) / viewportWidht;
        maskSpaceRect.y = (worldSpaceRect.y - viewportY) / viewportHeight;
        maskSpaceRect.width = worldSpaceRect.width / viewportWidht;
        maskSpaceRect.height = worldSpaceRect.height / viewportHeight;
        return maskSpaceRect;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        Vector2 rsp = new Vector2(sp.x / Screen.width, sp.y / Screen.height);
        foreach (var rect in m_RectList)
        {
            if (rect.Contains(rsp))
            {
                return false;
            }
        }
        foreach (var circle in m_CircleList)
        {
            Vector2 center = new Vector2(circle.x * Screen.width, circle.y * Screen.height);
            float radius = circle.z * Screen.height;
            if (Vector2.Distance(center, sp) < radius)
            {
                return false;
            }
        }
        return true;
    }
}

