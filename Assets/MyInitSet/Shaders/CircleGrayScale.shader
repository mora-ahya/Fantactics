Shader "MyShader/CircleGrayScale"
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

			half2 _Start_Point;
			float _Radius;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.y *= -_ProjectionParams.x;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				half4 col = tex2D(_MainTex, i.uv);
				int tmp = step(distance(i.uv.xy, _Start_Point), _Radius);
				col.rgb = lerp(col.rgb, dot(col.rgb, float3(0.2126f, 0.7152f, 0.0722f)), 1.0f * tmp);
				//col = col * i.color;
				//col.rgb = dot(col.rgb, a);
				//col.r *= 0.2126f;
				//col.g *= 0.7152f;
				//col.b *= 0.0722f;
				return col;
			}

			ENDCG
		}
	}
}
