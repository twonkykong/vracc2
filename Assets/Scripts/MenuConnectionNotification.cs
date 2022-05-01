using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuConnectionNotification : MonoBehaviour
{
    private Animation _anim;
    private Text _text;

    private void Awake()
    {
        _anim = GetComponent<Animation>();
        _text = GetComponent<Text>();
    }

    public void ShowNotification(string notificationText)
    {
        _text.text = notificationText;
        _anim.Stop();
        _anim.Play();
    }
}
