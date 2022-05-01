using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PreloadTexturePreview : MonoBehaviour
{
    public List<Texture> TexturesArray;

    public List<GameObject> RegularTextureButtons, NormalMapTextureButtons, PlayerImportedTextureButtons;

    public GameObject Buttons, ButtonPrefab;
    public SelectTexture SelectTexture;

    private RectTransform _buttonsRectTransform;

    int _amount = 0;

    private void Awake()
    {
        TexturesArray = new List<Texture>(Resources.LoadAll<Texture>("Textures/"));
    }

    public void PreloadStart()
    {
        _buttonsRectTransform = Buttons.GetComponent<RectTransform>();

        foreach (Texture texture in TexturesArray)
        {
            AddTexture(texture, false);
        }
    }

    public void AddTexture(Texture texture, bool setName, string name = "")
    {
        GameObject button = Instantiate(ButtonPrefab);
        button.GetComponent<RectTransform>().SetParent(Buttons.transform);
        button.GetComponent<TextureButton>().Texture = texture;
        button.GetComponent<TextureButton>().Preview.texture = texture;
        button.GetComponent<TextureButton>().SelectTexture = SelectTexture;

        _amount += 1;
        if (_amount % 4 == 0) _buttonsRectTransform.sizeDelta += new Vector2(0, 168);

        if (setName)
        {
            texture.name = name;
            TexturesArray.Add(texture);
        }
    }
}
