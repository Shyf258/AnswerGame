using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GL.Assetbundle
{
    public class Assetbundle_Tools
    {
        private const string StreamingName = "AssetPack"; //
        private const string ABconfigPath = "Assets/Scripts/App/AssetBundle/Editor/ABconfig.asset";    //配置文件
        private static List<SAssetNode> _leafNodes = new List<SAssetNode>();    //叶子节点列表
        private static Dictionary<string, SAssetNode> _allAssetNodes = new Dictionary<string, SAssetNode>();    //所有节点列表
        private static List<string> _buildMap = new List<string>(); //需要打包的目录

        [MenuItem("功能/打包/打包ab", false)]
        public static void BuildAB()
        {
            //BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            //return;

            _buildMap.Clear();
            _leafNodes.Clear();
            _allAssetNodes.Clear();

            CollectDependcy();
            BuildResourceBuildMap();
            DeleteTargetDirectory(StreamingName);

            //BuildAssetBundle();
            BuildAssetBundleWithBuildMap();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region 计算依赖和深度
        private static void CollectDependcy()
        {
            //读取打包配置
            AssetBundleConfig abConfig = AssetDatabase.LoadAssetAtPath<AssetBundleConfig>(ABconfigPath);
            foreach (var filePath in abConfig._allFilePath)
            {
                string path = Path.Combine(Application.dataPath, filePath);
                if (!Directory.Exists(path))
                {
                    DDebug.LogError("~~~abConfig配置路径 {0} 未找到", path);
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
                    for (int j = 0; j < files.Length; j++)
                    {
                        if (CheckFileSuffixNeedIgnore(files[j].Name))
                            continue;
                        //获得在文件相对于Assets下的目录，类似于Assets/Prefab/UI/xx.prefab
                        string fileRelativePath = GetReleativeToAssets(files[j].FullName);
                        SAssetNode root = null;
                        //文件可能在上一个文件的依赖关系中被处理了
                        _allAssetNodes.TryGetValue(fileRelativePath, out root);
                        if (root == null)
                        {
                            root = new SAssetNode();
                            root.path = fileRelativePath;
                            _allAssetNodes[root.path] = root;
                            GetDependcyRecursive(fileRelativePath, root);
                        }
                    }
                }
            }

            //按文件夹整体打包
            foreach (var filePath in abConfig._allFolderPath)
            {
                string path = Path.Combine(Application.dataPath, filePath);
                if (!Directory.Exists(path))
                {
                    DDebug.LogError("~~~abConfig配置路径 {0} 未找到", path);
                }
                else
                {
                    _buildMap.Add("Assets/" + filePath);
                    //DirectoryInfo dir = new DirectoryInfo(path);
                    //SetAssetName(dir);

                    //FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
                    //for (int j = 0; j < files.Length; j++)
                    //{
                    //    if (CheckFileSuffixNeedIgnore(files[j].Name))
                    //        continue;


                    //    //获得在文件相对于Assets下的目录，类似于Assets/Prefab/UI/xx.prefab
                    //    string fileRelativePath = GetReleativeToAssets(files[j].FullName);
                    //    SAssetNode root = null;
                    //    //文件可能在上一个文件的依赖关系中被处理了
                    //    _allAssetNodes.TryGetValue(fileRelativePath, out root);
                    //    if (root == null)
                    //    {
                    //        root = new SAssetNode();
                    //        root.path = fileRelativePath;
                    //        _allAssetNodes[root.path] = root;
                    //        GetDependcyRecursive(fileRelativePath, root);
                    //    }
                    //}
                }
            }

        }

        //计算是否是 叶子节点
        private static void GetDependcyRecursive(string path, SAssetNode parentNode)
        {
            string[] dependcy = AssetDatabase.GetDependencies(path, false);
            for (int i = 0; i < dependcy.Length; i++)
            {
                SAssetNode node = null;
                _allAssetNodes.TryGetValue(dependcy[i], out node);
                if (node == null)
                {
                    node = new SAssetNode();
                    node.path = dependcy[i];
                    node.depth = parentNode.depth + 1;
                    node.parents.Add(parentNode);
                    _allAssetNodes[node.path] = node;
                    GetDependcyRecursive(dependcy[i], node);
                }
                else
                {
                    if (!node.parents.Contains(parentNode))
                    {
                        node.parents.Add(parentNode);
                    }
                    if (node.depth < parentNode.depth + 1)
                    {
                        node.depth = parentNode.depth + 1;
                        GetDependcyRecursive(dependcy[i], node);
                    }

                }
                //Debug.Log("dependcy path is " +dependcy[i] + " parent is " + parentNode.path);
            }
            if (dependcy.Length == 0)
            {
                if (!_leafNodes.Contains(parentNode))
                {
                    _leafNodes.Add(parentNode);
                }
            }
        }
        #endregion

        #region 计算打包优先级
        //按照依赖关系的深度，从最底层往上遍历打包，如果一个叶子节点有多个父节点，则该叶子节点被多个资源依赖，该叶子节点需要打包，如果一个节点没有
        //父节点，则该叶子节点是最顶层的文件（prefab,lua...），需要打包。
        static void BuildResourceBuildMap()
        {
            int maxDepth = GetMaxDepthOfLeafNodes();
            while (_leafNodes.Count > 0)
            {
                List<SAssetNode> _curDepthNodesList = new List<SAssetNode>();
                for (int i = 0; i < _leafNodes.Count; i++)
                {
                    if (_leafNodes[i].depth == maxDepth)
                    {
                        //如果叶子节点有多个父节点或者没有父节点,打包该叶子节点
                        if (_leafNodes[i].parents.Count != 1)
                        {
                            if (!ShouldIgnoreFile(_leafNodes[i].path))
                            {

                                _buildMap.Add(_leafNodes[i].path);
                            }
                        }
                        _curDepthNodesList.Add(_leafNodes[i]);
                    }
                }
                //删除已经遍历过的叶子节点，并把这些叶子节点的父节点添加到新一轮的叶子节点中
                for (int i = 0; i < _curDepthNodesList.Count; i++)
                {
                    _leafNodes.Remove(_curDepthNodesList[i]);
                    foreach (SAssetNode node in _curDepthNodesList[i].parents)
                    {
                        if (!_leafNodes.Contains(node))
                        {
                            _leafNodes.Add(node);
                        }
                    }
                }
                maxDepth -= 1;
            }
        }

        static bool ShouldIgnoreFile(string path)
        {
            if (path.EndsWith(".cs"))
                return true;
            return false;
        }

        static int GetMaxDepthOfLeafNodes()
        {
            if (_leafNodes.Count == 0)
                return 0;
            _leafNodes.Sort((x, y) =>
            {
                return y.depth - x.depth;
            });
            return _leafNodes[0].depth;
        }
        #endregion

        #region 打包逻辑
        static void BuildAssetBundleWithBuildMap()
        {
            EditorUserBuildSettings.androidBuildSubtarget =  MobileTextureSubtarget.ASTC;

            EnsureTargetDirectoryExists(StreamingName);
            string prefix = "Assets";
            AssetBundleBuild[] buildMapArray = new AssetBundleBuild[_buildMap.Count];
            for (int i = 0; i < _buildMap.Count; i++)
            {
                //尝试删除后缀
                string str = _buildMap[i].Substring(prefix.Length + 1);
                //if(str.Contains(prefix))
                //str = str.Substring(prefix.Length + 1);

                int index = str.LastIndexOf('.');
                if(index < 0)
                    buildMapArray[i].assetBundleName = str;
                else
                    buildMapArray[i].assetBundleName = str.Substring(0, index);
                
                //buildMapArray[i].assetBundleName = _buildMap[i].Substring(prefix.Length + 1);
                buildMapArray[i].assetNames = new string[] { _buildMap[i] };
                Debug.Log(_buildMap[i]);
            }

            string targetPath = Path.Combine(Application.dataPath, StreamingName);

            Debug.Log(EditorUserBuildSettings.activeBuildTarget);
            BuildPipeline.BuildAssetBundles(targetPath, buildMapArray, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            //BuildPipeline.BuildAssetBundles(AssetBundle_Path+"/Ios", buildMapArray, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.iOS);
        }

        static void BuildAssetBundle()
        {
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

            EnsureTargetDirectoryExists(StreamingName);

            string targetPath = Path.Combine(Application.dataPath, StreamingName);
            //BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            //刷新资源库
            AssetDatabase.Refresh();

            //BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            //BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);

            //Debug.Log(EditorUserBuildSettings.activeBuildTarget);
            ////BuildPipeline.BuildAssetBundles(targetPath, buildMapArray,  BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            //BuildPipeline.BuildAssetBundles(targetPath, buildMapArray, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            ////BuildPipeline.BuildAssetBundles(AssetBundle_Path+"/Ios", buildMapArray, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.iOS);
        }

        #endregion

        #region 工具接口
        static bool CheckFileSuffixNeedIgnore(string fileName)
        {
            if (fileName.EndsWith(".meta") || fileName.EndsWith(".DS_Store") || fileName.EndsWith(".cs"))
                return true;
            return false;
        }
        static void DeleteTargetDirectory(string targetDirectory)
        {
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                string targetPath = Path.Combine(Application.dataPath, targetDirectory);
                if (Directory.Exists(targetPath))
                {
                    string assetsPath = "Assets" + Path.DirectorySeparatorChar + targetDirectory;
                    if (!AssetDatabase.DeleteAsset(assetsPath))
                    {
                        Debug.Log("Failed to delete: " + assetsPath);
                    }
                }
            }
        }
        static void EnsureTargetDirectoryExists(string targetDirectory)
        {
            string targetPath = Path.Combine(Application.dataPath, targetDirectory);
            if (Directory.Exists(targetPath) == false)
            {
                AssetDatabase.CreateFolder("Assets", targetDirectory);
            }
        }

        static string GetReleativeToAssets(string fullName)
        {
            //获得在文件在Assets下的目录，类似于Assets/path/of/yourfile
            string fileRelativePath = fullName.Substring(Application.dataPath.Length - 6);
            //如果在windows平台下运行，需要替换路径中的\为/;
            fileRelativePath = fileRelativePath.Replace("\\", "/");
            return fileRelativePath;
        }

        //查找所有文件, 并设置assetBundleName
        private static void SetAssetName(DirectoryInfo info)
        {
            string assetBundleName = info.Name;

            FileInfo[] files = info.GetFiles("*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (CheckFileSuffixNeedIgnore(file.Name))
                    continue;

                string filePath = file.FullName.Replace('\\', '/');
                filePath = filePath.Replace(Application.dataPath, "Assets");
                AssetImporter.GetAtPath(filePath).assetBundleName = assetBundleName;
            }
        }
        #endregion

        public class SAssetNode
        {
            public List<SAssetNode> parents = new List<SAssetNode>();
            public string path;
            public int depth = 0;
        }
    }
}
