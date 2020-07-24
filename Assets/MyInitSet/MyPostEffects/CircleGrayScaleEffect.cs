using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyInitSet
{
    public class CircleGrayScaleEffect : PostEffect
    {
        [Range(0f, 2f)]
        public float radius = 1f;
        float preRadius;

        public Vector2 startPoint = default;
        Vector2 preStartPoint;

        void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            isActive = false;
            material = new Material(shader);
            cb = new CommandBuffer();
            material.SetVector("_Start_Point", startPoint);
            material.SetFloat("_Radius", radius);

            int tmpTexIdentifier = Shader.PropertyToID("PostEffectTmpTexture");
            cb.GetTemporaryRT(tmpTexIdentifier, -1, -1);

            cb.Blit(BuiltinRenderTextureType.CameraTarget, tmpTexIdentifier);

            cb.Blit(tmpTexIdentifier, BuiltinRenderTextureType.CameraTarget, material);

            cb.ReleaseTemporaryRT(tmpTexIdentifier);
            //cb.Clear();

        }

        override public void Run()
        {
            //material.SetVector("_Start_Point", startPoint);
            if (preStartPoint != startPoint)
            {
                material.SetVector("_Start_Point", startPoint);
                preStartPoint = startPoint;
            }

            if (preRadius != radius)
            {
                material.SetFloat("_Radius", radius);
                preRadius = radius;
            }
            Graphics.ExecuteCommandBuffer(cb);
        }

        override public void Clear()
        {

        }
    }
}
