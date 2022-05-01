using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class SetPhysics : MonoBehaviourPun
{
    [SerializeField]
    private OnlineController _onlineController;
    [SerializeField]
    private Notifications _notifications;

    [SerializeField]
    private InputField _mass, _drag, _angularDrag, _dynamicFriction, _staticFriction, _xCenter, _yCenter, _zCenter;

    [SerializeField]
    private Toggle _useGravity, _isKinematic;

    [SerializeField]
    private Dropdown _interpolateType, _collisionDetectionType, _frictionCombine, _bounceCombine;

    [SerializeField]
    private Toggle[] _freezePos, _freezeRot;

    [SerializeField]
    private Slider _bounciness;

    [SerializeField]
    private Text _bouncinessText;

    [SerializeField]
    private PhysicMaterial _defaultPhysMat, _physMat;

    [SerializeField]
    private Rigidbody _rb, _defaultRb;

    public void SetValues(string name)
    {
        Rigidbody rb = _defaultRb;
        if (name.Split('%')[0] != "default") rb = _rb;
        if (name.Split('%')[0] != "don't-change")
        {
            _mass.text = rb.mass.ToString();
            _drag.text = rb.drag.ToString();
            _angularDrag.text = rb.angularDrag.ToString();

            _useGravity.isOn = rb.useGravity;
            _isKinematic.isOn = rb.isKinematic;

            RigidbodyInterpolation[] rbInterpolations = new RigidbodyInterpolation[3] { RigidbodyInterpolation.None, RigidbodyInterpolation.Interpolate, RigidbodyInterpolation.Extrapolate };
            _interpolateType.value = Array.IndexOf(rbInterpolations, rb.interpolation);

            CollisionDetectionMode[] rbCollisionDetections = new CollisionDetectionMode[4] { CollisionDetectionMode.Discrete, CollisionDetectionMode.Continuous, CollisionDetectionMode.ContinuousDynamic, CollisionDetectionMode.ContinuousSpeculative };
            _collisionDetectionType.value = Array.IndexOf(rbCollisionDetections, rb.collisionDetectionMode);

            bool xRotFrozen = (rb.constraints & RigidbodyConstraints.FreezePositionX) != RigidbodyConstraints.None;
            bool yRotFrozen = (rb.constraints & RigidbodyConstraints.FreezePositionY) != RigidbodyConstraints.None;
            bool zRotFrozen = (rb.constraints & RigidbodyConstraints.FreezePositionZ) != RigidbodyConstraints.None;

            bool xPosFrozen = (rb.constraints & RigidbodyConstraints.FreezeRotationX) != RigidbodyConstraints.None;
            bool yPosFrozen = (rb.constraints & RigidbodyConstraints.FreezeRotationY) != RigidbodyConstraints.None;
            bool zPosFrozen = (rb.constraints & RigidbodyConstraints.FreezeRotationZ) != RigidbodyConstraints.None;

            bool[] frozenPos = new bool[3] { xPosFrozen, yPosFrozen, zPosFrozen };
            bool[] frozenRot = new bool[3] { xRotFrozen, yRotFrozen, zRotFrozen };

            for (int i = 0; i < 3; i++)
            {
                _freezePos[i].isOn = frozenPos[i];
                _freezeRot[i].isOn = frozenRot[i];
            }

            _xCenter.text = rb.centerOfMass.x.ToString();
            _yCenter.text = rb.centerOfMass.y.ToString();
            _zCenter.text = rb.centerOfMass.z.ToString();
        }

        PhysicMaterial physMat = _defaultPhysMat;
        if (name.Split('%')[1] == "custom") physMat = _physMat;
        else if (name.Split('%')[1] != "default") physMat = Resources.Load<PhysicMaterial>("PhysicMaterials/" + name.Split('%')[1]);

        _dynamicFriction.text = _staticFriction.text = physMat.dynamicFriction.ToString();

        PhysicMaterialCombine[] physMatCombine = new PhysicMaterialCombine[4] { PhysicMaterialCombine.Average, PhysicMaterialCombine.Minimum, PhysicMaterialCombine.Multiply, PhysicMaterialCombine.Maximum };
        _frictionCombine.value = Array.IndexOf(physMatCombine, physMat.frictionCombine);
        _bounceCombine.value = Array.IndexOf(physMatCombine, physMat.bounceCombine);

        _bounciness.value = physMat.bounciness;

    }

    public void ChangeBouncinessText(Single value)
    {
        _bouncinessText.text = value.ToString();
    }

    public void CopyPhysics()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                _rb = obj.GetComponent<Rigidbody>();
                _physMat = obj.GetComponent<Collider>().material;
                SetValues("custom%custom");
                _notifications.PushToolgunNotification("physics settings copied");
            }
        }
    }

    public void ChangePhysics()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Prop obj))
            {
                _onlineController.photonView.RPC("SetPhysics", RpcTarget.AllBufferedViaServer, obj.gameObject.GetPhotonView().ViewID,
                    Convert.ToSingle(_mass.text), Convert.ToSingle(_drag.text), Convert.ToSingle(_angularDrag.text),
                     Convert.ToSingle(_dynamicFriction.text), Convert.ToSingle(_staticFriction.text), _bounciness.value,
                     Convert.ToSingle(_xCenter.text), Convert.ToSingle(_yCenter.text), Convert.ToSingle(_zCenter.text), 
                    _useGravity.isOn, _isKinematic.isOn,
                    _freezePos[0].isOn, _freezePos[1].isOn, _freezePos[2].isOn, 
                    _freezeRot[0].isOn, _freezeRot[1].isOn, _freezeRot[2].isOn,
                    _interpolateType.value, _collisionDetectionType.value, _frictionCombine.value, _bounceCombine.value);

                _notifications.PushToolgunNotification("physics settings set");
            }
        }
    }
}
