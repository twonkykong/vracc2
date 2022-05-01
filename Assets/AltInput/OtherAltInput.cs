using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherAltInput : MonoBehaviour
{
    [SerializeField] private Button _menuButton;
    [SerializeField] private PlayerManager _playerManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuButton.onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _playerManager.ProceedSitting();
        }
    }
}
