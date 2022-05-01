using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeleteByTimer : MonoBehaviourPun
{
    [SerializeField]
    private float _timer;

    [SerializeField]
    private bool _pun;

    private void Start()
    {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(_timer);
        if (_pun)
        {
            if (this.photonView.IsMine) PhotonNetwork.Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
}
