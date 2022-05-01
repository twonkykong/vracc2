using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class PlayerLook : MonoBehaviour
{
    #region Variables
    [SerializeField] private float _sensitivity = 10, _rightFingerIndex, _xAngleTemp, _yAngleTemp;

    [SerializeField] private Transform _body, _upperSpine, _head, _neck;
    [SerializeField] private Transform[] _cams, _camHolders;

    public Vector3 FirstPoint, SecondPoint;

    public float XAngle, YAngle, XAngleSaved, YAngleSaved, SlerpTime = 1;
    public bool EnableRot = true, WholeBody = true, ClampRot = true;

    private PlayerManager _playerManager;
    private LayerMask _bodylayermask;

    private float _middleScreenPoint;
    #endregion Variables

    private void Awake()
    {
        _middleScreenPoint = Screen.width / 2;
        _sensitivity = PlayerPrefs.GetFloat("sensitivity", 6) / 2;

        _bodylayermask = LayerMask.GetMask("body");
        _bodylayermask = ~_bodylayermask;
    }

    private void Start()
    {
        XAngle = 0;
        YAngle = 0;
        transform.rotation = Quaternion.Euler(YAngle, XAngle, 0);
        _body = transform.parent;

        _playerManager = GetComponentInParent<PlayerManager>();
    }

    private void Update()
    {
        if (_playerManager.CurrentGameMode == 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.x > _middleScreenPoint && _rightFingerIndex == -1)
                    {
                        _rightFingerIndex = touch.fingerId;
                        FirstPoint = touch.position;
                        _xAngleTemp = XAngle;
                        _yAngleTemp = YAngle;
                    }
                }

                if (touch.fingerId == _rightFingerIndex)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        SecondPoint = touch.position;
                        XAngle = _xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 * _sensitivity / Screen.width;
                        YAngle = _yAngleTemp + (SecondPoint.y - FirstPoint.y) * 90 * _sensitivity / Screen.height;
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        _rightFingerIndex = -1;
                    }
                }
            }
        }
        
        else if (_playerManager.CurrentGameMode == 1)
        {
            XAngle += Input.GetAxisRaw("RightXAxis") * _sensitivity * 2f;
            YAngle += Input.GetAxisRaw("RightYAxis") * _sensitivity * 2f;
        }

        else if (_playerManager.CurrentGameMode == 2)
        {
            XAngle += Input.GetAxisRaw("Mouse X") * _sensitivity;
            YAngle += Input.GetAxisRaw("Mouse Y") * _sensitivity;
        }

        if (ClampRot)
        {
            YAngle = Mathf.Clamp(YAngle, -90f, 90f);
            if (XAngle >= 360) XAngle -= 359;
            else if (XAngle < 0) XAngle += 359;
        }

        #region CameraRotation
        if (EnableRot)
        {
            if (WholeBody)
            {
                //X 90 = 20 + 50 + 20
                //Y 0
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle, 0, 0.0f), SlerpTime);

                //X 20 Y 0
                _neck.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle / 9 * 2, 0, 0.0f), SlerpTime);

                //X 50 Y 0
                _head.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle / 9 * 5, 0, 0.0f), SlerpTime);

                //X 20 Y 0
                _upperSpine.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle / 9 * 2, 0, 0.0f), SlerpTime);
                _body.transform.rotation = Quaternion.Slerp(_body.transform.rotation, Quaternion.Euler(0.0f, XAngle, 0.0f), SlerpTime);
            }
            else
            {
                //X 90 = 20 + 50 + 20
                //Y 90 = 0 + 60 + 30
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle, XAngle, 0.0f), SlerpTime);

                float xValue = XAngle;
                if (xValue > 180) xValue -= 360;
                xValue = Mathf.Clamp(xValue, -90, 90);
                //X 20 Y 0 
                _neck.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle / 9 * 2, 0, 0.0f), SlerpTime);

                //X 50 Y 60
                _head.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle / 9 * 5, xValue / 3 * 2, 0.0f), SlerpTime);

                //X 20 Y 30
                _upperSpine.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(-YAngle / 9 * 2, xValue / 3, 0.0f), SlerpTime);
            }
        }
        #endregion CameraRotation

        #region 3rdCamPosition
        _cams[0].position = Vector3.zero;
        if (Physics.Linecast(transform.position, _camHolders[0].position, out RaycastHit hit))
        {
            _cams[0].position = hit.point;
        }
        else _cams[0].position = _camHolders[0].position;
        #endregion 3rdCamPosition
    }
}
