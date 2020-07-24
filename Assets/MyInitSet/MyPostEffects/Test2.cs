using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Test2 : MonoBehaviour
{
    public Shader shader;

    Material material;

    CommandBuffer cb;

    [SerializeField, Range(1f, 10f)]
    float offset = 1f;

    [SerializeField, Range(10f, 1000f)]
    float blur = 100f;
    float preBlur;

    Vector4 tmp = new Vector4();

    float[] weights = new float[10];

    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        material = new Material(shader);
        cb = new CommandBuffer();
        cb.name = "test";
        SetWeights();

        int tmpTexIdentifier = Shader.PropertyToID("PostEffectTmpTexture");
        cb.GetTemporaryRT(tmpTexIdentifier, -1, -1);

        cb.Blit(BuiltinRenderTextureType.CameraTarget, tmpTexIdentifier);
        cb.SetGlobalFloatArray("_Weights", weights);

        tmp.Set(offset / Screen.width, 0, 0, 0);
        cb.SetGlobalVector("_Offsets", tmp);
        cb.Blit(tmpTexIdentifier, BuiltinRenderTextureType.CameraTarget, material);
        cb.Blit(BuiltinRenderTextureType.CameraTarget, tmpTexIdentifier);

        tmp.Set(0, offset / Screen.height, 0, 0);
        cb.SetGlobalVector("_Offsets", tmp);
        cb.Blit(tmpTexIdentifier, BuiltinRenderTextureType.CameraTarget, material);

        cb.ReleaseTemporaryRT(tmpTexIdentifier);
        //cb.Clear();

    }

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

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.ExecuteCommandBuffer(cb);
        Graphics.Blit(src, dest);
    }
}
