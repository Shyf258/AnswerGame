using UnityEngine.UI;

namespace SUIFW
{
    public interface IUIBase
    {
        void RigisterButtonObjectEvent(Button button, EventTriggerListener.VoidDelegate delHandle);
        
        void RigisterButtonObjectEvent(string buttonName, EventTriggerListener.VoidDelegate delHandle);
    }
}