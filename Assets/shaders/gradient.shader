Shader "Custom/gradient" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Col1 ("Col1",Color) = (0.26,0.19,0.16,0.0)
		_Col2 ("Col2",Color) = (0.26,0.19,0.16,0.0)
		_OffsetTop("OffsetTop", Float) = 0.0
		_OffsetBottom("OffsetBottom", Float) = 0.0
	}
	SubShader {
		//Tags { "RenderType"="Opaque" }
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
      Lighting Off
	  Fog {Mode Off}
      CGPROGRAM
      #pragma surface surf NoLighting


fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
      {
        fixed4 c; c.rgb = s.Albedo; c.a = s.Alpha;
        return c;
      }

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};
		fixed4 _Col1;
		fixed4 _Col2;
		float _OffsetTop;
		float _OffsetBottom;
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = lerp(_Col1, _Col2, min(1, max(0, (IN.uv_MainTex.y - _OffsetBottom)) / (1.0f - (_OffsetTop + _OffsetBottom))));
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
