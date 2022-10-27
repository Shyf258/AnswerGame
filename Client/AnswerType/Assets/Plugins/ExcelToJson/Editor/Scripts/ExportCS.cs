using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel;
using Newtonsoft.Json;
using UnityEngine;

namespace ExcelToJson
{
    class ExportCS
    {
        struct FieldDef
        {
            public string name;
            public string type;
            public string comment;
            public void TypeToGo()
            {
                switch (type)
                {
                    case "byteArray":
                        type = "[]byte";
                        break;
                    case "shortArray":
                        type = "[]int16";
                        break;
                    case "floatArray":
                        type = "[]float32";
                        break;
                    case "intArray":
                        type = "[]int32";
                        break;
                    case "uintArray":
                        type = "[]uint32";
                        break;
                    case "stringArray":
                        type = "[]string";
                        break;
                    case "short":
                        type = "int16";
                        break;
                    case "float":
                        type = "float32";
                        break;
                    case "int":
                        type = "int32";
                        break;
                    case "uint":
                        type = "uint32";
                        break;
                }
            }
        }

        private static string CSFileName;

        private static List<FieldDef> m_filedList = new List<FieldDef>();

        private static List<String> refList = new List<string>();

        public static StringBuilder sb = new StringBuilder();
        static UTF8Encoding utf8notbom = new UTF8Encoding(false);
        public static void ExportCSFile(DataTable sheet, string excelName, List<int> format, EJ_ExportToolInfo _exportToolInfo)
        {
            if (sheet.Rows.Count < 1 || sheet.Columns.Count < 1)
            {
                //                MessageBox.Show("生成cs文件失败，请检查" + sheet.TableName + "Sheet表是否有效！");
                return;
            }

            CSFileName = sheet.TableName;

            m_filedList.Clear();
            refList.Clear();

            for (int i = 1; i < sheet.Rows.Count; i++)
            {
                FieldDef field;
                DataRow dataRow = sheet.Rows[i];

                field.comment = dataRow[sheet.Columns[0]].ToString();
                field.name = dataRow[sheet.Columns[format[1]]].ToString();
                field.type = dataRow[sheet.Columns[format[1] + 1]].ToString();

                //防止空行影响cs文件
                if (field.name.Length < 1 || field.type.Length < 1)
                {
                    if (field.name.Length < 1 && field.type.Length < 1)
                    {
                        continue;
                    }
                    UnityEditor.EditorUtility.DisplayDialog("Ooops!", "CS生成失败!!\"中文说明\",\"字段名\",\"数据类型均不能为空\"!请检查", "Got it!");
                    break;
                }
                switch (field.type)
                {
                    case "byteArray":
                        field.type = "List<byte>";
                        break;
                    case "shortArray":
                        field.type = "List<short>";
                        break;
                    case "floatArray":
                        field.type = "List<float>";
                        break;
                    case "intArray":
                        field.type = "List<int>";
                        break;
                    case "stringArray":
                        field.type = "List<string>";
                        break;
                }

                m_filedList.Add(field);

                //字段以xxxID结尾
                if (field.name.EndsWith("ID") && field.name.Length != 2)
                {
                    var tmp = field.name.Remove(field.name.LastIndexOf("ID"));
                    refList.Add(tmp);
                }
            }
            string key = string.Empty;

            sb.Clear();
            sb.AppendLine("// FileName: " + CSFileName + "Data.cs");
            //sb.AppendLine("// ExcelName: " + new FileInfo(MainWindow.currentOpenFilePath).Name);
            sb.AppendLine("// ExcelTool自动生成于 " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
            sb.AppendLine("//");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using System.Runtime.Serialization;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using DataModule;");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("namespace DataModule");
            sb.AppendLine("{");
            sb.AppendFormat("\tpublic class {0}Data : DataModuleBase \r\n", CSFileName);
            sb.AppendLine("\t{");

            sb.AppendLine();
            sb.AppendLine("\t\t#region 字段");
            foreach (FieldDef fieldDef in m_filedList)
            {
                var comment = fieldDef.comment;
                //如果有[delete]会跳过写这个字段
                if (comment.Contains("[delete]"))
                    continue;

                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// " + fieldDef.comment);
                sb.AppendLine("\t\t/// </summary>");
                if (comment.Contains(@"[System.Obsolete("))
                {
                    var startindex = comment.IndexOf(@"[System.Obsolete(");
                    var endindex = comment.IndexOf(@"]", startindex);
                    sb.AppendLine("\t\t" + comment.Substring(startindex, endindex - startindex + 1));
                }
                if(string.IsNullOrEmpty(key))
                {
                    key = fieldDef.type;
                }
                sb.AppendFormat("\t\tpublic {0} {1};", fieldDef.type, fieldDef.name);
                sb.AppendLine();
            }
            sb.AppendLine("\t\t#endregion");

            sb.AppendLine();
            sb.AppendLine("\t\t#region 扩展");
            sb.AppendFormat("\t\tpublic static Dictionary<{0}, {1}> dic;",key, CSFileName + "Data");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine();
            sb.AppendLine("\t\t[OnDeserialized]");
            sb.AppendLine("\t\tprivate void OnDeserialized(StreamingContext context)");
            sb.AppendLine("\t\t{");

            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\t#endregion");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendFormat("\t\tpublic static Dictionary<{0}, {1}> loadAllData()",key, CSFileName + "Data");
            sb.AppendLine();
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tif(dic == null)");
            sb.AppendLine("\t\t\t{");
            sb.AppendFormat("\t\t\t\tdic = JsonHelper.FromJson<Dictionary<{0}, {1}Data>>(Resources.Load<TextAsset>({2} + @\"{1}\").text);\r\n", key, CSFileName, "JsonPath.JSON_PATH");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\treturn dic;");
            sb.AppendLine("\t\t}");
            sb.AppendLine();
            sb.AppendLine("\t}");
            sb.AppendLine("}");


            //-- 保存文件
            using (FileStream file = new FileStream(_exportToolInfo._csFileExportPath+ "/" + CSFileName + "Data.cs", FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, utf8notbom))
                    writer.Write(sb.ToString());
            }
            Debug.LogError(CSFileName + "生成成功");
            //MessageBox.Show(CSFileName + "生成成功");

        }

        public static void ExportCSFileVertical(DataTable sheet, string excelName, List<int> format, EJ_ExportToolInfo _exportToolInfo)
        {
            if (sheet.Rows.Count < 1 || sheet.Columns.Count < 1 || format == null || format.Count < 2)
            {
                return;
            }

            CSFileName = sheet.TableName;

            m_filedList.Clear();
            refList.Clear();

            for (int i = 1; i < sheet.Columns.Count; i++)
            {
                FieldDef field;
                DataColumn dataColumn = sheet.Columns[i];

                field.comment = sheet.Rows[0][dataColumn].ToString();
                field.name = sheet.Rows[format[0]][dataColumn].ToString();
                field.type = sheet.Rows[format[0] + 1][dataColumn].ToString();

                //防止空行影响cs文件
                if (field.name.Length < 1 || field.type.Length < 1)
                {
                    if (field.name.Length < 1 && field.type.Length < 1)
                    {
                        continue;
                    }
                    UnityEditor.EditorUtility.DisplayDialog("Ooops!", "CS生成失败!!\"中文说明\",\"字段名\",\"数据类型均不能为空\"!请检查", "Got it!");
                    break;
                }
                switch (field.type)
                {
                    case "byteArray":
                        field.type = "List<byte>";
                        break;
                    case "shortArray":
                        field.type = "List<short>";
                        break;
                    case "floatArray":
                        field.type = "List<float>";
                        break;
                    case "intArray":
                        field.type = "List<int>";
                        break;
                    case "stringArray":
                        field.type = "List<string>";
                        break;
                }

                m_filedList.Add(field);

                //字段以xxxID结尾
                if (field.name.EndsWith("ID") && field.name.Length != 2)
                {
                    var tmp = field.name.Remove(field.name.LastIndexOf("ID"));
                    refList.Add(tmp);
                }
            }
            string key = string.Empty;

            sb.Clear();
            sb.AppendLine("// FileName: " + CSFileName + "Data.cs");
            //sb.AppendLine("// ExcelName: " + new FileInfo(MainWindow.currentOpenFilePath).Name);
            //sb.AppendLine("// ExcelTool自动生成于 " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
            sb.AppendLine("//");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using System.Runtime.Serialization;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using DataModule;");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("namespace DataModule");
            sb.AppendLine("{");
            sb.AppendFormat("\tpublic class {0}Data : DataModuleBase \r\n", CSFileName);
            sb.AppendLine("\t{");

            sb.AppendLine();
            sb.AppendLine("\t\t#region 字段");
            foreach (FieldDef fieldDef in m_filedList)
            {
                var comment = fieldDef.comment;
                //如果有[delete]会跳过写这个字段
                if (comment.Contains("[delete]"))
                    continue;

                sb.AppendLine("\t\t/// <summary>");
                sb.AppendLine("\t\t/// " + comment);
                sb.AppendLine("\t\t/// </summary>");
                if (comment.Contains(@"[System.Obsolete("))
                {
                    var startindex = comment.IndexOf(@"[System.Obsolete(");
                    var endindex = comment.IndexOf(@"]", startindex);
                    sb.AppendLine("\t\t" + comment.Substring(startindex, endindex - startindex + 1));
                }
                if(string.IsNullOrEmpty(key))
                {
                    key = fieldDef.type;
                }

                sb.AppendFormat("\t\tpublic {0} {1};", fieldDef.type, fieldDef.name);
                sb.AppendLine();
            }
            sb.AppendLine("\t\t#endregion");

            sb.AppendLine();
            sb.AppendLine("\t\t#region 扩展");
            sb.AppendFormat("\t\tpublic static Dictionary<{0}, {1}> dic;",key, CSFileName + "Data");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine();
            sb.AppendLine("\t\t[OnDeserialized]");
            sb.AppendLine("\t\tprivate void OnDeserialized(StreamingContext context)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\t#endregion");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendFormat("\t\tpublic static Dictionary<{0}, {1}> loadAllData()\r\n", key, CSFileName + "Data");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tif(dic == null)");
            sb.AppendLine("\t\t\t{");
            sb.AppendFormat("\t\t\t\tdic = JsonHelper.FromJson<Dictionary<{0}, {1}Data>>(Resources.Load<TextAsset>({2} + @\"{1}\").text);\r\n", key, CSFileName, "JsonPath.JSON_PATH");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\treturn dic;");
            sb.AppendLine("\t\t}");
            sb.AppendLine();
            sb.AppendLine("\t}");
            sb.AppendLine("}");


            //-- 保存文件
            using (FileStream file = new FileStream(_exportToolInfo._csFileExportPath + "/" + CSFileName + "Data.cs", FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, utf8notbom))
                    writer.Write(sb.ToString());
            }
            //DDebug.LogError(CSFileName + "生成成功");
            //MessageBox.Show(CSFileName + "生成成功");
        }
    }
}
