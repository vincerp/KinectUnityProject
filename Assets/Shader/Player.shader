Shader "Custom/Player" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LerpColor ("Interpolation Color", Color) = (1,1,1,1)
		_LerpValue ("Interpolator", Range(0,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		half4 _LerpColor;
		fixed _LerpValue;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(c.rgb, _LerpColor.rgb, _LerpValue);
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
