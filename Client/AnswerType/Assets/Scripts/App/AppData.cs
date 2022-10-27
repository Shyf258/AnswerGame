using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AppData
{
    public static string NoticeAssetPath;
    public static string UserAgreement;
    public static string PrivacyAgreement;
    public static string WithdrawRule;
    
    public static void Init()
    {
        AppDataConfig config = Resources.Load<AppDataConfig>("AppDataConfig");
        NoticeAssetPath = config.noticeAssetPath;
        UserAgreement = config.userAgreementAssetPath;
        PrivacyAgreement = config.privacyAgreementAssetPath;
        WithdrawRule = config.withdrawRuleAssetPath;
    }
    
}
