using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    
    public Shader effectShader;

    [Range(0, 2)]
    public float saturation = 1;

    private Material effectMaterial;

    const int saturationPass = 0;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (effectMaterial == null) {
            effectMaterial = new Material(effectShader);
            effectMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        effectMaterial.SetFloat("_Saturation", saturation);
        Graphics.Blit(image, destination, effectMaterial, saturationPass);
    }
}