Shader "Custom/AlphaMaskTied2" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Mask ("Mask" , 2D) = "white" {}
		
		_Cutoff("Alpha cutoff" , Range(0,1)) = 0.1
		_Scale("scale" , float) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent" }
		Lighting off
		ZWrite off
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest GEqual [_Cutoff]
		Pass
		{
			SetTexture [_Mask] {combine texture}
			SetTexture [_MainTex] {combine texture,texture-previous}
		}
	} 
	FallBack "Diffuse"
}
