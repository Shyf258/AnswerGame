/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题： 语言国际化 
 *    Description: 
 *           功能： 使得我们发布的游戏，可以根据不同的国家，显示不同的语言信息。
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using DataModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUIFW
{
    public class LanguageMgr
    {
        //本类实例
        private static LanguageMgr _Instance;
        //语言翻译的缓存集合
        private Dictionary<string, TableTextData> _DicLauguageCache;
#if China_Version
        private Language _language = Language.CN;
#else
        private ELanguage _language = ELanguage.EN;
#endif

        public ELanguage Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
                UIManager.GetInstance().RefreshLanguage();
            }
        }


        public LanguageMgr(Dictionary<string, TableTextData> data)
        {
            _DicLauguageCache = data;
            if (_DicLauguageCache == null)
            {
                Debug.LogError("多语言字典为空");
                return;
            }
        }

        /// <summary>
        /// 得到本类实例
        /// </summary>
        /// <returns></returns>
	    public static LanguageMgr GetInstance(Dictionary<string, LanguageData> data = null)
        {
            if (_Instance == null)
            {
                _Instance = new LanguageMgr(DataModuleManager._instance.TableTextData_Dictionary);
            }
            return _Instance;
        }

        /// <summary>
        /// 为了解决多处引用编译报错问题，多语言键值应使用string类型 
        /// </summary>
        public string ShowText(int languageId)
        {
            return string.Empty;
        }
        
        /// <summary>
        /// 到显示文本信息
        /// </summary>
        /// <param name="languageId">语言的ID</param>
        /// <returns></returns>
	    public string ShowText(string languageId)
        {
            //LanguageData strQueryResult;           //查询结果
            string res = "";

            //参数检查
            if (string.IsNullOrEmpty(languageId)) return null;
            // if (lauguageID < 0) return null;

            //查询处理
            if (_DicLauguageCache != null && _DicLauguageCache.Count >= 1)
            {
                //_DicLauguageCache.TryGetValue(lauguageID, out strQueryResult);
                if (!_DicLauguageCache.ContainsKey(languageId))
                    return "null";
                var data = _DicLauguageCache[languageId];
                //if (strQueryResult!=null)
                if (data != null)
                {
                    switch (_language)
                    {
                        case ELanguage.CN:
                            res = data.CN;
                            break;
                        case ELanguage.EN:
                            res = data.EN;
                            break;
                        case ELanguage.HK:
                            res = data.HK;
                            break;
                        case ELanguage.FR:
                            res = data.FR;
                            break;
                        case ELanguage.DE:
                            res = data.DE;
                            break;
                        case ELanguage.IT:
                            res = data.IT;
                            break;
                        case ELanguage.JA:
                            res = data.JA;
                            break;
                        case ELanguage.PT:
                            res = data.PT;
                            break;
                        case ELanguage.ES:
                            res = data.ES;
                            break;
                        case ELanguage.KO:
                            res = data.KO;
                            break;
                        default:
                            break;
                    }
                    res = ReplaceJson(res);
                    return res;
                }
            }

            Debug.Log(GetType() + "/ShowText()/ Query is Null!  Parameter lauguageID: " + languageId);
            return null;
        }

        string ReplaceJson(string res)
        {
            if (string.IsNullOrEmpty(res)) res = string.Empty;
            res = res.Replace("\\n", "\n");
            return res;
        }
    }
}