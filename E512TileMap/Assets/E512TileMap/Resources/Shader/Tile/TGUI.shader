Shader "Custom/TGUI"{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TileSize("TileSize", Int) = 16
		_TexTileSize("_TexTileSize", float) = 0
	}
	
	SubShader{
		pass {// デプス上書き
			Tags{ "Queue" = "Overlay-1" "RenderType" = "Overlay-1" }
			Ztest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			int _TileSize;
			int _Layer;
			half _CellSize;
			half _OneBrightness;
			half _TexTileSize;

			struct VertOut {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			// 頂点シェーダ
			VertOut vert(appdata_full v) {
				VertOut o;
				// o.pos = mul(UNITY_MATRIX_MVP, v.vertex); // 2017_2
				o.pos = UnityObjectToClipPos(v.vertex);

#if defined(UNITY_REVERSED_Z)
				o.pos.z = 0.01;
#else
				o.pos.z = 0.99;
#endif
				o.uv = v.texcoord;

				if (o.uv.x < -_TexTileSize) {// 無
					o.pos.x = 0;
					return o;
				}

				if (o.uv.x > 1) {// タイルアニメーション
					half animsize = floor(o.uv.x + 1);
					o.uv.x = o.uv.x - floor(o.uv.x);
					o.uv.x += (_MainTex_TexelSize.x * _TileSize * 3) * floor((_Time.y * 8) % animsize);
				}

				return o;
			}

			// ピクセルシェーダ
			fixed4 frag(VertOut input) : COLOR {
				fixed4 c = tex2D(_MainTex, input.uv);
				//if (input.uv.x < -_TexTileSize) { discard; }// 無
				if (input.uv.x < 0) { return fixed4(0, 0, 0, 1); }// 黒
				if (c.a < 1) { discard; }// 透明
				return c;
			}
			ENDCG
		}
		
		pass {
			Tags{ "Queue" = "Overlay" "RenderType" = "Overlay" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			int _TileSize;
			int _Layer;
			half _CellSize;
			half _OneBrightness;
			half _TexTileSize;

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

				if (o.uv.x < -_TexTileSize) {// 無
					o.pos.x = 0;
					return o;
				}

				if (o.uv.x > 1) {// タイルアニメーション
					half animsize = floor(o.uv.x + 1);
					o.uv.x = o.uv.x - floor(o.uv.x);
					o.uv.x += (_MainTex_TexelSize.x * _TileSize * 3) * floor((_Time.y * 8) % animsize);
				}

				return o;
			}

			// ピクセルシェーダ
			fixed4 frag(VertOut input) : COLOR {
				fixed4 c = tex2D(_MainTex, input.uv);
				//if (input.uv.x < -_CellSize) { discard; }// 無
				if (input.uv.x < 0) { return fixed4(0, 0, 0, 1); }// 黒
				if (c.a < 1) { discard; }// 透明
				return c;
			}
			ENDCG
		}

	}
	Fallback "Diffuse"
}