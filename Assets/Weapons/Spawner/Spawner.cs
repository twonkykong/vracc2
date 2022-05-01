using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Transform _body;

    public void Spawn(GameObject objectPrefab)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            Vector3 objScale = objectPrefab.transform.localScale;
            float rayLength = objScale.x + objScale.y + objScale.z;

            Vector3 objRot = objectPrefab.transform.eulerAngles;
            GameObject obj = PhotonNetwork.Instantiate(objectPrefab.name, hit.point, Quaternion.Euler(objRot.x, _body.eulerAngles.y, objRot.z));

            obj.layer = 10;
            LayerMask mask = LayerMask.GetMask("temp");

            Physics.Raycast(hit.point - hit.normal * rayLength, hit.normal, out RaycastHit hit1, rayLength, mask);
            obj.transform.position += hit.normal * Vector3.Distance(hit.point, hit1.point);

            obj.layer = 0;
        }
    }
}
