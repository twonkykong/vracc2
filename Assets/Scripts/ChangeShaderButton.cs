using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShaderButton : MonoBehaviour
{
    public SetMaterial SetMaterial;
    public string ShaderName;

    public void ChangeShader()
    {
        SetMaterial.CurrentShader = ShaderName;
    }
}
