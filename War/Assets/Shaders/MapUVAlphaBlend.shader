Shader "Custom/MapUVAlphaBlend" 
{  
    Properties 
    {  
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Mask ("Mask (RGB)", 2D) = "white" {}  


        _Color ("Tint", Color) = (1,1,1,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15


		_MaskOffsetX ("Mask Offset X", Range(-0.5,0.5)) = -0.5
		_MaskOffsetY ("Mask Offset Y", Range(-0.5,0.5)) = -0.5

		_PixelTex ("PixelTex (RGB)", 2D) = "white" {}

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader 
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {         
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct a2v
            {
                fixed2 uv : TEXCOORD0;
                half4 vertex : POSITION;
                float4 color    : COLOR;
            };

            fixed4 _Color;

			float _MaskOffsetX;
			float _MaskOffsetY;

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                float4 color    : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _Mask;  
			sampler2D _PixelTex;

            v2f vert (a2v i)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
                o.uv = i.uv;

                o.color = i.color * _Color;
				//o.color = i.color;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
				fixed2 uvOffset = i.uv + fixed2(_MaskOffsetX , _MaskOffsetY);

				fixed4 color = tex2D(_MainTex, uvOffset) * i.color; 
				fixed4 mask = tex2D(_Mask, i.uv); 
				fixed4 pixelColor = tex2D(_PixelTex, uvOffset);

				fixed xcha = step(abs(saturate(uvOffset.x) - uvOffset.x) , 0);
				fixed ycha = step(abs(saturate(uvOffset.y) - uvOffset.y) , 0);

				fixed edge = (xcha * ycha);

				
				if(pixelColor.b < 0.5)
				{
					color.rgb = pixelColor.rgb;
					color.a = mask.a  * pixelColor.a;
				}else{
					fixed t = mask.a * edge;
					color.a = t * color.a;
				}
				
				return color;

            }
            ENDCG
        }  
    }   
}