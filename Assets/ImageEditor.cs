using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ImageEditor : MonoBehaviour {

    public Texture image;
    public Shader effectShader, blendModesShader, filterShader;
    public ComputeShader noiseGenerator;
    public bool showUnedited = false;

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

    [Range(0, 35)]
    public float blur = 0;

    [Range(1, 35)]
    public int blurRadius = 0;

    [Range(0, 10)]
    public float sharpness = 0;

    [Range(0, 1)]
    public float grain = 0;

    [Range(0.01f, 1.0f)]
    public float grainResolution = 1;

    public bool toonShading = false;
    public bool dithering = false;
    public bool interpolateThreshold = false;

    [Range(1, 12)]
    public int bayerLevel = 1;

    public bool invertLuminance = false;
    
    [Range(0.0f, 1.0f)]
    public float luminanceThreshold = 0.5f;

    private Material effects, blendModes, filters;
    private RenderTexture noise, output, bayerTex, bayerTexUpscale;
    private int currentBayerLevel = 1;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        InitMaterials(source);

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

        (currentDestination, currentSource) = Blend(currentSource, currentDestination);
        (currentDestination, currentSource) = FilterPass(currentSource, currentDestination);
        if (sharpness > 0)
            (currentDestination, currentSource) = Sharpness(currentSource, currentDestination);
        if (blur > 0)
            (currentDestination, currentSource) = Blur(currentSource, currentDestination);
        if (grain > 0)
            (currentDestination, currentSource) = Grain(currentSource, currentDestination);
        if (toonShading)
            (currentDestination, currentSource) = Toon(currentSource, currentDestination);
        if (dithering)
            (currentDestination, currentSource) = Dithering(currentSource, currentDestination);

        Graphics.Blit(currentDestination, output);
        Graphics.Blit(showUnedited ? image : currentDestination, destination);
        RenderTexture.ReleaseTemporary(currentDestination);
        RenderTexture.ReleaseTemporary(currentSource);
    }

    private void InitMaterials(RenderTexture source) {
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

        if (output == null) {
            output = new RenderTexture(image.width, image.height, 0, source.format, RenderTextureReadWrite.Linear);
            output.Create();
        }

        if (noise == null) {
            noise = new RenderTexture(source.width, source.height, 0, source.format, RenderTextureReadWrite.Linear);
            noise.enableRandomWrite = true;
            noise.Create();
        }
    }

    private (RenderTexture, RenderTexture) Blend(RenderTexture source, RenderTexture destination) {
        blendModes.SetTexture("_BlendTex", (blendTexture == null) ? source : blendTexture);
        blendModes.SetFloat("_BlendStrength", blendStrength);

        destination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(source, destination, blendModes, (int)blendMode);
        RenderTexture.ReleaseTemporary(source);

        return (destination, destination);
    }

    private (RenderTexture, RenderTexture) FilterPass(RenderTexture source, RenderTexture destination) {
        destination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(source, destination, filters, (int)filter);
        RenderTexture.ReleaseTemporary(source);

        return (destination, destination);
    }

    private (RenderTexture, RenderTexture) Blur(RenderTexture source, RenderTexture destination) {
        effects.SetFloat("_Blur", blur);
        effects.SetFloat("_Radius", blurRadius);
        destination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(source, destination, effects, 5);
        RenderTexture.ReleaseTemporary(source);

        return (destination, destination);
    }

    private (RenderTexture, RenderTexture) Sharpness(RenderTexture source, RenderTexture destination) {
        effects.SetFloat("_Blur", 3.0f);
        effects.SetFloat("_Radius", 3);
        RenderTexture blurred = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(source, blurred, effects, 5);

        effects.SetFloat("_Sharpness", sharpness);
        effects.SetTexture("_BlurredTex", blurred);
        destination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(source, destination, effects, 6);
        RenderTexture.ReleaseTemporary(source);
        RenderTexture.ReleaseTemporary(blurred);

        return (destination, destination);
    }

    private (RenderTexture, RenderTexture) Grain(RenderTexture source, RenderTexture destination) {
        noiseGenerator.SetTexture(0, "Result", noise);
        noiseGenerator.SetFloat("_Seed", Random.Range(2, 1000));
        noiseGenerator.Dispatch(0, Mathf.CeilToInt(noise.width / 8.0f) + 1, Mathf.CeilToInt(noise.height / 8.0f) + 1, 1);

        int width = Mathf.CeilToInt(source.width * grainResolution);
        int height = Mathf.CeilToInt(source.height * grainResolution);
        RenderTexture grainTex = RenderTexture.GetTemporary(width, height, 0, source.format);
        Graphics.Blit(noise, grainTex);

        effects.SetTexture("_GrainTex", grainTex);
        effects.SetFloat("_Grain", grain);

        destination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(source, destination, effects, 4);
        RenderTexture.ReleaseTemporary(source);
        RenderTexture.ReleaseTemporary(grainTex);

        return (destination, destination);
    }

    private (RenderTexture, RenderTexture) Toon(RenderTexture source, RenderTexture destination) {
        RenderTexture averageColor = RenderTexture.GetTemporary(1, 1, 0, source.format);
        Graphics.Blit(source, averageColor);

        effects.SetFloat("_LuminanceThreshold", luminanceThreshold);
        effects.SetFloat("_Gamma", gamma);
        destination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(source, destination, effects, 7);
        RenderTexture.ReleaseTemporary(source);
        RenderTexture.ReleaseTemporary(averageColor);

        return (destination, destination);
    }

    private (RenderTexture, RenderTexture) Dithering(RenderTexture source, RenderTexture destination) {        
        if (bayerTex == null || bayerLevel != currentBayerLevel) {
            if (bayerTex != null && bayerLevel != currentBayerLevel) bayerTex.Release();
            int bayerDim = (int)Mathf.Pow(2, bayerLevel);
            bayerTex = new RenderTexture(bayerDim, bayerDim, 0, source.format, RenderTextureReadWrite.Linear);
            bayerTex.enableRandomWrite = true;
            bayerTex.Create();

            noiseGenerator.SetInt("_BayerLevel", bayerLevel);
            noiseGenerator.SetInt("_BayerDimension", bayerDim);
            noiseGenerator.SetFloat("_Scalar", bayerLevel != 1 ? 1 / Mathf.Pow(2 * bayerLevel, bayerLevel) : 0.25f);
            noiseGenerator.SetTexture(1, "Result", bayerTex);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            noiseGenerator.Dispatch(1, Mathf.CeilToInt(bayerTex.width / 8.0f) + 1, Mathf.CeilToInt(bayerTex.height / 8.0f) + 1, 1);

            timer.Stop();
            
            System.TimeSpan ts = timer.Elapsed;
            string elapsedTime = System.String.Format("{0:00}.{1:00}", ts.Seconds, ts.Milliseconds / 10);
            UnityEngine.Debug.Log(elapsedTime);

            if (bayerTexUpscale != null) bayerTexUpscale.Release();
            bayerTexUpscale = new RenderTexture(source.width, source.height, 0, source.format, RenderTextureReadWrite.Linear);
            bayerTexUpscale.enableRandomWrite = true;
            bayerTexUpscale.Create();

            noiseGenerator.SetTexture(2, "Result", bayerTexUpscale);
            noiseGenerator.SetTexture(2, "_BayerTex", bayerTex);
            noiseGenerator.Dispatch(2, Mathf.CeilToInt(bayerTexUpscale.width / 8.0f), Mathf.CeilToInt(bayerTexUpscale.height / 8.0f), 1);

            currentBayerLevel = bayerLevel;
        }

        RenderTexture averageColor = RenderTexture.GetTemporary(1, 1, 0, source.format);
        Graphics.Blit(source, averageColor);

        effects.SetFloat("_LuminanceThreshold", luminanceThreshold);
        effects.SetTexture("_AverageColorTex", averageColor);
        effects.SetTexture("_ThresholdTex", bayerTexUpscale);
        effects.SetInt("_InvertLuminance", invertLuminance ? 1 : 0);
        effects.SetFloat("_Gamma", gamma);
        effects.SetInt("_Interpolate", interpolateThreshold ? 1 : 0);
        destination = RenderTexture.GetTemporary(image.width, image.height, 0, source.format);
        Graphics.Blit(source, destination, effects, 8);
        RenderTexture.ReleaseTemporary(source);
        RenderTexture.ReleaseTemporary(averageColor);

        return (destination, destination);
    }

    private void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Texture2D screenshot = new Texture2D(image.width, image.height, TextureFormat.RGB24, false, true);
            RenderTexture.active = output;
            
            /* Fix Gamma Correction */
            screenshot.ReadPixels(new Rect(0, 0, image.width, image.height), 0, 0);
            Color[] pixels = screenshot.GetPixels();
            for (int p = 0; p < pixels.Length; ++p)
                pixels[p] = pixels[p].gamma;
            screenshot.SetPixels(pixels);
            screenshot.Apply();

            string fileName = string.Format("{0}/../Examples/snap_{1}.png", Application.dataPath, System.DateTime.Now.ToString("HH-mm-ss"));
            System.IO.File.WriteAllBytes(fileName, screenshot.EncodeToPNG());
        }
    }
}