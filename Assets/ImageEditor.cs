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

        RenderTexture brightnessOutput = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        effects.SetFloat("_Brightness", brightness);
        Graphics.Blit(image, brightnessOutput, effects, brightnessPass);

        RenderTexture contrastOutput = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        effects.SetFloat("_Contrast", contrast);

        Graphics.Blit(brightnessOutput, contrastOutput, effects, contrastPass);

        RenderTexture saturationOutput = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        effects.SetFloat("_Saturation", saturation);
        Graphics.Blit(contrastOutput, saturationOutput, effects, saturationPass);

        Graphics.Blit(saturationOutput, destination);
        RenderTexture.ReleaseTemporary(saturationOutput);
        RenderTexture.ReleaseTemporary(brightnessOutput);
        RenderTexture.ReleaseTemporary(contrastOutput);
    }
}