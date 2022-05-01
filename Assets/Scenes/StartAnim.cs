using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAnim : MonoBehaviour
{
    [SerializeField]
    private Animation _anim;
    private void Start()
    {
        StartCoroutine(AnimCoroutine());
    }

    IEnumerator AnimCoroutine()
    {
        yield return new WaitUntil(() => _anim.isPlaying == false);
        SceneManager.LoadScene("Menu");
    }
}
