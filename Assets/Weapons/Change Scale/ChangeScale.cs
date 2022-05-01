using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChangeScale : MonoBehaviourPun
{
    private Boolean _x = true, _y = true, _z = true, _inPersentages = true;
    private float _value = 1.1f;

    public void ChangeX(Boolean value)
    {
        _x = value;
    }

    public void ChangeY(Boolean value)
    {
        _y = value;
    }

    public void ChangeZ(Boolean value)
    {
        _z = value;
    }

    public void ChangeInPersentages(Boolean value)
    {
        _inPersentages = value;
    }

    public void ChangeScalingValue(String input)
    {
        _value = Convert.ToSingle(input);
    }

    public void IncreaseSize()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                obj.gameObject.GetPhotonView().TransferOwnership(this.photonView.Owner);
                float x = obj.transform.localScale.x;
                float y = obj.transform.localScale.y;
                float z = obj.transform.localScale.z;


                if (_inPersentages)
                {
                    if (_x) x *= _value;
                    if (_y) y *= _value;
                    if (_z) z *= _value;
                }
                else
                {
                    if (_x) x += _value;
                    if (_y) y += _value;
                    if (_z) z += _value;
                }

                if (x <= 0) x = 0.01f;
                if (y <= 0) y = 0.01f;
                if (z <= 0) z = 0.01f;

                obj.transform.localScale = new Vector3(x, y, z);
            }
        }
    }

    public void ReduceSize()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                obj.gameObject.GetPhotonView().TransferOwnership(this.photonView.Owner);
                float x = obj.transform.localScale.x;
                float y = obj.transform.localScale.y;
                float z = obj.transform.localScale.z;


                if (_inPersentages)
                {
                    if (_x) x /= _value;
                    if (_y) y /= _value;
                    if (_z) z /= _value;
                }
                else
                {
                    if (_x) x -= _value;
                    if (_y) y -= _value;
                    if (_z) z -= _value;
                }

                if (x <= 0) x = 0.01f;
                if (y <= 0) y = 0.01f;
                if (z <= 0) z = 0.01f;

                obj.transform.localScale = new Vector3(x, y, z);
            }
        }
    }
}
