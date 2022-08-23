using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AppDataConfig", menuName = "Create AppData", order = 1)]
public class AppDataConfig : ScriptableObject
{
    [Header("需要手动输入")]
    [GL_Name("通知数据资源路径")]
    public string noticeAssetPath = "Idiom/Notice";
    [GL_Name("用户协议数据资源路径")]
    public string userAgreementAssetPath = "Idiom/UserAgreement";
    [GL_Name("隐私协议数据资源路径")]
    public string privacyAgreementAssetPath = "Idiom/PrivacyAgreement";
    [GL_Name("提现规则数据资源路径")]
    public string withdrawRuleAssetPath = "Idiom/WithdrawRule";
}