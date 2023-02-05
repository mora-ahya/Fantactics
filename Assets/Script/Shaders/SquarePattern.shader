Shader "MyShader/SquarePattern"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BoardWidth("BoardWidth", Int) = 11
		_BoardHeight("BoardHeight", Int) = 14
    }
    SubShader
    {
        Pass
        {
			Tags { "Queue" = "Transparent" "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
			LOD 200

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				SHADOW_COORDS(1)
			};

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				TRANSFER_SHADOW(o)

                return o;
            }

            sampler2D _MainTex;
            sampler2D _RedBitFlags; // いずれテクスチャフラグにする予定だが、場合によってはこの方式自体を変更するかも
			int _BoardHeight;
			int _BoardWidth;
			float _RedBitFlag[5];

			static const float LineWidth = 0.05;

            fixed4 frag (v2f i) : SV_Target
            {
				float time = frac(_Time * 50) * 0.5;
				float tmp = frac(i.uv.x * _BoardWidth);
				float tmp2 = frac(i.uv.y * _BoardHeight);
				int tmp3 = floor(i.uv.x * _BoardWidth) + floor(i.uv.y * _BoardHeight) * _BoardWidth;
				float tmp4 = pow(2.0, tmp3 - (32 * (tmp3 >> 5)));
				fixed4 color = tex2D(_MainTex, i.uv) * SHADOW_ATTENUATION(i);
				float tmp5 = (abs(tmp - 0.5) < time && abs(tmp2 - 0.5) < time) ? time : 0.5;
				color.rgb += abs((floor(_RedBitFlag[tmp3 >> 5] / tmp4))) % 2 == 1 ? (fixed3(1, 0, 0) - color.rgb) * tmp5 : 0;

				return tmp < LineWidth * 0.5 || (1.0 - tmp) < LineWidth * 0.5 || tmp2 < LineWidth * 0.5 || (1.0 - tmp2) < LineWidth * 0.5 ? 0.5 : color;
            }
            ENDCG
        }

			Pass
        {
            Tags{ "LightMode"="ShadowCaster" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vert (appdata v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
