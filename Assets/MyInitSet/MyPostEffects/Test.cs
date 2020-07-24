using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Shader shader;

    Material mosaic;

    RenderTexture t = null;

    [SerializeField, Range(1f, 10f)]
    float offset = 1f;

    [SerializeField, Range(10f, 1000f)]
    float blur = 1000f;
    float preBlur;

    Vector4 tmp = new Vector4();

    float[] weights = new float[10];

    void SetWeights()
    {
        float total = 0f;
        float d = blur * blur * 0.001f;

        for (int i = 0; i < weights.Length; i++)
        {
            float x = i * 2f;
            float w = Mathf.Exp(-0.5f * (x * x) / d);
            weights[i] = w;

            if (i > 0)
                w *= 2f;

            total += w;
        }

        for (int i = 0; i < weights.Length; i++)
            weights[i] /= total;
    }

    void Awake()
    {
        mosaic = new Material(shader);
        SetWeights();
        //preBlur = blur;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (t == null)
        {
            t = new RenderTexture(src.width, src.height, src.depth);
        }

        if (preBlur != blur)
            SetWeights();
        mosaic.SetFloatArray("_Weights", weights);

        tmp.Set(offset / Screen.width, 0, 0, 0);
        mosaic.SetVector("_Offsets", tmp);
        Graphics.Blit(src, t, mosaic);

        tmp.Set(0, offset / Screen.height, 0, 0);
        mosaic.SetVector("_Offsets", tmp);
        Graphics.Blit(t, dest, mosaic);

    }
}
