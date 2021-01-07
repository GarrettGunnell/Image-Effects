using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    public Shader effectShader, blendModesShader, filterShader;
    public ComputeShader noiseGenerator;

    [Range(1, 5)]
    public float gamma = 1;

    [Range(0, 5)]
    public float brightness = 1;

    [Range(-10, 10)]
    public float contrast = 0;

    [Range(0, 5)]
    public float saturation = 1;

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

    public enum Filter {
        None = 0,
        Sepia,
        Filter1
    } public Filter filter;

    [Range(0, 1)]
    public float grain = 0;

    [Range(0.01f, 1.0f)]
    public float grainResolution = 1;

    private Material effects, blendModes, filters;
    private RenderTexture noise;
    private RenderTexture output;
    private Camera cam;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        InitMaterials();

        if (noise == null) {
            noise = new RenderTexture(source.width, source.height, 0, source.format, RenderTextureReadWrite.Linear);
            noise.enableRandomWrite = true;
            noise.Create();
        }
        
        if (output == null) {
            output = new RenderTexture(image.width, image.height, 0, source.format, RenderTextureReadWrite.Linear);
            output.Create();
        }

        RenderTexture currentDestination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(image, currentDestination);
        RenderTexture currentSource = currentDestination;

        effects.SetFloat("_Gamma", gamma);
        effects.SetFloat("_Brightness", brightness);
        effects.SetFloat("_Contrast", contrast);
        effects.SetFloat("_Saturation", saturation);

        for (int pass = 0; pass < 4; ++pass) {
            currentDestination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
            Graphics.Blit(currentSource, currentDestination, effects, pass);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }

        blendModes.SetTexture("_BlendTex", (blendTexture == null) ? currentDestination : blendTexture);
        blendModes.SetFloat("_BlendStrength", blendStrength);

        currentDestination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(currentSource, currentDestination, blendModes, (int)blendMode);
        RenderTexture.ReleaseTemporary(currentSource);
        currentSource = currentDestination;

        currentDestination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(currentSource, currentDestination, filters, (int)filter);
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

            currentDestination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
            Graphics.Blit(currentSource, currentDestination, effects, 3);
            RenderTexture.ReleaseTemporary(currentSource);
            RenderTexture.ReleaseTemporary(grainTex);
        }

        Graphics.Blit(currentDestination, output);
        Graphics.Blit(currentDestination, destination);
        RenderTexture.ReleaseTemporary(currentDestination);
    }

    private void InitMaterials() {
        if (effects == null) {
            effects = new Material(effectShader);
            effects.hideFlags = HideFlags.HideAndDontSave;
        }

        if (blendModes == null) {
            blendModes = new Material(blendModesShader);
            blendModes.hideFlags = HideFlags.HideAndDontSave;
        }

        if (filters == null) {
            filters = new Material(filterShader);
            filters.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Texture2D screenshot = new Texture2D(image.width, image.height, TextureFormat.RGB24, false);
            RenderTexture.active = output;
            screenshot.ReadPixels(new Rect(0, 0, image.width, image.height), 0, 0, false);
            string fileName = string.Format("{0}/snap_{1}.png", Application.dataPath, System.DateTime.Now.ToString("HH-mm-ss"));
            System.IO.File.WriteAllBytes(fileName, screenshot.EncodeToPNG());
        }
    }
}