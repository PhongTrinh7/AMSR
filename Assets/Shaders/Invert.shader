Shader "Custom/Invert"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}

        _Color("Tint", Color) = (1,1,1,1)

        // required for UI.Mask
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
    }
    SubShader
    {
        // required for UI.Mask
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        ColorMask[_ColorMask]

        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            

            float2 center = float2(0.5f, 0.5f);

            fixed4 frag(v2f i) : SV_Target
            {
                float2 offset = float2(tex2D(_NoiseTex, float2(i.uv.x, 0)).r, tex2D(_NoiseTex, float2(0,i.uv.y)).r);

                //fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - sin(col.rgb * _Time[2]);

                half4 color = tex2D(_MainTex, i.uv + (float2(0, sin(i.uv.x * 2 + _Time[2])/10) + sin(_Time[1] + offset)/25));

                color.rgb = 1 - color.rgb;
                
                return color;
            }
            ENDCG
        }
    }
}
