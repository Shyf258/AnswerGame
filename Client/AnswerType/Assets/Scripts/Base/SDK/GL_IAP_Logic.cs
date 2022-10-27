////2018.10.22    关林
////内购逻辑

//using System.Collections;
//using System.Collections.Generic;
//using SUIFW;
//using UnityEngine;
//using UnityEngine.Purchasing.Security;

//public class GL_IAP_Logic : Singleton<GL_IAP_Logic>
//{
//    //public string _noadsUnit;   //货币单位
//    //public string _unlockAllModel;  //解锁所有模型, 的货币单位

//    //public Dictionary<string, string> _currencyUnit = new Dictionary<string, string>();

//    //public string GetUnitByProductId(string productId)
//    //{
//    //    _currencyUnit.TryGetValue(productId, out var ret);
//    //    return ret;
//    //}
    
//    public bool _hasPrice;
//    //是否无广告状态
//    public void SetNoAds(bool set)
//    {
//        Debug.LogError("~~~Get Noads: " + set);
//        if (set != GL_CoreData._instance.IsNoAds)
//        {
//            if (set)
//            {
//                GL_CoreData._instance.IsNoAds = true;
//                //GL_Game._instance.IAP_NoadsSucceedRefresh();
//                //GL_CoreData._instance.SaveData();
//            }
//        }

//        if (GL_CoreData._instance.IsNoAds)
//        {
//            GL_AD_Interface._instance.ChangeSiteVisibility(GL_AD_Interface.AD_Banner, false);
//        }
//    }

//    /// <summary>
//    /// 购买产品
//    /// </summary>
//    /// <param name="product"></param>
//    public void BuyProduct(string product)
//    {
//        DDebug.LogError("~~~BuyProduct: " + product);
//        //UI_Diplomats._instance.ShowLoadingAD();
//        IAPManager._instance.BuyProductID(product);

//    }

//    /// <summary>
//    /// 购买成功
//    /// </summary>
//    public void CB_BuySucceed(UnityEngine.Purchasing.Product product)
//    {
//        //打点数据
//        GL_ServerCommunication._instance.Send(Cmd.PostOrderReport, null, null);

//        //UI_Diplomats._instance.CloseLoadingAD();
//        //string name = product.definition.id;
//        //ShopCtrl._instance.CB_BuyShopSuccess(name);

//        //GL_SDK._instance.LogPurchase(name, "type_" + name, (float)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);

//        //PlayerDataMgr._instance.SaveFiles();
//    }

//    /// <summary>
//    /// 购买失败
//    /// </summary>
//    public void CB_BuyFailure(UnityEngine.Purchasing.Product product)
//    {
//        GL_AudioPlayback._instance.Play(49);
//        ShopCtrl._instance.CB_BuyFailed();
//        //UI_Diplomats._instance.CloseLoadingAD();
//    }
//    //恢复购买,ios功能
//    public void ReStore()
//    {
//        //UI_Diplomats._instance.ShowLoadingAD();
//        IAPManager._instance.RestorePurchases();
//    }



//    //public void RefreshPrice(string type, UnityEngine.Purchasing.Product product)
//    //{

//    //    if (product != null && product.metadata != null)
//    //    {
//    //        _currencyUnit[type] = product.metadata.localizedPriceString.ToString();
//    //        //_unlockWeapon = product.metadata.localizedPriceString.ToString();
//    //    }
//    //}
//}
