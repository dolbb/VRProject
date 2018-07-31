Shader "Custom/Rest" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1, 0, 0, 1)
        _StencilVal ("stencilVal", Int) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        Stencil {
            Ref [_StencilVal]
            Comp NotEqual
        }
       
        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _MainTex;
        uniform fixed4 _Color;
 
        struct Input {
            float2 uv_MainTex;
        }; 
 
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}