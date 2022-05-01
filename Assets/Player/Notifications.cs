using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notifications : MonoBehaviour
{
    [SerializeField]
    private Animation _small, _toolgun, _online;

    public void PushSmallNotification(string notification)
    {
        _small.GetComponentInChildren<Text>().text = notification;
        _small.Stop();
        _small.Play();
    }

    public void PushToolgunNotification(string notification)
    {
        _toolgun.GetComponentInChildren<Text>().text = notification;
        _toolgun.Stop();
        _toolgun.Play();
    }

    public void PushOnlineNotification(string notification)
    {
        _online.GetComponentInChildren<Text>().text = notification;
        _online.Stop();
        _online.Play();
    }
}
