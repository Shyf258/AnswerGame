//2019.12.03    关林
//SDK 广告位自动生成工具

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;

public class Editor_SDK
{
    public static StringBuilder sb = new StringBuilder();
    static UTF8Encoding utf8notbom = new UTF8Encoding(false);

    [MenuItem("功能/工具/广告位生成")]
    public static void CreateAD()
    {
        GameObject gsdk = GameObject.Find("GSdk");
        GL_SDK sdk = gsdk.GetComponent<GL_SDK>();

        string csFileName = "GL_AD_Interface";
        sb.Clear();
        //sb.AppendLine("// ExcelName: " + new FileInfo(MainWindow.currentOpenFilePath).Name);
        sb.AppendLine("// 广告位自动生成于 " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString());
        sb.AppendLine();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine();
        sb.AppendLine();
        sb.AppendFormat("public class {0} : Singleton<{1}>\r\n", csFileName, csFileName);
        sb.AppendLine("{");
        sb.AppendLine();
        sb.AppendLine("\t#region 广告位定义");
        sb.AppendLine();
        sb.AppendLine("//1.激励视频");
        foreach (var adSite in sdk._rewardADList)
        {
            sb.AppendFormat("\tpublic const string AD_Reward_{0} = \"{0}\";\r\n", adSite, adSite);
        }
        sb.AppendLine();
        sb.AppendLine("//2.插屏");
        foreach (var adSite in sdk._interstitialList)
        {
            sb.AppendFormat("\tpublic const string AD_Interstitial_{0} = \"{0}\";\r\n", adSite, adSite);
        }
        sb.AppendLine();
        sb.AppendLine("//3.原生广告");
        foreach (var adSite in sdk._nativeList)
        {
            sb.AppendFormat("\tpublic const string AD_Native_{0} = \"{0}\";\r\n", adSite, adSite);
        }
        sb.AppendLine();
        sb.AppendLine("//4.banner广告");
        foreach (var adSite in sdk._bannerList)
        {
            sb.AppendFormat("\tpublic const string AD_Banner_{0} = \"{0}\";\r\n", adSite, adSite);
        }
        sb.AppendLine();
        sb.AppendLine("//5.开屏广告");
        foreach (var adSite in sdk._splashList)
        {
            sb.AppendFormat("\tpublic const string AD_Splash_{0} = \"{0}\";\r\n", adSite, adSite);
        }
        sb.AppendLine();
        sb.AppendLine("\t#endregion");
        sb.AppendLine();

        sb.AppendLine("\t#region 广告位计数");
        foreach (var adSite in sdk._rewardADList)
        {
            sb.AppendFormat("\tprivate int _{0} = 0;\r\n", adSite);
        }
        sb.AppendLine("\t#endregion");
        sb.AppendLine();
        #region 接口
        //判断广告
        sb.AppendLine("\t//判断是否有广告");
        sb.AppendLine("\tpublic bool IsAvailableAD(EADType type)");
        sb.AppendLine("\t{");
        sb.AppendLine("#if GuanGuan_Test || UNITY_EDITOR");
        sb.AppendLine("\t\treturn true;");
        sb.AppendLine("#endif");
        sb.AppendLine("\t\tbool result = GL_SDK._instance.IsAvailableAD((int)type);");
        sb.AppendLine("\t\tDebug.LogError(\"~~~IsAvailableAD:\" + type  + \" result: \" + result);");
        sb.AppendLine("\t\treturn result;");
        sb.AppendLine("\t}");
        
        //播放广告
        sb.AppendLine("\t//播放广告");
        sb.AppendLine("\tpublic void PlayAD(string ad)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tDebug.LogError(\"~~~PlayAD: \" + ad);");
        sb.AppendLine("#if UNITY_IOS && !UNITY_EDITOR");
        sb.AppendLine("\t\tswitch (ad)");
        sb.AppendLine("\t\t{");
        foreach (var adSite in sdk._rewardADList)
        {
            sb.AppendFormat("\t\t\tcase AD_Reward_{0}:\r\n", adSite);
            sb.AppendFormat("\t\t\t\t_{0} = 0;\r\n", adSite);
            sb.AppendLine("\t\t\t\tbreak;");
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("#endif");
        sb.AppendLine();
        sb.AppendLine("#if UNITY_EDITOR");
        sb.AppendLine("\t\tSJson sj = new SJson();");
        sb.AppendLine("\t\tsj.adSite = ad;");
        sb.AppendLine("\t\tstring str = JsonUtility.ToJson(sj);");
        sb.AppendLine("\t\tCB_AdPlayCompleted(str);");
        sb.AppendLine("\t\treturn;");
        sb.AppendLine("#else");
        sb.AppendLine("\t\tif(AppSetting.IsSkipAD)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\tSJson sj = new SJson();");
        sb.AppendLine("\t\t\tsj.adSite = ad;");
        sb.AppendLine("\t\t\tstring str = JsonUtility.ToJson(sj);");
        sb.AppendLine("\t\t\tCB_AdPlayCompleted(str);");
        sb.AppendLine("\t\t}");
        sb.AppendLine("#endif");
        sb.AppendLine("\t\tGL_SDK._instance.DisplayAd(ad);");
        sb.AppendLine("\t}");

        //关闭原生
        sb.AppendLine("\t//关闭原生广告");
        sb.AppendLine("\tpublic void CloseNativeAd()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tDebug.LogError(\"~~~CloseNativeAd.\");");
        sb.AppendLine("\t\tGL_SDK._instance.CloseNativeAd();");
        sb.AppendLine("\t}");

        //关闭banner
        sb.AppendLine("\t//关闭banner广告");
        sb.AppendLine("\tpublic void CloseBannerAd()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tDebug.LogError(\"~~~CloseBannerAd.\");");
        sb.AppendLine("\t\tGL_SDK._instance.CloseBannerAd();");
        sb.AppendLine("\t}");
        #endregion


        #region 回调
        sb.AppendLine();
        sb.AppendLine("#region 回调");

        sb.AppendLine("\t//一次广告请求，加载到广告时调⽤");
        sb.AppendLine("\tpublic void CB_AdAvailable(string param)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tDebug.LogError(\"~~~onAdAvailable: \" + param);");
        sb.AppendLine("#if GuanGuan_Test");
        sb.AppendLine("\t\treturn;");
        sb.AppendLine("#endif");
        //sb.AppendLine("\t\tSJson sj = JsonUtility.FromJson<SJson>(param);");
        sb.AppendLine("\t}");

        sb.AppendLine("\t//广告点击时调⽤");
        sb.AppendLine("\tpublic void CB_AdClicked(string param)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tGL_AD_Logic._instance.AdClicked(param);");
        sb.AppendLine("\t\tDebug.LogError(\"~~~onAdClicked: \" + param);");
        sb.AppendLine("\t}");

        sb.AppendLine("\t//广告关闭时调用");
        sb.AppendLine("\tpublic void CB_AdClosed(string param)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tDebug.LogError(\"~~~onAdClosed: \" + param);");
        sb.AppendLine("\t\tGL_AD_Logic._instance.AdClosed(param);");
        sb.AppendLine("\t\tCloseLoadingAD();");
        sb.AppendLine("#if GuanGuan_Test");
        sb.AppendLine("\t\treturn;");
        sb.AppendLine("#endif");
        sb.AppendLine("#if UNITY_IOS && !UNITY_EDITOR");
        sb.AppendLine("\t\tSJson sj = JsonUtility.FromJson<SJson>(param);");
        sb.AppendLine("\t\tTime.timeScale = 1;");
        sb.AppendLine("\t\tRealAdPlayCompleted(sj);");
        sb.AppendLine("\t\tGL_CoreData._instance.RefreshAudio(true);");
        sb.AppendLine("\t\treturn;");
        sb.AppendLine("#endif");
        sb.AppendLine("\t}");

        sb.AppendLine("\t//广告展示时调用");
        sb.AppendLine("\tpublic void CB_AdImpressed(string param)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tDebug.LogError(\"~~~onAdImpressed: \" + param);");
        sb.AppendLine("\t\tGL_AD_Logic._instance.AdImpressed(param);");
        sb.AppendLine("#if UNITY_IOS && !UNITY_EDITOR");
        sb.AppendLine("\t\tTime.timeScale = 0;");
        sb.AppendLine("\t\tGL_CoreData._instance.RefreshAudio(false);");
        sb.AppendLine("#endif");
        sb.AppendLine("\t}");


        sb.AppendLine("\t//视频播放完成后调用");
        sb.AppendLine("\tpublic void CB_AdPlayCompleted(string param)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tDebug.LogError(\"~~~onAdPlayCompleted: \" + param);");
        sb.AppendLine("\t\tSJson sj = JsonUtility.FromJson<SJson>(param);");
        sb.AppendLine();
        sb.AppendLine("#if UNITY_IOS && !UNITY_EDITOR");
        sb.AppendLine("\t\tswitch (sj.adSite)");
        sb.AppendLine("\t\t{");
        foreach (var adSite in sdk._rewardADList)
        {
            sb.AppendFormat("\t\t\tcase AD_Reward_{0}:\r\n", adSite);
            sb.AppendFormat("\t\t\t\t_{0} = 1;\r\n", adSite);
            sb.AppendLine("\t\t\t\tbreak;");
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t\treturn;");
        sb.AppendLine("#endif");
        sb.AppendLine("\t\tRealAdPlayCompleted(sj);");
        sb.AppendLine("\t}");

        sb.AppendLine("\t//广告展示失败");
        sb.AppendLine("\tpublic void CB_AdShowFailed(string param)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tDebug.LogError(\"~~~onAdShowFailed: \" + param);");
        sb.AppendLine("\t\tSJson sj = JsonUtility.FromJson<SJson>(param);");
        sb.AppendLine("\t\tGL_AD_Logic._instance.AdShowFailed(sj);");
        sb.AppendLine("\t\tCloseLoadingAD();");
        sb.AppendLine("#if GuanGuan_Test");
        sb.AppendLine("\t\treturn;");
        sb.AppendLine("#endif");
        sb.AppendLine("#if UNITY_IOS && !UNITY_EDITOR");
        sb.AppendLine("\t\tSJson sj = JsonUtility.FromJson<SJson>(param);");
        sb.AppendLine("\t\tTime.timeScale = 1;");
        sb.AppendLine("\t\tGL_CoreData._instance.RefreshAudio(true);");
        sb.AppendLine("\t\treturn;");
        sb.AppendLine("#endif");
        sb.AppendLine("\t}");

        sb.AppendLine("\t//真实广告激励成功");
        sb.AppendLine("\tprivate void RealAdPlayCompleted(SJson sj)");
        sb.AppendLine("\t{");
        sb.AppendLine("#if !GuanGuan_Test");
        sb.AppendLine("\t\tswitch (sj.adSite)");
        sb.AppendLine("\t\t{");
        foreach (var adSite in sdk._rewardADList)
        {
            sb.AppendFormat("\t\t\tcase AD_Reward_{0}:\r\n", adSite);
            sb.AppendLine("#if UNITY_IOS && !UNITY_EDITOR");
            sb.AppendFormat("\t\t\t\tif(_{0} == 0)\r\n", adSite);
            sb.AppendLine("\t\t\t\t\treturn;");
            sb.AppendLine("#endif");
            sb.AppendFormat("\t\t\t\t_{0} = 0;\r\n", adSite);
            sb.AppendLine("\t\t\t\tbreak;");
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("#endif");
        sb.AppendLine("\t\tGL_AD_Logic._instance.RealAdPlayCompleted(sj);");
        sb.AppendLine("\t}");





        sb.AppendLine("\t//关闭广告加载Loading");
        sb.AppendLine("\tprivate void CloseLoadingAD()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\t//UI_Diplomats._instance.CloseLoadingAD();");
        sb.AppendLine("\t}");

        sb.AppendLine("#endregion");
        sb.AppendLine();
        #endregion


        sb.AppendLine("}");




        string _csPath = Application.dataPath + "/Scripts/Base/SDK/";
        //-- 保存文件
        using (FileStream file = new FileStream(_csPath + "/" + csFileName + ".cs", FileMode.Create, FileAccess.Write))
        {
            using (TextWriter writer = new StreamWriter(file, utf8notbom))
                writer.Write(sb.ToString());
        }
        Debug.LogError(csFileName + "生成成功");
    }
}
