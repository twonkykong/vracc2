using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Prop : MonoBehaviourPun, IPunObservable
{
    public bool IsKinematic = false, IsHolded = false, CanHold = true;
    public Bounds Bounds;

    [SerializeField] private Collider _colliderBounds;

    public List<Joint> JointsList;

    [SerializeField] private AudioSource _audioSrc;

    private Rigidbody _rb;

    [SerializeField] private float _hitSpeed = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.solverIterations = 60;
    }

    void Start()
    {
        if (_colliderBounds != null)
        {
            Bounds = _colliderBounds.bounds;
            Destroy(_colliderBounds);
        }
        else
        {
            if (GetComponent<Collider>() != null)
            {
                Bounds = GetComponent<Collider>().bounds;
            }

            if (GetComponentInChildren<Collider>() != null)
            {
                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    Bounds.Encapsulate(col.bounds);
                }
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (_rb.velocity.magnitude > _hitSpeed)
        {
            _audioSrc.Stop();
            _audioSrc.Play();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(IsHolded);
        }
        if (stream.IsReading)
        {
            IsHolded = (bool)stream.ReceiveNext();
        }
    }
}
