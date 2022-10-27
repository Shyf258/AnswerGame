//2019.05.08    关林
//材质球克隆

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GL_MaterialCloner : MonoBehaviour
{
    [GL_Name("克隆排除序号")]
    public int _index = 1;

    private void Awake()
    {
        Activate();
    }

    public void Activate()
    {
        var renderer = GetComponentInChildren<Renderer>();
        var materials = renderer.sharedMaterials;


        for (int i = 0; i < materials.Length; i++)
        {
            if (i == _index)
                continue;

            var oldMaterial = materials[i];

            if (oldMaterial != null)
            {
                var newMaterial = Instantiate(oldMaterial);
                materials[i] = newMaterial;
            }
        }
        renderer.sharedMaterials = materials;
    }
}
