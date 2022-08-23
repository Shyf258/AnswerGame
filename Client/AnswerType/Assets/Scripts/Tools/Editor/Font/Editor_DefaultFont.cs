using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using ExcelToJson;

namespace Tools.Editor
{
    /// <summary>
    /// UGUI涉及到字体的可以自动设置为默认字体
    /// </summary>
    public class Editor_DefaultFont
    {
        [InitializeOnLoadMethod]
        private static void Init()
        {
            Action OnEvent = delegate { ChangeDefaultFont(); };

            EditorApplication.hierarchyChanged += delegate () { OnEvent(); };
        }

        private static void ChangeDefaultFont()
        {
            if (Selection.activeGameObject != null)
            {
                Text text = Selection.activeGameObject.GetComponent<Text>();

                if (text != null)
                {
                    text.font = ToolCacheManager.GetFont();
                }

                //********** 
                Text[] text_arr = Selection.activeGameObject.transform.GetComponentsInChildren<Text>();
                if (text_arr != null)
                {
                    for (int i = 0; i < text_arr.Length; i++)
                    {
                        text_arr[i].font = ToolCacheManager.GetFont();
                    }
                }
            }

        }
    }

    public class SetDefaultFont : EditorWindow
    {
        private static Font m_font;
        private static EditorWindow window;

        public static Font Font
        {
            get { return m_font; }
        }

        [MenuItem("功能/工具/设置默认字体", false, 40)]
        public static void OpenWindow()
        {
            window = GetWindow(typeof(SetDefaultFont));
            window.minSize = new Vector2(500, 300);
            m_font = ToolCacheManager.GetFont();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("选择默认字体");
            EditorGUILayout.Space();
            m_font = (Font)EditorGUILayout.ObjectField(m_font, typeof(Font), true);
            EditorGUILayout.Space();
            if (GUILayout.Button("确定"))
            {
                ToolCacheManager.SaveFont(m_font);
                window.Close();
            }
        }
    }

    public class ToolCacheManager
    {
        private static readonly string cachePath =
            Application.dataPath.Substring(0, Application.dataPath.Length - 6) + "Library/BlueToolkitCache/";

        private static void Init()
        {
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
        }

        public static void SaveFont(Font font)
        {
            //string path = "Assets/Art/Font/FontData.asset";
            //FontData data  =ScriptableObjectUtility.CreateAsset<FontData>(path);

            FontData data = ScriptableObject.CreateInstance<FontData>();
            data.defaultFont = font;
            AssetDatabase.CreateAsset(data, "Assets/Art/Font/FontData.asset");
            AssetDatabase.SaveAssets();
        }
        public static Font GetFont()
        {
            FontData data = AssetDatabase.LoadAssetAtPath<FontData>("Assets/Art/Font/FontData.asset");
            if (data == null)
            {
                return null;
            }
            return data.defaultFont;
        }
    }


}

