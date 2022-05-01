using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SetMaterial : MonoBehaviourPun
{
    [SerializeField]
    private Material _material;

    [SerializeField]
    private SelectTexture _selectTexture;

    [SerializeField]
    private OnlineController _onlineController;
    [SerializeField]
    private Notifications _notifications;

    [SerializeField]
    private Boolean _paintFullModel;

    private float _offsetX, _offsetY, _tilingX = 1, _tilingY = 1, _specularIntensity = 0.5f, _glossiness = 0.5f, _normalIntensity = 1, _transparency;

    [SerializeField]
    private Text _specularText, _glossinessText, _normalText, _transparencyText,
        _offsetXText, _offsetYText,
        _tilingXText, _tilingYText,
        _r1Text, _g1Text, _b1Text, _a1Text,
        _r2Text, _g2Text, _b2Text, _a2Text;

    [SerializeField]
    private RawImage _mainTexturePreview, _normalMapPreview, _specularMapPreview, _difuseColorPreview, _specularColorPreview, _preview;

    [SerializeField]
    private Texture _mainTexture, _normalMap, _specularMap;

    [SerializeField]
    private Slider _r1, _g1, _b1, _a1, _r2, _g2, _b2, _a2;

    //0 - main, 1 - normal, 2 - specular
    public int TextureSelectType;

    public Material[] ShaderBases;

    public string CurrentShader;

    public void UpdatePreview()
    {
        Material mat = new Material(Array.Find(ShaderBases, t => t.name == CurrentShader));

        if (_mainTexture != null)
        {
            mat.SetTexture("_MainTex", _mainTexture);
            mat.SetTextureOffset("_MainTex", new Vector2(_offsetX, _offsetY));
            mat.SetTextureScale("_MainTex", new Vector2(_tilingX, _tilingY));

        }

        if (_normalMap != null)
        {
            mat.SetTexture("_BumpMap", _normalMap);
            mat.SetTextureOffset("_BumpMap", new Vector2(_offsetX, _offsetY));
            mat.SetTextureScale("_BumpMap", new Vector2(_tilingX, _tilingY));

        }

        if (_specularMap != null)
        {
            mat.SetTexture("_SpecGlossMap", _specularMap);
            mat.SetTextureOffset("_SpecGlossMap", new Vector2(_offsetX, _offsetY));
            mat.SetTextureScale("_SpecGlossMap", new Vector2(_tilingX, _tilingY));
        }

        mat.SetFloat("_SpecularIntensity", _specularIntensity);
        mat.SetFloat("_Glossiness", _glossiness);
        mat.SetFloat("_NormalIntensity", _normalIntensity);
        mat.SetFloat("_Transparency", _transparency);

        mat.SetColor("_Color", new Color(_r1.value / 255, _g1.value / 255, _b1.value / 255, _a1.value / 255));
        mat.SetColor("_SpecColor", new Color(_r2.value / 255, _g2.value / 255, _b2.value / 255, _a2.value / 255));

        _preview.texture = RuntimePreviewGenerator.GenerateMaterialPreview(mat, PrimitiveType.Cube, 256, 256);
    }

    public void UpdateDifuseColorPreview()
    {
        _difuseColorPreview.color = new Color(_r1.value / 255, _g1.value / 255, _b1.value / 255, _a1.value / 255);
    }

    public void UpdateSpecularColorPreview()
    {
        _specularColorPreview.color = new Color(_r2.value / 255, _g2.value / 255, _b2.value / 255, _a2.value / 255);
    }

    public void ChangeTextureSelectType(int value)
    {
        TextureSelectType = value;
    }

    public void ChangeTexture()
    {
        if (TextureSelectType == 0)
        {
            _mainTexturePreview.texture = _mainTexture = _selectTexture.Texture;
        }
        else if (TextureSelectType == 1)
        {
            _normalMapPreview.texture = _normalMap = _selectTexture.Texture;
        }
        else if (TextureSelectType == 2)
        {
            _specularMapPreview.texture = _specularMap = _selectTexture.Texture;
        }
    }

    public void SelectMaterial(Material mat)
    {
        _material = mat;

        _offsetX = mat.GetTextureOffset("_MainTex").x;
        _offsetY = mat.GetTextureOffset("_MainTex").y;
        _tilingX = mat.GetTextureScale("_MainTex").x;
        _tilingY = mat.GetTextureScale("_MainTex").y;

        _offsetXText.text = _offsetX.ToString();
        _offsetYText.text = _offsetY.ToString();
        _tilingXText.text = _tilingX.ToString();
        _tilingYText.text = _tilingY.ToString();

        SetSpecularIntensity(mat.GetFloat("_SpecularIntensity"));
        SetGlossiness(mat.GetFloat("_Glossiness"));
        SetNormalIntensity(mat.GetFloat("_NormalIntensity"));
        SetTransparency(mat.GetFloat("_Transparency"));

        _mainTexturePreview.texture = _mainTexture = mat.GetTexture("_MainTex");
        _normalMapPreview.texture = _normalMap = mat.GetTexture("_BumpMap");
        _specularMapPreview.texture = _specularMap = mat.GetTexture("_SpecGlossMap");

        _r1.value = mat.GetColor("_Color").r * 255;
        _g1.value = mat.GetColor("_Color").g * 255;
        _b1.value = mat.GetColor("_Color").b * 255;
        _a1.value = mat.GetColor("_Color").a * 255;

        _r2.value = mat.GetColor("_SpecColor").r * 255;
        _g2.value = mat.GetColor("_SpecColor").g  *255;
        _b2.value = mat.GetColor("_SpecColor").b * 255;
        _a2.value = mat.GetColor("_SpecColor").a * 255;

        _r1Text.text = _r1.value.ToString();
        _g1Text.text = _g1.value.ToString();
        _b1Text.text = _b1.value.ToString();

        _r2Text.text = _r2.value.ToString();
        _g2Text.text = _g2.value.ToString();
        _b2Text.text = _b2.value.ToString();
    }

    public void SetOffsetX(String value)
    {
        _offsetX = Convert.ToSingle(value);
    }

    public void SetOffsetY(String value)
    {
        _offsetY = Convert.ToSingle(value);
    }

    public void SetTilingX(String value)
    {
        _tilingX = Convert.ToSingle(value);
    }

    public void SetTilingY(String value)
    {
        _tilingY = Convert.ToSingle(value);
    }

    public void SetSpecularIntensity(Single value)
    {
        _specularIntensity = value;
        _specularText.text = value.ToString();
    }

    public void SetGlossiness(Single value)
    {
        _glossiness = value;
        _glossinessText.text = value.ToString();
    }

    public void ChangeShaderBase(string name)
    {
        CurrentShader = name;
    }

    public void SetNormalIntensity(Single value)
    {
        _normalIntensity = value;
        _normalText.text = value.ToString();
    }

    public void SetTransparency(Single value)
    {
        _transparency = value;
        _transparencyText.text = value.ToString();
    }

    public void SwitchMode(Boolean value)
    {
        _paintFullModel = value;
    }

    public void ChangeR1Value(Single value)
    {
        _r1Text.text = value.ToString();
    }
    public void ChangeG1Value(Single value)
    {
        _g1Text.text = value.ToString();
    }
    public void ChangeB1Value(Single value)
    {
        _b1Text.text = value.ToString();
    }
    public void ChangeA1Value(Single value)
    {
        _a1Text.text = value.ToString();
    }

    public void ChangeR2Value(Single value)
    {
        _r2Text.text = value.ToString();
    }
    public void ChangeG2Value(Single value)
    {
        _g2Text.text = value.ToString();
    }
    public void ChangeB2Value(Single value)
    {
        _b2Text.text = value.ToString();
    }
    public void ChangeA2Value(Single value)
    {
        _a2Text.text = value.ToString();
    }

    public void ChangeMaterial()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (_paintFullModel)
            {
                if (hit.collider.TryGetComponent(out Prop prop))
                {
                    foreach (Renderer renderer in prop.GetComponentsInChildren<Renderer>())
                    {
                        string mainName = "no-texture";
                        if (_mainTexture != null) mainName = _mainTexture.name;
                        
                        string normalName = "no-texture";
                        if (_normalMap != null) normalName = _normalMap.name;

                        string specularName = "no-texture";
                        if (_specularMap != null) specularName = _specularMap.name;

                        _onlineController.photonView.RPC("SetMaterial", RpcTarget.AllBufferedViaServer, renderer.gameObject.GetPhotonView().ViewID,
                                CurrentShader,
                                mainName, normalName, specularName,
                                _offsetX, _offsetY,
                                _tilingX, _tilingY,
                                _specularIntensity, _glossiness, _normalIntensity, _transparency,
                                _r1.value / 255, _g1.value / 255, _b1.value / 255, _a1.value / 255,
                                _r2.value / 255, _g2.value / 255, _b2.value / 255, _a2.value / 255);
                    }
                }
            }
            else
            {
                if (hit.collider.TryGetComponent(out Renderer renderer))
                {
                    string mainName = "no-texture";
                    if (_mainTexture != null) mainName = _mainTexture.name;

                    string normalName = "no-texture";
                    if (_normalMap != null) normalName = _normalMap.name;

                    string specularName = "no-texture";
                    if (_specularMap != null) specularName = _specularMap.name;

                    _onlineController.photonView.RPC("SetMaterial", RpcTarget.AllBufferedViaServer, renderer.gameObject.GetPhotonView().ViewID, 
                            CurrentShader,
                            mainName, normalName, specularName,
                            _offsetX, _offsetY,
                            _tilingX, _tilingY,
                            _specularIntensity, _glossiness, _normalIntensity, _transparency,
                            _r1.value/255, _g1.value/255, _b1.value/255, _a1.value/255,
                            _r2.value/255, _g2.value/255, _b2.value/255, _a2.value/255);
                }
            }
        }
    }

    public void CopyMaterial()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            if (hit.collider.TryGetComponent(out Renderer renderer))
            {
                SelectMaterial(renderer.material);
                _notifications.PushToolgunNotification("material copied");
            }
        }
    }
}
