Shader "Custom/TileColor"{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	    _BaseColor("BaseColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_X("X", Int) = 0
		_Y("Y", Int) = 0
		_Anim("Anim", Int) = 0
		_AnimSpeed("AnimSpeed", Range(1, 16)) = 1
		_TileSizeX("TileSizeX", Int) = 16
		_TileSizeY("TileSizeY", Int) = 16
		_Emission("Emission", float) = 1
	}
	SubShader{
		pass {
			Tags{ "LightMode" = "Vertex" }
			//Tags{"Queue" = "Transparent"}
			//Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			half4 _BaseColor;
			int _TileSizeX;
			int _TileSizeY;
			int _Layer;
			int _X;
			int _Y;
			int _Anim;
			int _AnimSpeed;
			half _Emission;

			struct VertOut {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				half2 light_dist0 : TEXCOORD1;
				half2 light_dist1 : TEXCOORD2;
				half2 light_dist2 : TEXCOORD3;
				half2 light_dist3 : TEXCOORD4;
				half2 light_dist4 : TEXCOORD5;
				half2 light_dist5 : TEXCOORD6;
				half2 light_dist6 : TEXCOORD7;
				half2 light_dist7 : TEXCOORD8;
			};

			// 頂点シェーダ
			VertOut vert(appdata_full v) {
				VertOut o;
				int tx = _MainTex_TexelSize.z / _TileSizeX;
				float sx = 1.0 / tx;
				int ty = _MainTex_TexelSize.w / _TileSizeY;
				float sy = 1.0 / ty;
				// o.pos = mul(UNITY_MATRIX_MVP, v.vertex); // 2017_2
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.uv.y = sy * (ty - _Y - 1) + sy * o.uv.y;
				if (_Anim > 1) {
					o.uv.x = sx * (_X + floor((_Time.y * _AnimSpeed) % _Anim)) + sx * o.uv.x;
				} else {
					o.uv.x = sx * _X + sx * o.uv.x;
				}

				half2 vp = mul(UNITY_MATRIX_M, v.vertex).xy;
				half2 lp;
				lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[0]).xy;
				lp.x = floor(lp.x) + 0.5;
				lp.y = floor(lp.y) + 0.5;
				o.light_dist0 = half2(abs(vp.x - lp.x) - 0.5, abs(vp.y - lp.y) - 0.5);

				lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[1]).xy;
				lp.x = floor(lp.x) + 0.5;
				lp.y = floor(lp.y) + 0.5;
				o.light_dist1 = half2(abs(vp.x - lp.x) - 0.5, abs(vp.y - lp.y) - 0.5);

				lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[2]).xy;
				lp.x = floor(lp.x) + 0.5;
				lp.y = floor(lp.y) + 0.5;
				o.light_dist2 = half2(abs(vp.x - lp.x) - 0.5, abs(vp.y - lp.y) - 0.5);

				lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[3]).xy;
				lp.x = floor(lp.x) + 0.5;
				lp.y = floor(lp.y) + 0.5;
				o.light_dist3 = half2(abs(vp.x - lp.x) - 0.5, abs(vp.y - lp.y) - 0.5);

				lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[4]).xy;
				lp.x = floor(lp.x) + 0.5;
				lp.y = floor(lp.y) + 0.5;
				o.light_dist4 = half2(abs(vp.x - lp.x) - 0.5, abs(vp.y - lp.y) - 0.5);

				lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[5]).xy;
				lp.x = floor(lp.x) + 0.5;
				lp.y = floor(lp.y) + 0.5;
				o.light_dist5 = half2(abs(vp.x - lp.x) - 0.5, abs(vp.y - lp.y) - 0.5);

				lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[6]).xy;
				lp.x = floor(lp.x) + 0.5;
				lp.y = floor(lp.y) + 0.5;
				o.light_dist6 = half2(abs(vp.x - lp.x) - 0.5, abs(vp.y - lp.y) - 0.5);

				lp = mul(UNITY_MATRIX_I_V, unity_LightPosition[7]).xy;
				lp.x = floor(lp.x) + 0.5;
				lp.y = floor(lp.y) + 0.5;
				o.light_dist7 = half2(abs(vp.x - lp.x) - 0.5, abs(vp.y - lp.y) - 0.5);

				return o;
			}

			// ピクセルシェーダ
			float4 frag(VertOut input) : COLOR{
				fixed4 c = tex2D(_MainTex, input.uv);
				fixed a = c.w;
				fixed m = (c.x + c.y + c.z) / 3;
				if (m != 1) {
					c = _BaseColor * m * 2;
				} else {
					c *= (_BaseColor.x + _BaseColor.y + _BaseColor.z) / 3 * 2;
				}
				if (a < 1) { discard; }


				half g = _Emission;
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