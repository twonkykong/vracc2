using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropMenuAltInput : MonoBehaviour
{
    [SerializeField] private Button _openPropMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) _openPropMenu.onClick.Invoke();
    }
}
