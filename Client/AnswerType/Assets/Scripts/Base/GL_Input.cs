//2018.09.30    关林
//输入

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GL_Input : Mono_Singleton_DontDestroyOnLoad<GL_Input>
{
    public class Finger
    {
        public int Index; //fingerID 多指索引
        public bool Marked;
        public float Pressure;  //按压力度
        public bool Down;       //按下
        public bool Set;        //按键在响应
        public bool Up;         //抬起
        public bool Move;   //是否移动
        public bool Moved;  //是否移动过
        public bool StartedOverGui; //输入是否为UI
        public Vector2 StartScreenPosition; //开始位置
        public Vector2 LastScreenPosition;  //上一帧位置
        public Vector2 ScreenPosition;      //当前位置
        //public Action<float, Finger> action;
    }


    private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

    private static PointerEventData tempPointerEventData;

    private static EventSystem tempEventSystem;

    private static List<Finger> fingers = new List<Finger>();

    private static List<Finger> tempFingers = new List<Finger>();

    public static float ScaleFactor
    {
        get
        {
            var dpi = Screen.dpi;

            if (dpi <= 0)
            {
                dpi = 200.0f;
            }

            return 200.0f / dpi;
        }
    }

    //是否有 任意鼠标按下
    public static bool AnyMouseButtonSet
    {
        get
        {
            for (var i = 0; i < 4; i++)
            {
                if (Input.GetMouseButton(i) == true)
                {
                    return true;
                }
            }

            return false;
        }
    }

    //是否有 任何按下
    public static bool AnyFingersSet
    {
        get
        {
            List<Finger> list = GetFingers(true);
            bool isDown = false;
            foreach (var item in list)
            {
                if (item.Set)
                {
                    isDown = true;
                    break;
                }
            }

            return isDown;
        }
    }
    public static bool AnyFingersDown
    {
        get
        {
            List<Finger> list = GetFingers(true);
            bool isDown = false;
            foreach (var item in list)
            {
                if (item.Down)
                {
                    isDown = true;
                    break;
                }
            }

            return isDown;
        }
    }


    //忽略了 点击UI的按键
    public static List<Finger> GetFingers(bool ignoreIfStartedOverGui)
    {
        if (InstanceCount == 0)
        {
            new GameObject(typeof(GL_Input).Name).AddComponent<GL_Input>();
        }

        tempFingers.Clear();

        for (var i = 0; i < fingers.Count; i++)
        {
            var finger = fingers[i];

            if (finger.Set == false)
            {
                continue;
            }

            if (ignoreIfStartedOverGui == true && finger.StartedOverGui == true)
            {
                //DDebug.LogError("~~~点击了UI");
                continue;
            }

            tempFingers.Add(finger);
        }

        return tempFingers;
    }

    //总位移
    public static Vector2 GetScaledDelta(List<Finger> fingers)
    {
        var total = Vector2.zero;
        var count = 0;

        for (var i = fingers.Count - 1; i >= 0; i--)
        {
            var finger = fingers[i];

            if (finger != null)
            {
                total += finger.ScreenPosition - finger.LastScreenPosition;
                count += 1;
            }
        }

        if (count > 0)
        {
            total *= ScaleFactor;
            total /= count;

        }

        return total;
    }

    public static Vector2 GetLastScreenCenter(List<Finger> fingers)
    {
        var total = Vector2.zero;
        var count = 0;

        for (var i = fingers.Count - 1; i >= 0; i--)
        {
            var finger = fingers[i];

            if (finger != null)
            {
                total += finger.LastScreenPosition;
                count += 1;
            }
        }

        return count > 0 ? total / count : total;
    }

    public static Vector2 GetScreenCenter(List<Finger> fingers)
    {
        var total = Vector2.zero;
        var count = 0;

        for (var i = fingers.Count - 1; i >= 0; i--)
        {
            var finger = fingers[i];

            if (finger != null)
            {
                total += finger.ScreenPosition;
                count += 1;
            }
        }

        return count > 0 ? total / count : total;
    }

    public static float GetScreenDistance(List<Finger> fingers, Vector2 center)
    {
        var total = 0.0f;
        var count = 0;

        for (var i = fingers.Count - 1; i >= 0; i--)
        {
            var finger = fingers[i];

            if (finger != null)
            {
                total += Vector2.Distance(center, finger.ScreenPosition);
                count += 1;
            }
        }

        return count > 0 ? total / count : total;
    }

    public static float GetLastScreenDistance(List<Finger> fingers, Vector2 center)
    {
        var total = 0.0f;
        var count = 0;

        for (var i = fingers.Count - 1; i >= 0; i--)
        {
            var finger = fingers[i];

            if (finger != null)
            {
                total += Vector2.Distance(center, finger.LastScreenPosition);
                count += 1;
            }
        }

        return count > 0 ? total / count : total;
    }

    public static float GetPinchScale(List<Finger> fingers, float wheelSensitivity = 0.0f)
    {
        var center = GetScreenCenter(fingers);
        var lastCenter = GetLastScreenCenter(fingers);
        var distance = GetScreenDistance(fingers, center);
        var lastDistance = GetLastScreenDistance(fingers, lastCenter);

        if (lastDistance > 0.0f)
        {
            return distance / lastDistance;
        }

        if (wheelSensitivity != 0.0f)
        {
            var scroll = Input.mouseScrollDelta.y;

            if (scroll > 0.0f)
            {
                return 1.0f - wheelSensitivity;
            }

            if (scroll < 0.0f)
            {
                return 1.0f + wheelSensitivity;
            }
        }

        return 1.0f;
    }

    public static bool PointOverGui(Vector2 screenPosition)
    {
        return RaycastGui(screenPosition).Count > 0;
    }

    public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
    {
        return RaycastGui(screenPosition, 1 << 5);
    }

    public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
    {
        tempRaycastResults.Clear();

        var currentEventSystem = EventSystem.current;

        if (currentEventSystem != null)
        {
            // Create point event data for this event system?
            if (currentEventSystem != tempEventSystem)
            {
                tempEventSystem = currentEventSystem;

                if (tempPointerEventData == null)
                {
                    tempPointerEventData = new PointerEventData(tempEventSystem);
                }
                else
                {
                    tempPointerEventData.Reset();
                }
            }

            // Raycast event system at the specified point
            tempPointerEventData.position = screenPosition;

            currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

            // Loop through all results and remove any that don't match the layer mask
            if (tempRaycastResults.Count > 0)
            {
                for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
                {
                    var raycastResult = tempRaycastResults[i];
                    var raycastLayer = 1 << raycastResult.gameObject.layer;

                    if ((raycastLayer & layerMask) == 0)
                    {
                        tempRaycastResults.RemoveAt(i);
                    }
                }
            }
        }

        return tempRaycastResults;
    }

    public void DoUpdate()
    {
        if (this == _instance)
        {
            Mark(); //标记
            Poll();
            Sweep();
        }
    }

    private void Mark()
    {
        for (var i = fingers.Count - 1; i >= 0; i--)
        {
            var finger = fingers[i];

            if (finger.Up == true)
            {
                finger.Up = false;
                finger.Set = false;
            }

            finger.Marked = true;
        }
    }

    public bool Sweep()
    {
       
        for (var i = fingers.Count - 1; i >= 0; i--)
        {
            var finger = fingers[i];

            if (finger.Marked == true && finger.Set == true)
            {
                finger.Up = true;
                return true;
            }
        }
        return false;
    }

    private void Poll()
    {
        if (Input.touchCount > 0)
        {
            for (var i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    AddFinger(touch.fingerId, touch.position, touch.pressure);
                    RefreshMove(touch.fingerId, touch.phase == TouchPhase.Moved);
                }
            }
        }
        // If there are no real touches, simulate some from the mouse?
        else
        {
            var screen = new Rect(0, 0, Screen.width, Screen.height);
            var mousePosition = (Vector2)Input.mousePosition;

            for (var i = 0; i < 4; i++)
            {
                // Is the mouse within the screen?
                if (Input.GetMouseButton(i) == true && screen.Contains(mousePosition) == true)
                {
                    AddFinger(-1 - i, mousePosition, 1.0f);
                    //pc端 伪装双指操作
                    if (Input.GetKey(KeyCode.LeftControl) == true)
                    {
                        var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

                        AddFinger(-5, center - (mousePosition - center), 1.0f);
                        //AddFinger(-2, finger0.StartScreenPosition - finger0.SwipeScreenDelta, 1.0f);
                    }
                }


            }
        }
        
    }

    private void AddFinger(int index, Vector2 screenPosition, float pressure)
    {
        var finger = GetFinger(index);

        if (finger.Set == true)
        {
            finger.Index = index;
            finger.Marked = false;
            finger.Pressure = pressure;
            finger.Down = false;
            finger.Set = true;
            finger.Up = false;
            finger.StartedOverGui      = PointOverGui(screenPosition);
            //finger.StartScreenPosition = screenPosition;
            finger.LastScreenPosition = finger.ScreenPosition;
            finger.ScreenPosition = screenPosition;
            //Debug.LogError("~~~finger.LastScreenPosition:" + finger.LastScreenPosition + "   finger.ScreenPosition: " + screenPosition);
        }
        else
        {
            finger.Index = index;
            finger.Marked = false;
            finger.Pressure = pressure;
            finger.Down = true;
            finger.Moved = false;
            finger.Set = true;
            finger.Up = false;
            finger.StartedOverGui = PointOverGui(screenPosition);
            finger.StartScreenPosition = screenPosition;
            finger.LastScreenPosition = screenPosition;
            finger.ScreenPosition = screenPosition;
        }
    }
    private void RefreshMove(int index, bool isMove)
    {
        var finger = GetFinger(index);
        finger.Move = isMove;
        if(!finger.Moved)
        {
            if((finger.LastScreenPosition - finger.ScreenPosition).sqrMagnitude > 400)
            {
                finger.Moved = true;
            }
        }
    }

    private Finger GetFinger(int index)
    {
        for (var i = fingers.Count - 1; i >= 0; i--)
        {
            var finger = fingers[i];

            if (finger.Index == index)
            {
                return finger;
            }
        }

        var newFinger = new Finger();

        newFinger.Index = index;

        fingers.Add(newFinger);

        return newFinger;
    }

    public bool GetTouchDown()
    {
        List<Finger> fingers = GetFingers(true);
        if (fingers.Count <= 0) return false;
        for( int i = 0; i<fingers.Count; i++ )
        {
            if (fingers[i].Down == true)
                return true;
        }
        return false;
    }

    public bool GetTouchSet()
    {
        List<Finger> fingers = GetFingers(true);
        if (fingers.Count <= 0) return false;
        for (int i = 0; i < fingers.Count; i++)
        {
            if (fingers[i].Set == true)
                return true;
        }
        return false;
    }

    public bool GetTouchMove()
    {
        List<Finger> fingers = GetFingers(true);
        if (fingers.Count <= 0) return false;
        for (int i = 0; i < fingers.Count; i++)
        {
            if (fingers[i].Move == true)
                return true;
        }
        return false;
    }

    #region 双指缩放操作
    public static float GetPinchScale(List<Finger> fingers)
    {
        var scale = 1.0f;
        var center = GetScreenCenter(fingers);
        var lastCenter = GetLastScreenCenter(fingers);

        TryGetPinchScale(fingers, center, lastCenter, ref scale);
        return scale;
    }
    public static bool TryGetPinchScale(List<Finger> fingers, Vector2 center, Vector2 lastCenter, ref float scale, float wheelSensitivity = 0.0f)
    {
        var distance = GetScreenDistance(fingers, center);
        var lastDistance = GetLastScreenDistance(fingers, lastCenter);

        if (lastDistance > 0.0f)
        {
            scale = distance / lastDistance; return true;
        }

        if (wheelSensitivity != 0.0f)
        {
            var scroll = Input.mouseScrollDelta.y;

            if (scroll > 0.0f)
            {
                scale = 1.0f - wheelSensitivity; return true;
            }

            if (scroll < 0.0f)
            {
                scale = 1.0f + wheelSensitivity; return true;
            }
        }

        return false;
    }
    #endregion
}