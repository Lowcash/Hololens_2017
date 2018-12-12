// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SpreadWaveShader" {
	Properties {
		_ObjectAlpha("Object aplha", RANGE(0.0, 1.0)) = 0.0
		_Color("Main color", COLOR) = (1.0, 1.0, 1.0, 1.0)
		_DiffuseTex("Diffuse texture", 2D) = "" {}
		_BumpTex("Bump texture", 2D) = "" {}
		_NoiseTex("Noise texture", 2D) = "" {}
		_CutOff("Cut off", RANGE(0.0, 1.0)) = 0.1
		_BumpPower("Bump power", RANGE(0.0, 1.0)) = 1.0
		_NoisePower("Noise power", RANGE(0.0, 1.0)) = 1.0
		_SpreadRange("Spread range", FLOAT) = 0.5
		_SpreadSpeed("Spread speed", FLOAT) = 0.5
	}
	SubShader {
		Tags{"RenderType"="Transparent" "Queue"="Transparent"}
		
		CGPROGRAM
        #pragma surface surf Lambert vertex:vert alpha

        fixed _ObjectAlpha;
        fixed4 _Color;
		fixed _BumpPower;
		fixed _NoisePower;
		sampler2D _DiffuseTex;
        sampler2D _BumpTex;
        sampler2D _NoiseTex;
        half _SpreadRange;
        half _SpreadSpeed;
        half _ActualMaxWaveSpread[10];
        half _AlphaAttenuation[10];
        float _Timers[10];
        half4 _ContactPoints[10];
        float _ActualPointsDistances[10];
        fixed _MaxAlpha = 0.0;
		fixed _CutOff;

		struct Input {
            float3 localPos;
			half2 uv_DiffuseTex;
			half2 uv_BumpTex;
			half2 uv_NoiseTex;
        };

		void vert(inout appdata_full v, out Input o){
        	UNITY_INITIALIZE_OUTPUT(Input, o);
        	o.localPos = v.vertex.xyz;
        }

        fixed wave(int index, float3 localPos, half3 _Noise, fixed _MaxAlpha) {
        	_ActualMaxWaveSpread[index] = (5 - _Timers[index]) * _SpreadSpeed;
        	_ActualPointsDistances[index] = distance(mul(unity_WorldToObject, _ContactPoints[index]), localPos);
			
			if (_ActualMaxWaveSpread[index] > _ActualPointsDistances[index]) {
				_AlphaAttenuation[index] = _ActualPointsDistances[index] / _SpreadRange;

				return sin((_ActualPointsDistances[index] * 2) / _ActualMaxWaveSpread[index]) - _AlphaAttenuation[index] - _Noise;
			}

			return _MaxAlpha;
        }

		void surf (Input IN, inout SurfaceOutput o) {
			half3 _DiffuseTexture = tex2D(_DiffuseTex, IN.uv_DiffuseTex);
			half3 _NormalTexture = UnpackNormal(tex2D(_BumpTex, IN.uv_BumpTex));
			half3 _NoiseTexture = tex2D(_NoiseTex, IN.uv_NoiseTex);
			
			if (_DiffuseTexture.r > _CutOff) 
				_DiffuseTexture = _Color;		
			else
				discard;
			_NormalTexture *= _BumpPower;
			_NoiseTexture /= _NoisePower;

			_MaxAlpha = max(wave(0, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(1, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(2, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(3, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(4, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(5, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(6, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(7, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(8, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);
			_MaxAlpha = max(wave(9, IN.localPos, _NoiseTexture, _MaxAlpha), _MaxAlpha);

           	o.Albedo = _Color * _DiffuseTexture;
            o.Alpha = _MaxAlpha + _ObjectAlpha;
            o.Normal = _NormalTexture;
		}
		ENDCG
	}
}
