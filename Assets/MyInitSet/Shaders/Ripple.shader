Shader "MyShader/Ripple"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[Toggle]_Light("Light", Int) = 0

		[Enum(UnityEngine.Rendering.BlendMode)]
		_SrcFactor("Src Factor", Float) = 1  //spriteに充てるときは5(SrcAlpha)に設定

		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstFactor("Dst Factor", Float) = 0  //spriteに充てるときは10(OneMinusSrcAlpha)に設定
	}

		SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Blend[_SrcFactor][_DstFactor]

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

			float2 _Origin;
			float _Scope;
			float _Speed;
			float _CharaTime;
			float _Interval;
			float _WaveStrength;
			float _DecreaseRate;

			static const float pi = 3.14159265;

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.y *= -_ProjectionParams.x;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				//_Interval = 0.2;
				//_Origin = float2(0, 0);
				//_Scope = 0.1;
				//_WaveStrength = 0.75;
				//_DecreaseRate = 0;
				//float tmp = (_Time * 16) % _Interval;
				//_CharaTime = _Time * 16;
				//_Origin += _Time / 16;
				float tmp = (_CharaTime * _Speed) % _Interval;
				float r = (float)(floor((distance(_Origin, i.uv) - tmp + _Scope) / _Interval)) * _Interval + tmp;
				float l = r - distance(_Origin, i.uv);

				float2 st = i.uv;
				st -= _Origin;
				float2 tmp2 = st * r / length(st);
				st -= tmp2;
				tmp = _WaveStrength - r * _DecreaseRate;
				st *= (1 - tmp) + tmp * sin((abs(l) / _Scope) * (pi / 2));
				st += _Origin + tmp2;

				tmp = step(tmp, 1) * step(0, tmp) * step(abs(l), _Scope) * step(r, _CharaTime * _Speed);

				return tex2D(_MainTex, tmp == 1 ? st : i.uv);
			}

			ENDCG
		}
	}
}