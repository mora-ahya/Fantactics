using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyInitSet
{
    public class Ripple : PostEffect
    {
        public Vector2 generatePosition;
        [Range(0.01f, 1f)]
        public float scope = 0.01f;
        [Range(0.01f, 30f)]
        public float interval = 0.01f;
        [Range(0f, 1f)]
        public float strength;
        [Range(0f, 1f)]
        public float decreaseRate;
        [Range(0f, 30f)]
        public float speed;

        float charaTime;
        float endTime;
        Vector2 origin;
        Vector2 preOrigin;
        float preScope;
        float preInterval;
        float preWaveStrength;
        float preDecreaseRate;
        float preSpeed;


        void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            isActive = false;
            material = new Material(shader);
            cb = new CommandBuffer();
            material.SetVector("_Origin", origin);
            material.SetFloat("_Scope", scope);
            material.SetFloat("_Interval", interval);
            material.SetFloat("_WaveStrength", strength);
            material.SetFloat("_DecreaseRate", decreaseRate);
            material.SetFloat("_Speed", speed);
            preOrigin = origin;
            preScope = scope;
            preInterval = interval;
            preWaveStrength = strength;
            preDecreaseRate = decreaseRate;
            preSpeed = speed;

            int tmpTexIdentifier = Shader.PropertyToID("PostEffectTmpTexture");
            cb.GetTemporaryRT(tmpTexIdentifier, -1, -1);

            cb.Blit(BuiltinRenderTextureType.CameraTarget, tmpTexIdentifier);
            cb.Blit(tmpTexIdentifier, BuiltinRenderTextureType.CameraTarget, material);

            cb.ReleaseTemporaryRT(tmpTexIdentifier);
            //cb.Clear();

        }

        public void Set(Vector2 genarateWavePosition, float waveScope, float waveInterval, float waveStrength, float waveDecreaseRate, float waveSpeed, float duration)
        {
            charaTime = 0f;
            generatePosition = genarateWavePosition;
            origin.Set(generatePosition.x / Screen.width, generatePosition.y / Screen.height);
            scope = waveScope;
            interval = waveInterval;
            strength = waveStrength;
            decreaseRate = waveDecreaseRate;
            speed = waveSpeed;
            endTime = duration;

            material.SetVector("_Origin", origin);
            material.SetFloat("_Scope", scope);
            material.SetFloat("_Interval", interval);
            material.SetFloat("_WaveStrength", strength);
            material.SetFloat("_DecreaseRate", decreaseRate);
            material.SetFloat("_Speed", speed);

            preOrigin = origin;
            preScope = scope;
            preInterval = interval;
            preWaveStrength = strength;
            preDecreaseRate = decreaseRate;
            preSpeed = speed;

            isActive = true;
        }

        public override void Run()
        {
            charaTime += Time.deltaTime;

            if (charaTime >= endTime)
            {
                isActive = false;
                return;
            }

            material.SetFloat("_CharaTime", charaTime);

            origin = Camera.main.WorldToScreenPoint(generatePosition);
            origin.Set(origin.x / Screen.width, origin.y / Screen.height);

            if (preOrigin != origin)
            {
                material.SetVector("_Origin", origin);
                preOrigin = origin;
            }

            if (preScope != scope)
            {
                material.SetFloat("_Scope", scope);
                preScope = scope;
            }

            if (preInterval != interval)
            {
                material.SetFloat("_Interval", interval);
                preInterval = interval;
            }

            if (preWaveStrength != strength)
            {
                material.SetFloat("_WaveStrength", strength);
                preWaveStrength = strength;
            }

            if (preDecreaseRate != decreaseRate)
            {
                material.SetFloat("_DecreaseRate", decreaseRate);
                preDecreaseRate = decreaseRate;
            }

            if (preSpeed != speed)
            {
                material.SetFloat("_Speed", speed);
                preSpeed = speed;
            }

            Graphics.ExecuteCommandBuffer(cb);
        }

        override public void Clear()
        {
            charaTime = 0f;
        }
    }
}

