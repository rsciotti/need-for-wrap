Shader "Unlit/InverseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Pass
        {
            CGPROGRAM

            #pragma vertex vertexFunc
            #pragma fragment fragmentFunc

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _MainTex;

            v2f vertexFunc (appdata IN)
            {
                v2f OUT;

                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;

                return OUT;
            }

            fixed4 fragmentFunc (v2f IN) : SV_Target
            {
                fixed4 pixelColor = tex2D(_MainTex, IN.uv);

                return _Color - pixelColor;
            }
            ENDCG
        }
    }
}
