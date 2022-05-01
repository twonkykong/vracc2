using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraLook : MonoBehaviour
{
    public float SlerpTime = 1;

    [SerializeField] private float _sensitivity = 10, _rightFingerIndex;
    private float _middleScreenPoint, xAngleDifference;

    private Vector3 _firstPoint, _secondPoint;

    public float XAngle, YAngle, XAngleSaved, YAngleSaved;
    private float _xAngleTemp, _yAngleTemp;

    private LayerMask _bodylayermask;

    [SerializeField] private Transform _cam, _camHolder;

    public PlayerManager PlayerManager;

    private void Awake()
    {
        _middleScreenPoint = Screen.width / 2;
        _sensitivity = PlayerPrefs.GetFloat("sensitivity", 6) / 2;

        _bodylayermask = LayerMask.GetMask("body");
        _bodylayermask = ~_bodylayermask;
    }

    public void SetCameraSettings(PlayerLook originCamera)
    {
        _firstPoint = originCamera.FirstPoint;
        xAngleDifference = XAngle = originCamera.XAngle;
        YAngle = originCamera.YAngle;
    }

    private void Update()
    {
        if (PlayerManager.CurrentGameMode == 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.x > _middleScreenPoint && _rightFingerIndex == -1)
                    {
                        _rightFingerIndex = touch.fingerId;
                        _firstPoint = touch.position;
                        _xAngleTemp = XAngle;
                        _yAngleTemp = YAngle;
                    }
                }

                if (touch.fingerId == _rightFingerIndex)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        _secondPoint = touch.position;
                        XAngle = _xAngleTemp + (_secondPoint.x - _firstPoint.x) * 180 * _sensitivity / Screen.width;
                        YAngle = _yAngleTemp + (_secondPoint.y - _firstPoint.y) * 90 * _sensitivity / Screen.height;
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        _rightFingerIndex = -1;
                    }
                }
            }
        }

        else if (PlayerManager.CurrentGameMode == 1)
        {
            XAngle += Input.GetAxisRaw("RightXAxis") * _sensitivity * 2f;
            YAngle += Input.GetAxisRaw("RightYAxis") * _sensitivity * 2f;
        }

        else if (PlayerManager.CurrentGameMode == 2)
        {
            XAngle += Input.GetAxisRaw("Mouse X") * _sensitivity;
            YAngle += Input.GetAxisRaw("Mouse Y") * _sensitivity;
        }

        YAngle = Mathf.Clamp(YAngle, -90f, 90f);
        if (XAngle >= 360) XAngle -= 359;
        else if (XAngle < 0) XAngle += 359;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle, XAngle - xAngleDifference, 0.0f), SlerpTime);

        _cam.position = Vector3.zero;
        if (Physics.Linecast(transform.position, _camHolder.position, out RaycastHit hit))
        {
            _cam.position = hit.point;
        }
        else _cam.position = _camHolder.position;
    }
}
