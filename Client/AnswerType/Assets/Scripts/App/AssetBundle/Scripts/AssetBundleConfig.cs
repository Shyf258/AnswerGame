using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GL.Assetbundle
{
    [CreateAssetMenu(fileName = "ABconfig", menuName = "Create ABConfig", order = 0)]
    public class AssetBundleConfig : ScriptableObject
    {
        [Header("按文件夹打包")]
        public List<string> _allFolderPath = new List<string>();

        [Header("根据依赖,分别打包")]
        public List<string> _allFilePath = new List<string>();
    }
}


