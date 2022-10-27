using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GuideConfig : MonoBehaviour {

    public RectTransform MaskRect
    {
        get { return this.GetComponent<RectTransform>(); }
    }

}
