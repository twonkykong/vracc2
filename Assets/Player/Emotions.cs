using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;

public class Emotions : MonoBehaviourPun
{
    [SerializeField]
    private GameObject _emotePrefab, _canvasEmotePrefab;

    [SerializeField]
    private Transform _emoteSpawnPoint, _canvasEmoteSpawnPoint, _canvasParent;

    [SerializeField]
    private UnityEvent _playerControllsEvents;

    [SerializeField]
    private PlayerManager _playerManager;

    public void Emote(Texture emoteTex)
    {
        if (_playerManager.CurrentCam != 1)
        {
            GameObject canvasObj = Instantiate(_canvasEmotePrefab, _canvasEmoteSpawnPoint.position
               + new Vector3(UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(-30, 30)), Quaternion.identity);
            canvasObj.transform.SetParent(_canvasParent);
            canvasObj.GetComponentInChildren<RawImage>().texture = emoteTex;
        }

        GameObject obj = PhotonNetwork.Instantiate(_emotePrefab.name, _emoteSpawnPoint.position
            + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.15f, 0.15f), UnityEngine.Random.Range(-0.3f, 0.3f)), Quaternion.identity);
        this.photonView.RPC("ChangeEmoteTexture", RpcTarget.All, obj.GetPhotonView().ViewID, emoteTex.name);
        _playerControllsEvents?.Invoke();
        obj.GetComponentInChildren<Renderer>().gameObject.layer = 11;
    }

    [PunRPC]
    public void ChangeEmoteTexture(int viewID, string texName)
    {
        PreloadEmotes preloadEmotes = FindObjectOfType<PreloadEmotes>();
        Renderer r = PhotonView.Find(viewID).GetComponentInChildren<Renderer>();
        r.material.SetTexture("_MainTex", Array.Find(preloadEmotes.EmotesArray.ToArray(), emote => emote.name == texName));
    }
}
