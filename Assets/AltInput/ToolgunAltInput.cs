using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolgunAltInput : MonoBehaviour
{
    [SerializeField] private Toolgun _toolgun;
    [SerializeField] private Button _openTgMenu;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) _toolgun.Work();
        if (Input.GetMouseButtonDown(1)) _toolgun.AltWork();
        
        if (Input.GetKeyDown(KeyCode.BackQuote)) _openTgMenu.onClick.Invoke();
    }
}
