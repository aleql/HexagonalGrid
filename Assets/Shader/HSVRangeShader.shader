// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/HSVRangeShader"
{
    Properties
    {
       _MainTex("Sprite Texture", 2D) = "white" {}
       _Color("Alpha Color Key", Color) = (0,0,0,1)
       _HSVRangeMin("HSV Affect Range Min", Range(0, 1)) = 0
       _HSVRangeMax("HSV Affect Range Max", Range(0, 1)) = 1
       _HSVAAdjust("HSVA Adjust", Vector) = (0, 0, 0, 0)
       _StencilComp("Stencil Comparison", Float) = 8
       _Stencil("Stencil ID", Float) = 0
       _StencilOp("Stencil Operation", Float) = 0
       _StencilWriteMask("Stencil Write Mask", Float) = 255
       _StencilReadMask("Stencil Read Mask", Float) = 255
       _ColorMask("Color Mask", Float) = 15
    }
        SubShader
       {
           Tags
           {
               "RenderType" = "Transparent"
               "Queue" = "Transparent"
           }

           Stencil
           {
               Ref[_Stencil]
               Comp[_StencilComp]
               Pass[_StencilOp]
               ReadMask[_StencilReadMask]
               WriteMask[_StencilWriteMask]
           }
           ColorMask[_ColorMask]

           Pass
           {
               Cull Off
               ZWrite Off
               Blend SrcAlpha OneMinusSrcAlpha

               CGPROGRAM
               #pragma vertex vert
               #pragma fragment frag
               #pragma multi_compile DUMMY PIXELSNAP_ON

               sampler2D _MainTex;
               float4 _Color;
               float _HSVRangeMin;
               float _HSVRangeMax;
               float4 _HSVAAdjust;

               struct Vertex
               {
                   float4 vertex : POSITION;
                   float2 uv_MainTex : TEXCOORD0;
               };

               struct Fragment
               {
                   float4 vertex : POSITION;
                   float2 uv_MainTex : TEXCOORD0;
               };

               Fragment vert(Vertex v)
               {
                   Fragment o;

                   o.vertex = UnityObjectToClipPos(v.vertex);
                   o.uv_MainTex = v.uv_MainTex;

                   return o;
               }

               float3 rgb2hsv(float3 c) {
                 float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                 float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                 float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                 float d = q.x - min(q.w, q.y);
                 float e = 1.0e-10;
                 return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
               }

               float3 hsv2rgb(float3 c) {
                 c = float3(c.x, clamp(c.yz, 0.0, 1.0));
                 float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                 float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                 return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
               }

               float4 frag(Fragment IN) : COLOR
               {
                   float4 o = float4(1, 0, 0, 0.2);

                   float4 color = tex2D(_MainTex, IN.uv_MainTex);
                   float3 hsv = rgb2hsv(color);
                   float affectMult = step(_HSVRangeMin, hsv.r) * step(hsv.r, _HSVRangeMax);
                   float3 rgb = hsv2rgb(hsv + _HSVAAdjust.xyz * affectMult);
                   //float3 alpha = lerp(_HSVAAdjust.a, color.a, 1);
                   return float4(rgb, color.a + _HSVAAdjust.a);
               }

               ENDCG
           }
       }
}