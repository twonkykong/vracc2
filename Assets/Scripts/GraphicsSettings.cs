using System;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [SerializeField]
    private Slider _qualitySlider, _maxLodLevel;

    [SerializeField]
    private Dropdown _shadowmask, _shadows, _shadowResolution, _shadowProjection, _shadowCascades, _textureQuality, _anisotropicTextures, _antiAliasing, _skinWeights, _vSyncCount;

    [SerializeField]
    private InputField _shadowDistance, _nearPlaneOffset, _pixelLightCount, _resolutionScalingFixedDpiFactor, _lodBias, _particleRaycastBudget, _asyncUploadTimeSlice, _asyncUploadBufferTime;

    [SerializeField]
    private Toggle _softParticles, _realtimeReflectionProbes, _asyncUploadPersistentBuffer;

    [SerializeField]
    private Text _qualityValue, _maximumLodValue;

    //arrays
    private ShadowmaskMode[] shadowmaskArray;
    private ShadowQuality[] shadowsArray;
    private ShadowResolution[] shadowResolutionArray;
    private ShadowProjection[] shadowProjectionArray;
    private AnisotropicFiltering[] anisotropicTexturesArray;
    private SkinWeights[] skinWeightsArray;

    private void Start()
    {
        shadowmaskArray = new ShadowmaskMode[2] { ShadowmaskMode.Shadowmask, ShadowmaskMode.DistanceShadowmask };
        shadowsArray = new ShadowQuality[3] { ShadowQuality.Disable, ShadowQuality.HardOnly, ShadowQuality.All };
        shadowResolutionArray = new ShadowResolution[4] { ShadowResolution.Low, ShadowResolution.Medium, ShadowResolution.High, ShadowResolution.VeryHigh };
        shadowProjectionArray = new ShadowProjection[2] { ShadowProjection.StableFit, ShadowProjection.CloseFit };
        anisotropicTexturesArray = new AnisotropicFiltering[3] { AnisotropicFiltering.Disable, AnisotropicFiltering.Enable, AnisotropicFiltering.ForceEnable };
        skinWeightsArray = new SkinWeights[4] { SkinWeights.OneBone, SkinWeights.TwoBones, SkinWeights.FourBones, SkinWeights.Unlimited };

        LoadGraphicsSettings();
    }

    public void OnQualitySliderChange(Single value)
    {
        _qualityValue.text = value.ToString();
        QualitySettings.SetQualityLevel(Convert.ToInt32(value));
        SetValuesToUI();
    }

    public void OnMaximumLodChange(Single value)
    {
        _maximumLodValue.text = value.ToString();
    }

    public void OnAsyncTimeSliceChange(String value)
    {
        _asyncUploadTimeSlice.text = Mathf.Clamp(Convert.ToInt32(value), 1, 33).ToString();
    }

    public void OnAsyncBufferChange(String value)
    {
        _asyncUploadBufferTime.text = Mathf.Clamp(Convert.ToInt32(value), 2, 512).ToString();
    }

    public void SaveGraphicsSettings()
    {
        PlayerPrefs.SetInt("shadowmask", _shadowmask.value);
        PlayerPrefs.SetInt("shadows", _shadows.value);
        PlayerPrefs.SetInt("shadow resolution", _shadowResolution.value);
        PlayerPrefs.SetInt("shadow projection", _shadowProjection.value);
        PlayerPrefs.SetInt("shadow cascades", _shadowCascades.value);
        PlayerPrefs.SetFloat("shadow distance", Convert.ToSingle(_shadowDistance.text));
        PlayerPrefs.SetFloat("near plane offset", Convert.ToSingle(_nearPlaneOffset.text));
        PlayerPrefs.SetInt("texture quality", _textureQuality.value);
        PlayerPrefs.SetInt("anisotropicTextures", _anisotropicTextures.value);
        PlayerPrefs.SetInt("anti-aliasing", _antiAliasing.value);
        PlayerPrefs.SetInt("pixel light count", Convert.ToInt32(_pixelLightCount.text));
        PlayerPrefs.SetFloat("resolution scaling fixed dpi factor", Convert.ToSingle(_resolutionScalingFixedDpiFactor.text));
        PlayerPrefs.SetInt("soft particles", Convert.ToInt32(_softParticles.isOn));
        PlayerPrefs.SetInt("realtime reflection probes", Convert.ToInt32(_realtimeReflectionProbes.isOn));
        PlayerPrefs.SetInt("skin weights", _skinWeights.value);
        PlayerPrefs.SetInt("vsync count", _vSyncCount.value);
        PlayerPrefs.SetFloat("lod bias", Convert.ToSingle(_lodBias.text));
        PlayerPrefs.SetInt("maximum lod level", Convert.ToInt32(_maxLodLevel.value));
        PlayerPrefs.SetInt("particle raycast budget", Convert.ToInt32(_particleRaycastBudget.text));
        PlayerPrefs.SetInt("async upload time slice", Convert.ToInt32(_asyncUploadTimeSlice.text));
        PlayerPrefs.SetInt("async upload buffer time", Convert.ToInt32(_asyncUploadBufferTime.text));
        PlayerPrefs.SetInt("async upload persistent buffer", Convert.ToInt32(_asyncUploadPersistentBuffer.isOn));
    }

    public void LoadGraphicsSettings()
    {
        QualitySettings.shadowmaskMode = shadowmaskArray[PlayerPrefs.GetInt("shadowmask")];
        QualitySettings.shadows = shadowsArray[PlayerPrefs.GetInt("shadows")];
        QualitySettings.shadowResolution = shadowResolutionArray[PlayerPrefs.GetInt("shadow resolution")];
        QualitySettings.shadowProjection = shadowProjectionArray[PlayerPrefs.GetInt("shadow projection")];
        QualitySettings.shadowCascades = PlayerPrefs.GetInt("shadow cascades");
        QualitySettings.shadowDistance = PlayerPrefs.GetFloat("shadow distance");
        QualitySettings.shadowNearPlaneOffset = PlayerPrefs.GetFloat("near plane offset");
        QualitySettings.masterTextureLimit = PlayerPrefs.GetInt("texture quality");
        Texture.anisotropicFiltering = anisotropicTexturesArray[PlayerPrefs.GetInt("anisotropic textures")];
        QualitySettings.antiAliasing = PlayerPrefs.GetInt("anti-aliasing");
        QualitySettings.pixelLightCount = PlayerPrefs.GetInt("pixel light count");
        QualitySettings.resolutionScalingFixedDPIFactor = PlayerPrefs.GetFloat("resolution scaling fixed dpi factor");
        QualitySettings.softParticles = Convert.ToBoolean(PlayerPrefs.GetInt("soft particles"));
        QualitySettings.realtimeReflectionProbes = Convert.ToBoolean(PlayerPrefs.GetInt("realtime reflection probes"));
        QualitySettings.skinWeights = skinWeightsArray[PlayerPrefs.GetInt("skin weights")];
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("vsync count");
        QualitySettings.lodBias = PlayerPrefs.GetFloat("lod bias");
        QualitySettings.maximumLODLevel = PlayerPrefs.GetInt("maximum lod level");
        QualitySettings.particleRaycastBudget = PlayerPrefs.GetInt("particle raycast budget");
        QualitySettings.asyncUploadTimeSlice = PlayerPrefs.GetInt("async upload time slice");
        QualitySettings.asyncUploadBufferSize = PlayerPrefs.GetInt("async upload buffer size");
        QualitySettings.asyncUploadPersistentBuffer = Convert.ToBoolean(PlayerPrefs.GetInt("async upload persistent buffer"));
        SetValuesToUI();
    }

    public void SetValuesToUI()
    {
        _shadowmask.value = Array.IndexOf(shadowmaskArray, QualitySettings.shadowmaskMode);
        _shadows.value = Array.IndexOf(shadowsArray, QualitySettings.shadows);
        _shadowResolution.value = Array.IndexOf(shadowResolutionArray, QualitySettings.shadowResolution);
        _shadowProjection.value = Array.IndexOf(shadowProjectionArray, QualitySettings.shadowProjection);
        _shadowCascades.value = QualitySettings.shadowCascades / 2;
        _shadowDistance.text = QualitySettings.shadowDistance.ToString();
        _nearPlaneOffset.text = QualitySettings.shadowNearPlaneOffset.ToString();
        _textureQuality.value = QualitySettings.masterTextureLimit;
        _anisotropicTextures.value = Array.IndexOf(anisotropicTexturesArray, Texture.anisotropicFiltering);
        _antiAliasing.value = Array.IndexOf(new int[4] { 0, 2, 4, 8 }, QualitySettings.antiAliasing);
        _pixelLightCount.text = QualitySettings.pixelLightCount.ToString();
        _resolutionScalingFixedDpiFactor.text = QualitySettings.resolutionScalingFixedDPIFactor.ToString();
        _softParticles.isOn = QualitySettings.softParticles;
        _realtimeReflectionProbes.isOn = QualitySettings.realtimeReflectionProbes;
        _skinWeights.value = Array.IndexOf(skinWeightsArray, QualitySettings.skinWeights);
        _vSyncCount.value = QualitySettings.vSyncCount;
        _lodBias.text = QualitySettings.lodBias.ToString();
        _maxLodLevel.value = QualitySettings.maximumLODLevel;
        _particleRaycastBudget.text = QualitySettings.particleRaycastBudget.ToString();
        _asyncUploadTimeSlice.text = QualitySettings.asyncUploadTimeSlice.ToString();
        _asyncUploadBufferTime.text = QualitySettings.asyncUploadBufferSize.ToString();
        _asyncUploadPersistentBuffer.isOn = QualitySettings.asyncUploadPersistentBuffer;
    }
}
