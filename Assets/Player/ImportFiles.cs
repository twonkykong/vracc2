using System;
using UnityEngine;
using Photon.Pun;

public class ImportFiles : MonoBehaviourPun
{
    [SerializeField]
    private PreloadTexturePreview _texturesPreview;
    [SerializeField]
    private PreloadEmotes _emotesPreviews;

    private ImportManager _importManager;

    private void Start()
    {
        if (!this.photonView.IsMine) return;
        _importManager = FindObjectOfType<ImportManager>();
        _importManager.ImportFiles = this;

        _texturesPreview = GameObject.Find("preload manager").GetComponent<PreloadTexturePreview>();
        _emotesPreviews = GameObject.Find("preload manager").GetComponent<PreloadEmotes>();
    }

    public void ChooseTexture()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, -1, false);

                string name = _emotesPreviews.EmotesArray.Count.ToString() + "_" + DateTime.Now.ToString();
                _importManager.photonView.RPC("ImportTexture", RpcTarget.AllBufferedViaServer, ImageConversion.EncodeToPNG(texture), texture.width, texture.height, name);
            }
        });
    }

    public void ChooseEmote()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path, -1, false);
                Texture newTex = ModifyTexture.ResizeTexture(texture, 120, 120);
                Texture2D finalTex = ModifyTexture.ConvertTextureToTexture2D(newTex);
                string name = _texturesPreview.TexturesArray.Count.ToString() + "_" + DateTime.Now.ToString();
                _importManager.photonView.RPC("ImportEmote", RpcTarget.AllBufferedViaServer, ImageConversion.EncodeToPNG(finalTex), finalTex.width, finalTex.height, name);
            }
        });
    }

    public void ImportTexture(Texture tex, string name)
    {
        _texturesPreview.AddTexture(tex, true, name);
    }

    public void ImportEmote(Texture tex, string name)
    {
        _emotesPreviews.AddEmote(tex, true, name);
    }
}
