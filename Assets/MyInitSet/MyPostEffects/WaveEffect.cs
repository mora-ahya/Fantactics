using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyInitSet
{
    public class WaveEffect : PostEffect
    {
        float x = 1f;
        public float waveSpeed = 0.05f;

        void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            isActive = false;
            material = new Material(shader);
            cb = new CommandBuffer();

            int tmpTexIdentifier = Shader.PropertyToID("PostEffectTmpTexture");
            cb.GetTemporaryRT(tmpTexIdentifier, -1, -1);
            //cb.SetGlobalFloat("_WaveSize", x);
            //material.SetFloat("_WaveSize", x);

            cb.Blit(BuiltinRenderTextureType.CameraTarget, tmpTexIdentifier);
            cb.Blit(tmpTexIdentifier, BuiltinRenderTextureType.CameraTarget, material);

            cb.ReleaseTemporaryRT(tmpTexIdentifier);
            //cb.Clear();

        }

        override public void Run()
        {
            material.SetFloat("_WaveSize", x);
            Graphics.ExecuteCommandBuffer(cb);
            x += waveSpeed;
        }

        override public void Clear()
        {
            x = 1f;
        }
    }
}
