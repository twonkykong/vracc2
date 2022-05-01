using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeTextureEvent : MonoBehaviour
{
    public UnityEvent Event;

    [SerializeField]
    private SelectTexture _selectTexture;

    public void InvokeEvent()
    {
        Event.Invoke();
    }

    public void ChangeEvent()
    {
        _selectTexture.Event.AddListener(InvokeEvent);
    }
}
