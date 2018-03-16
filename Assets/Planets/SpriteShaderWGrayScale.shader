// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/GrayScale"
{
	Properties{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_FlashColor("Flash Color",Color) = (1,1,1,1)
			[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
			 _CutOff("CutOff", Range(0, 1)) = 0
			[MaterialToggle] _Flash("Flash",Float) = 0
	}

	SubShader
	{
		Tags{
			"Queue" = "Transparent"
					  "IgnoreProjector" = "True"
										  "RenderType" = "Transparent"
														 "PreviewType" = "Plane"
																		 "CanUseSpriteAtlas" = "True"}

		Cull Off
			Lighting Off
				ZWrite Off
					Blend One OneMinusSrcAlpha

						Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;
			float _CutOff;
			half4 _FlashColor;
			float _Flash;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D(_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 c = SampleSpriteTexture(IN.texcoord);
				half4 grayScale = dot(c.rgb, float3(0.3, 0.59, 0.11));
				float coord = IN.texcoord.y + sin(degrees(IN.texcoord.x) + _Time.y * 2) * 0.01 * step(0, _CutOff - 0.001);
				coord *= step(0, 0.999 - _CutOff);
				half4 finalcolor = half4(0, 0, 0, 1);
				finalcolor += lerp(grayScale, c, step(0.0, _CutOff - coord));
				finalcolor = lerp(finalcolor,_FlashColor,_Flash/2);
				finalcolor = saturate(finalcolor);
				return finalcolor;
			}
			ENDCG
		}
	}
}