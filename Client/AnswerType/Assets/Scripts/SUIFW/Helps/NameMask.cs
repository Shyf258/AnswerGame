using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameMask : MonoBehaviour
{

    private void Start()
    {
    }

    private void Update()
    {

        float width = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x * 3.2f;
        transform.localScale = new Vector3(width, 153, 1);
    }

}
