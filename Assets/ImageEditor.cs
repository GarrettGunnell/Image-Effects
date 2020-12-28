using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    
    public Shader effectShader;

    [Range(0, 5)]
    public float saturation = 1;

    private Material effects;

    const int saturationPass = 0;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (effects == null) {
            effects = new Material(effectShader);
            effects.hideFlags = HideFlags.HideAndDontSave;
        }

        RenderTexture saturationOutput = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        effects.SetFloat("_Saturation", saturation);
        Graphics.Blit(image, saturationOutput, effects, saturationPass);

        Graphics.Blit(saturationOutput, destination);
        RenderTexture.ReleaseTemporary(saturationOutput);
    }
}