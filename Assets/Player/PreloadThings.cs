using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PreloadThings : MonoBehaviourPun
{
    [SerializeField]
    private GameObject _textureButtonsParent, _materialButtonsParent, _emotesButtonsParent;

    [SerializeField]
    private SelectTexture _selectTexture;

    [SerializeField]
    private SetMaterial _setMaterial;

    [SerializeField]
    private Emotions _emotions;

    private void Start()
    {
        if (!this.photonView.IsMine) return;
        GameObject preloadManager = GameObject.Find("preload manager");
        preloadManager.GetComponent<PreloadTexturePreview>().Buttons = _textureButtonsParent;
        preloadManager.GetComponent<PreloadTexturePreview>().SelectTexture = _selectTexture;
        preloadManager.GetComponent<PreloadTexturePreview>().PreloadStart();

        preloadManager.GetComponent<PreloadMaterialsPreview>().Buttons = _materialButtonsParent;
        preloadManager.GetComponent<PreloadMaterialsPreview>().SetMaterial = _setMaterial;
        preloadManager.GetComponent<PreloadMaterialsPreview>().PreloadStart();

        preloadManager.GetComponent<PreloadEmotes>().Buttons = _emotesButtonsParent;
        preloadManager.GetComponent<PreloadEmotes>().Emotions = _emotions;
        preloadManager.GetComponent<PreloadEmotes>().PreloadStart();
    }
}
