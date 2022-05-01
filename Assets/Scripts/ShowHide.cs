using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHide : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _show, _hide;

    public void Click()
    {
        foreach (GameObject obj in _hide) obj.SetActive(false);
        foreach (GameObject obj in _show) obj.SetActive(true);
    }
}
