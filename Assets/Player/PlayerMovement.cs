using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSrc;
    [SerializeField] private AudioClip _walkClip, _jumpClip, _landClip;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _joystick, _jumpRayPoint;
    [SerializeField] private float _speed, _jumpForce;

    public Coroutine MovementCoroutine, FollowingObjectCoroutine;
    public Transform ObjectToFollow;

    private PlayerManager _playerManager;
    private Rigidbody _rb;
    private float _joystickSize;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        _joystickSize = _joystick.GetComponent<RectTransform>().sizeDelta.x;
        MovementCoroutine = StartCoroutine(MovementUpdate());
    }

    public IEnumerator MovementUpdate()
    {
        while (true)
        {
            Vector3 moveAxis = _joystick.localPosition / _joystickSize;

            if (_playerManager.CurrentGameMode != 0)
            {
                if (_playerManager.CurrentGameMode == 1)
                {
                    moveAxis = new Vector2(Input.GetAxisRaw("LeftXAxis"), Input.GetAxisRaw("LeftYAxis"));
                    if (Input.GetKey("joystick 1 button 1")) Jump();
                }
                else if (_playerManager.CurrentGameMode == 2)
                {
                    moveAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                    if (Input.GetKey("space")) Jump();
                }
            }

            Vector3 pos = transform.forward * (moveAxis.y * _speed) + transform.right * (moveAxis.x * _speed);
            Vector3 newPos = new Vector3(pos.x, _rb.velocity.y, pos.z);
            _rb.velocity = newPos;

            if (Vector3.Distance(moveAxis, Vector3.zero) != 0) _anim.SetBool("walk", true);
            else _anim.SetBool("walk", false);

            _anim.SetFloat("speed", Vector3.Distance(moveAxis, Vector3.zero));
            if (_joystick.localPosition.y < 0) _anim.SetFloat("speed", -Mathf.Abs(_anim.GetFloat("speed")));

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FollowObjectUpdate()
    {
        while (true)
        {
            try
            {
                transform.position = ObjectToFollow.position;
                transform.rotation = ObjectToFollow.rotation;
            }
            catch
            {
                StopCoroutine(FollowingObjectCoroutine);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void Jump()
    {
        if (Physics.Raycast(_jumpRayPoint.position, -_jumpRayPoint.up, 0.15f))
        {
            _audioSrc.PlayOneShot(_jumpClip);
            _rb.AddForce(_jumpRayPoint.up * _jumpForce, ForceMode.Impulse);
        }
    }

    public void StepSound()
    {
        _audioSrc.PlayOneShot(_walkClip);
    }
}
