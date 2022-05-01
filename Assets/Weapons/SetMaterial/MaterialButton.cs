using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialButton : MonoBehaviour
{
    public Material Material;

    public SetMaterial SetMaterial;

    [SerializeField]
    private RawImage _preview;

    public void MakePreview()
    {
        _preview.texture = RuntimePreviewGenerator.GenerateMaterialPreview(Material, PrimitiveType.Cube, 128, 128);
    }

    public void ChangeMaterial()
    {
        SetMaterial.SelectMaterial(Material);
    }
}
