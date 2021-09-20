Shader "MyShader/Frost"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("ColorTex", 2D) = "white" {}
		[Slider]
		_Vanish("Vanish", Range(0.0, 1.0)) = 0.0
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" }
			LOD 200

			Cull back
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

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			#pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				o.Emission = _Color;
				//o.Emission.r -= t / 5.0f;

				float border = 1 - distance(IN.uv_MainTex, float2(0.5f, 0.5f)) / 0.5f;
				clip(border);
				o.Alpha = border * tex2D(_MainTex, IN.uv_MainTex/* + frac(float2(_Time.x, _Time.x / 2))*/).x * _Vanish;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
