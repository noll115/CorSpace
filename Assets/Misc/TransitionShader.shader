Shader "Unlit/TransitionShader"
{
	Properties
	{
		_Transit("Transition",Range(0,1)) = 1
		_TransColorStart("Transition Start Color",Color) = (0,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"= "Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _Transit;
			half4 _TransColorStart;
			half4 _TransColorEnd;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv =  v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				half4 finalCol = _TransColorStart;
				finalCol.a = _Transit;
				return finalCol;
			}
			ENDCG
		}
	}
}
