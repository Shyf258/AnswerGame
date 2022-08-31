using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;

namespace SUIFW.Diplomats.Game
{
    public class UI_IF_FailTips : BaseUIForm
    {


        private Action<int> _action;
        
        
        public override void Refresh(bool recall)
        {
        
        }

        public override void onUpdate()
        {
        
        }

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;
        
            RigisterButtonObjectEvent("Exit", go =>
            {
                CloseUIForm();
            });
        }
    
        public override void InitData(object data)
        {

            var datas = data as Object[];
            if (datas == null)
                return;
            if (datas.Length > 0 && datas[0] is Action<int> action)
            {
                _action = action;
            }

            Invoke("Close", 2f);
        }

        private void Close()
        {
            CloseUIForm();
        }
        
        public override void OnHide()
        {
            base.OnHide();
            _action?.Invoke(2);
            _action = null;
        }
    }

}
