//2022.3.30 关林
//自动化打包工具 界面绘制

using DataModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using UnityEditor;
using UnityEngine;

namespace BuildTool
{
    public class BT_BuildToolEditorWindow : EditorWindow
    {
        public static BT_BuildToolEditorWindow _buildToolEditorWindow;
        private static AppSettingConfig _appSettingConfigInfo;
        private static Dictionary<int, TableBuildAppData> _tableBuildAppData;    //表格信息

        private Vector2 scrollPos;

        [MenuItem("功能/打包工具", false)]
        public static void Init()
        {
            _buildToolEditorWindow = EditorWindow.GetWindow<BT_BuildToolEditorWindow>(false, "打包工具", true);
            _buildToolEditorWindow.Show();
            _buildToolEditorWindow.RefreshWindow();
            _buildToolEditorWindow.RefreshData();
        }
        private string buildPath;
        #region 窗口样式
        private string titleStyle;
        private string addButtonStyle;
        private string borderBarStyle;
        private string rootGroupStyle;
        private string fillBarStyle1;
        private string subGroupStyle;
        private string arrayElementStyle;
        private string subArrayElementStyle;
        private string foldStyle;
        private string enumStyle;
        private GUIStyle labelStyle;

        private bool isSpineName;
        private bool isCamera;
        private bool isInput;
        private bool isKeyboard;
        private bool isGamepad;
        //刷新窗口
        private void RefreshWindow()
        {
            titleStyle = "MeTransOffRight";
            borderBarStyle = "ProgressBarBack";
            addButtonStyle = "CN CountBadge";
            rootGroupStyle = "GroupBox";
            subGroupStyle = "ObjectFieldThumb";
            arrayElementStyle = "flow overlay box";
            fillBarStyle1 = "ProgressBarBar";
            subArrayElementStyle = "HelpBox";
            foldStyle = "Foldout";
            enumStyle = "MiniPopup";
            _appSettingConfigInfo = AssetDatabase.LoadAssetAtPath<AppSettingConfig>("Assets/Resources/AppSettingConfig.asset");
            buildPath = "../../../Build";
        }

        private void RefreshData()
        {

            
            //查找编辑器文件
            if (_tableBuildAppData == null)
            {
                _tableBuildAppData = TableBuildAppData.loadAllData();
                //_appSettingConfigInfo.Init();
                //UpdateExcelComboBox(_appSettingConfigInfo._excelFileImportPath);
                //UpdateExcelCbSelected();
            }
        }
        //关闭窗口
        private void CloseWindow()
        {
            _appSettingConfigInfo = null;
        }
        #endregion

        #region 绘制
        public void OnGUI()
        {
            RefreshData();
            //if (_appSettingConfigInfo == null)
            //{
            //    RefreshWindow();
            //    RefreshData();
            //    if (_appSettingConfigInfo == null)
            //    {
            //        //没有数据时, 创建一个导出 编辑器文件
            //        string path = "Assets/Plugins/ExcelToJson/ExprotTool.asset";
            //        ScriptableObjectUtility.CreateAsset<EJ_ExportToolInfo>(path);
            //    }
            //    return;
            //}

            GUIStyle fontStyle = new GUIStyle();
            fontStyle.font = (Font)Resources.Load("EditorFont");
            fontStyle.fontSize = 30;
            fontStyle.alignment = TextAnchor.UpperCenter;
            fontStyle.normal.textColor = Color.white;
            fontStyle.hover.textColor = Color.white;

            //标题
            EditorGUILayout.BeginVertical(titleStyle);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("", (""), fontStyle, GUILayout.Height(32));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            //窗口主体
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                #region excel 预留区
                EditorGUILayout.BeginVertical(rootGroupStyle);
                {

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("打包设置");
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.TextField("安装包路径:", buildPath);
                        if (GUILayout.Button("选择", GUILayout.Width(72), GUILayout.Height(18)))
                        {
                            buildPath = UnityEditor.EditorUtility.OpenFolderPanel("选择cs文件存放路径", "", "");
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    // EditorGUILayout.BeginHorizontal();
                    // {
                    //     EditorGUILayout.LabelField("是否开启LOG");
                    //     
                    //     if (GUILayout.Button(_appSettingConfigInfo._isLog ? "\u2714" : " ", GUILayout.Width(25)))
                    //     {
                    //         _appSettingConfigInfo._isLog = !_appSettingConfigInfo._isLog;
                    //     }
                    // }
                    // EditorGUILayout.EndHorizontal();
                    // EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                #endregion

                #region 打包区域
                
                EditorGUILayout.BeginHorizontal(titleStyle);
                EditorGUILayout.LabelField("点击对应打包配置:");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal(subGroupStyle);
                {
                    int index = 0;
                    foreach (var table in _tableBuildAppData.Values)
                    {
                        index++;

                        if (index <= 4)
                        {
                            SetVerticalButton(table);

                            if (index == 4)
                            {
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal(subGroupStyle);
                            }
                        }
                        else
                        {
                            SetVerticalButton(table);
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

                #endregion
            }
            EditorGUILayout.EndScrollView();


            if (GUI.changed)
            {
                Undo.RecordObject(_appSettingConfigInfo, "打包工具");
                EditorUtility.SetDirty(_appSettingConfigInfo);
            }
        }

        private void SetVerticalButton(TableBuildAppData table)
        {
            EditorGUILayout.BeginVertical(subGroupStyle);
            {
                string name = table.ProductName;
                GUILayout.Label(name, GUILayout.Width(200), GUILayout.Height(30));

                EditorGUILayout.BeginVertical();
                {
                    string buttonName = "正式版";
                    if (GUILayout.Button(buttonName, GUILayout.Width(200), GUILayout.Height(30)))
                    {
                        OnClickBuild(table, EBuildType.Normal);
                    }
                    EditorGUILayout.Space();
                    buttonName = "测试版";
                    if (GUILayout.Button(buttonName, GUILayout.Width(200), GUILayout.Height(30)))
                    {
                        OnClickBuild(table, EBuildType.Test);
                    }
                    EditorGUILayout.Space();
                    buttonName = "纯净版";
                    if (GUILayout.Button(buttonName, GUILayout.Width(200), GUILayout.Height(30)))
                    {
                        OnClickBuild(table, EBuildType.PureVersion);
                    }
                }
                            
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        //绘制标题
        private void SubGroupTitle(string _name)
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_name);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }
        //绘制按钮
        public bool StyledButton(string label)
        {
            EditorGUILayout.Space();
            GUILayoutUtility.GetRect(1, 20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //bool clickResult = GUILayout.Button(label, addButtonStyle);
            bool clickResult = GUILayout.Button(label, GUILayout.Width(100), GUILayout.Height(60));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            return clickResult;
        }


        #endregion

        #region API
        public void Update()
        {
        }

        void OnSelectionChange()
        {
            RefreshWindow();
            Repaint();
        }
        void OnEnable()
        {
            RefreshWindow();
        }

        void OnFocus()
        {
            RefreshWindow();
        }

        void OnDisable()
        {
            CloseWindow();
        }

        void OnDestroy()
        {
            CloseWindow();
        }

        #endregion

        #region Logic

        private void OnClickBuild(TableBuildAppData data, EBuildType type)
        {
            //PlayerSettings.defaultCursor
            #region 1.修改AppSettingConfig配置
            _appSettingConfigInfo._buildApp = (EBuildApp)data.ID;

            //记录打包时间
            DateTime dt = DateTime.UtcNow;
            TimeSpan ts = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            _appSettingConfigInfo._buildTime = ts.TotalSeconds;

            if (type == EBuildType.Normal)
            {
                _appSettingConfigInfo._serverType = EServerType.Noraml;
                _appSettingConfigInfo._isCloseGuide = false;
                _appSettingConfigInfo._isLog = false;
                _appSettingConfigInfo._isTestUnity = false;
                _appSettingConfigInfo._enterType = EGameEnterType.WaitServer;
                _appSettingConfigInfo._isSkipAD = false;
            }
            else if (type == EBuildType.Test)
            {
                _appSettingConfigInfo._serverType = EServerType.Test;
                _appSettingConfigInfo._isCloseGuide = false;
                _appSettingConfigInfo._isLog = true;
                _appSettingConfigInfo._isTestUnity = false;
                _appSettingConfigInfo._enterType = EGameEnterType.WaitServer;
                _appSettingConfigInfo._isSkipAD = false;
            }
            else if (type == EBuildType.PureVersion)
            {
                _appSettingConfigInfo._serverType = EServerType.Noraml;
                _appSettingConfigInfo._isCloseGuide = false;
                _appSettingConfigInfo._isLog = false;
                _appSettingConfigInfo._isTestUnity = true;
                _appSettingConfigInfo._enterType = EGameEnterType.PureVersion;
                _appSettingConfigInfo._isSkipAD = true;
            }
            #endregion

            #region 2.PlaySetting配置
            //icon
            string iconPath = "Assets/Art/UI/Image/Icon/" + data.IconPath + ".png";
            SetDefaultIcon(iconPath);
            //游戏名
            PlayerSettings.productName = data.ProductName;
            PlayerSettings.SplashScreen.show = false;
            //包名


            #endregion

            //处理切换逻辑
            string path = $"{buildPath}/{data.ProductName}_{type}(" +
                          $"{DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day} {DateTime.Now.Hour}：{DateTime.Now.Minute})";

            //删除老文件夹
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
            
            BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.CompressWithLz4);
            
            // BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
            DDebug.LogError("~~~打包完成: " + path);

        }
        //获取所有待打包场景
        private static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();
            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;
                if (e.enabled)
                    names.Add(e.path);
            }
            return names.ToArray();
        }

        #region 设置icon
        private void SetDefaultIcon(string iconPath)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (texture == null)
            {
                DDebug.LogError("获取icon失败，Path:" + iconPath);
                return;
            }
            
            int[] iconSize = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Android);
            Texture2D[] textureArray = new Texture2D[iconSize.Length];
            for (int i = 0; i < textureArray.Length; i++)
            {
                textureArray[i] = texture;
            }
            textureArray[0] = texture;

            //设置Legacy icon
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, textureArray);

            //设置Round icon
            SetIcons(BuildTargetGroup.Android, UnityEditor.Android.AndroidPlatformIconKind.Round, texture);

            AssetDatabase.SaveAssets();
        }
        #endregion

        private static Texture2D[] GetIconsFromAsset(BuildTargetGroup target, PlatformIconKind kind, PlatformIcon[] icons, Texture2D texture)
        {
            Texture2D[] texArray = new Texture2D[icons.Length];

            //因为Android设置会带有" API (xx)"等附加信息，为了文件夹不出现空格，只取空格前单词
            for (int i = 0; i < texArray.Length; ++i)
            {
                //不需要再通过GetIconSizesForTargetGroup了来获得Icon尺寸数组，
                //直接由对应的PlatformIcon.width来获取Icon大小
                texArray[i] = texture;
            }
            return texArray;
        }

        private static void SetIcons(BuildTargetGroup platform, PlatformIconKind kind, Texture2D texture)
        {
            //获得当前平台和当前Icon类型的PlatformIcon数组
            PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(platform, kind);

            //将Asset转为Texture2D
            Texture2D[] iconSources = GetIconsFromAsset(platform, kind, icons, texture);

            for (int i = 0, length = icons.Length; i < length; ++i)
            {
                icons[i].SetTexture(iconSources[i]);
            }

            PlayerSettings.SetPlatformIcons(platform, kind, icons);

            //AssetDatabase.SaveAssets();
        }
        #endregion

        private enum EBuildType
        {
            Normal,
            Test,
            PureVersion,
        }
    }

}


