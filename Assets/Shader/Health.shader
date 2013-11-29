Shader "Custom/Health" {
	Properties {
		_MainColor ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
		// The texture should have ALPHA channel even if it es grayscale
		// Check "Alpha from grayscale" box in the texture importer
		_MaskTex ("Cutout Mask", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 1
	}
	
	SubShader {
	
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
			
		Pass
		{		
			CGPROGRAM
			// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
			#pragma exclude_renderers gles
			#pragma fragment frag
			
			fixed4 _MainColor;
			sampler2D _MainTex;
			sampler2D _MaskTex;
			float _Cutoff;
			
			fixed4 frag (float2 uv : TEXCOORD0) : COLOR
			{
				clip( tex2D(_MaskTex, uv).a - (1 - _Cutoff) );
			    return tex2D(_MainTex, uv) * _MainColor;
			}
			ENDCG
		} 
	}

}
