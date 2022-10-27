//using UnityEngine;
//using UnityEngine.Purchasing;
//using UnityEngine.Purchasing.Security;
//using System.Collections.Generic;
//using System;

//#region google 内购数据
////class GooglePurchaseData
////{
////    // INAPP_PURCHASE_DATA
////    public string inAppPurchaseData;
////    // INAPP_DATA_SIGNATURE
////    public string inAppDataSignature;

////    public GooglePurchaseJson json;

////    [System.Serializable]
////    private struct GooglePurchaseReceipt
////    {
////        public string Payload;
////    }

////    [System.Serializable]
////    private struct GooglePurchasePayload
////    {
////        public string json;
////        public string signature;
////    }

////    [System.Serializable]
////    public struct GooglePurchaseJson
////    {
////        public string autoRenewing;
////        public string orderId;
////        public string packageName;
////        public string productId;
////        public string purchaseTime;
////        public string purchaseState;
////        public string developerPayload;
////        public string purchaseToken;
////    }

////    public GooglePurchaseData(string receipt)
////    {
////        try
////        {
////            var purchaseReceipt = JsonUtility.FromJson<GooglePurchaseReceipt>(receipt);
////            var purchasePayload = JsonUtility.FromJson<GooglePurchasePayload>(purchaseReceipt.Payload);
////            var inAppJsonData = JsonUtility.FromJson<GooglePurchaseJson>(purchasePayload.json);

////            inAppPurchaseData = purchasePayload.json;
////            inAppDataSignature = purchasePayload.signature;
////            json = inAppJsonData;
////        }
////        catch
////        {
////            Debug.Log("Could not parse receipt: " + receipt);
////            inAppPurchaseData = "";
////            inAppDataSignature = "";
////        }
////    }
////}
//#endregion

//public class IAPManager : Mono_Singleton_DontDestroyOnLoad<IAPManager>, IStoreListener
//{
//    private IStoreController _controller;
//    private IExtensionProvider _extensions;

//    private ConfigurationBuilder _builder;

//    #region 初始化
//    public void Init()
//    {
//        InitializePurchasing();
//    }
//    public void InitializePurchasing()
//    {

//        if (IsInitialized())
//            return;

//        _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

//        // Use your own products
//        var infos = ShopCtrl._instance._productInfos;
//        foreach (var info in infos.Values)
//        {
//            _builder.AddProduct(info._productID, info._productType);
//        }
//        UnityPurchasing.Initialize(this, _builder);
//    }

//    bool IsInitialized()
//    {
//        // Only say we are initialized if both the Purchasing references are set.
//        return _controller != null && _extensions != null;
//    }
//    #endregion

//    #region 初始化回调
//    /// <summary>
//    /// 初始化成功
//    /// </summary>
//    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//    {
//        Debug.LogError("~~~IAP: InitComplete");
//        this._controller = controller;
//        this._extensions = extensions;
//        //检测国家 和商品价格

//        ShopCtrl._instance.RefreshPrice(controller);
//        GL_IAP_Logic._instance._hasPrice = true;
//        GL_GameEvent._instance.SendEvent(EEventID.IapGetPrice);
//    }

//    /// <summary>
//    /// Called when Unity IAP encounters an unrecoverable initialization error.
//    ///
//    /// Note that this will not be called if Internet is unavailable; Unity IAP
//    /// will attempt initialization until it becomes available.
//    /// </summary>
//    public void OnInitializeFailed(InitializationFailureReason error)
//    {
//        Debug.LogError("~~~IAP: InitFailure" + error);
//        //GL_Game._instance.StartCoroutine(GL_Game._instance.PlayBanner());
//    }
//    #endregion

//    #region 购买行为
//    public void BuyProductID(string productId)
//    {
//        if (IsInitialized())
//        {
//            Product product = _controller.products.WithID(productId);
//            if (product != null && product.availableToPurchase)
//            {
//                //购买商品
//                _controller.InitiatePurchase(product);
//            }
//            else
//            {
//                Debug.Log("~~~BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
//            }
//        }
//        else
//        {
//            Debug.Log("~~~BuyProductID FAIL. Not initialized.");
//        }
//    }

//    //购买成功回调
//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
//    {
//        bool validPurchase = true; // Presume valid for platforms with no R.V.

//        //验证逻辑只 包含这些平台
//#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
//        //混淆逻辑
//        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
//        try
//        {
//            //Google Play, 只有一个商品
//            //Apple stores, 可能是多个商品
//            var result = validator.Validate(e.purchasedProduct.receipt);
//#if UNITY_ANDROID
//            DDebug.LogError("~~~商品1: " + result.Length);
//            foreach (IPurchaseReceipt productReceipt in result)
//            {
//                DDebug.LogError("~~~商品2: " + productReceipt.productID);
//                //DDebug.LogError("~~~商品: " + productReceipt.productID);
//                SendGoogleOrder(productReceipt as GooglePlayReceipt);

//            }
//            return PurchaseProcessingResult.Pending;
//            //return PurchaseProcessingResult.Complete;
//#elif UNITY_IOS

//            validPurchase = false;
//            foreach (IPurchaseReceipt productReceipt in result)
//            {
//                if (productReceipt.productID == e.purchasedProduct.definition.id)
//                {
//                    Debug.Log("Invalid receipt data");
//                    validPurchase = true;
//                    break;
//                }
//            }
//#endif
//        }
//        catch (IAPSecurityException)
//        {
//            Debug.Log("Invalid receipt, not unlocking content");
//            validPurchase = false;
//#if UNITY_EDITOR
//            validPurchase = true;
//#endif
//        }
//#endif

//        //apply the purchasing in case if the transaction is valid
//        if (validPurchase)
//        {
//            ProductConsume(e.purchasedProduct, true);
//        }


//        return PurchaseProcessingResult.Complete;
//    }


//    private void SendGoogleOrder(GooglePlayReceipt gpr)
//    {
//        if (gpr == null)
//            return;
//        SProductInfo info = ShopCtrl._instance.GetProductInfoByName(gpr.productID);
//        if (info == null)
//            return;

//        Net_GoogleOrders msg = new Net_GoogleOrders();
//        msg.app_id = info._tableID;
//        msg.product_id = gpr.productID;
//        msg.token = gpr.purchaseToken;

//        GL_ServerCommunication._instance.Send(Cmd.PostGoogleOrders, JsonUtility.ToJson(msg), CB_OrderCheck);
//    }

//    //服务器订单验证
//    private void CB_OrderCheck(string json)
//    {
//        //json = " {\"status\":true,\"code\":0,\"message\":\"\\u8bf7\\u6c42\\u6210\\u529f\",\"data\":{\"order_id\":\"GPA.3335-0394-8949-98439\",\"product_id\":\"ma_superbag1\"}}";
//        //DDebug.LogError("~~~orderCheck: " + json);
//        if (string.IsNullOrEmpty(json))
//            return;
//        Net_CB_GoogleOrders recivieMsg = JsonUtility.FromJson<Net_CB_GoogleOrders>(json);
//        if (recivieMsg == null)
//            return;
//        if(recivieMsg.status && recivieMsg.data != null&& recivieMsg.code == 0)
//        {
//            recivieMsg.data = GL_Tools.Decrypt(recivieMsg.data, GL_ServerCommunication._instance.CalculateParamKey());
//            //DDebug.LogError("~~~recivieMsg.data : " + recivieMsg.data);
//            if (string.IsNullOrEmpty(recivieMsg.data))
//                return;
//            Net_GoogleOrdersInfo info = JsonUtility.FromJson<Net_GoogleOrdersInfo>(recivieMsg.data);
//            //验证通过
//            Product product = _controller.products.WithID(info.product_id);
//            //GL_IAP_Logic._instance.CB_BuySucceed(product);
//            ProductConsume(product, true);
            
//        }
//    }

//    /// <summary>
//    /// 商品发货
//    /// </summary>
//    /// <param name="product"></param>
//    /// <param name="isConsume">是否需要销单</param>

//    private void ProductConsume(Product product, bool isConsume = false)
//    {
//        if (isConsume)
//            _controller.ConfirmPendingPurchase(product);

//        UI_Diplomats._instance.CloseLoadingAD();
//        string name = product.definition.id;
//        ShopCtrl._instance.CB_BuyShopSuccess(name);

//        GL_SDK._instance.LogPurchase(name, "type_" + name, (float)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);

//        PlayerDataMgr._instance.SaveFiles();
//    }
//    #endregion

//    /// <summary>
//    /// Called when a purchase fails.
//    /// </summary>
//    public void OnPurchaseFailed(Product p, PurchaseFailureReason r)
//    {
//        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
//        // this reason with the user to guide their troubleshooting actions.
//        DDebug.LogError("~~~购买失败: " + p.definition.storeSpecificId + " 失败原因: " + r);
//        //Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", p.definition.storeSpecificId, r));
//        GL_IAP_Logic._instance.CB_BuyFailure(p);
//    }

//    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
//    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
//    public void RestorePurchases()
//    {
//        if (!IsInitialized())
//        {
//            //UI_Diplomats._instance.CloseLoadingAD();
//            Debug.Log("RestorePurchases FAIL. Not initialized.");
//            return;
//        }

//        // If we are running on an Apple device ... 
//        if (Application.platform == RuntimePlatform.IPhonePlayer
//            || Application.platform == RuntimePlatform.OSXPlayer
//            || Application.platform == RuntimePlatform.tvOS)
//        {
//            Debug.Log("RestorePurchases started ...");

//            var apple = _extensions.GetExtension<IAppleExtensions>();
//            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
//            // the Action below, and ProcessPurchase if there are previously purchased products to restore.
//            apple.RestoreTransactions(OnTransactionsRestored);
//        }
//        else
//        {
//            //UI_Diplomats._instance.CloseLoadingAD();
//            // We are not running on an Apple device. No work is necessary to restore purchases.
//            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
//        }
//    }

//    void OnTransactionsRestored(bool success)
//    {
//        Debug.Log("Transactions restored " + success.ToString());
//        if (success)
//        {
//            //UI_Diplomats._instance.ShowTips("RESTORE", "Purchase has been restored.");
//        }
//    }

//    //public void MyBuy()
//    //{

//    //    BuyProductID(INAPP_ID);
//    //}

//    #region 订阅(这两种方式的 google是否生效, 有待测试

//    //public bool CheckSub(string id)
//    //{
//    //    return CheckSub(controller.products.WithID(GL_ConstData.MA_VIP));
//    //}

//    //public DateTime GetSubExpireDate(string id)
//    //{
//    //    var item = controller.products.WithID(GL_ConstData.MA_VIP);
//    //    var m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
//    //    Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

//    //    if (item.receipt != null)
//    //    {
//    //        if (item.definition.type == ProductType.Subscription)
//    //        {
//    //            string intro_json = (introductory_info_dict == null ||
//    //                                 !introductory_info_dict.ContainsKey(item.definition.storeSpecificId))
//    //                ? null
//    //                : introductory_info_dict[item.definition.storeSpecificId];

//    //            SubscriptionManager p = new SubscriptionManager(item, intro_json);
//    //            SubscriptionInfo info = p.getSubscriptionInfo();

//    //            return info.getExpireDate();
//    //        }
//    //    }

//    //    return default;
//    //}
    
//    public bool CheckSub(Product item)//使用SubscriptionManager检查订阅是否到期
//    {
//#if UNITY_EDITOR
//        return true;
//#endif
//        try
//        {
//            var m_AppleExtensions = _extensions.GetExtension<IAppleExtensions>();
//            Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
//            DDebug.LogError("~~~introductory_info_dict : " + introductory_info_dict.Count);
//            if (item.receipt != null)
//            {
//                if (item.definition.type == ProductType.Subscription)
//                {
//                    string intro_json = (introductory_info_dict == null ||
//                        !introductory_info_dict.ContainsKey(item.definition.storeSpecificId))
//                        ? null : introductory_info_dict[item.definition.storeSpecificId];

//                    SubscriptionManager p = new SubscriptionManager(item, intro_json);
//                    SubscriptionInfo info = p.getSubscriptionInfo();

//                    DDebug.LogError("~~~产品ID: " + info.getProductId());
//                    DDebug.LogError("~~~购买日期: " + info.getPurchaseDate());
//                    DDebug.LogError("~~~到期时间为: " + info.getExpireDate());//返回产品下次自动续订或到期的日期（对于已取消的自动续订订阅）
//                    DDebug.LogError("~~~是否已订阅 ：" + info.isSubscribed().ToString());//非自动更新订阅返回“Result.Unsupported”
//                    DDebug.LogError("~~~是否已过期 ：" + info.isExpired().ToString());//非自动更新订阅返回“Result.Unsupported”
//                    DDebug.LogError("~~~是否续期： " + info.isCancelled());//意味着已订阅，但不会续期，非自动更新订阅返回“Result.Unsupported”
//                    DDebug.LogError("~~~产品是否为免费试用版 ：" + info.isFreeTrial());//非自动更新订阅返回“Result.Unsupported”
//                    DDebug.LogError("~~~是否自动续订： " + info.isAutoRenewing());//非自动更新订阅返回“Result.Unsupported”
//                    DDebug.LogError("~~~下个结算日期前剩余时间： " + info.getRemainingTime());//下一个结算日期之前剩余的时间,返回“TimeSpan”
//                    DDebug.LogError("~~~是否在介绍期内： " + info.isIntroductoryPricePeriod());//非自动更新订阅返回“Result.Unsupported”
//                    DDebug.LogError("~~~介绍价格: " + info.getIntroductoryPrice());//该产品的介绍价格，返回格式为“0.99USD”的值
//                    DDebug.LogError("~~~介绍期剩余时间: " + info.getIntroductoryPricePeriod()); //没有介绍价格期的订阅产品返回“TimeSpan.Zero”
//                    DDebug.LogError("~~~介绍期内可应用此产品的数量: " + info.getIntroductoryPricePeriodCycles());//返回int

//                    if (info.isSubscribed() == Result.True)
//                    {
//                        if (info.isExpired() == Result.False)
//                        {
//                            //if (String.Equals(info.getProductId(), newableSub, StringComparison.Ordinal))
//                            //{
//                            //    Console.WriteLine(string.Format("订阅有效：", info.getProductId()));
//                            //    //这里写执行
//                            //}
//                            return true;
//                        }
//                        //else
//                        //    Console.WriteLine("订阅过期");
//                    }
//                }
//            }
//        }
//        catch (Exception)
//        {
//            return false;
//        }


//        return false;
//    }

//    public void CheckSubscriptionReceipt()//另一种检查订阅方法，通过receipt里的expiredate和当前时间对比来判断是否过期
//    {
//        var appleConfig = _builder.Configure<IAppleConfiguration>();//关于applestore的配置
//        var receiptData = Convert.FromBase64String(appleConfig.appReceipt);//将receipt转换为64位
//        AppleReceipt receipt = new AppleValidator(AppleTangle.Data()).Validate(receiptData);//AppleReceiptParser收据验证
//        //这里如果不使用unity的收据混淆则为：AppleReceipt receipt = new AppleReceiptParser().Parse(receiptData);
//        foreach (AppleInAppPurchaseReceipt productReceipt in receipt.inAppPurchaseReceipts)
//        {//这里如果有多个订阅的话，可以先匹配productID，再读取productType
//            if (productReceipt.productType == 3)//有三种类型：消耗/非消耗/订阅 
//            {
//                Console.WriteLine("订阅productID = " + productReceipt.productID);
//                DateTime expirationDate = productReceipt.subscriptionExpirationDate;//订阅到期时间
//                Console.WriteLine("订阅到期时间 = " + expirationDate.ToString());

//                DateTime now = DateTime.Now.ToUniversalTime();//转换为世界时间
//                if (DateTime.Compare(now, expirationDate) < 0)//DateTime.Compare(t1,t2)；t1早于t2小于0（等于0，大于0）
//                    Console.WriteLine("订阅未过期");
//                //在这里写逻辑
//                else
//                    //订阅无效，在这里写逻辑
//                    Console.WriteLine("订阅已过期");
//            }
//        }
//    }
//#endregion

//    /// <summary>
//    /// 检测订阅状态
//    /// </summary>
//    /// <returns></returns>
//    public bool CheckIsSubcripted(string pID)
//    {

//#if UNITY_ANDROID
//        if (_controller != null)
//        {
//            // Fetch the currency Product reference from Unity Purchasing
//            Product product = _controller.products.WithID(pID);

//            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

//            if (null == product || null == product.receipt)
//                return false;
//            var result = validator.Validate(product.receipt);

//            return isSubscriptionActive(result[0] as GooglePlayReceipt);
//        }
//#endif

//        return false;

//    }

//#if UNITY_ANDROID

//    public bool isSubscriptionActive(GooglePlayReceipt googleReceipt)
//    {
//        bool isActive = false;
//        GooglePlayReceipt google = googleReceipt;
//        if (null != google)
//        {
//            // Is this correct?
//            if (google.purchaseState == GooglePurchaseState.Purchased)
//            {
//                isActive = true;
//            }
//        }

//        return isActive;
//    }

//#endif

//}

///*
//var result = validator.Validate(e.purchasedProduct.receipt);
//Debug.Log("Receipt is valid. Contents:");
//foreach (IPurchaseReceipt productReceipt in result) {
//	Debug.Log(productReceipt.productID);
//	Debug.Log(productReceipt.purchaseDate);
//	Debug.Log(productReceipt.transactionID);

//	GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
//	if (null != google) {
//		// This is Google's Order ID.
//		// Note that it is null when testing in the sandbox
//		// because Google's sandbox does not provide Order IDs.
//		Debug.Log(google.transactionID);
//		Debug.Log(google.purchaseState);
//		Debug.Log(google.purchaseToken);
//	}

//	AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
//	if (null != apple) {
//		Debug.Log(apple.originalTransactionIdentifier);
//		Debug.Log(apple.subscriptionExpirationDate);
//		Debug.Log(apple.cancellationDate);
//		Debug.Log(apple.quantity);
//	}
//}

//public bool isSubscriptionActive(AppleInAppPurchaseReceipt appleReceipt)
//    {
//        bool isActive = false;
 
//        AppleInAppPurchaseReceipt apple = appleReceipt;
//        if (null != apple)
//        {
//            DateTime expirationDate = apple.subscriptionExpirationDate;
//            DateTime now = DateTime.Now;
//            //DateTime cancellationDate = apple.cancellationDate;
 
//            if(DateTime.Compare(now, expirationDate) < 0)
//            {
//                isActive = true;
//            }
//        }
 
//        return isActive;
//    }
//*/
