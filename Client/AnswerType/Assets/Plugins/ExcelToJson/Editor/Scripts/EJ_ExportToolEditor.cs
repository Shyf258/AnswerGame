//2019.11.11    关林
//导出工具

using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExcelToJson
{
    public class EJ_ExportToolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("开启 导表工具"))
                EJ_ExportToolEditorWindow.Init();
        }
    }
}

