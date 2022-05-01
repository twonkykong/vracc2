using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Chat : MonoBehaviourPun
{
    [SerializeField]
    private Text _chat;

    [SerializeField]
    private Transform _player;

    private GameManager _gameManager;

    private void Start()
    {
        if (!this.photonView.IsMine) return;
        _gameManager = GameObject.Find("Game manager").GetComponent<GameManager>();
        _gameManager.ChatText = _chat;
    }

    public void SendPlayerMessage(InputField msg)
    {
        if (msg.text == "") return;
        if (msg.text.ToCharArray()[0] == '/')
        {
            string[] splittedMsg = msg.text.Split(' ');

            if (splittedMsg[0] == "/help")
            {
                _gameManager.GetMessage("available commands:\n/cords - gives you your current cords\n/tp x y z - teleport to given cords", "grey");
            }

            else if (splittedMsg[0] == "/cords")
            {
                double x = Math.Round(_player.position.x, 1);
                double y = Math.Round(_player.position.y, 1);
                double z = Math.Round(_player.position.z, 1);

                _gameManager.GetMessage("your cords are: (" + x + ", " + y + ", " + z + ")", "grey");
            }

            else if (splittedMsg[0] == "/tp")
            {
                try
                {
                    float x = Convert.ToSingle(splittedMsg[1]);
                    float y = Convert.ToSingle(splittedMsg[2]);
                    float z = Convert.ToSingle(splittedMsg[3]);

                    _player.position = new Vector3(x, y, z);
                    SendServerMessage("teleported " + PhotonNetwork.NickName + " to (" + x + ", " + y + ", " + z + ")", "silver");
                }
                catch
                {
                    _gameManager.GetMessage("wrong parameters for /tp", "red");
                }
            }
            else _gameManager.GetMessage("unknown command: " + splittedMsg[0], "red");
        }
        else _gameManager.photonView.RPC("GetMessage", RpcTarget.AllViaServer, this.photonView.Owner.NickName + " - " + msg.text, "white");
        msg.text = "";
    }

    public void SendServerMessage(string msg, string color = "yellow")
    {
        _gameManager.photonView.RPC("GetMessage", RpcTarget.AllViaServer, msg, color);
    }
}
