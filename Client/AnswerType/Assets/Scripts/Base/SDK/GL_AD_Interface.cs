// 广告位自动生成于 2022年10月19日  12:30:05

using UnityEngine;


public class GL_AD_Interface : Singleton<GL_AD_Interface>
{

	#region 广告位定义

//1.激励视频
	public const string AD_Reward_DoubleReward = "DoubleReward";
	public const string AD_Reward_RightPassivity = "RightPassivity";
	public const string AD_Reward_ProductionGrow = "ProductionGrow";
	public const string AD_Reward_WithDrawCoin = "WithDrawCoin";
	public const string AD_Reward_GetLevelReward = "GetLevelReward";
	public const string AD_Reward_GetDoubleReward = "GetDoubleReward";
	public const string AD_Reward_FreeCoinGet = "FreeCoinGet";
	public const string AD_Reward_FreeTips = "FreeTips";
	public const string AD_Reward_NextLevel = "NextLevel";
	public const string AD_Reward_TaskRedPage = "TaskRedPage";
	public const string AD_Reward_TaskNormal = "TaskNormal";
	public const string AD_Reward_WithDrawSign = "WithDrawSign";
	public const string AD_Reward_WithDrawPool = "WithDrawPool";
	public const string AD_Reward_OpenRedPack = "OpenRedPack";
	public const string AD_Reward_UnlockWithDraw = "UnlockWithDraw";
	public const string AD_Reward_SignInReport = "SignInReport";
	public const string AD_Reward_TaskPageGetCoin = "TaskPageGetCoin";
	public const string AD_Reward_NormalGetCoin = "NormalGetCoin";
	public const string AD_Reward_NewbieSign = "NewbieSign";
	public const string AD_Reward_ActivityVideo = "ActivityVideo";
	public const string AD_Reward_ActivitySign = "ActivitySign";
	public const string AD_Reward_RedGroupOpenRed = "RedGroupOpenRed";
	public const string AD_Reward_Reright = "Reright";
	public const string AD_Reward_NewPlayer = "NewPlayer";
	public const string AD_Reward_LoginWithDraw = "LoginWithDraw";
	public const string AD_Reward_NormalLevelUp = "NormalLevelUp";
	public const string AD_Reward_WithDrawRed = "WithDrawRed";
	public const string AD_Reward_WithDrawGetCoin = "WithDrawGetCoin";
	public const string AD_Reward_WithDrawGetRed = "WithDrawGetRed";

//2.插屏
	public const string AD_Interstitial_AllDialog = "AllDialog";

//3.原生广告
	public const string AD_Native_LevelReward = "LevelReward";

//4.banner广告

//5.开屏广告
	public const string AD_Splash_Splash = "Splash";

	#endregion

	#region 广告位计数
	private int _DoubleReward = 0;
	private int _RightPassivity = 0;
	private int _ProductionGrow = 0;
	private int _WithDrawCoin = 0;
	private int _GetLevelReward = 0;
	private int _GetDoubleReward = 0;
	private int _FreeCoinGet = 0;
	private int _FreeTips = 0;
	private int _NextLevel = 0;
	private int _TaskRedPage = 0;
	private int _TaskNormal = 0;
	private int _WithDrawSign = 0;
	private int _WithDrawPool = 0;
	private int _OpenRedPack = 0;
	private int _UnlockWithDraw = 0;
	private int _SignInReport = 0;
	private int _TaskPageGetCoin = 0;
	private int _NormalGetCoin = 0;
	private int _NewbieSign = 0;
	private int _ActivityVideo = 0;
	private int _ActivitySign = 0;
	private int _RedGroupOpenRed = 0;
	private int _Reright = 0;
	private int _NewPlayer = 0;
	private int _LoginWithDraw = 0;
	private int _NormalLevelUp = 0;
	private int _WithDrawRed = 0;
	private int _WithDrawGetCoin = 0;
	private int _WithDrawGetRed = 0;
	#endregion

	//判断是否有广告
	public bool IsAvailableAD(EADType type)
	{
#if GuanGuan_Test || UNITY_EDITOR
		return true;
#endif
		bool result = GL_SDK._instance.IsAvailableAD((int)type);
		Debug.LogError("~~~IsAvailableAD:" + type  + " result: " + result);
		return result;
	}
	//播放广告
	public void PlayAD(string ad)
	{
		Debug.LogError("~~~PlayAD: " + ad);
#if UNITY_IOS && !UNITY_EDITOR
		switch (ad)
		{
			case AD_Reward_DoubleReward:
				_DoubleReward = 0;
				break;
			case AD_Reward_RightPassivity:
				_RightPassivity = 0;
				break;
			case AD_Reward_ProductionGrow:
				_ProductionGrow = 0;
				break;
			case AD_Reward_WithDrawCoin:
				_WithDrawCoin = 0;
				break;
			case AD_Reward_GetLevelReward:
				_GetLevelReward = 0;
				break;
			case AD_Reward_GetDoubleReward:
				_GetDoubleReward = 0;
				break;
			case AD_Reward_FreeCoinGet:
				_FreeCoinGet = 0;
				break;
			case AD_Reward_FreeTips:
				_FreeTips = 0;
				break;
			case AD_Reward_NextLevel:
				_NextLevel = 0;
				break;
			case AD_Reward_TaskRedPage:
				_TaskRedPage = 0;
				break;
			case AD_Reward_TaskNormal:
				_TaskNormal = 0;
				break;
			case AD_Reward_WithDrawSign:
				_WithDrawSign = 0;
				break;
			case AD_Reward_WithDrawPool:
				_WithDrawPool = 0;
				break;
			case AD_Reward_OpenRedPack:
				_OpenRedPack = 0;
				break;
			case AD_Reward_UnlockWithDraw:
				_UnlockWithDraw = 0;
				break;
			case AD_Reward_SignInReport:
				_SignInReport = 0;
				break;
			case AD_Reward_TaskPageGetCoin:
				_TaskPageGetCoin = 0;
				break;
			case AD_Reward_NormalGetCoin:
				_NormalGetCoin = 0;
				break;
			case AD_Reward_NewbieSign:
				_NewbieSign = 0;
				break;
			case AD_Reward_ActivityVideo:
				_ActivityVideo = 0;
				break;
			case AD_Reward_ActivitySign:
				_ActivitySign = 0;
				break;
			case AD_Reward_RedGroupOpenRed:
				_RedGroupOpenRed = 0;
				break;
			case AD_Reward_Reright:
				_Reright = 0;
				break;
			case AD_Reward_NewPlayer:
				_NewPlayer = 0;
				break;
			case AD_Reward_LoginWithDraw:
				_LoginWithDraw = 0;
				break;
			case AD_Reward_NormalLevelUp:
				_NormalLevelUp = 0;
				break;
			case AD_Reward_WithDrawRed:
				_WithDrawRed = 0;
				break;
			case AD_Reward_WithDrawGetCoin:
				_WithDrawGetCoin = 0;
				break;
			case AD_Reward_WithDrawGetRed:
				_WithDrawGetRed = 0;
				break;
		}
#endif

#if UNITY_EDITOR
		SJson sj = new SJson();
		sj.adSite = ad;
		string str = JsonUtility.ToJson(sj);
		CB_AdPlayCompleted(str);
		return;
#else
		if(AppSetting.IsSkipAD)
		{
			SJson sj = new SJson();
			sj.adSite = ad;
			string str = JsonUtility.ToJson(sj);
			CB_AdPlayCompleted(str);
		}
#endif
		GL_SDK._instance.DisplayAd(ad);
	}
	//关闭原生广告
	public void CloseNativeAd()
	{
		Debug.LogError("~~~CloseNativeAd.");
		GL_SDK._instance.CloseNativeAd();
	}
	//关闭banner广告
	public void CloseBannerAd()
	{
		Debug.LogError("~~~CloseBannerAd.");
		GL_SDK._instance.CloseBannerAd();
	}

#region 回调
	//一次广告请求，加载到广告时调⽤
	public void CB_AdAvailable(string param)
	{
		Debug.LogError("~~~onAdAvailable: " + param);
#if GuanGuan_Test
		return;
#endif
	}
	//广告点击时调⽤
	public void CB_AdClicked(string param)
	{
		GL_AD_Logic._instance.AdClicked(param);
		Debug.LogError("~~~onAdClicked: " + param);
	}
	//广告关闭时调用
	public void CB_AdClosed(string param)
	{
		Debug.LogError("~~~onAdClosed: " + param);
		GL_AD_Logic._instance.AdClosed(param);
		CloseLoadingAD();
#if GuanGuan_Test
		return;
#endif
#if UNITY_IOS && !UNITY_EDITOR
		SJson sj = JsonUtility.FromJson<SJson>(param);
		Time.timeScale = 1;
		RealAdPlayCompleted(sj);
		GL_CoreData._instance.RefreshAudio(true);
		return;
#endif
	}
	//广告展示时调用
	public void CB_AdImpressed(string param)
	{
		Debug.LogError("~~~onAdImpressed: " + param);
		GL_AD_Logic._instance.AdImpressed(param);
#if UNITY_IOS && !UNITY_EDITOR
		Time.timeScale = 0;
		GL_CoreData._instance.RefreshAudio(false);
#endif
	}
	//视频播放完成后调用
	public void CB_AdPlayCompleted(string param)
	{
		Debug.LogError("~~~onAdPlayCompleted: " + param);
		SJson sj = JsonUtility.FromJson<SJson>(param);

#if UNITY_IOS && !UNITY_EDITOR
		switch (sj.adSite)
		{
			case AD_Reward_DoubleReward:
				_DoubleReward = 1;
				break;
			case AD_Reward_RightPassivity:
				_RightPassivity = 1;
				break;
			case AD_Reward_ProductionGrow:
				_ProductionGrow = 1;
				break;
			case AD_Reward_WithDrawCoin:
				_WithDrawCoin = 1;
				break;
			case AD_Reward_GetLevelReward:
				_GetLevelReward = 1;
				break;
			case AD_Reward_GetDoubleReward:
				_GetDoubleReward = 1;
				break;
			case AD_Reward_FreeCoinGet:
				_FreeCoinGet = 1;
				break;
			case AD_Reward_FreeTips:
				_FreeTips = 1;
				break;
			case AD_Reward_NextLevel:
				_NextLevel = 1;
				break;
			case AD_Reward_TaskRedPage:
				_TaskRedPage = 1;
				break;
			case AD_Reward_TaskNormal:
				_TaskNormal = 1;
				break;
			case AD_Reward_WithDrawSign:
				_WithDrawSign = 1;
				break;
			case AD_Reward_WithDrawPool:
				_WithDrawPool = 1;
				break;
			case AD_Reward_OpenRedPack:
				_OpenRedPack = 1;
				break;
			case AD_Reward_UnlockWithDraw:
				_UnlockWithDraw = 1;
				break;
			case AD_Reward_SignInReport:
				_SignInReport = 1;
				break;
			case AD_Reward_TaskPageGetCoin:
				_TaskPageGetCoin = 1;
				break;
			case AD_Reward_NormalGetCoin:
				_NormalGetCoin = 1;
				break;
			case AD_Reward_NewbieSign:
				_NewbieSign = 1;
				break;
			case AD_Reward_ActivityVideo:
				_ActivityVideo = 1;
				break;
			case AD_Reward_ActivitySign:
				_ActivitySign = 1;
				break;
			case AD_Reward_RedGroupOpenRed:
				_RedGroupOpenRed = 1;
				break;
			case AD_Reward_Reright:
				_Reright = 1;
				break;
			case AD_Reward_NewPlayer:
				_NewPlayer = 1;
				break;
			case AD_Reward_LoginWithDraw:
				_LoginWithDraw = 1;
				break;
			case AD_Reward_NormalLevelUp:
				_NormalLevelUp = 1;
				break;
			case AD_Reward_WithDrawRed:
				_WithDrawRed = 1;
				break;
			case AD_Reward_WithDrawGetCoin:
				_WithDrawGetCoin = 1;
				break;
			case AD_Reward_WithDrawGetRed:
				_WithDrawGetRed = 1;
				break;
		}
		return;
#endif
		RealAdPlayCompleted(sj);
	}
	//广告展示失败
	public void CB_AdShowFailed(string param)
	{
		Debug.LogError("~~~onAdShowFailed: " + param);
		SJson sj = JsonUtility.FromJson<SJson>(param);
		GL_AD_Logic._instance.AdShowFailed(sj);
		CloseLoadingAD();
#if GuanGuan_Test
		return;
#endif
#if UNITY_IOS && !UNITY_EDITOR
		SJson sj = JsonUtility.FromJson<SJson>(param);
		Time.timeScale = 1;
		GL_CoreData._instance.RefreshAudio(true);
		return;
#endif
	}
	//真实广告激励成功
	private void RealAdPlayCompleted(SJson sj)
	{
#if !GuanGuan_Test
		switch (sj.adSite)
		{
			case AD_Reward_DoubleReward:
#if UNITY_IOS && !UNITY_EDITOR
				if(_DoubleReward == 0)
					return;
#endif
				_DoubleReward = 0;
				break;
			case AD_Reward_RightPassivity:
#if UNITY_IOS && !UNITY_EDITOR
				if(_RightPassivity == 0)
					return;
#endif
				_RightPassivity = 0;
				break;
			case AD_Reward_ProductionGrow:
#if UNITY_IOS && !UNITY_EDITOR
				if(_ProductionGrow == 0)
					return;
#endif
				_ProductionGrow = 0;
				break;
			case AD_Reward_WithDrawCoin:
#if UNITY_IOS && !UNITY_EDITOR
				if(_WithDrawCoin == 0)
					return;
#endif
				_WithDrawCoin = 0;
				break;
			case AD_Reward_GetLevelReward:
#if UNITY_IOS && !UNITY_EDITOR
				if(_GetLevelReward == 0)
					return;
#endif
				_GetLevelReward = 0;
				break;
			case AD_Reward_GetDoubleReward:
#if UNITY_IOS && !UNITY_EDITOR
				if(_GetDoubleReward == 0)
					return;
#endif
				_GetDoubleReward = 0;
				break;
			case AD_Reward_FreeCoinGet:
#if UNITY_IOS && !UNITY_EDITOR
				if(_FreeCoinGet == 0)
					return;
#endif
				_FreeCoinGet = 0;
				break;
			case AD_Reward_FreeTips:
#if UNITY_IOS && !UNITY_EDITOR
				if(_FreeTips == 0)
					return;
#endif
				_FreeTips = 0;
				break;
			case AD_Reward_NextLevel:
#if UNITY_IOS && !UNITY_EDITOR
				if(_NextLevel == 0)
					return;
#endif
				_NextLevel = 0;
				break;
			case AD_Reward_TaskRedPage:
#if UNITY_IOS && !UNITY_EDITOR
				if(_TaskRedPage == 0)
					return;
#endif
				_TaskRedPage = 0;
				break;
			case AD_Reward_TaskNormal:
#if UNITY_IOS && !UNITY_EDITOR
				if(_TaskNormal == 0)
					return;
#endif
				_TaskNormal = 0;
				break;
			case AD_Reward_WithDrawSign:
#if UNITY_IOS && !UNITY_EDITOR
				if(_WithDrawSign == 0)
					return;
#endif
				_WithDrawSign = 0;
				break;
			case AD_Reward_WithDrawPool:
#if UNITY_IOS && !UNITY_EDITOR
				if(_WithDrawPool == 0)
					return;
#endif
				_WithDrawPool = 0;
				break;
			case AD_Reward_OpenRedPack:
#if UNITY_IOS && !UNITY_EDITOR
				if(_OpenRedPack == 0)
					return;
#endif
				_OpenRedPack = 0;
				break;
			case AD_Reward_UnlockWithDraw:
#if UNITY_IOS && !UNITY_EDITOR
				if(_UnlockWithDraw == 0)
					return;
#endif
				_UnlockWithDraw = 0;
				break;
			case AD_Reward_SignInReport:
#if UNITY_IOS && !UNITY_EDITOR
				if(_SignInReport == 0)
					return;
#endif
				_SignInReport = 0;
				break;
			case AD_Reward_TaskPageGetCoin:
#if UNITY_IOS && !UNITY_EDITOR
				if(_TaskPageGetCoin == 0)
					return;
#endif
				_TaskPageGetCoin = 0;
				break;
			case AD_Reward_NormalGetCoin:
#if UNITY_IOS && !UNITY_EDITOR
				if(_NormalGetCoin == 0)
					return;
#endif
				_NormalGetCoin = 0;
				break;
			case AD_Reward_NewbieSign:
#if UNITY_IOS && !UNITY_EDITOR
				if(_NewbieSign == 0)
					return;
#endif
				_NewbieSign = 0;
				break;
			case AD_Reward_ActivityVideo:
#if UNITY_IOS && !UNITY_EDITOR
				if(_ActivityVideo == 0)
					return;
#endif
				_ActivityVideo = 0;
				break;
			case AD_Reward_ActivitySign:
#if UNITY_IOS && !UNITY_EDITOR
				if(_ActivitySign == 0)
					return;
#endif
				_ActivitySign = 0;
				break;
			case AD_Reward_RedGroupOpenRed:
#if UNITY_IOS && !UNITY_EDITOR
				if(_RedGroupOpenRed == 0)
					return;
#endif
				_RedGroupOpenRed = 0;
				break;
			case AD_Reward_Reright:
#if UNITY_IOS && !UNITY_EDITOR
				if(_Reright == 0)
					return;
#endif
				_Reright = 0;
				break;
			case AD_Reward_NewPlayer:
#if UNITY_IOS && !UNITY_EDITOR
				if(_NewPlayer == 0)
					return;
#endif
				_NewPlayer = 0;
				break;
			case AD_Reward_LoginWithDraw:
#if UNITY_IOS && !UNITY_EDITOR
				if(_LoginWithDraw == 0)
					return;
#endif
				_LoginWithDraw = 0;
				break;
			case AD_Reward_NormalLevelUp:
#if UNITY_IOS && !UNITY_EDITOR
				if(_NormalLevelUp == 0)
					return;
#endif
				_NormalLevelUp = 0;
				break;
			case AD_Reward_WithDrawRed:
#if UNITY_IOS && !UNITY_EDITOR
				if(_WithDrawRed == 0)
					return;
#endif
				_WithDrawRed = 0;
				break;
			case AD_Reward_WithDrawGetCoin:
#if UNITY_IOS && !UNITY_EDITOR
				if(_WithDrawGetCoin == 0)
					return;
#endif
				_WithDrawGetCoin = 0;
				break;
			case AD_Reward_WithDrawGetRed:
#if UNITY_IOS && !UNITY_EDITOR
				if(_WithDrawGetRed == 0)
					return;
#endif
				_WithDrawGetRed = 0;
				break;
		}
#endif
		GL_AD_Logic._instance.RealAdPlayCompleted(sj);
	}
	//关闭广告加载Loading
	private void CloseLoadingAD()
	{
		//UI_Diplomats._instance.CloseLoadingAD();
	}
#endregion

}
