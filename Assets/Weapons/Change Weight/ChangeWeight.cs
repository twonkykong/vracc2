using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class ChangeWeight : MonoBehaviourPun
{
    [SerializeField]
    private OnlineController _onlineController;

    private Boolean _inPersentages = true;
    private float _value = 1.1f;

    public void ChangeInPersentages(Boolean value)
    {
        _inPersentages = value;
    }

    public void ChangeScalingValue(String input)
    {
        _value = Convert.ToSingle(input);
    }

    public void IncreaseWeight()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                float mass = obj.GetComponent<Rigidbody>().mass;

                if (_inPersentages)
                {
                    mass *= _value;
                }
                else
                {
                    mass += _value;
                }

                _onlineController.photonView.RPC("ChangeWeight", RpcTarget.AllBufferedViaServer, obj.gameObject.GetPhotonView().ViewID, mass);
            }
        }
    }

    public void ReduceWeight()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                float mass = obj.GetComponent<Rigidbody>().mass;

                if (_inPersentages)
                {
                    mass /= _value;
                }
                else
                {
                    mass -= _value;
                }

                _onlineController.photonView.RPC("ChangeWeight", RpcTarget.AllBufferedViaServer, obj.gameObject.GetPhotonView().ViewID, mass);
            }
        }
    }
}
