using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Welding : MonoBehaviourPun
{
    [SerializeField]
    private OnlineController _onlineController;
    [SerializeField]
    private Notifications _notifications;

    [SerializeField]
    private Toolgun _toolgun;

    [SerializeField]
    private GameObject _firstObj, _secondObj;

    [SerializeField]
    private Vector3 _firstPoint;

    //0 - fixed, 1 - spring, 2 - rope (not now)
    private int _weldingType;

    public void ChangeWeldingType(int value)
    {
        _weldingType = value;
    }

    public void StartWelding()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                _firstObj = obj.gameObject;
                _firstPoint = hit.point;
                _toolgun.SecondFunc = true;
                _notifications.PushToolgunNotification("started welding");
            }
        }
    }

    public void EndWelding()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                _secondObj = obj.gameObject;
                _onlineController.photonView.RPC("Weld", RpcTarget.AllBufferedViaServer, _firstObj.GetPhotonView().ViewID,
                    _secondObj.GetPhotonView().ViewID, _weldingType,
                    _firstPoint.x, _firstPoint.y, _firstPoint.z,
                    hit.point.x, hit.point.y, hit.point.z);

                _notifications.PushToolgunNotification("ended welding");
            }
        }

        _toolgun.SecondFunc = false;
        _firstPoint = Vector3.zero;
        _firstObj = null;
        _secondObj = null;
    }

    public void AbortWelding()
    {
        _toolgun.SecondFunc = false;
        _firstPoint = Vector3.zero;
        _firstObj = null;
        _secondObj = null;
        _notifications.PushToolgunNotification("canceled welding");
    }

    public void RemoveWelding()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                if (obj.JointsList.Count == 0) return;
                _onlineController.photonView.RPC("RemoveWeld", RpcTarget.AllBufferedViaServer, obj.gameObject.GetPhotonView().ViewID);
                _notifications.PushToolgunNotification("removed welding");
            }
        }
    }
}
