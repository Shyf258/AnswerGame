using UnityEngine;
using UnityEngine.UI;

public class LocalizationSprite : MonoBehaviour
{
    [GL_Name("中文图片")] public Sprite cnSprite;
    [GL_Name("英文图片")] public Sprite enSprite;
    [GL_Name("设置成图片大小")] public bool setNativeSize = true;

    private Image _image;
    private ELanguage _curLanguage = ELanguage.EN;

    private void Start()
    {
        _image = GetComponent<Image>();
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
        if (param is EventParam<ELanguage> p) ChangeLanguage(p._param);
    }
    
    private void ChangeLanguage(ELanguage lan)
    {
        _curLanguage = lan;
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        if (_image == null) return;

        switch (_curLanguage)
        {
            case ELanguage.CN:
                _image.overrideSprite = cnSprite;
                break;
            default:
                _image.overrideSprite = enSprite;
                break;
        }
        
        if (setNativeSize) _image.SetNativeSize();
    }
}
