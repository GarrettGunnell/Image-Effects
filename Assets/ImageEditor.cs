using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    public Shader effectShader;
    public Shader blendModesShader;

    [Range(0, 5)]
    public float brightness = 1;

    [Range(-10, 10)]
    public float contrast = 0;

    [Range(0, 5)]
    public float saturation = 1;

    public enum BlendMode {
        None = 0,
        Multiply,
        Screen,
        SoftLight,
        ColorBurn,
        ColorDodge
    } public BlendMode blendMode;

    [Range(0, 1)]
    public float blendStrength = 1;

    private Material effects;
    private Material blendModes;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (effects == null) {
            effects = new Material(effectShader);
            effects.hideFlags = HideFlags.HideAndDontSave;
        }

        if (blendModes == null) {
            blendModes = new Material(blendModesShader);
            blendModes.hideFlags = HideFlags.HideAndDontSave;
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

        blendModes.SetFloat("_BlendStrength", blendStrength);
        Graphics.Blit(currentDestination, destination, blendModes, (int)blendMode);
        RenderTexture.ReleaseTemporary(currentDestination);
    }
}