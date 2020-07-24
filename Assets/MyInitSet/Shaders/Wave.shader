Shader "MyShader/Wave"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float2 _MainTex_TexelSize;
			float _WaveSize;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.y *= -_ProjectionParams.x;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float time = _WaveSize * _WaveSize;
				time = time > 200.f ? 200.f : time;
				i.uv.x += time * _MainTex_TexelSize.x * sin((_WaveSize * _WaveSize * 10 + (i.uv.y / _MainTex_TexelSize.y)) / 100);
				fixed4 col = (step(i.uv.x, 1.0f) && step(0, i.uv.x)) * tex2D(_MainTex, i.uv);
				return col;
			}

			ENDCG
		}
	}
}
