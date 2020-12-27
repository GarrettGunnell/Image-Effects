using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    
    public Shader contrastShader;
    
    [Range(0, 10)]
    public int contrast = 1;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {

        Graphics.Blit(image, destination);
    }
}