using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerLook _playerLook;

    private bool _sitting = false;

    private Rigidbody _rb;
    private Collider _col;
    [SerializeField] private Animator _anim;

    [SerializeField] private Transform _joystick, _camRaycastPoint;
    [SerializeField]  private GameObject _movementButtons, _crosshair;
    [SerializeField] private Camera[] _cams;

    public int CurrentCam = 0, 
        CurrentGameMode = 0; //0 - mobile, 1 - gamepad, 2 - pc

    private bool _canChangeSeatQuickly;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    public void EnableMove(bool value)
    {
        if (value)
        {
            _playerMovement.MovementCoroutine = StartCoroutine(_playerMovement.MovementUpdate());
        }
        else
        {
            _playerMovement.StopCoroutine(_playerMovement.MovementCoroutine);
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
    }

    public void EnableLook(bool value)
    {
        _playerLook.EnableRot = value;

        if (!value)
        {
            _playerLook.XAngleSaved = _playerLook.XAngle;
            _playerLook.YAngleSaved = _playerLook.YAngle;
        }
        else
        {
            _playerLook.XAngle = _playerLook.XAngleSaved;
            _playerLook.YAngle = _playerLook.YAngleSaved;
        }
    }

    public void SwitchLook()
    {
        _cams[CurrentCam].enabled = false;
        CurrentCam += 1;
        if (CurrentCam >= _cams.Length) CurrentCam = 0;
        _cams[CurrentCam].enabled = true;

        if (CurrentCam == 0) _crosshair.SetActive(true);
        else _crosshair.SetActive(false);
    }

    #region Sitting
    [SerializeField]
    private GameObject _sitButton, _standUpButton;

    public void ProceedSitting()
    {
        if (Physics.Raycast(_camRaycastPoint.position, _camRaycastPoint.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.CompareTag("prop"))
            {
                Transform obj = hit.transform;
                while (obj.parent != null) obj = obj.parent;

                if (obj.GetComponentInChildren<ChairProp>() != null)
                {
                    if (!(_sitting && !_canChangeSeatQuickly))
                    {
                        ChairProp chair = obj.GetComponentInChildren<ChairProp>();
                        Sit(chair);
                        return;
                    }
                }
            }
        }

        if (_sitting) StandUp();
    }

    public void Sit(ChairProp chair)
    {
        _playerMovement.ObjectToFollow = chair.SitPoint;
        _playerMovement.FollowingObjectCoroutine = StartCoroutine(_playerMovement.FollowObjectUpdate());
        _anim.SetBool("sit", true);
        _col.enabled = false;
        _rb.isKinematic = true;
        EnableMove(false);
        _sitButton.SetActive(false);
        _standUpButton.SetActive(true);
        _movementButtons.SetActive(false);
        _canChangeSeatQuickly = chair.CanChangeSeatQuickly;

        _playerLook.YAngle = 0;
        _playerLook.XAngle = transform.rotation.y;

        _playerLook.WholeBody = false;

        _sitting = true;
    }

    private void StandUp()
    {
        _playerMovement.ObjectToFollow = null;
        _anim.SetBool("sit", false);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.position += Vector3.up * 2;
        _col.enabled = true;
        _rb.isKinematic = false;
        EnableMove(true);
        _sitButton.SetActive(true);
        _standUpButton.SetActive(false);
        _movementButtons.SetActive(true);
        _playerLook.WholeBody = true;

        _sitting = false;
    }
    #endregion Sitting

    public void LockCursor(bool value)
    {
        if (CurrentGameMode == 2)
        {
            if (value) Cursor.lockState = CursorLockMode.Locked;
            else Cursor.lockState = CursorLockMode.None;
        }
    }
}
