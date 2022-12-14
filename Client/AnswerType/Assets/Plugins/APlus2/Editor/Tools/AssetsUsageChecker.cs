//  Copyright (c) 2020-present amlovey
//  
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace APlus2
{
    public static class AssetsUsageChecker
    {
        private const string ASSETCHECKERKEY = "APLUS2_ASSETCHECKERKEY";

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuildTarget)
        {
            EditorCoroutine.StartCoroutine(DelayExecute());
        }

        private static IEnumerator DelayExecute()
        {
            yield return new WaitForSecondsRealtime(2);
            CheckUnusedStateIfNeeds();
            UpdateUIIfNeeds();
        }

        const string SEARCH_KEY_WORDS = "Used Assets, sorted by uncompressed size:";
        const string SEARCH_KEY_WORDS_NEW = "Used Assets and files from the Resources folder, sorted by uncompressed size:";

        private static void UpdateUIIfNeeds()
        {
            if (MainWindow.Instance != null)
            {
                if (MainWindow.Instance.state != null)
                {
                    MainWindow.Instance.state.SyncAssetsDataFromCache();
                }

                MainWindow.Instance.RefreshUIIfNeeds();
            }
        }

        private static void CheckUnusedStateIfNeeds()
        {
            CacheManager.LoadCacheIfNotExist();
            List<string> assets = Check();
            if (assets != null && assets.Count > 0)
            {
                HashSet<string> usedFiles = new HashSet<string>();
                foreach (var item in assets)
                {
                    usedFiles.Add(item);
                }

                CacheManager.cache.UpdateUsedStatus(usedFiles);
                CacheManager.SaveToLocal();
            }
        }

        private static List<string> Check()
        {
            string editorLogPath = GetEdtiorFilePath();
            if (string.IsNullOrEmpty(editorLogPath) || !File.Exists(editorLogPath))
            {
                return null;
            }

            string logContent = ReadLogContent(editorLogPath);
            if (string.IsNullOrEmpty(logContent))
            {
                return null;
            }

            string[] lines = logContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            bool inRegion = false;
            int startIndex = lines.Length;

            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].Trim().Contains(SEARCH_KEY_WORDS) || lines[i].Trim().Contains(SEARCH_KEY_WORDS_NEW))
                {
                    inRegion = true;
                    startIndex = i;
                    break;
                }
            }

            List<string> usedFiles = new List<string>();

            for (int i = startIndex + 1; i < lines.Length; i++)
            {
                if (!inRegion)
                {
                    break;
                }

                if (char.IsDigit(lines[i].Trim(), 0))
                {
                    string assetsPath = GetAssetPath(lines[i]);
                    if (!string.IsNullOrEmpty(assetsPath))
                    {
                        usedFiles.Add(assetsPath);
                    }
                }
                else
                {
                    inRegion = false;
                    break;
                }
            }

            return usedFiles;
        }

        public static string GetMarkFile()
        {
            return string.Format("Library/{0}", ASSETCHECKERKEY);
        }

        private static string GetAssetPath(string log)
        {
            // 0.1 kb	 0.0% Assets/Test/SampleScenes/Scripts/CameraSwitch.cs
            //
            string[] temp = log.Split(new char[] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length == 4)
            {
                return temp[3];
            }

            return string.Empty;
        }

        private static string ReadLogContent(string logPath)
        {
            if (string.IsNullOrEmpty(logPath))
            {
                return string.Empty;
            }

            string data = string.Empty;

            using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    data = reader.ReadToEnd();
                }
            }

            return data;
        }

        public static string GetEdtiorFilePath()
        {
            string editorLogPath = "Editor.log";

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string unityLogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Logs/Unity");
                editorLogPath = Path.Combine(unityLogFolder, editorLogPath);
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                editorLogPath = Path.Combine(Environment.ExpandEnvironmentVariables(@"%localappdata%\Unity\Editor\"), editorLogPath);
            }
            else
            {
                Debug.LogError("Not support by A+ Assets Explorer for now!");
                return null;
            }

            return editorLogPath;
        }
    }
}