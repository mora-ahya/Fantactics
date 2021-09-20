Shader "MyShader/Ice"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("ColorTex", 2D) = "white" {}
		[Slider]
		_Vanish ("Vanish", Range(0.0, 1.0)) = 0.0
		[Slider]
		_EmissionRange ("EmissionRange", Range(0.0, 0.1)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 200
/*
		Cull Front
		ZWrite On

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        //UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        //UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            o.Albedo = _Color;
			//float alpha = 1 - (abs(dot(IN.viewDir, IN.worldNormal)));
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            o.Alpha = 0.9f;
        }
        ENDCG*/

		Cull Back
		ZWrite On
		Blend SrcAlpha One

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;
        };

        fixed4 _Color;
		float _Vanish;
		float _EmissionRange;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			o.Albedo = _Color;
			o.Emission = _Color;//float4(1.0f, 1.0f, 1.0f, 1.0f);

            //ノイズテクスチャの値が閾値より小さいときピクセルを破棄
			float t = tex2D(_MainTex, IN.uv_MainTex).x;
            clip(t - _Vanish);
			
            half alpha = dot(IN.viewDir, IN.worldNormal);
			o.Emission *= (1.0f - pow(saturate(alpha), 3.0f));
		    alpha = 1.0f - (abs(alpha));
			alpha = (alpha * (1 - 0.25f) + 0.25f);

            //消える箇所の周辺なら色を白にし、透明度をリセットする
            t = step(t, _Vanish + _EmissionRange);
			o.Emission += t;
            o.Alpha = max(t, alpha);
			//o.Alpha = 1.0f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
