// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/HeightColorShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Height ("Height", FLOAT) = 1.0
	}
	SubShader {
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

		CGPROGRAM
		#pragma surface surf Standard vertex:vert alpha:blend

		#pragma target 3.0

		struct Input {
			float4 localPos;
		};
		
		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = mul(unity_ObjectToWorld, v.vertex);
		}

		fixed4 _Color;
		float _Height;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			if (IN.localPos.y < _Height) {
				o.Albedo = _Color.rgb;
				o.Alpha = _Color.a;
			}
			else {
				o.Alpha = 0;
			}
		}
		ENDCG
	}
	Fallback "Diffuse"
}
