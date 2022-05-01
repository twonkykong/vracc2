using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickManager : MonoBehaviour
{
    [SerializeField] private RectTransform _handle;

    private void OnDisable()
    {
        _handle.localPosition = Vector2.zero;
    }
}
