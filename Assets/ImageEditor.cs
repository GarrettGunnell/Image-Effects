using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    
    public Shader saturationShader;

    [Range(0, 2)]
    public float saturation = 1;

    private Material saturationMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (saturationMaterial == null) {
            saturationMaterial = new Material(saturationShader);
            saturationMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        Graphics.Blit(image, destination, saturationMaterial);
    }
}