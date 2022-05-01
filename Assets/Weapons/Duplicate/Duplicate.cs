using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Duplicate : MonoBehaviourPun
{
    [SerializeField]
    private OnlineController _onlineController;
    [SerializeField]
    private Notifications _notifications;

    [SerializeField]
    private Transform _body;

    [SerializeField]
    private string _poolName;

    private DefaultPool _pool;

    private GameObject _copy;

    private void Start()
    {
        _pool = PhotonNetwork.PrefabPool as DefaultPool;
    }

    public void CopyObj()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop prop))
            {
                _copy = Instantiate(prop.gameObject);
                _copy.SetActive(false);
                _poolName = _copy.name + "_" + System.DateTime.Now;
                _pool.ResourceCache.Add(_poolName, _copy);
                _notifications.PushToolgunNotification("object copied");
            }
        }
    }

    public void DuplicateObj()
    {
        //if (string.IsNullOrEmpty(_poolName)) return;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {

            GameObject prefab = _pool.ResourceCache[_poolName];
            Vector3 objScale = prefab.transform.localScale;
            float rayLength = objScale.x + objScale.y + objScale.z;

            Vector3 objRot = prefab.transform.eulerAngles;
            GameObject obj = PhotonNetwork.Instantiate(_poolName, hit.point, Quaternion.Euler(objRot.x, _body.eulerAngles.y, objRot.z));

            obj.layer = 10;
            LayerMask mask = LayerMask.GetMask("temp");

            Physics.Raycast(hit.point - hit.normal * rayLength, hit.normal, out RaycastHit hit1, rayLength, mask);
            obj.transform.position += hit.normal * Vector3.Distance(hit.point, hit1.point);

            obj.layer = 0;
        }
    }
}
