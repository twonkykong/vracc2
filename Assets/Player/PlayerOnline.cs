using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerOnline : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject[] _hide, _show;

    [SerializeField]
    private PhotonView _skin;

    [SerializeField]
    private TextMeshPro _nickname;

    [SerializeField]
    private GameObject _propMenuButton, _weaponsButtons, _playerButtonPrefab, _skinObj;

    [SerializeField]
    private Notifications _notifications;

    [SerializeField]
    private Text _roomInfo;

    [SerializeField]
    private Transform _playersButtonsBG;

    [SerializeField]
    private Chat _chat;

    [SerializeField]
    private MonoBehaviour[] _removeComponents;

    [SerializeField]
    private List<GameObject> _playerButtons;

    private void Start()
    {
        _nickname.text = this.photonView.Owner.NickName;

        if (!this.photonView.IsMine)
        {
            foreach (GameObject obj in _hide) obj.SetActive(false);
            foreach (GameObject obj in _show) obj.SetActive(true);
            GetComponent<PlayerMovement>().enabled = false;
            if (_removeComponents != null) foreach (MonoBehaviour component in _removeComponents) component.enabled = false;
            _skinObj.layer = 0;
        }
        else
        {
            this.photonView.RPC("ChangeObjectColor", RpcTarget.AllBuffered, _skin.ViewID, PlayerPrefs.GetFloat("skin color R") / 255 + "/" 
                + PlayerPrefs.GetFloat("skin color G") / 255 + "/" 
                + PlayerPrefs.GetFloat("skin color B") / 255);

            GameObject.Find("Game manager").GetComponent<GameManager>().EventsNotification = _notifications;

            if (!Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties["spawn props"])) _propMenuButton.SetActive(false);
            if (!Convert.ToBoolean(PhotonNetwork.CurrentRoom.CustomProperties["weapons"])) _weaponsButtons.SetActive(false);
            UpdateInfo();
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void ChangeObjectColor(int ViewID, string color)
    {
        string[] colors = color.Split('/');
        float r = Convert.ToSingle(colors[0]);
        float g = Convert.ToSingle(colors[1]);
        float b = Convert.ToSingle(colors[2]);

        Renderer renderer = PhotonView.Find(ViewID).GetComponent<Renderer>();
        Material mat = renderer.material;
        mat.color = new Color(r, g, b);
        renderer.material = mat;
    }

    public void UpdateInfo()
    {
        _roomInfo.text = "room name: " + PhotonNetwork.CurrentRoom.Name +
            "\nowner: " + PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.MasterClientId).NickName +
            "\nplayers in room: " + PhotonNetwork.CurrentRoom.PlayerCount +
            "\n\nspawn props: " + PhotonNetwork.CurrentRoom.CustomProperties["spawn props"] +
            "\nweapons: " + PhotonNetwork.CurrentRoom.CustomProperties["weapons"] + 
            "\n";

        if (_playerButtons.Count > 0) foreach (GameObject obj in _playerButtons) Destroy(obj);

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            GameObject button = Instantiate(_playerButtonPrefab);
            button.GetComponent<RectTransform>().SetParent(_playersButtonsBG);
            button.GetComponent<RectTransform>().localPosition = new Vector2(22.767f, 221.4f - (50 * i));
            _playerButtons.Add(button);

            string star = "";
            if (PhotonNetwork.PlayerList[i].IsMasterClient) star = "★ ";
            button.GetComponentInChildren<Text>().text = star + PhotonNetwork.PlayerList[i].NickName;
            button.GetComponentInChildren<PlayerKickButton>().Player = PhotonNetwork.PlayerList[i];
            
            Texture2D tex = new Texture2D(128, 128);
            string str = PhotonNetwork.PlayerList[i].CustomProperties["pfp"].ToString(); 
            ImageConversion.LoadImage(tex, Convert.FromBase64String(str));
            button.GetComponentInChildren<RawImage>().texture = tex;
            if (!PhotonNetwork.IsMasterClient || PhotonNetwork.PlayerList[i].IsLocal)
            {
                button.GetComponent<RectTransform>().localPosition = new Vector2(0, 221.4f - (50 * i));
                button.GetComponent<RectTransform>().sizeDelta = new Vector3(310, 45);
                button.GetComponentInChildren<Button>().gameObject.SetActive(false);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateInfo();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateInfo();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Menu");
    }
}
