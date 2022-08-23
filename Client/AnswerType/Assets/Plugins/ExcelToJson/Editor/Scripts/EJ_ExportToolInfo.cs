//2019.11.11    关林
//主窗口 信息

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ExcelToJson
{
    [System.Serializable]
    public class EJ_ExportToolInfo : ScriptableObject
    {
        public string _excelFileImportPath;   //excel 存放路径

        public string _jsonFileExportPath;    //json 导出路径
        public string _csFileExportPath;    //cs文件 导出路径

        public bool _isExportCs = false;    //是否导出cs文件

        public void Init()
        {
            //excel 路径
            //if(_excelFileImportPath == string.Empty || _excelFileImportPath == null)
            {
                string path = Application.dataPath;
                string[] tokens = path.Split('/');
                if (tokens.Length == 0)
                    return;
                path = string.Empty;
                for (int i = 0; i < tokens.Length - 2; i++)
                {
                    path += tokens[i];
                    path += "/";
                }
                path += "Table/";
                _excelFileImportPath = path;
            }

            //json 导出路径
            //if(_jsonFileExportPath == string.Empty || _jsonFileExportPath == null)
                _jsonFileExportPath = Application.dataPath + "/DataModule/Resources/Json/";

            //cs文件 导出路径
            //if(_csFileExportPath == string.Empty || _csFileExportPath == null)
                _csFileExportPath = Application.dataPath + "/DataModule/Scripts/DataModule/";
        }
    }

}

