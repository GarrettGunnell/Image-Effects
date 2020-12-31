using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    public Shader effectShader;

    [Range(0, 5)]
    public float brightness = 1;

    [Range(-10, 10)]
    public float contrast = 0;

    [Range(0, 5)]
    public float saturation = 1;

    private Material effects;

    const int brightnessPass = 0;
    const int contrastPass = 1;
    const int saturationPass = 2;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (effects == null) {
            effects = new Material(effectShader);
            effects.hideFlags = HideFlags.HideAndDontSave;
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

        Graphics.Blit(currentDestination, destination);
        RenderTexture.ReleaseTemporary(currentDestination);
    }
}