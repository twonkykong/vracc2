using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private GameObject _ragdollPrefab;
    [SerializeField] private GameObject[] _hide, _hideMultiplayer;
    [SerializeField] private Transform[] _limbs;

    [SerializeField] private MonoBehaviour[] _disableComponents;

    [SerializeField] private RectTransform _healthBar;
    [SerializeField] private int _maxHealth = 100, _health, _deathSeconds = 5;

    public PlayerLook PlayerLook;

    private GameManager _gameManager;
    private PlayerOnline _playerOnline;
    private PlayerManager _playerManager;
    private Rigidbody _rb;

    private float _hpBarRatio;

    private bool _spawnedRagdoll = false;

    private void Awake()
    {
        _playerOnline = GetComponent<PlayerOnline>();
        _playerManager = GetComponent<PlayerManager>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _hpBarRatio = _healthBar.sizeDelta.x / _maxHealth;
        _health = _maxHealth;

        _gameManager = GameObject.Find("Game manager").GetComponent<GameManager>();
    }

    [PunRPC]
    public void GetDamage(int damage, int viewID)
    {
        if (this.photonView.IsMine)
        {
            _health -= damage;
            _healthBar.sizeDelta = new Vector2(_health * _hpBarRatio, _healthBar.sizeDelta.y);
            
            if (_health <= 0)
            {
                this.photonView.RPC("BeforeDeath", RpcTarget.All, viewID);
                StartCoroutine(DeathCoroutine());
                if (_takeDamageCoroutine != null) StopCoroutine(_takeDamageCoroutine);
            }
        }
    }

    [PunRPC]
    public void BeforeDeath(int viewID)
    {
        GetComponent<Collider>().enabled = false;
        _rb.isKinematic = true;

        foreach (GameObject obj in _hideMultiplayer) obj.SetActive(false);
        foreach (MonoBehaviour monoBeh in _disableComponents) monoBeh.enabled = false;

        string message = " died";
        if (PhotonView.Find(viewID) != null) message = " was killed by " + PhotonView.Find(viewID).Owner.NickName;
        _gameManager.GetMessage(this.photonView.Owner.NickName + message, "red");
    }

    private IEnumerator DeathCoroutine()
    {
        if (_spawnedRagdoll) yield break;

        foreach (GameObject obj in _hide) obj.SetActive(false);

        PlayerRagdoll ragdoll = PhotonNetwork.Instantiate(_ragdollPrefab.name, transform.position, transform.rotation).GetComponent<PlayerRagdoll>();
        _spawnedRagdoll = true;
        _playerOnline.photonView.RPC("ChangeObjectColor", RpcTarget.AllBuffered, ragdoll.SkinObject.GetPhotonView().ViewID, PlayerPrefs.GetFloat("skin color R") / 255 + "/"
                + PlayerPrefs.GetFloat("skin color G") / 255 + "/"
                + PlayerPrefs.GetFloat("skin color B") / 255);

        ragdoll.PlayerLimbs = _limbs;
        ragdoll.SetLimbsPositions();
        ragdoll.LowerSpine.velocity = _rb.velocity;
        foreach (Transform limb in ragdoll.PlayerLimbs) limb.gameObject.layer = 11;

        SimpleCameraLook camera = ragdoll.Camera;
        camera.SetCameraSettings(PlayerLook);
        camera.PlayerManager = _playerManager;

        yield return new WaitForSeconds(_deathSeconds);

        PhotonNetwork.Destroy(ragdoll.gameObject);
        foreach (GameObject obj in _hide) obj.SetActive(true);
        transform.position = _gameManager.SpawnPoints[Random.Range(0, _gameManager.SpawnPoints.Count - 1)].position;

        this.photonView.RPC("AfterDeath", RpcTarget.All);
        _healthBar.sizeDelta = new Vector2(_health * _hpBarRatio, _healthBar.sizeDelta.y);

        _spawnedRagdoll = false;
    }

    [PunRPC]
    public void AfterDeath()
    {
        GetComponent<Collider>().enabled = true;
        _rb.isKinematic = false;
        foreach (GameObject obj in _hideMultiplayer) obj.SetActive(true);
        foreach (MonoBehaviour monoBeh in _disableComponents) monoBeh.enabled = true;
        _health = 100;
    }

    [PunRPC]
    public void AddHealth(int healthValue)
    {
        if (this.photonView.IsMine)
        {
            _health += healthValue;
            _health = Mathf.Clamp(_health, 0, 100);
            _healthBar.sizeDelta = new Vector2(_health * _hpBarRatio, _healthBar.sizeDelta.y);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_health);
        }
        if (stream.IsReading)
        {
            _health = (int)stream.ReceiveNext();
        }
    }

    #region TakeDamage
    private DamageProp _dProp;
    private Coroutine _takeDamageCoroutine;

    private IEnumerator TakeDamageEnumerator()
    {
        while (true)
        {
            GetDamage(_dProp.Damage, 0);
            yield return new WaitForSeconds(_dProp.DamageTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("prop"))
        {
            Debug.Log(collision.collider.name);
            Transform obj = collision.transform;

            if (obj.GetComponent<DamageProp>() != null)
            {
                if (_dProp == obj.GetComponent<DamageProp>()) return;

                _dProp = obj.GetComponent<DamageProp>();
                if (_takeDamageCoroutine != null) StopCoroutine(_takeDamageCoroutine);
                _takeDamageCoroutine = StartCoroutine(TakeDamageEnumerator());
            }

            while (obj.parent != null) obj = obj.parent;

            if (obj.GetComponentInChildren<AddHealthProp>() != null)
            {
                AddHealth(obj.GetComponentInChildren<AddHealthProp>().HealthPoints);
                obj.gameObject.GetPhotonView().TransferOwnership(this.photonView.Owner);

                PhotonNetwork.Destroy(obj.gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("prop"))
        {
            Transform obj = collision.transform;

            if (obj.GetComponent<DamageProp>() != null)
            {
                if (obj == _dProp.transform)
                {
                    StopCoroutine(_takeDamageCoroutine);
                    _dProp = null;
                }
            }
        }
    }
    #endregion TakeDamage
}