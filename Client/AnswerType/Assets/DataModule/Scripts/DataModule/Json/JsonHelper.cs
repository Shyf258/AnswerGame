using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataModule
{
    public class JsonHelper
    {

        #region 反序列化
        /// <summary>
        /// 从json字符串反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T FromJson<T>(string jsonStr)
        {
            T obj = JsonConvert.DeserializeObject<T>(jsonStr);
            return obj;
        }

        public static string GetJsonValue(string json, string key)
        {
            JObject jsonObject = JObject.Parse(json);

            try
            {
                return jsonObject[key].ToString();
            }
            catch (Exception)
            {
                //DDebug.LogError("~~~服务器回调json报错: " + json);
                return string.Empty;
            }
        }

        /// <summary>
        /// json转list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> FromJsonList<T>(string json)
        {
            var arrdata = Newtonsoft.Json.Linq.JArray.Parse(json);
            List<T> list = arrdata.ToObject<List<T>>();

            return list;
        }

        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T">反序列化的类</typeparam>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string path)
        {
            string jsonStr = null;
            try
            {
                if (File.Exists(path))
                {
                    jsonStr = File.ReadAllText(path);
                }
                else
                {
                    Debug.Log(path + "文件不存在！");
                    // 初始化默认值
                    jsonStr = "{\"509100001\":{\"ID\":509100001, \"Data\":{\"Step\":0}}}";
                }
            }
            catch (Exception)
            {
            }
            return FromJson<T>(jsonStr);
        }

        /// <summary>
        /// 从PlayerPrefs获取json字符串并反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
#if UNITY_CLIENT
        public static T DeserializeFromPlayerPrefs<T>(string key){
            return DeserializeFromJsonString<T>(PlayerPrefs.GetString(key));
        }
#endif
        #endregion

        #region 序列化
        public static string ToJson(object jsonObj)
        {
            return JsonUtility.ToJson(jsonObj);
            //return JsonConvert.SerializeObject(jsonObj, Formatting.Indented, new JsonSerializerSettings()
            //{
            //    NullValueHandling = NullValueHandling.Ignore
            //});
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="jsonObj">序列化对象</param>
        /// <param name="path">保存的路径</param>
        public static void SerializeToFile(object jsonObj, string path)
        {
            string json = ToJson(jsonObj);
            using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, Encoding.UTF8))
                    writer.Write(json);
            }

        }

        /// <summary>
        /// 序列化对象并保存到PlayerPrefs
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <param name="key">PlayerPrefs对应的key(取值的时候用)</param>
#if UNITY_CLIENT
        public static void SerializeToPlayerPrefs(object jsonObj, string key){
            string json = SerializeToJsonString(jsonObj);
            PlayerPrefs.SetString(key,json);
        }
#endif
        #endregion
    }
}