using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWhenLookAt : MonoBehaviour
{
    [SerializeField]
    private string _componentName;
    [SerializeField]
    private GameObject[] _show;

    private void Update()
    {
        bool show = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f) && hit.collider.GetComponent(_componentName);
        foreach (GameObject obj in _show) obj.SetActive(show);
    }
}
