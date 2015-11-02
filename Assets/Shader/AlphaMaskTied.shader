Shader "Custom/AlphaMaskTied" {
   Properties
    {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _MaskTex ("Mask (A)", 2D) = "white" {}
    _Progress ("Progress", Range(0,1)) = 0.5
    _Alpha("Alpha" , Range(0,1)) = 1
    
    _PosX ("Position X" , float) = 0
    _PosY ("Position Y" , float) = 0
    
    _Depth ("render queue depth" , int) = 0
   
    
    _Scale("Scale" , float ) = 0 
    }
 
    Category
    {
        Lighting Off
        ZWrite Off
        Cull back
        Fog { Mode Off }
        Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                sampler2D _MainTex;
                sampler2D _MaskTex;
//                half4 _MainTex_TexelSize;
                float _Progress;
                float _Resize;
                float _Alpha;
                float _PosX;
                float _PosY;
                float _Scale;
                
                struct appdata
                {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };
                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float2 muv : TEXCOORD1;
                };
                v2f vert (appdata v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = v.texcoord.xy;
                   
                    float2 pos;
                    o.muv.x = ( o.uv.x - 0.5 - _PosX ) / _Scale + 0.5;
                    o.muv.y = ( o.uv.y - 0.5 - _PosY ) / _Scale + 0.5;
                    
                    o.muv.x = max ( min(o.muv.x , 1 ) , 0 );
                    o.muv.y = max ( min(o.muv.y , 1 ) , 0 );
//					o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
//                   	float3 wpos = mul(_Object2World, v.vertex).xyz;
//                   	float4 p = UNITY_MATRIX_MVP[3];
//                   	p.xy /= p.w;
//                   	o.muv = 0.5*(p.xy+1.0);
                    return o;
                }
                half4 frag(v2f i) : COLOR
                {
                  fixed4 mainColor = tex2D( _MainTex, i.uv );
                  fixed4 maskColor = tex2D( _MaskTex , i.muv );
                  fixed4 col = mainColor;
                  col.a = ( 1 - maskColor.a) * _Alpha;
                    return col;
                }
                ENDCG
            }
        }
 
        SubShader
        {          
             AlphaTest LEqual [_Progress] 
              Pass 
              { 
                 SetTexture [_MaskTex] {combine texture} 
                 SetTexture [_MainTex] {combine texture, previous} 
              } 
        }
         
    }
    Fallback "Transparent/VertexLit"
}