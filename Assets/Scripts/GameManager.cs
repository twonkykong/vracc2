using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _playerPrefab;

    public Notifications EventsNotification;

    public Text ChatText;

    public List<Transform> SpawnPoints;

    private void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Count-1)].position, Quaternion.identity);

        foreach (GameObject nickname in GameObject.FindGameObjectsWithTag("nickname"))
        {
            nickname.GetComponent<TextMeshPro>().text = nickname.GetPhotonView().Owner.NickName;
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Menu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        EventsNotification.PushOnlineNotification(newPlayer.NickName + " joined room");
        GetMessage(newPlayer.NickName + " joined room", "yellow");

        foreach (GameObject nickname in GameObject.FindGameObjectsWithTag("nickname"))
        {
            if (nickname.GetPhotonView().Owner.UserId == newPlayer.UserId)
            {
                nickname.GetComponent<TextMeshPro>().text = newPlayer.NickName;
                return;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        EventsNotification.PushOnlineNotification(otherPlayer.NickName + " left room");
        GetMessage(otherPlayer.NickName + " left room", "yellow");
    }

    [PunRPC]
    public void GetMessage(string msg, string color)
    {
        ChatText.text += "\n<color=" + color + ">[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + "] " + msg + "</color>";
    }
}
