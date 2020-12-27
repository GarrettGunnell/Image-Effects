using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    
    public Shader contrastShader;

    [Range(0, 10)]
    public int contrast = 1;

    private Material contrastMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (contrastMaterial == null) {
            contrastMaterial = new Material(contrastShader);
            contrastMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        Graphics.Blit(image, destination, contrastMaterial);
    }
}