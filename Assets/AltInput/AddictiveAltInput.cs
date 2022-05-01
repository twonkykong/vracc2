using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddictiveAltInput : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;
    [SerializeField] private string[] _buttonNames;

    private void Update()
    {
        for (int i = 0; i < _buttonNames.Length; i++)
        {
            if (Input.GetKeyDown(_buttonNames[i])) _buttons[i].onClick.Invoke();
        }
    }
}
