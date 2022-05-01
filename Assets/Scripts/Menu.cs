using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public class Menu : MonoBehaviourPunCallbacks
{
    private string _nickname;

    [SerializeField]
    private MenuConnectionNotification _connectionNotification, _errorNotification;

    [SerializeField]
    private Text _info, _sensitivityValue, _maxPlayersValue;

    [SerializeField]
    private GameObject[] _onlineObjects;

    [SerializeField]
    private GameObject _reconnectButtons, _reconnectingCircle, _debugConsole;

    [SerializeField]
    private Slider[] _skinColorSliders;

    [SerializeField]
    private Slider _sensitivity;

    [SerializeField]
    private Text[] _skinColorSlidersValues;

    [SerializeField]
    private Image _skinColorPreview;

    [SerializeField]
    private RawImage _pfpPreview;

    [SerializeField]
    private Texture _defaultPfp;

    [SerializeField]
    private AudioSource _audio;

    [SerializeField]
    private AudioClip _audioDisconnected, _audioConnected;

    [SerializeField]
    private Toggle _spawnProps, _weapons, _fallDamage, _bulletDamage, _spawnPropsOffline, _weaponsOffline, _fallDamageOffline, _bulletDamageOffline;

    private bool _keepReconnecting = true;

    private string _mapName = "flatness";

    void Start()
    {
         _nickname = PlayerPrefs.GetString("nickname", "Player " + UnityEngine.Random.Range(0, 101));

        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.NickName = _nickname;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        _skinColorSliders[0].value = PlayerPrefs.GetFloat("skin color R", 255);
        _skinColorSliders[1].value = PlayerPrefs.GetFloat("skin color G", 255);
        _skinColorSliders[2].value = PlayerPrefs.GetFloat("skin color B", 255);

        _sensitivity.value = PlayerPrefs.GetFloat("sensitivity", 10);

        _skinColorPreview.color = new Color(_skinColorSliders[0].value / 255, _skinColorSliders[1].value / 255, _skinColorSliders[2].value / 255);
        for (int i = 0; i < _skinColorSliders.Length; i++)
        {
            _skinColorSlidersValues[i].text = "" + _skinColorSliders[i].value;
        }

        Texture2D tex = new Texture2D(128, 128);

        string base64 = PlayerPrefs.GetString("pfp", null);
        if (!String.IsNullOrEmpty(base64))
        {
            ImageConversion.LoadImage(tex, Convert.FromBase64String(base64));
            _pfpPreview.texture = tex;
        }
        else
        {
            Texture2D tex2D = ModifyTexture.ConvertTextureToTexture2D(_pfpPreview.texture);
            PlayerPrefs.SetString("pfp", Convert.ToBase64String(ImageConversion.EncodeToPNG(tex2D)));
        }

        ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
        ht.Add("pfp", PlayerPrefs.GetString("pfp"));
        PhotonNetwork.SetPlayerCustomProperties(ht);
    }

    private void Update()
    {
        _info.text = "Nickname: " + _nickname + "\nRooms: " + PhotonNetwork.CountOfRooms + "\nPlayers: " + PhotonNetwork.CountOfPlayers;
    }

    public void ConnectToServer()
    {
        _reconnectingCircle.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ReconnectionToggle(Boolean value)
    {
        _keepReconnecting = value;
    }

    public override void OnConnectedToMaster()
    {
        _audio.PlayOneShot(_audioConnected);
        _connectionNotification.ShowNotification("connected to server");
        _reconnectButtons.SetActive(false);
        _reconnectingCircle.SetActive(false);

        foreach (GameObject obj in _onlineObjects) obj.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _audio.PlayOneShot(_audioDisconnected);
        _connectionNotification.ShowNotification("lost connection (" + cause + ")");
        if (_keepReconnecting) ConnectToServer();
        else _reconnectingCircle.SetActive(false);
        _reconnectButtons.SetActive(true);
        
        foreach (GameObject obj in _onlineObjects) obj.SetActive(false);
    }

    public override void OnCreatedRoom()
    {
        SceneManager.LoadScene(_mapName);
    }

    public void SelectMap(string mapName)
    {
        _mapName = mapName;
    }

    public void CreateRoom(InputField createRoomName = null)
    {
        string roomName = "123";
        if (createRoomName != null)
        {
            roomName = "Room " + PhotonNetwork.CountOfRooms + 1;
            if (createRoomName.text != "") roomName = createRoomName.text;
        }

        ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
        ht.Add("spawn props", _spawnProps.isOn);
        ht.Add("weapons", _weapons.isOn);
        ht.Add("fall damage", _fallDamage.isOn);
        ht.Add("bullet damage", _bulletDamage.isOn);

        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = Convert.ToByte(_maxPlayersValue.text), CustomRoomProperties = ht });
    }

    public void JoinRoom(InputField joinRoomName)
    {
        if (joinRoomName.text == "") PhotonNetwork.JoinRandomRoom();
        else PhotonNetwork.JoinRoom(joinRoomName.text);
    }

    public void SinglePlayer()
    {
        StartCoroutine(SinglePlayerCoroutine());
    }
    
    IEnumerator SinglePlayerCoroutine()
    {
        _keepReconnecting = false;
        PhotonNetwork.Disconnect();
        yield return new WaitWhile(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected);
        PhotonNetwork.OfflineMode = true;

        yield return new WaitForSeconds(0.5f);

        string roomName = "offline";

        ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
        ht.Add("spawn props", _spawnPropsOffline.isOn);
        ht.Add("weapons", _weaponsOffline.isOn);
        ht.Add("fall damage", _fallDamageOffline.isOn);
        ht.Add("bullet damage", _bulletDamageOffline.isOn);

        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = Convert.ToByte(_maxPlayersValue.text), CustomRoomProperties = ht });
    }

    public void ChangeNickname(InputField inputField)
    {
        _nickname = inputField.text;
        inputField.text = "";
        if (_nickname == "94") _nickname = "no u";
        _nickname.Replace('★', '*');
        PlayerPrefs.SetString("nickname", _nickname);
        PhotonNetwork.NickName = _nickname;
    }

    public void ChangeSensitivity(Single value)
    {
        _sensitivityValue.text = "" + value;
    }

    public void ChangeMaxPlayers(Single value)
    {
        _maxPlayersValue.text = "" + value;
    }

    public void ApplySkinColor()
    {
        _skinColorPreview.color = new Color(_skinColorSliders[0].value / 255, _skinColorSliders[1].value / 255, _skinColorSliders[2].value / 255);
        for (int i = 0; i < _skinColorSliders.Length; i++)
        {
            _skinColorSlidersValues[i].text = "" + _skinColorSliders[i].value;
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("sensitivity", _sensitivity.value);
    }

    public void SavePlayerSettings()
    {
        PlayerPrefs.SetFloat("skin color R", _skinColorSliders[0].value);
        PlayerPrefs.SetFloat("skin color G", _skinColorSliders[1].value);
        PlayerPrefs.SetFloat("skin color B", _skinColorSliders[2].value);

        Texture2D tex = ModifyTexture.ConvertTextureToTexture2D(_pfpPreview.texture);
        PlayerPrefs.SetString("pfp", Convert.ToBase64String(ImageConversion.EncodeToPNG(ModifyTexture.ConvertTextureToTexture2D(ModifyTexture.ResizeTexture(tex, 100, 100)))));

        ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
        ht.Add("pfp", PlayerPrefs.GetString("pfp"));
        PhotonNetwork.SetPlayerCustomProperties(ht);
    }

    public void DiscardSettings()
    {
        _sensitivity.value = PlayerPrefs.GetFloat("sensitivity", 10);
        _sensitivityValue.text = "" + _sensitivity.value;
    }

    public void DiscardPlayerSettings()
    {
        _skinColorSliders[0].value = PlayerPrefs.GetFloat("skin color R", 255);
        _skinColorSliders[1].value = PlayerPrefs.GetFloat("skin color G", 255);
        _skinColorSliders[2].value = PlayerPrefs.GetFloat("skin color B", 255);

        _skinColorPreview.color = new Color(_skinColorSliders[0].value / 255, _skinColorSliders[1].value / 255, _skinColorSliders[2].value / 255);
        for (int i = 0; i < _skinColorSliders.Length; i++)
        {
            _skinColorSlidersValues[i].text = "" + _skinColorSliders[i].value;
        }

        Texture2D tex = new Texture2D(100, 100);
        ImageConversion.LoadImage(tex, Convert.FromBase64String(PlayerPrefs.GetString("pfp")));
        _pfpPreview.texture = tex;
    }

    public void GetImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, 1, false);
                
                _pfpPreview.texture = ModifyTexture.ResizeTexture(texture, 100, 100);
            }
        });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _connectionNotification.ShowNotification("create room failed (" + returnCode + "):\n" + message);
    }

    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        _connectionNotification.ShowNotification("custom aunthentication failed:\n" + debugMessage);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        _connectionNotification.ShowNotification("join random room failed (" + returnCode + "):\n" + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _connectionNotification.ShowNotification("join room failed (" + returnCode + "):\n" + message);
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        _errorNotification.ShowNotification("error accured:\n" + errorInfo.Info);
    }

    public void ChangeGraphicsSettings(Single value)
    {
        QualitySettings.SetQualityLevel(Convert.ToInt32(value));
    }

    public void SetActiveDebugConsole(Boolean value)
    {
        _debugConsole.SetActive(value);
    }

    public void ResetPfp()
    {
        _pfpPreview.texture = _defaultPfp;
    }
}
