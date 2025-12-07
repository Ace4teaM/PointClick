Shader "Custom/CircleTransitionShader"
{
    Properties
    {
        // couleur de l’overlay (le fond qui va recouvrir l’écran)
        _Color("Overlay Color", Color) = (0,0,0,0.75)
        // position du “trou” dans les coordonnées UV (0..1 sur l’écran), généralement 0.5,0.5 = centre
        _HolePos("Hole Position (UV)", Vector) = (0.5,0.5,0,0)
        // rayon du trou (normalisé 0..1 selon l’écran)
        _HoleRadius("Hole Radius (norm)", Float) = 0.2
        // bord flou du trou (pour que la transition soit douce)
        _Softness("Softness (norm)", Float) = 0.02
    }
    
    SubShader
    {
        // ShaderLab tags (Unity)
        // rendu transparent pour UI ou overlay 
        Tags { 
            "RenderType" = "Transparent" // Indique que le shader est transparent pour que le moteur sache comment le trier et gérer l’éclairage.
            "Queue" = "Transparent" // Spécifie la file de rendu : "Transparent", rendu après les objets opaques, avec tri alpha.
            "IgnoreProjector" = "True" // Indique que les projecteurs (Projector) Unity ne doivent pas affecter ce shader.
            "CanvasShader" = "True" // Utilisé pour les shaders UI, signale que le shader est compatible avec le système Canvas.
        }
        Blend SrcAlpha OneMinusSrcAlpha
        // affiche les deux faces du quad (utile pour UI fullscreen)
        Cull Off
        // n’écrit pas dans le depth buffer, sinon ça pourrait bloquer ce qui est derrière
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // données du vertex venant du mesh (quad full screen)
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            // données transmises au fragment shader
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            // couleur de l’overlay
            fixed4 _Color;
            // centre du trou
            float4 _HolePos; // xy = uv position
            // rayon du trou
            float _HoleRadius;
            // bord flou
            float _Softness;

            // Transforme les coordonnées du quad pour l’écran
            // Passe les UV et couleur au fragment shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            // Calcule la couleur du pixel à l'emplacement donnée
            // La couleur alpha est determiné par rapport à la distance du pixel par rapport au centre+dimetre du tour normalisé
            // 0 = à l'interieur
            // 1 = à l'exterieur
            // >0 à <1 = bordure du trou
            fixed4 frag(v2f i) : SV_Target
            {
                // UV 0..1 across the image (full screen)
                float2 uv = i.uv;

                // distance du pixel au centre du trou
                float d = distance(uv, _HolePos.xy);

                // soft mask: 1 outside the hole, 0 inside the hole with soft edge
                float edge0 = _HoleRadius - _Softness * 0.5;
                float edge1 = _HoleRadius + _Softness * 0.5;

                // crée un bord flou entre le trou et l’overlay
                // 1 à l’intérieur du trou, 0 à l’extérieur
                float holeMask = 1.0 - smoothstep(edge0, edge1, d);
                // alpha de l’overlay, 0 à l’intérieur du trou, 1 à l’extérieur
                float overlayMask = 1.0 - holeMask;

                fixed4 col = _Color;
                // applique le masque sur l’alpha du shader
                col.a *= saturate(overlayMask);

                // Retourne la couleur avec alpha, donc le trou devient transparent,
                // le reste de l’écran est recouvert par _Color
                return col;
            }
            ENDCG
        }
    }
}
