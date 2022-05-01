using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Holder : MonoBehaviourPun
{
    [SerializeField] private OnlineController _onlineController;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private PlayerLook _playerLook;

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform[] _linePoints;

    [SerializeField] private GameObject _obj;
    private Rigidbody _objRB;

    [SerializeField] private Transform _holdPoint;
    private Joint _holdJoint;
    private Quaternion _holdRot;

    private bool _enableRot = true;
    private Coroutine _holdRotCoroutine, _enableRotCoroutine;
    private float _zoomValue;

    private Prop _holdedProp;

    private void Start()
    {
        _holdJoint = _holdPoint.GetComponent<Joint>();
    }

    public void RotToggle(Boolean value)
    {
        _holdRot = _holdPoint.rotation;
        _enableRot = !value;
    }

    IEnumerator HoldRotation()
    {
        while (true)
        {
            if (!_enableRot) _holdPoint.rotation = _holdRot;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Hold()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop prop))
            {
                if (!prop.CanHold || prop.IsHolded) return;

                _holdedProp = prop;
                _holdedProp.IsHolded = true;

                _holdRot = _holdPoint.rotation;
                _holdPoint.position = hit.point;
                _obj = hit.collider.gameObject;
                _obj.GetPhotonView().TransferOwnership(this.photonView.Owner);
                _objRB = _obj.GetComponent<Rigidbody>();
                _holdJoint.connectedBody = _objRB;

                Color color = Color.cyan;

                _onlineController.photonView.RPC("SetKinematic", RpcTarget.AllBuffered, _obj.GetPhotonView().ViewID, false);
                _onlineController.photonView.RPC("ChangeOutlineColor", RpcTarget.AllBuffered, _obj.GetPhotonView().ViewID, color.r + "/" + color.g + "/" + color.b);
                _onlineController.photonView.RPC("EnableOutline", RpcTarget.AllBuffered, _obj.GetPhotonView().ViewID, true);

                _holdRotCoroutine = StartCoroutine(HoldRotation());

                _linePoints[2].position = hit.point;
                _linePoints[2].GetComponent<Joint>().connectedBody = _objRB;
                _linePoints[0].LookAt(_linePoints[2]);
                _linePoints[1].position = _linePoints[0].position + _linePoints[0].forward * Vector3.Distance(_linePoints[0].position, _linePoints[2].position) / 3;

                _lineRenderer.enabled = true;
            }
        }
    }

    public void ForgetObj()
    {
        _onlineController.photonView.RPC("EnableOutline", RpcTarget.AllBuffered, _obj.GetPhotonView().ViewID, false);
        _obj = null;
        _objRB = null;
        StopCoroutine(_holdRotCoroutine);
        _lineRenderer.enabled = false;
        _linePoints[2].GetComponent<Joint>().connectedBody = null;

        _holdedProp.IsHolded = false;
        _holdedProp = null;
    }

    public void Fix()
    {
        _holdJoint.connectedBody = null;
        _onlineController.photonView.RPC("SetKinematic", RpcTarget.AllBuffered, _obj.GetPhotonView().ViewID, true);
        ForgetObj();

    }

    public void Throw()
    {
        _holdJoint.connectedBody = null;
        _onlineController.photonView.RPC("SetKinematic", RpcTarget.AllBuffered, _obj.GetPhotonView().ViewID, false);
        _objRB.AddForce(transform.forward * _objRB.mass, ForceMode.Impulse);
        ForgetObj();
    }

    public void Drop()
    {
        _holdJoint.connectedBody = null;
        _onlineController.photonView.RPC("SetKinematic", RpcTarget.AllBuffered, _obj.GetPhotonView().ViewID, false);
        _objRB.AddForce(transform.forward * 0.0001f, ForceMode.Impulse);
        ForgetObj();
    }

    public void EnableRot(bool value)
    {
        _playerManager.EnableLook(!value);
        _playerLook.ClampRot = !value;

        if (value)
        {
            _enableRotCoroutine = StartCoroutine(RotationCoroutine());
        }
        else
        {
            StopCoroutine(_enableRotCoroutine);
        }
    }

    public void ResetRot()
    {
        _holdPoint.rotation = Quaternion.Euler(Vector3.zero);
    }

    IEnumerator RotationCoroutine()
    {
        _holdRot = _holdPoint.rotation;
        float startX = _playerLook.XAngle;
        float startY = _playerLook.YAngle;

        while (true)
        {
            Quaternion rot = Quaternion.Euler(_playerLook.YAngle - startY, _playerLook.XAngle - startX, _holdPoint.eulerAngles.z);
            _holdPoint.rotation = Quaternion.Euler(_holdRot.eulerAngles.x + rot.eulerAngles.x, _holdRot.eulerAngles.y + rot.eulerAngles.y, rot.eulerAngles.z);
            yield return new WaitForEndOfFrame();
        }
    }

    private Coroutine _zoomCoroutine;

    public void Zoom(float value)
    {
        if (value > 0 || value < 0)
        {
            _zoomValue = value;
            _zoomCoroutine = StartCoroutine(ZoomObject());
        }
        else
        {
            StopCoroutine(_zoomCoroutine);
        }
    }

    IEnumerator ZoomObject()
    {
        while (true)
        {
            if (_zoomValue > 0)
            {
                _holdPoint.position += transform.forward * _zoomValue;
            }
            else
            {
                Debug.Log(Vector3.Distance(_obj.GetComponent<Prop>().Bounds.ClosestPoint(transform.position), transform.position));
                if (Vector3.Distance(_obj.GetComponent<Prop>().Bounds.ClosestPoint(transform.position), transform.position) > 1f) _holdPoint.position += transform.forward * _zoomValue;
            }
            _linePoints[0].LookAt(_linePoints[2]);
            _linePoints[1].position = _linePoints[0].position + _linePoints[0].forward * Vector3.Distance(_linePoints[0].position, _linePoints[2].position) / 3;
            
            yield return new WaitForEndOfFrame();
        }
    }
}
