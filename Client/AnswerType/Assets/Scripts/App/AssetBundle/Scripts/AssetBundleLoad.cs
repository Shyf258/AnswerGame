using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GL.Assetbundle
{

    public class AssetbundleLoad
    {
        //public static string RootPath = Application.streamingAssetsPath;

        //private const string MANIFEST_SUFFIX = ".manifest";
        //public static string Download_Path = DownloadConfig.downLoadPath;
        private static AssetBundleManifest _manifest;
        public static AssetBundle LoadAssetBundleDependcy(string path)
        {
            if (_manifest == null)
            {
                LoadManifest();
            }
            if (_manifest != null)
            {
                string[] dependencies = _manifest.GetAllDependencies(path);
                if (dependencies.Length > 0)
                {
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        LoadAssetBundle(dependencies[i]);
                    }

                }
                return LoadAssetBundle(path);
            }
            return null;
        }

        static AssetBundle LoadAssetBundle(string path)
        {
            path = path.ToLower();
            string downPath = GL_VersionManager._instance.GetPath(path);
            string assetbundleFile = string.Format("{0}/{1}", DownloadConfig.downLoadPath, downPath);

            AssetBundle bundle = AssetBundle.LoadFromFile(assetbundleFile);
            return bundle;
        }


        public static T LoadRes<T>(string path) where T : Object
        {
            AssetBundle bundle = LoadAssetBundleDependcy(path);
            if (bundle != null)
            {
                int assetNameStart = path.LastIndexOf("/") + 1;
                int assetNameEnd = path.LastIndexOf(".");
                string assetName = path.Substring(assetNameStart, assetNameEnd - assetNameStart);
                T obj = bundle.LoadAsset(assetName) as T;
                return obj;
            }
            return null;
        }



        static void LoadManifest()
        {
            try
            {
                string downPath = GL_VersionManager._instance.GetPath("AssetPack");
                string assetbundleFile = string.Format("{0}/{1}", DownloadConfig.downLoadPath, downPath);
                //assetbundleFile = Path.Combine(Application.dataPath, "AssetPack/AssetPack");
                AssetBundle bundle = AssetBundle.LoadFromFile(assetbundleFile);
                UnityEngine.Object obj = bundle.LoadAsset("AssetBundleManifest");
                bundle.Unload(false);
                _manifest = obj as AssetBundleManifest;
                Debug.Log("Load Manifest Finished");
            }
            catch (System.Exception e)
            {
                DDebug.LogError("~~~load AssetBundleManifest fail : " + e);
            }

        }
    }
}

