using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectTexture : MonoBehaviour
{
    public UnityEvent Event;
    public Texture Texture;

    public void ChangeTexture(Texture tex)
    {
        Texture = tex;
    }

    public void InvokeEvent()
    {
        Event.Invoke();
    }
}
