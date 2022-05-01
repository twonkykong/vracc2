using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsSelectAltInput : MonoBehaviour
{
    [SerializeField] private Button _openWeaponsMenu;

    private void Update()
    {
        if (Input.GetMouseButtonDown(2)) _openWeaponsMenu.onClick.Invoke();
    }
}
