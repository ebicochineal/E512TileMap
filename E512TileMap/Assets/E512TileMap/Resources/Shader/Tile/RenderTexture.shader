Shader "Custom/RenderTexture"{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	SubShader{
		pass {
			Tags{ "Queue" = "Overlay+1" "RenderType" = "Overlay+1" }
			//Tags{"Queue" = "Transparent"}
			//Blend SrcAlpha OneMinusSrcAlpha 
			

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;

			struct VertOut {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			// 頂点シェーダ
			VertOut vert(appdata_full v) {
				VertOut o;
				// o.pos = mul(UNITY_MATRIX_MVP, v.vertex); // 2017_2
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			// ピクセルシェーダ
			fixed4 frag(VertOut input) : COLOR{
				return tex2D(_MainTex, input.uv);
			}

		ENDCG
		}
	}
	Fallback "Diffuse"
}