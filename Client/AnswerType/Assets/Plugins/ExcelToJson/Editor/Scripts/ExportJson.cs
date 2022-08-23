using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using UnityEngine;

namespace ExcelToJson
{
    internal class ExportJson
    {
        private static bool isError;
        private static StringBuilder errorMessage = new StringBuilder();

        private static string JsonFileName;

        public static Dictionary<string, Dictionary<string, object>> m_data =
            new Dictionary<string, Dictionary<string, object>>();
        static UTF8Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);


        public static bool IsVertical(DataTable sheet, ref List<int> format)
        {
            bool result = true;
            if (sheet.Rows.Count < 1 || sheet.Columns.Count < 1)
            {
                UnityEditor.EditorUtility.DisplayDialog("Ooops!", sheet.TableName + "Sheet为空，请检查!", "Got it!");
                return result;
            }

            format = array2List<int>(sheet.Rows[0][0].ToString().Trim());
            if (format == null)
                format = array2List1<int>(sheet.Rows[0][0].ToString().Trim());
            if (null == format)
            {
                return result;
            }
            else
            {
                format[0] -= 1;
                format[1] -= 1;
            }

            //检测是否是横表
            if(sheet.Rows[1][0].ToString().Trim() == "1")
            {
                result = false;
            }

            return result;
        }


        public static void ExportJsonFile(DataTable sheet, List<int> format, EJ_ExportToolInfo _exportToolInfo)
        {
            m_data.Clear();
            isError = false;
            errorMessage.Clear();
            errorMessage.AppendLine("json生成失败!!!");

            JsonFileName = sheet.TableName;

            for (var currentCol = format[1] + 2; currentCol < sheet.Columns.Count; currentCol++)
            {
                //当前数据列
                var id = string.Empty;
                var dic = new Dictionary<string, object>();
                for (var currentRow = format[0]; currentRow < sheet.Rows.Count; currentRow++)
                {
                    var row = sheet.Rows[currentRow];
                    if (row == null || (row[sheet.Columns[currentCol]].ToString().Trim() == "" && currentRow == 1))
                    {
                        break;
                    }
                    if (currentRow == format[0])
                    {
                        //string temp = row[sheet.Columns[currentCol]].ToString();
                        id = row[sheet.Columns[currentCol]].ToString().Trim();
                    }

                    //获取key
                    var key = row[sheet.Columns[format[1]]].ToString();

                    //获取value
                    var value = row[sheet.Columns[currentCol]];

                    if (value.ToString().Trim() == "")
                    {
                        //json文件基础类型为null，反序列基础类型报错
                        //                        value = null;  
                        //excel数据为空，json忽略改数据，反序列化基础类型默认值为0，对象默认值为null
                        continue;
                    }
                    else
                    {
                        //根据type特判
                        switch (row[sheet.Columns[format[1] + 1]].ToString().Trim())
                        {
                            case "stringArray":
                                value = array2List<string>(value.ToString().Trim());
                                break;
                            case "intArray":
                                value = array2List<int>(value.ToString().Trim());
                                break;
                            case "floatArray":
                                value = array2List<float>(value.ToString().Trim());
                                break;
                            case "shortArray":
                                value = array2List<short>(value.ToString().Trim());
                                break;
                            case "byteArray":
                                value = array2List<byte>(value.ToString().Trim());
                                break;
                            case "float":
                                try
                                {
                                    if (value.ToString().Trim().EndsWith("%"))
                                        value = Convert.ToSingle(value.ToString().TrimEnd('%')) / 100f;
                                    else
                                        value = Convert.ToSingle(value.ToString().Trim());
                                }
                                catch (Exception)
                                {
                                    isError = true;
                                    string error = string.Format("{0}行的数据类型为float,ID为{1}列所填类型与其不相符!", key, id);
                                    //                                    MessageBox.Show(error);
                                    errorMessage.AppendLine(error);
                                }
                                break;
                            case "int":
                                try
                                {
                                    value = Convert.ToInt32(value.ToString().Trim());
                                }
                                catch (Exception)
                                {
                                    isError = true;
                                    string error = string.Format("{0}行的数据类型为int,ID为{1}列所填类型与其不相符!", key, id);
                                    errorMessage.AppendLine(error);
                                }
                                break;
                            case "short":
                                try
                                {
                                    value = Convert.ToInt32(value.ToString().Trim());
                                }
                                catch (Exception)
                                {
                                    isError = true;
                                    string error = string.Format("{0}行的数据类型为short,ID为{1}列所填类型与其不相符!", key, id);
                                    errorMessage.AppendLine(error);
                                }
                                break;
                            case "byte":
                                try
                                {
                                    value = Convert.ToInt32(value.ToString().Trim());
                                }
                                catch (Exception)
                                {
                                    isError = true;
                                    string error = string.Format("{0}行的数据类型为byte,ID为{1}列所填类型与其不相符!", key, id);
                                    errorMessage.AppendLine(error);
                                }
                                break;
                            case "bool":
                                value = value.ToString() == "1" ? true : false;
                                break;
                            case "string":
                                value = value.ToString().Trim();
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(key))
                    {
                        dic[key] = value;
                    }
                }

                if (!string.IsNullOrEmpty(id))
                {
                    m_data[id] = dic;
                }
            }

            //策划是否填错数据然后打算甩锅
            if (isError)
            {
                UnityEditor.EditorUtility.DisplayDialog("Ooops!", errorMessage.ToString(), "Got it!");
            }


            if (m_data == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("Ooops!", "数据为空，转换失败!", "Got it!");
                return;
            }
            //-- 转换为JSON字符串
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var json = JsonConvert.SerializeObject(m_data, Formatting.Indented, settings);
            //var json = JsonConvert.SerializeObject(m_data);
            //-- 保存文件

            using (var file = new FileStream(_exportToolInfo._jsonFileExportPath+ "/" + JsonFileName + ".json", FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, utf8WithoutBom))
                    writer.Write(json);
            }
            //--服务器json
            //using (
            //   var fileS = new FileStream(MainWindow.jsonSExportFilePath + "\\" + JsonFileName + ".json", FileMode.Create,
            //       FileAccess.Write))
            //{
            //    using (TextWriter writer = new StreamWriter(fileS, utf8WithoutBom))
            //        writer.Write(json);
            //}
        }

        public static void ExportJsonFileVertical(DataTable sheet, List<int> format, EJ_ExportToolInfo _exportToolInfo)
        {
            m_data.Clear();
            isError = false;
            errorMessage.Clear();
            errorMessage.AppendLine("json生成失败!!!");
            if (sheet.Rows.Count < 1 || sheet.Columns.Count < 1)
            {
                UnityEditor.EditorUtility.DisplayDialog("Ooops!", sheet.TableName + "Sheet为空，请检查!", "Got it!");
                return;
            }

            JsonFileName = sheet.TableName;

            for (int currentRow = format[0] + 2; currentRow < sheet.Rows.Count; currentRow++)
            {
                DataRow row = sheet.Rows[currentRow];
                
                string id = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                for (int currentCol = format[1]; currentCol < sheet.Columns.Count; currentCol++)
                {
                    DataColumn column = sheet.Columns[currentCol];

                    if (column == null)
                    {
                        break;
                    }
                    if (currentCol == format[1])
                    {
                        id = row[column].ToString().Trim();
                        if ("" == id || null == id)
                        {
                            Debug.LogError("~~~" + JsonFileName + "生成失败, 表格第" + currentRow + "行ID为空" + "Sheet为空，请检查!");
                            //UnityEditor.EditorUtility.DisplayDialog("Ooops!", JsonFileName + "生成失败, 表格第" + currentRow + "行ID为空" + "Sheet为空，请检查!", "Got it!");
                            break;
                        }
                    }

                    //获取key
                    string key = sheet.Rows[format[0]][column].ToString();

                    //获取value
                    object value = row[column];

                    if (value.ToString().Trim() == "")
                    {
                        //json文件基础类型为null，反序列基础类型报错
                        //                        value = null;  
                        //excel数据为空，json忽略改数据，反序列化基础类型默认值为0，对象默认值为null
                        continue;
                    }
                    else
                    {
                        //根据type特判
                        switch (sheet.Rows[format[0] + 1][column].ToString().Trim())
                        {
                            case "stringArray":
                                value = array2List<string>(value.ToString().Trim());
                                break;
                            case "intArray":
                                value = array2List<int>(value.ToString().Trim());
                                break;
                            case "floatArray":
                                value = array2List<float>(value.ToString().Trim());
                                break;
                            case "shortArray":
                                value = array2List<short>(value.ToString().Trim());
                                break;
                            case "byteArray":
                                value = array2List<byte>(value.ToString().Trim());
                                break;
                            case "float":
                                try
                                {
                                    if (value.ToString().Trim().EndsWith("%"))
                                        value = Convert.ToSingle(value.ToString().TrimEnd('%')) / 100f;
                                    else
                                        value = Convert.ToSingle(value.ToString().Trim());
                                }
                                catch (Exception)
                                {
                                    isError = true;
                                    string error = string.Format("{0}行的数据类型为float,ID为{1}列所填类型与其不相符!", key, id);
                                    //                                    MessageBox.Show(error);
                                    errorMessage.AppendLine(error);
                                }
                                break;
                            case "int":
                                try
                                {
                                    value = Convert.ToInt32(value.ToString().Trim());
                                }
                                catch (Exception)
                                {
                                    isError = true;
                                    string error = string.Format("{0}行的数据类型为int,ID为{1}列所填类型与其不相符!", key, id);
                                    errorMessage.AppendLine(error);
                                }
                                break;
                            case "short":
                                try
                                {
                                    value = Convert.ToInt32(value.ToString().Trim());
                                }
                                catch (Exception)
                                {
                                    isError = true;
                                    string error = string.Format("{0}行的数据类型为short,ID为{1}列所填类型与其不相符!", key, id);
                                    errorMessage.AppendLine(error);
                                }
                                break;
                            case "byte":
                                try
                                {
                                    value = Convert.ToInt32(value.ToString().Trim());
                                }
                                catch (Exception)
                                {
                                    isError = true;
                                    string error = string.Format("{0}行的数据类型为byte,ID为{1}列所填类型与其不相符!", key, id);
                                    errorMessage.AppendLine(error);
                                }
                                break;
                            case "bool":
                                value = value.ToString() == "1" ? true : false;
                                break;
                            case "string":
                                value = value.ToString().Trim();
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(key))
                    {
                        dic[key] = value;
                    }
                }

                if (!string.IsNullOrEmpty(id))
                {
                    m_data[id] = dic;
                }
            }

            //策划是否填错数据然后打算甩锅
            if (isError)
            {
                UnityEditor.EditorUtility.DisplayDialog("Ooops!", errorMessage.ToString(), "Got it!");
            }


            if (m_data == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("Ooops!", "数据为空，转换失败!", "Got it!");
                return;
            }
            //-- 转换为JSON字符串
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var json = JsonConvert.SerializeObject(m_data, Formatting.Indented, settings);
            //var json = JsonConvert.SerializeObject(m_data);
            //-- 保存文件
            using (var file = new FileStream(_exportToolInfo._jsonFileExportPath + "/" + JsonFileName + ".json", FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, utf8WithoutBom))
                    writer.Write(json);
            }
            Debug.LogError(JsonFileName + "生成成功");
            //--服务器json
            //using (
            //   var fileS = new FileStream(MainWindow.jsonSExportFilePath + "\\" + JsonFileName + ".json", FileMode.Create,
            //       FileAccess.Write))
            //{
            //    using (TextWriter writer = new StreamWriter(fileS, utf8WithoutBom))
            //        writer.Write(json);
            //}
        }


        /// <summary>
        ///     字符串转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static List<T> array2List<T>(string arr)
        {
            var strList = new List<string>(arr.Split(new[] { ";", "[", "]" }, StringSplitOptions.RemoveEmptyEntries));
            if(strList.Count == 1)
                strList = new List<string>(arr.Split(new[] { ",", "[", "]" }, StringSplitOptions.RemoveEmptyEntries));
            try
            {
                var list = strList.Select(q => (T)Convert.ChangeType(q, typeof(T)));
                return list.ToList();
            }
            catch (System.Exception ex)
            {
                return null;
            }

        }

        public static List<T> array2List1<T>(string arr)
        {
            var strList =
                new List<string>(arr.Split(new[] { ",", "[", "]" }, StringSplitOptions.RemoveEmptyEntries));
            try
            {
                var list = strList.Select(q => (T)Convert.ChangeType(q, typeof(T)));
                return list.ToList();
            }
            catch (System.Exception ex)
            {
                return null;
            }

        }
    }
}