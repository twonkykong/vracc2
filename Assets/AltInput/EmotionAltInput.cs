using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionAltInput : MonoBehaviour
{
    [SerializeField] private Button _openEmotesMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) _openEmotesMenu.onClick.Invoke();
    }
}
