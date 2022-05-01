using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSrc;

    [SerializeField]
    private AudioClip _walkClip;

    [SerializeField]
    private Transform _footRay;

    private void StepSound()
    {
        if (Physics.Raycast(_footRay.position, -_footRay.up, 0.1f)) _audioSrc.PlayOneShot(_walkClip);
    }
}
