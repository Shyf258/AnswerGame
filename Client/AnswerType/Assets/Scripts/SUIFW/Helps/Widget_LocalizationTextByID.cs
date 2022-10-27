using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_2018_3_OR_NEWER
//using TMPro;
#endif

public class Widget_LocalizationTextByID : MonoBehaviour
{
    [Tooltip("id")]
    public string _id;

    public bool isTextMesh = false;

    public bool isBold = false;

    private Text text;
    //private TextMesh textMesh;
#if UNITY_2017_3_OR_NEWER
    //private TextMeshPro textMeshPro;
    public bool isTextMeshPro = false;
#endif

    private string EN_FONT_PATH = "Fonts/GrilledCheese BTN Toasted";
    private string CN_FONT_PATH = "Fonts/方正胖娃简体";

    ELanguage _curLanguage = ELanguage.EN;

    void Start()
    {
        if (isTextMesh)
        {
         //   textMesh = this.GetComponent<TextMesh>();
        //    textMesh.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;

        }
        else if (isTextMeshPro)
        {
        //    textMeshPro = this.GetComponent<TextMeshPro>();
        //    textMeshPro.fontStyle = isBold ? FontStyles.Bold : FontStyles.Normal;
        }
        else
        {
            text = this.GetComponent<Text>();
            if(text != null)
                text.fontStyle = isBold ? FontStyle.Bold : FontStyle.Normal;

        }
        _curLanguage = GL_CoreData._instance.Language;
        ChangeLanguage(GL_CoreData._instance.Language);
    }

    private void OnEnable()
    {
        GL_GameEvent._instance.RegisterEvent(EEventID.EID_ChangeLanguage, Event_ChangeLanguage);
    }

    private void OnDisable()
    {
        GL_GameEvent._instance.UnregisterEvent(EEventID.EID_ChangeLanguage, Event_ChangeLanguage);
    }

    public void Event_ChangeLanguage(EventParam param)
    {
        var p = param as EventParam<ELanguage>;
        ChangeLanguage(p._param);
    }
    private void ChangeLanguage(ELanguage lan)
    {
        _id = _id.Trim();
        var str = SUIFW.LanguageMgr.GetInstance().ShowText(_id);
        if (isTextMesh)
        {
//            textMesh.text = str;
//                         var f = SetFont(lan);
//                         var font = Resources.Load<Font>(f);
//                         textMesh.font = font;//string.IsNullOrEmpty(f) ? new Font("Arial") : Resources.Load<Font>(f);
//                         MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
//                         if (meshRenderer != null)
//                         {
//                             meshRenderer.materials = new Material[] { font.material };
//                         }
        }
        else if(isTextMeshPro)
        {
         //   textMeshPro.text = str;
        }
        else
        {
            if(text !=null)
                text.text = str;
           // var f = SetFont(lan);
            //text.font = Resources.Load<Font>(f);//string.IsNullOrEmpty(f) ? new Font("Arial") : Resources.Load<Font>(f);

        }
        _curLanguage = lan;

    }

    string SetFont(ELanguage lan)
    {
        string res = "";
        //switch (lan)
        //{
        //    case ELanguage.CN:
        //        res = CN_FONT_PATH;
        //        break;
        //    case ELanguage.EN:
        //        res = EN_FONT_PATH;
        //        break;
        //    default:
        //        res = EN_FONT_PATH;
        //        break;
        //}
#if China_Version
            res = CN_FONT_PATH;
#else
        res = EN_FONT_PATH;
#endif
        return res;
    }
}
