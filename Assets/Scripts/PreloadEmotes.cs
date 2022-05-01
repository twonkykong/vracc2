using System;
using System.Collections.Generic;
using UnityEngine;

public class PreloadEmotes : MonoBehaviour
{
    public List<Texture> EmotesArray;

    public GameObject Buttons, ButtonPrefab;
    public Emotions Emotions;
    private RectTransform _buttonsRectTransform;

    int _amount = 0;

    private void Awake()
    {
        EmotesArray = new List<Texture>(Resources.LoadAll<Texture>("Emotes/"));
    }

    public void PreloadStart()
    {
        _buttonsRectTransform = Buttons.GetComponent<RectTransform>();

        foreach (Texture texture in EmotesArray)
        {
            AddEmote(texture, false);
        }
    }

    public void AddEmote(Texture texture, bool setName, string name = "")
    {
        GameObject button = Instantiate(ButtonPrefab);
        button.GetComponent<RectTransform>().SetParent(Buttons.transform);
        button.GetComponent<EmoteButton>().EmoteTexture = texture;
        button.GetComponent<EmoteButton>().Preview.texture = texture;
        button.GetComponent<EmoteButton>().Emotions = Emotions;

        _amount += 1;
        if (_amount % 4 == 0) _buttonsRectTransform.sizeDelta += new Vector2(0, 168);

        if (setName)
        {
            texture.name = name;
            EmotesArray.Add(texture);
        }
    }
}
