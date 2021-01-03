using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    public Shader effectShader;
    public Shader blendModesShader;
    public ComputeShader noiseGenerator;

    [Range(0, 5)]
    public float brightness = 1;

    [Range(-10, 10)]
    public float contrast = 0;

    [Range(0, 5)]
    public float saturation = 1;

    [Range(0, 1)]
    public float grain = 0;

    [Range(0.01f, 1.0f)]
    public float grainResolution = 1;

    public Texture blendTexture;

    public enum BlendMode {
        None = 0,
        Darken,
        Lighten,
        Multiply,
        Screen,
        SoftLight,
        ColorBurn,
        ColorDodge,
        LinearBurn,
        LinearDodge
    } public BlendMode blendMode;

    [Range(0, 1)]
    public float blendStrength = 1;

    private Material effects;
    private Material blendModes;
    private RenderTexture noise;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (effects == null) {
            effects = new Material(effectShader);
            effects.hideFlags = HideFlags.HideAndDontSave;
        }

        if (blendModes == null) {
            blendModes = new Material(blendModesShader);
            blendModes.hideFlags = HideFlags.HideAndDontSave;
        }

        if (noise == null) {
            noise = new RenderTexture(source.width, source.height, 0, source.format, RenderTextureReadWrite.Linear);
            noise.enableRandomWrite = true;
            noise.Create();
        }
        
        RenderTexture currentDestination = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.Blit(image, currentDestination);
        RenderTexture currentSource = currentDestination;

        effects.SetFloat("_Brightness", brightness);
        effects.SetFloat("_Contrast", contrast);
        effects.SetFloat("_Saturation", saturation);

        for (int i = 0; i < 3; ++i) {
            currentDestination = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

            Graphics.Blit(currentSource, currentDestination, effects, i);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }

        blendModes.SetTexture("_BlendTex", (blendTexture == null) ? currentDestination : blendTexture);
        blendModes.SetFloat("_BlendStrength", blendStrength);
        currentDestination = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        Graphics.Blit(currentSource, currentDestination, blendModes, (int)blendMode);
        RenderTexture.ReleaseTemporary(currentSource);
        currentSource = currentDestination;

        if (grain > 0) {
            noiseGenerator.SetTexture(0, "Result", noise);
            noiseGenerator.SetFloat("_Seed", Random.Range(2, 1000));
            noiseGenerator.Dispatch(0, Mathf.CeilToInt(noise.width / 8.0f) + 1, Mathf.CeilToInt(noise.height / 8.0f) + 1, 1);

            int width = Mathf.CeilToInt(source.width * grainResolution);
            int height = Mathf.CeilToInt(source.height * grainResolution);
            RenderTexture grainTex = RenderTexture.GetTemporary(width, height, 0, source.format);
            Graphics.Blit(noise, grainTex);

            effects.SetTexture("_GrainTex", grainTex);
            effects.SetFloat("_Grain", grain);

            currentDestination = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            Graphics.Blit(currentSource, currentDestination, effects, 3);
            RenderTexture.ReleaseTemporary(currentSource);
            RenderTexture.ReleaseTemporary(grainTex);
        }

        Graphics.Blit(currentDestination, destination);
        RenderTexture.ReleaseTemporary(currentDestination);
    }
}