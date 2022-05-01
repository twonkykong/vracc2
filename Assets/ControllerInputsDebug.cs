using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputsDebug : MonoBehaviour
{
    private void Update()
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                Debug.Log("key: " + vKey);
            }
        }
    }
}
