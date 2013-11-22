Shader "Custom/LavaShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TextureDirection ("Tex Move Dir", Vector) = (0,0,0,0)
		_WavesFrequency ("Waves Frequency", Float) = 1
		_WavesPeriod ("Waves Period", Float) = 1
		_WavesAmplitude ("Waves Amplitude", Float) = 1
	}
	
	SubShader {
		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members sv_pos)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _TextureDirection;
			float _WavesFrequency;
			float _WavesPeriod;
			float _WavesAmplitude;
			
			struct v2f {
			    float4 pos : SV_POSITION;
			    float4 sv_pos;
			};
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			 
			 	v.vertex.y += _WavesAmplitude * sin(_WavesPeriod * (_WavesFrequency * _Time + v.vertex.x));
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    
			    o.sv_pos = o.pos;
			    o.sv_pos += _TextureDirection * _Time;
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    return tex2D(_MainTex, i.sv_pos.xy);
			}
			ENDCG
		} 
	}

}
