using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInputMode : MonoBehaviour
{
    [SerializeField] private GameObject[] _mobileButtons;

    private PlayerManager _playerManager;

    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    void Update()
    {
        if (Input.anyKey || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (_playerManager.CurrentGameMode != 2) Cursor.lockState = CursorLockMode.Locked;
            _playerManager.CurrentGameMode = 2;
            foreach (GameObject obj in _mobileButtons) obj.SetActive(false);
        }

        for (int i = 0; i < 20; i++) 
        {
            if (Input.GetKeyDown("joystick 1 button " + i) || Input.GetAxis("LeftXAxis") != 0 || Input.GetAxis("LeftYAxis") != 0 || Input.GetAxis("RightXAxis") != 0 || Input.GetAxis("RightYAxis") != 0)
            {
                _playerManager.CurrentGameMode = 1;
                foreach (GameObject obj in _mobileButtons) obj.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (Input.touchCount > 0)
        {
            _playerManager.CurrentGameMode = 0;
            foreach (GameObject obj in _mobileButtons) obj.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
