Shader "Custom/ConverShader" {
    Properties {
        _MainTex ("Tint Color (RGB)", 2D) = "white" {}
        _ChangeAngle("Change Angle" , float ) = 0
    }
   
    Category {
   
        // We must be transparent, so other objects are drawn before this one.
        Tags { "Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="Opaque" }
   
        SubShader {
            // Horizontal blur
            GrabPass {                     
                Tags { "LightMode" = "Always" }
            }
            
            Pass {
                AlphaTest Greater [_Cutoff]  
                
                Tags { "LightMode" = "Always" }
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
               
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float4 uv: TEXCOORD1;
                };
                
				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _ChangeAngle;
               
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    
                    //for the image
					o.uv.xy = TRANSFORM_TEX (v.texcoord, _MainTex);
                    return o;
                }
               
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Size;
               
                half4 frag( v2f i ) : COLOR {
	              half4 col_grab = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
	              half4 col_tex = tex2Dproj( _MainTex , i.uv );
				  half4 col = 0;
				  
				  
	//				  if ( col.a > 0 )
//				  {
//					  col.r = 1 - col_grab.r;
//					  col.g = 1 - col_grab.g;
//					  col.b = 1 - col_grab.b;
//				  }
//				if ( col_tex.a <= 0 )
//					return col_grab;
                
	                float cmin = min(col_grab.r , min(col_grab.g,col_grab.b));
	                float cmax = max(col_grab.r , max(col_grab.g,col_grab.b));
	                  
	                float h = 0;  
	                float s = 1;
	                float v = 1;
	                if ( cmax == cmin )
	                	h = 0;
	                if ( cmax == col_grab.r && col_grab.g >= col_grab.b )
	                	h = 60 * ( col_grab.g - col_grab.b)/(cmax - cmin);
	                if ( cmax == col_grab.r && col_grab.g < col_grab.b )
	                	h = 60 * ( col_grab.g - col_grab.b ) / (cmax - cmin) + 360;
	                if ( cmax == col_grab.g )
	                	h = 60 * (col_grab.b - col_grab.r ) / (cmax - cmin) + 120;
	                if ( cmax == col_grab.b )
	                	h = 60 * (col_grab.r - col_grab.g ) / (cmax - cmin ) + 240;
	                  
	                if ( cmax == 0 )
	                	s = 0;
	                else
	                	s = 1 - cmin / cmax;
	                
	                v = cmax;
	                
	                // change h 
	                h = fmod(lerp( h , _ChangeAngle , 0.33) , 360);
	                ////////////
	                  
	                float hi = floor(fmod(floor(h/60),6));
					float f = h / 60-hi;
					float p = v * ( 1 - s );
					float q = v * ( 1 - f * s );
					float t = v * ( 1 - ( 1 - f ) * s );
					
					if ( hi == 0 )
					{
						col.r = v; col.g = t; col.b = p;
					}
					if ( hi == 1 )
					{
						col.r = q; col.g = v; col.b = p;
					}
					if ( hi == 2 )
					{
						col.r = p; col.g = v; col.b = t;
					}
					if ( hi == 3 )
					{
						col.r = p; col.g = q; col.b = v;
					}
					if ( hi == 4 )
					{
						col.r = t; col.g = p; col.b = v;
					}
					if ( hi == 5 )
					{
						col.r = v; col.g = p; col.b = q;
					}
					
					
	                return col;
	                
                }
                ENDCG
            }
        }
    }
	FallBack "Diffuse"
}
