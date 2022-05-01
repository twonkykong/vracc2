using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private Button _propMenuButton;

    private Coroutine _updateCoroutine;

    private bool _isInPropMenu, _isInMenu;

    private void Awake()
    {
        _updateCoroutine = StartCoroutine(UpdateEnumerator());
    }

    private IEnumerator UpdateEnumerator()
    {
        while (true)
        {

            yield return new WaitForEndOfFrame();
        }
    }
}
