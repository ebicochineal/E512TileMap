Shader "Custom/RenderScreenView"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;// z 1~じゃないと映らない
				o.vertex = v.vertex;
				o.vertex.x = o.vertex.x > 0 ? 1 : -1;
				o.vertex.y = o.vertex.y > 0 ? 1 : -1;
				o.vertex.z = 0.5;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : Color
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				/*
				if ((int)(i.uv.x * _ScreenParams.x) % 4 == 0) {
					col = fixed4(col.x, 0, 0, 0);
				}
				if ((int)(i.uv.x * _ScreenParams.x+1) % 4 == 0) {
					col = fixed4(0, col.y, 0, 0);
				}
				if ((int)(i.uv.x * _ScreenParams.x+2) % 4 == 0) {
					col = fixed4(0, 0, col.z, 0);
				}
				
				if ((int)(i.uv.y * _ScreenParams.y) % 4 == 0) {
					col = fixed4(0, 0, 0, 0);
				}
				*/
				return col;
			}
			ENDCG
		}
	}
}
