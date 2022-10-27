/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题： Unity 帮助脚本
 *    Description: 
 *           功能： 提供程序用户一些常用的功能方法实现，方便程序员快速开发。
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SUIFW
{
    public class UnityHelper : MonoBehaviour
    {

        /// <summary>
        /// 查找子节点对象
        /// 内部使用“递归算法”
        /// </summary>
        /// <param name="goParent">父对象</param>
        /// <param name="chiildName">查找的子对象名称</param>
        /// <returns></returns>
        public static Transform FindTheChildNode(GameObject goParent, string chiildName)
        {
            Transform searchTrans = null;                   //查找结果

            searchTrans = goParent.transform.Find(chiildName);
            if (searchTrans == null)
            {
                foreach (Transform trans in goParent.transform)
                {
                    searchTrans = FindTheChildNode(trans.gameObject, chiildName);
                    if (searchTrans != null)
                    {
                        return searchTrans;
                    }
                }
            }
            return searchTrans;
        }

        /// <summary>
        /// 获取子节点（对象）脚本
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="goParent">父对象</param>
        /// <param name="childName">子对象名称</param>
        /// <returns></returns>
	    public static T GetTheChildNodeComponetScripts<T>(GameObject goParent, string childName) where T : Component
        {
            Transform searchTranformNode = null;            //查找特定子节点

            searchTranformNode = FindTheChildNode(goParent, childName);

            if (searchTranformNode != null)
            {
                return searchTranformNode.gameObject.GetComponent<T>();
            }
            else
            {
                Debug.Log(childName + "   is not found    parent :" + goParent.name);
                return null;
            }
        }
        

        /// <summary>
        /// 给子节点添加脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="goParent">父对象</param>
        /// <param name="childName">子对象名称</param>
        /// <returns></returns>
	    public static T AddChildNodeCompnent<T>(GameObject goParent, string childName) where T : Component
        {
            Transform searchTranform = null;                //查找特定节点结果

            //查找特定子节点
            searchTranform = FindTheChildNode(goParent, childName);
            //如果查找成功，则考虑如果已经有相同的脚本了，则先删除，否则直接添加。
            if (searchTranform != null)
            {
                //如果已经有相同的脚本了，则先删除
                T[] componentScriptsArray = searchTranform.GetComponents<T>();
                for (int i = 0; i < componentScriptsArray.Length; i++)
                {
                    if (componentScriptsArray[i] != null)
                    {
                        Destroy(componentScriptsArray[i]);
                    }
                }
                return searchTranform.gameObject.AddComponent<T>();
            }
            else
            {
                return null;
            }
            //如果查找不成功，返回Null.
        }

        /// <summary>
        /// 给子节点添加父对象
        /// </summary>
        /// <param name="parents">父对象的方位</param>
        /// <param name="child">子对象的方法</param>
	    public static void AddChildNodeToParentNode(Transform parents, Transform child)
        {
            child.SetParent(parents, false);
            child.localPosition = Vector3.zero;
            child.localScale = Vector3.one;
            child.localEulerAngles = Vector3.zero;
        }
        /// <summary>
        /// 获取UI名字
        /// </summary>
        /// <returns>The UIN ame.</returns>
        /// <param name="ui">User interface.</param>
        public static string GetUIName(BaseUIForm ui)
        {
            string strUIFromName = string.Empty;            //处理后的UIFrom 名称
            string[] arr;

            strUIFromName = ui.GetType().ToString();             //命名空间+类名
            arr = strUIFromName.Split('_');
            if (arr != null && arr.Length > 0)
            {
                //剪切字符串中“.”之间的部分
                strUIFromName = arr[arr.Length - 1];
            }
            return strUIFromName;
        }

        /// <summary>
        /// Clear子物体
        /// </summary>
        /// <param name="go">Go.</param>
        public static void removeChildren(GameObject go)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                GameObject.Destroy(child);
            }
        }

        public static Camera GetUICamera()
        {
            return GameObject.FindGameObjectWithTag("TagUICamera").GetComponent<Camera>();
        }


        public static IEnumerator TextAlpha_Change(UnityEngine.UI.Text root, float start, float end, float time)
        {
            float dur = 0.0f;
            while (dur <= time)
            {
                dur += Time.deltaTime;
                float value = Mathf.Lerp(start, end, dur / time);
                if (root != null)
                {
                    root.color = new Color(root.color.r, root.color.g, root.color.b, value);
                }
                yield return null;
            }
            yield return null;
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
                return string.Format("{0:D2}", second / 3600) + "h " + string.Format("{0:D2}", second % 3600 / 60) + "m " + string.Format("{0:D2}", second % 60)+"s";
            }
            else if(second / 60 >0)
            {
                return string.Format("{0:D2}", second % 3600 / 60) + "m " + string.Format("{0:D2}", second % 60)+"s";
            }
            else 
            {
                return string.Format("{0:D2}", second % 60) + "s";
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

        public static IEnumerator NumChange(int startNum, int endNum, float time, Action<int> action = null)
        {
            var interval = endNum - startNum;
            float dur = 0f;
            while (dur < time)
            {
                dur += Time.deltaTime;
                if (dur > time)
                {
                    dur = time;
                }
                //var res = dur * interval + startNum;
                int res = (int)Mathf.Lerp(startNum, endNum, dur / time);
                if (action != null)
                {
                    action(res);
                }
                yield return null;
            }
            yield return null;
        }
    }
}