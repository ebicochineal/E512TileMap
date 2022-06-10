Shader "Custom/AutoTileMap"{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TileSize("TileSize", Int) = 16
	}
	SubShader{
		//Lighting Off
		pass {
			Tags{ "LightMode" = "Vertex" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			int _TileSize;
			int _Layer;
			half _TexTileSize;
			half _OneBrightness;

			struct VertOut {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				half brightness : TEXCOORD1;
				half2 light_dist0 : TEXCOORD2;
				half2 light_dist1 : TEXCOORD3;
				half2 light_dist2 : TEXCOORD4;
				half2 light_dist3 : TEXCOORD5;
				half2 light_dist4 : TEXCOORD6;
				half2 light_dist5 : TEXCOORD7;
				half2 light_dist6 : TEXCOORD8;
				half2 light_dist7 : TEXCOORD9;
			};

			// 頂点シェーダ
			VertOut vert(appdata_full v) {
				VertOut o;
				// o.pos = mul(UNITY_MATRIX_MVP, v.vertex); // 2017_2
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv = v.texcoord;

				if (o.uv.x < -_TexTileSize) {// 無
					o.pos.x = 0;
				} else {
					if (o.uv.x > 1) {// タイルアニメーション
						half animsize = floor(o.uv.x + 1);
						o.uv.x = o.uv.x - floor(o.uv.x);
						o.uv.x += (_MainTex_TexelSize.x * _TileSize) * floor((_Time.y * 2) % animsize);
					}

					o.brightness = 1;
					if (o.uv.y > 1) {// 明るさ
						o.brightness = 1.0 - _OneBrightness * floor(o.uv.y);
						o.uv.y = o.uv.y - floor(o.uv.y);
					}

					// ライト距離
					half2 vp = mul(UNITY_MATRIX_M, v.vertex).xy;
					half2 lp;
					lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[0]).xy;
					lp.x = floor(lp.x) + 0.5;
					lp.y = floor(lp.y) + 0.5;
					o.light_dist0 = half2(vp.x - lp.x - 0.5, vp.y - lp.y - 0.5);

					lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[1]).xy;
					lp.x = floor(lp.x) + 0.5;
					lp.y = floor(lp.y) + 0.5;
					o.light_dist1 = half2(vp.x - lp.x - 0.5, vp.y - lp.y - 0.5);

					lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[2]).xy;
					lp.x = floor(lp.x) + 0.5;
					lp.y = floor(lp.y) + 0.5;
					o.light_dist2 = half2(vp.x - lp.x - 0.5, vp.y - lp.y - 0.5);

					lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[3]).xy;
					lp.x = floor(lp.x) + 0.5;
					lp.y = floor(lp.y) + 0.5;
					o.light_dist3 = half2(vp.x - lp.x - 0.5, vp.y - lp.y - 0.5);

					lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[4]).xy;
					lp.x = floor(lp.x) + 0.5;
					lp.y = floor(lp.y) + 0.5;
					o.light_dist4 = half2(vp.x - lp.x - 0.5, vp.y - lp.y - 0.5);

					lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[5]).xy;
					lp.x = floor(lp.x) + 0.5;
					lp.y = floor(lp.y) + 0.5;
					o.light_dist5 = half2(vp.x - lp.x - 0.5, vp.y - lp.y - 0.5);

					lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[6]).xy;
					lp.x = floor(lp.x) + 0.5;
					lp.y = floor(lp.y) + 0.5;
					o.light_dist6 = half2(vp.x - lp.x - 0.5, vp.y - lp.y - 0.5);

					lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[7]).xy;
					lp.x = floor(lp.x) + 0.5;
					lp.y = floor(lp.y) + 0.5;
					o.light_dist7 = half2(vp.x - lp.x - 0.5, vp.y - lp.y - 0.5);
				}
				return o;
			}

			// ピクセルシェーダ
			fixed4 frag(VertOut input) : COLOR {
				fixed4 c = tex2D(_MainTex, input.uv);
				if (input.uv.x < 0) { return fixed4(0, 0, 0, 1); }// 黒
				if (c.a < 1) { discard; }// 透明
				half g = input.brightness;
				half d;
				half w;
				half zero = half2(0, 0);

				// sqrt(unity_LightAtten[0].w)でライトのrange
				if (unity_LightColor[0][0] != 0) {
					w = ceil(sqrt(unity_LightAtten[0].w));
					d = distance(half2(ceil(input.light_dist0.x), ceil(input.light_dist0.y)), zero);
					g = max(d > w ? 0 : (w - d) / w, g);
				} else {
					return c * g;
				}

				if (unity_LightColor[1][0] != 0) {
					w = ceil(sqrt(unity_LightAtten[1].w));
					d = distance(half2(ceil(input.light_dist1.x), ceil(input.light_dist1.y)), zero);
					g = max(d > w ? 0 : (w - d) / w, g);
				} else {
					return c * g;
				}

				if (unity_LightColor[2][0] != 0) {
					w = ceil(sqrt(unity_LightAtten[2].w));
					d = distance(half2(ceil(input.light_dist2.x), ceil(input.light_dist2.y)), zero);
					g = max(d > w ? 0 : (w - d) / w, g);
				} else {
					return c * g;
				}

				if (unity_LightColor[3][0] != 0) {
					w = ceil(sqrt(unity_LightAtten[3].w));
					d = distance(half2(ceil(input.light_dist3.x), ceil(input.light_dist3.y)), zero);
					g = max(d > w ? 0 : (w - d) / w, g);
				} else {
					return c * g;
				}

				if (unity_LightColor[4][0] != 0) {
					w = ceil(sqrt(unity_LightAtten[0].w));
					d = distance(half2(ceil(input.light_dist4.x), ceil(input.light_dist4.y)), zero);
					g = max(d > w ? 0 : (w - d) / w, g);
				} else {
					return c * g;
				}

				if (unity_LightColor[5][0] != 0) {
					w = ceil(sqrt(unity_LightAtten[1].w));
					d = distance(half2(ceil(input.light_dist5.x), ceil(input.light_dist5.y)), zero);
					g = max(d > w ? 0 : (w - d) / w, g);
				} else {
					return c * g;
				}

				if (unity_LightColor[6][0] != 0) {
					w = ceil(sqrt(unity_LightAtten[2].w));
					d = distance(half2(ceil(input.light_dist6.x), ceil(input.light_dist6.y)), zero);
					g = max(d > w ? 0 : (w - d) / w, g);
				} else {
					return c * g;
				}

				if (unity_LightColor[7][0] != 0) {
					w = ceil(sqrt(unity_LightAtten[3].w));
					d = distance(half2(ceil(input.light_dist7.x), ceil(input.light_dist7.y)), zero);
					g = max(d > w ? 0 : (w - d) / w, g);
				} else {
					return c * g;
				}

				return c * g;
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}