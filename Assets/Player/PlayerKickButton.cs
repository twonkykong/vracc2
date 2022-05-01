using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerKickButton : MonoBehaviourPun
{
    public Player Player;
    public void Click()
    {
        PhotonNetwork.CloseConnection(Player);
    }
}
