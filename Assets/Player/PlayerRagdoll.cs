using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour
{
    public Transform[] PlayerLimbs;

    [SerializeField]
    private Transform[] _ragdollLimbs;

    public SimpleCameraLook Camera;

    public GameObject SkinObject;

    public Rigidbody LowerSpine;

    private Transform _lowerSpineTransform, _camParent;

    private void Start()
    {
        _lowerSpineTransform = LowerSpine.transform;
        _camParent = Camera.transform.parent;
    }

    private void Update()
    {
        _camParent.position = _lowerSpineTransform.position;
    }

    public void SetLimbsPositions()
    {
        for (int i = 0; i < _ragdollLimbs.Length; i++)
        {
            _ragdollLimbs[i].position = PlayerLimbs[i].position;
            _ragdollLimbs[i].rotation = PlayerLimbs[i].rotation;
        }
    }
}
