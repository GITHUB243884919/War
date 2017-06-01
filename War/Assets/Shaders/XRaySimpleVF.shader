Shader "Custom/XRaySimpleVF" {  
    Properties {  
        _RimColor("RimColor",Color) = (1,1,0,1)  
        _RimPower ("Rim Power", Range(0.1,8.0)) = 3.0  
  
    }  
    SubShader {  
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }  
        LOD 200  
          
        Pass  
        {  
            Blend SrcAlpha One  
            ZWrite off  
            Lighting off  
  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  
            #include "UnityCG.cginc"  
  
            float4 _RimColor;  
            float _RimPower;  
              
            struct appdata_t {  
                float4 vertex : POSITION;  
                float2 texcoord : TEXCOORD0;  
                float4 color:COLOR;  
                float4 normal:NORMAL;  
            };  
  
            struct v2f {  
                float4  pos : SV_POSITION;  
                float4  color:COLOR;  
            } ;  
            v2f vert (appdata_t v)  
            {  
                v2f o;  
                o.pos = mul(UNITY_MATRIX_MVP,v.vertex);  
                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));  
                        float rim = 1 - saturate(dot(viewDir,v.normal ));  
                        o.color = _RimColor*pow(rim,_RimPower);  
                return o;  
            }  
            float4 frag (v2f i) : COLOR  
            {  
                return i.color;   
            }  
            ENDCG  
        }  
          
    }   
    FallBack "Diffuse"  
}  


