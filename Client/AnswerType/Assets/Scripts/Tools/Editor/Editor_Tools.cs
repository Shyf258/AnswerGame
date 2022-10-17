//2021.11.22    关林
//编辑器工具

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Editor_Tools 
{
    #region 调试工具

    #region GM工具
    [MenuItem("功能/调试/当前GM已关闭,开启", true, 1)]
    public static bool SetScriptingDefineSymbolsForGrouptrue1()
    {
#if GuanGuan_Test
        return false;
#else
        return true;
#endif        
    }
    [MenuItem("功能/调试/当前GM已开启,关闭", true,1)]
    public static bool SetScriptingDefineSymbolsForGroupFalse1()
    {
#if GuanGuan_Test
        return true;
#else
        return false;
#endif        
    }


    [MenuItem("功能/调试/当前GM已关闭,开启", false,1)]
    public static void SetScriptingDefineSymbolsForGrouptrue()
    {
        SetScriptingDefineSymbolsForGroup("GuanGuan_Test", true);
    }
    [MenuItem("功能/调试/当前GM已开启,关闭", false,1)]
    public static void SetScriptingDefineSymbolsForGroupFalse()
    {
        SetScriptingDefineSymbolsForGroup("GuanGuan_Test", false);
    }
    #endregion

    #region 存档
    [UnityEditor.MenuItem("功能/存档/打开存档地址", false, 23)]
    public static void OpenPersistentDataPath()
    {
        Reveal(Application.persistentDataPath);
    }

    [UnityEditor.MenuItem("功能/存档/删除存档", false, 23)]
    public static void DeleteData()
    {
        string filePath = Application.persistentDataPath + "/CharacterData";
        if (IsFileExists(filePath))
        {
            File.Delete(filePath);
           
        }
        
        GL_PlayerPrefs.Clear();
        
        AssetDatabase.Refresh();

    }

    public static bool IsFileExists(string filePath)
    {
        if (filePath.Equals(string.Empty))
        {
            return false;
        }
        return File.Exists(@filePath);
    }

    public static void Reveal(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning(string.Format("Folder '{0}' is not Exists", folderPath));
            return;
        }

        EditorUtility.RevealInFinder(folderPath);
    }
    #endregion




    //根据平台宏切换
    private static void SetScriptingDefineSymbolsForGroup(string targetDefine, bool set)
    {
        //获取当前宏
        string curDefine;
        BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;
#if UNITY_ANDROID
        targetGroup = BuildTargetGroup.Android;
#elif UNITY_IOS
        targetGroup = BuildTargetGroup.iOS;
#endif
        curDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        if (set)
        {
            //添加宏
            if (curDefine.Contains(targetDefine))
                return;
            curDefine += ";";
            curDefine += targetDefine;
            curDefine.Trim();
        }
        else
        {
            int index = curDefine.IndexOf(targetDefine);
            if (index < 0)
            {
                return;
            }
            else
            {
                StringBuilder sb = new StringBuilder(curDefine);
                //if(index == 0)
                {
                    sb.Remove(index, targetDefine.Length);
                    curDefine = sb.ToString();
                }
            }
        }


        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, curDefine);

    }
    #endregion
}
