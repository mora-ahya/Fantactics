using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyInitSet
{
    public class GaussianBlurEffect : PostEffect
    {
        [Range(1f, 10f)]
        public float offset = 1f;
        float preOffset;

        [Range(10f, 1000f)]
        public float blur = 100f;
        float preBlur;

        Vector4 tmp = new Vector4();

        float[] weights = new float[5];

        void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            isActive = false;
            material = new Material(shader);
            cb = new CommandBuffer();
            SetWeights();
            material.SetFloatArray("_Weights", weights);
            material.SetFloat("_Offset", offset);

            int tmpTexIdentifier = Shader.PropertyToID("PostEffectTmpTexture");
            int tmpTexIdentifier2 = Shader.PropertyToID("PostEffectTmpTexture2");
            int directionID = Shader.PropertyToID("_Direction");
            cb.GetTemporaryRT(tmpTexIdentifier, -1, -1);
            cb.GetTemporaryRT(tmpTexIdentifier2, -1, -1);

            cb.Blit(BuiltinRenderTextureType.CameraTarget, tmpTexIdentifier);
            //cb.SetGlobalFloatArray("_Weights", weights);

            tmp.Set(1f / Screen.width, 0, 0, 0);
            cb.SetGlobalVector(directionID, tmp);
            //cb.Blit(tmpTexIdentifier, BuiltinRenderTextureType.CameraTarget, material);
            cb.Blit(tmpTexIdentifier, tmpTexIdentifier2, material);
            //cb.Blit(BuiltinRenderTextureType.CameraTarget, tmpTexIdentifier);

            tmp.Set(0, 1f / Screen.height, 0, 0);
            cb.SetGlobalVector(directionID, tmp);
            //cb.Blit(tmpTexIdentifier, BuiltinRenderTextureType.CameraTarget, material);
            cb.Blit(tmpTexIdentifier2, BuiltinRenderTextureType.CameraTarget, material);

            cb.ReleaseTemporaryRT(tmpTexIdentifier);
            cb.ReleaseTemporaryRT(tmpTexIdentifier2);
            //cb.Clear();

        }

        override public void Run()
        {
            if (preBlur != blur)
            {
                SetWeights();
                material.SetFloatArray("_Weights", weights);
                preBlur = blur;
            }

            if (preOffset != offset)
            {
                material.SetFloat("_Offset", offset);
                preOffset = offset;
            }
            //material.SetFloatArray("_Weights", weights);
            Graphics.ExecuteCommandBuffer(cb);
        }

        override public void Clear()
        {

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
    }
}
