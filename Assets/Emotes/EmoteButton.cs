using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteButton : MonoBehaviour
{
    public Emotions Emotions;
    public Texture EmoteTexture;

    public RawImage Preview;

    public void PlayEmote()
    {
        Emotions.Emote(EmoteTexture);
    }
}
