// FileName: TableBuildAppData.cs
//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using DataModule;


namespace DataModule
{
	public class TableBuildAppData : DataModuleBase 
	{

		#region 字段
		/// <summary>
		/// 
		/// </summary>
		public int ID;
		/// <summary>
		/// 产品名
		/// </summary>
		public string ProductName;
		/// <summary>
		/// 包名
		/// </summary>
		public string PackageName;
		/// <summary>
		/// AppId
		/// </summary>
		public string WeChatAppID;
		/// <summary>
		/// 图标路径
		/// </summary>
		public string IconPath;
		/// <summary>
		/// 加载界面图
		/// </summary>
		public string LoadingImage;
		/// <summary>
		/// 显示图标
		/// </summary>
		public string VersionIcon;
		/// <summary>
		/// 标题图片显示
		/// </summary>
		public string TitleImage;
		#endregion

		#region 扩展
		public static Dictionary<int, TableBuildAppData> dic;


		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
		}
		#endregion



		public static Dictionary<int, TableBuildAppData> loadAllData()
		{
			if(dic == null)
			{
				dic = JsonHelper.FromJson<Dictionary<int, TableBuildAppData>>(Resources.Load<TextAsset>(JsonPath.JSON_PATH + @"TableBuildApp").text);
			}
			return dic;
		}

	}
}
