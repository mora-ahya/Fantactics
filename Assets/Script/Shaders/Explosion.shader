Shader "MyShader/Explosion"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[PowerSlider(1.0)]
		_Rotation("Rotation", Range(0, 1)) = 0.0
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" }
			LOD 200
			ZWrite Off

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
		float _Rotation;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //上ほど透明度を高くして揺らぎを与えれば、炎ぽくなるかもしれない
			float2 offset = float2(frac(_Time.x), 1.0f - frac(_Time.y));
            fixed4 c = tex2D (_MainTex, frac(IN.uv_MainTex + offset));
            //テクスチャをずらして重ねる
			IN.uv_MainTex.x *= 1.25f;
			c += tex2D(_MainTex, frac(IN.uv_MainTex + offset));
			IN.uv_MainTex.y *= 1.25f;
			c += tex2D(_MainTex, frac(IN.uv_MainTex + offset));
			IN.uv_MainTex.x *= 1.25f;
			c += tex2D(_MainTex, frac(IN.uv_MainTex + offset));
            o.Emission = c.rgb * 2.0f;
			o.Alpha = c.r * pow(saturate(dot(IN.viewDir, IN.worldNormal)), 2.0f);
            clip(o.Alpha);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
