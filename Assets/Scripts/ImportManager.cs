using System;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class ImportManager : MonoBehaviourPun
{
    public ImportFiles ImportFiles;

    [PunRPC]
    public void ImportTexture(byte[] bytes, int width, int height, string name)
    {
        StartCoroutine(ImportTextureCoroutine(bytes, width, height, name));
    }

    private IEnumerator ImportTextureCoroutine(byte[] bytes, int width, int height, string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D(width, height);
        ImageConversion.LoadImage(tex, bytes);
        ImportFiles.ImportTexture(tex, name);
    }

    [PunRPC]
    public void ImportEmote(byte[] bytes, int width, int height, string name)
    {
        StartCoroutine(ImportEmoteCoroutine(bytes, width, height, name));
    }

    private IEnumerator ImportEmoteCoroutine(byte[] bytes, int width, int height, string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D(width, height);
        ImageConversion.LoadImage(tex, bytes);
        ImportFiles.ImportEmote(tex, name);
    }
}
