using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMask : MonoBehaviour
{
    [HideInInspector]
    public Material mat;
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (GetComponent<Image>() != null)
        {
            GetComponent<Image>().material = GetMaterial();
            mat = GetComponent<Image>().material;

        }


        if (GetComponent<Text>() != null)
        {
            GetComponent<Text>().material = GetMaterial();
            mat = GetComponent<Text>().material;
        }
    }

    public Material GetMaterial()
    {
        Material mat = new Material(Shader.Find("UI/DefaultMask"));
        
        return mat;
    }


}
