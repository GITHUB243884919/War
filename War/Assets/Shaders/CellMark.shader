Shader "Custom/CellMark" 
{  
    Properties 
    {  
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}


        _Speed ("Speed", Range(0, 30)) = 10  
    }

    SubShader 
    {
        Tags
        { 
            "Queue"="Transparent" 
        }



        //Cull Off
        Lighting Off
        ZWrite Off
        ZTest Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {         
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //#include "UnityCG.cginc"


            struct a2v
            {
                fixed2 uv : TEXCOORD0;
                half4 vertex : POSITION;
                float4 color    : COLOR;
            };


            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                float4 color    : COLOR;
            };

            sampler2D _MainTex;

			float _Speed;

            v2f vert (a2v i)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
                o.uv = i.uv;
                o.color = i.color;

                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
				half4 color = tex2D(_MainTex,  i.uv ) * i.color; 
				color.a = (cos(_Speed * _Time.y)+1)/2 * color.a;  
				return color;
            }
            ENDCG
        }  
    }   
}