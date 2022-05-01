using System;
using UnityEngine;

public class PreloadMaterialsPreview : MonoBehaviour
{
    [SerializeField]
    private Material[] _materialsArray;

    public GameObject Buttons, ButtonPrefab;
    public SetMaterial SetMaterial;

    private RectTransform _contentContainer;

    int _amount = 0;

    private void Awake()
    {
        _materialsArray = Resources.LoadAll<Material>("Materials/");
    }

    public void PreloadStart()
    {
        _contentContainer = Buttons.transform.parent.transform.parent.GetComponent<RectTransform>();

        foreach (Material material in _materialsArray)
        {
            GameObject button = Instantiate(ButtonPrefab);
            button.GetComponent<RectTransform>().SetParent(Buttons.transform);
            button.GetComponent<MaterialButton>().Material = material;
            button.GetComponent<MaterialButton>().MakePreview();
            button.GetComponent<MaterialButton>().SetMaterial = SetMaterial;

            _amount += 1;
            if (_amount % 4 == 0) _contentContainer.sizeDelta += new Vector2(0, 168);
        }
    }
}
