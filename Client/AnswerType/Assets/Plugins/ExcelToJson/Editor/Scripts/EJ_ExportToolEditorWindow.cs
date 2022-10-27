//2019.11.11    关林
//excel转json  导表工具
//主窗口绘制

using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Excel;
using System.Data;

namespace ExcelToJson
{
    public class EJ_ExportToolEditorWindow : EditorWindow
    {
        public static EJ_ExportToolEditorWindow _exportToolEditorWindow;
        private static EJ_ExportToolInfo _exportToolInfo;

        private Vector2 scrollPos;

        private int _selectIndex;

        [MenuItem("功能/工具/导表工具", false)]
        public static void Init()
        {
            _exportToolEditorWindow = EditorWindow.GetWindow<EJ_ExportToolEditorWindow>(false, "导表工具", true);
            _exportToolEditorWindow.Show();
            _exportToolEditorWindow.RefreshWindow();
            _exportToolEditorWindow.RefreshData();
        }

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


            _exportToolInfo = AssetDatabase.LoadAssetAtPath<EJ_ExportToolInfo>("Assets/Plugins/ExcelToJson/ExprotTool.asset");

            //检测选中的对象
            //UnityEngine.Object[] selection = Selection.GetFiltered(typeof(EJ_ExportToolEditorWindow), SelectionMode.Assets);

            //if (0 < selection.Length)
            //{
            //    if (null == selection[0]) return;
            //    _exportToolInfo = (EJ_ExportToolInfo)selection[0];
            //}


                

        }

        private void RefreshData()
        {
            //查找编辑器文件
            if (_exportToolInfo != null)
            {
                _exportToolInfo.Init();
                UpdateExcelComboBox(_exportToolInfo._excelFileImportPath);
                UpdateExcelCbSelected();
            }
        }
        //关闭窗口
        private void CloseWindow()
        {
            _exportToolInfo = null;
        }
        #endregion

        #region 绘制
        public void OnGUI()
        {
            if (_exportToolInfo == null)
            {
                RefreshWindow();
                RefreshData();
                if (_exportToolInfo == null)
                {
                    //没有数据时, 创建一个导出 编辑器文件
                    string path = "Assets/Plugins/ExcelToJson/ExprotTool.asset";
                    ScriptableObjectUtility.CreateAsset<EJ_ExportToolInfo>(path);
                }
                return;
            }

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
                #region excel 路径
                EditorGUILayout.BeginVertical(rootGroupStyle);
                {

                    //选择excel 路径
                    EditorGUILayout.BeginHorizontal();
                    {
                        _exportToolInfo._excelFileImportPath = EditorGUILayout.TextField("Excel路径:", _exportToolInfo._excelFileImportPath);
                        if (GUILayout.Button("选择", GUILayout.Width(72), GUILayout.Height(18)))
                        {
                            string tempPath = UnityEditor.EditorUtility.OpenFolderPanel("选择Excel文件夹路径", _exportToolInfo._excelFileImportPath, "");
                            if (tempPath != null && tempPath != string.Empty)
                                _exportToolInfo._excelFileImportPath = tempPath;
                            UpdateExcelComboBox(_exportToolInfo._excelFileImportPath);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();


                    //选择json 存放路径
                    EditorGUILayout.BeginHorizontal();
                    {
                        _exportToolInfo._jsonFileExportPath = EditorGUILayout.TextField("Json存放路径:", _exportToolInfo._jsonFileExportPath);
                        if (GUILayout.Button("选择", GUILayout.Width(72), GUILayout.Height(18)))
                        {
                            string tempPath = UnityEditor.EditorUtility.OpenFolderPanel("选择Json文件存放路径", _exportToolInfo._jsonFileExportPath, "");
                            if (tempPath != null && tempPath != string.Empty)
                                _exportToolInfo._jsonFileExportPath = tempPath;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    //选择cs 存放路径
                    EditorGUILayout.BeginHorizontal();
                    {
                        _exportToolInfo._csFileExportPath = EditorGUILayout.TextField("cs存放路径:", _exportToolInfo._csFileExportPath);
                        if (GUILayout.Button("选择", GUILayout.Width(72), GUILayout.Height(18)))
                        {
                            string tempPath = UnityEditor.EditorUtility.OpenFolderPanel("选择cs文件存放路径", _exportToolInfo._csFileExportPath, "");
                            if (tempPath != null && tempPath != string.Empty)
                                _exportToolInfo._csFileExportPath = tempPath;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("导出CS文件");
                        
                        if (GUILayout.Button(_exportToolInfo._isExportCs ? "\u2714" : " ", GUILayout.Width(25)))
                        {
                            _exportToolInfo._isExportCs = !_exportToolInfo._isExportCs;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                #endregion

                #region Excel 选择区
                EditorGUILayout.BeginVertical(rootGroupStyle);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        //_exportToolInfo._excelFileImportPath = EditorGUILayout.TextField("Excel路径:", _exportToolInfo._excelFileImportPath);
                        string[] options = EJ_ExportLogic._instance._excelFileList.ToArray();
                        int index = EditorGUILayout.Popup("Excel:", _selectIndex, options);
                        if(index != _selectIndex)
                        {
                            _selectIndex = index;
                            UpdateExcelCbSelected();
                        }
                            
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginVertical(subGroupStyle);
                    {
                        if(EJ_ExportLogic._instance._excelDataList.Count == 0)
                        {
                            //没有table
                            EditorGUILayout.LabelField("请选择正确Excel");
                        }
                        else
                        {
                            foreach (var data in EJ_ExportLogic._instance._excelDataList)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    if(GUILayout.Button(data._isSelected ? "\u2714" : " ", GUILayout.Width(25)))
                                    {
                                        data._isSelected = !data._isSelected;
                                    }
                                    EditorGUILayout.LabelField(data._data.TableName);

                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                #endregion

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                {
                    if(StyledButton("刷新"))
                    {
                        RefreshData();
                    }
                    if (StyledButton("导出"))
                    {
                        EJ_ExportLogic._instance.ExportExcel(_exportToolInfo);
                        AssetDatabase.Refresh();
                    }
                    //if (GUILayout.Button("刷新", GUILayout.Width(100), GUILayout.Height(72)))
                    //{
                    //    //RefreshModulePrefab();
                    //}

                    //if (GUILayout.Button("刷新", GUILayout.Width(200), GUILayout.Height(100)))
                    //{
                    //    //_sceneModule.PrefabSaveData();
                    //    //PrefabUtility.ReplacePrefab(_sceneModule.gameObject, PrefabUtility.GetPrefabParent(_sceneModule), ReplacePrefabOptions.ConnectToPrefab);
                    //}
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                {
                    if (StyledButton("全部导出"))
                    {
                        EJ_ExportLogic._instance.ExportExcelAll(_exportToolInfo);
                        AssetDatabase.Refresh();
                    }
                }
                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndScrollView();


            if (GUI.changed)
            {
                Undo.RecordObject(_exportToolInfo, "导表工具");
                EditorUtility.SetDirty(_exportToolInfo);
            }
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
        ///更新Excel文件ComboBox
        private void UpdateExcelComboBox(string excelFilePath)
        {
            if (string.IsNullOrEmpty(excelFilePath))
            {
                return;
            }
            var di = new DirectoryInfo(excelFilePath);
            EJ_ExportLogic._instance._excelFileList.Clear();
            foreach (var path in di.GetDirectories())
            {
                foreach (var fileInfo in path.GetFiles())
                {
                    if (fileInfo.Name.EndsWith(".xlsm") || fileInfo.Name.EndsWith(".xlsx"))
                    {
                        if (!fileInfo.Name.Contains("~$"))
                            EJ_ExportLogic._instance._excelFileList.Add(path.Name + "/" + fileInfo.Name);
                    }
                }
            }
            foreach (var fileInfo in di.GetFiles())
            {
                if (fileInfo.Name.EndsWith(".xlsm") || fileInfo.Name.EndsWith(".xlsx"))
                {
                    if(!fileInfo.Name.Contains("~$"))
                        EJ_ExportLogic._instance._excelFileList.Add(fileInfo.Name);
                }
            }

            //if (ExcelFileCb.Items.Count > 0)
            //{
            //    ExcelFileCb.SelectedIndex = 0;
            //}
            //else
            //{
            //    MessageBox.Show("海峰，这个目录下面没有Excel文件! 困了累了喝红牛！╮(╯▽╰)╭");
            //}
        }

        /// <summary>
        /// Excel下拉列表更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateExcelCbSelected()
        {
            if (EJ_ExportLogic._instance._excelFileList.Count == 0)
                return;
            if (_selectIndex >= EJ_ExportLogic._instance._excelFileList.Count)
                _selectIndex = 0;

            string currentOpenFilePath = _exportToolInfo._excelFileImportPath + "/" + EJ_ExportLogic._instance._excelFileList[_selectIndex];
            //currentOpenFilePath = currentOpenFilePath.Replace("/", "\\");
            using (FileStream excelFile = File.Open(currentOpenFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(excelFile);
                var book = excelReader.AsDataSet();

                EJ_ExportLogic._instance._excelDataList.Clear();
                foreach (DataTable dataTable in book.Tables)
                {
                    SDataTable data = new SDataTable();
                    data._data = dataTable;
                    EJ_ExportLogic._instance._excelDataList.Add(data);
                }
            }
        }
        #endregion
    }

}

