using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ImageView : MonoBehaviour
{
    public RawImage colorImage;
    public RawImage maskedColorImage;

    //Render Texture for the backgroud
    private RenderTexture backgroundTexture;

    private Material erodeBodyMaterial;
    private Material dilateBodyMaterial;
    private Material blurBodyMaterial;

    [Range(0, 20)]
    public int erodeIterations = 0;
    [Range(0, 20)]
    public int dilateIterations = 0;


    [Range(0, 20)]
    public int blurIterations = 0;
    [Range(0.0f, 20.0f)]
    public float blurSpread = 0.0f;


    // Use this for initialization
    void OnEnable()
    {

        colorImage.gameObject.SetActive(true);
        colorImage.texture = AstraManager.Instance.ColorTexture;

        maskedColorImage.gameObject.SetActive(true);
        maskedColorImage.texture = AstraManager.Instance.MaskedColorTexture;
    }


    void Start()
    {
        Shader erodeBodyShader = Shader.Find("Custom/Erode");
        erodeBodyMaterial = new Material(erodeBodyShader);


        Shader dilateBodyShader = Shader.Find("Custom/Dilate");
        dilateBodyMaterial = new Material(dilateBodyShader);

        Shader blurBodyShader = Shader.Find("Custom/BlurShader5");
        blurBodyMaterial = new Material(blurBodyShader);


        erodeBodyMaterial.SetFloat("_TexResX", (float)AstraManager.Instance.DepthTexture.width);
        erodeBodyMaterial.SetFloat("_TexResY", (float)AstraManager.Instance.DepthTexture.height);

        dilateBodyMaterial.SetFloat("_TexResX", (float)AstraManager.Instance.DepthTexture.width);
        dilateBodyMaterial.SetFloat("_TexResY", (float)AstraManager.Instance.DepthTexture.height);


        backgroundTexture = new RenderTexture(640, 480, 16, RenderTextureFormat.ARGB32);
        backgroundTexture.Create();
    }

    private void Update()
    {
        Graphics.CopyTexture(AstraManager.Instance.MaskedColorTexture, backgroundTexture);

        backgroundTexture = ApplyErodeDilate(backgroundTexture, backgroundTexture, erodeBodyMaterial, dilateBodyMaterial, erodeIterations, dilateIterations);
        backgroundTexture = ApplyImageBlur(backgroundTexture, backgroundTexture, blurBodyMaterial, blurIterations, blurSpread);

        maskedColorImage.texture = backgroundTexture;
    }



    private static RenderTexture ApplyErodeDilate(RenderTexture source, RenderTexture destination, Material erodeMaterial,
                                         Material dilateMaterial, int erodeIterations, int dilateIterations)
    {
        if (!source || !erodeMaterial || !dilateMaterial)
            return null;

        RenderTexture[] tempTexture = new RenderTexture[2];
        tempTexture[0] = RenderTexture.GetTemporary(source.width, source.height, 0);
        tempTexture[1] = RenderTexture.GetTemporary(source.width, source.height, 0);

        Graphics.Blit(source, tempTexture[0]);

        for (int i = 0; i < erodeIterations; i++)
        {
            Graphics.Blit(tempTexture[i % 2], tempTexture[(i + 1) % 2], erodeMaterial);
        }

        if ((erodeIterations % 2) != 0)
        {
            Graphics.Blit(tempTexture[1], tempTexture[0]);
        }

        for (int i = 0; i < dilateIterations; i++)
        {
            Graphics.Blit(tempTexture[i % 2], tempTexture[(i + 1) % 2], dilateMaterial);
        }

        Graphics.Blit(tempTexture[dilateIterations % 2], destination);

        RenderTexture.ReleaseTemporary(tempTexture[0]);
        RenderTexture.ReleaseTemporary(tempTexture[1]);
        return destination;
    }

    private static RenderTexture ApplyImageBlur(RenderTexture source, RenderTexture destination, Material blurMaterial, int blurIterations, float blurSpread)
    {
        if (!source || !blurMaterial)
            return null;


        int rtW = source.width / 4;
        int rtH = source.height / 4;
        RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

        Downsample4x(source, buffer, blurMaterial);

        for (int i = 0; i < blurIterations; i++)
        {
            RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
            FourTapCone(buffer, buffer2, blurMaterial, i, blurSpread);
            RenderTexture.ReleaseTemporary(buffer);
            buffer = buffer2;
        }

        Graphics.Blit(buffer, destination);
        RenderTexture.ReleaseTemporary(buffer);
        return destination;
    }


    private static void Downsample4x(RenderTexture source, RenderTexture dest, Material material)
    {
        float off = 1.0f;

        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    private static void FourTapCone(RenderTexture source, RenderTexture dest, Material material, int iteration, float blurSpread)
    {
        float off = 0.5f + iteration * blurSpread;

        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }
}
