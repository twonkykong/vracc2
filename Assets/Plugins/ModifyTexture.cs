using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyTexture : MonoBehaviour
{
    public static Texture2D ConvertTextureToTexture2D(Texture originTex)
    {
        Texture mainTexture = originTex;
        Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 32);
        Graphics.Blit(mainTexture, renderTexture);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        Color[] pixels = texture2D.GetPixels();
        RenderTexture.active = currentRT;

        return texture2D;
    }

    public static Texture ResizeTexture(Texture originTex, int width, int height)
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;
        Graphics.Blit(originTex, rt);
        Texture2D newTex = new Texture2D(width, height);
        newTex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        newTex.Apply();

        return newTex;
    }
}
