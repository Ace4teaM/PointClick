Shader "Custom/SpriteWhiteToAlphaPerObject"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Threshold("Threshold", Range(0, 1)) = 0.5
        _Smoothness("Smoothness", Range(0, 1)) = 0.1
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
                "RenderPipeline" = "UniversalPipeline"
            }
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR;
                    half2 texcoord : TEXCOORD0;
                };

                sampler2D _MainTex;
                float _Threshold;
                float _Smoothness;
                float _GlobalAlpha; // Cette propriété sera définie par script
                fixed4 _TintColor; // Cette propriété sera définie par script
                float tintStrength = 1.0;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = v.texcoord;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                //float luminance = (col.r + col.g + col.b) / 3.0;
                    float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));
                    //col.a = smoothstep(_Threshold, _Threshold + _Smoothness, luminance);
                    // applique la teinte
                    col.rgb = _TintColor.rgb;
                    //col.rgb = saturate(col.rgb);
                    // applique la luminosité
//                    col = col * luminance;
                    col.a = luminance * _GlobalAlpha; // Applique l'opacité globale
                    return col;
                }
                ENDCG
            }
        }
}
