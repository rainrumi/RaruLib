Shader "RaruLib/LineBoil/LineBoil_Outline"
{
    Properties
    {
        _Amount ("Amount", Range(0, 1)) = 0.02
        _Factor ("Factor", float) = 0.05
        _fps ("fps", float) = 10.0

        _FaceTex ("Face Texture", 2D) = "white" {}
        _FaceUVSpeedX ("Face UV Speed X", Range(-5, 5)) = 0.0
        _FaceUVSpeedY ("Face UV Speed Y", Range(-5, 5)) = 0.0
        _FaceColor ("Face Color", Color) = (1,1,1,1)
        _FaceDilate ("Face Dilate", Range(-1,1)) = 0

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineTex ("Outline Texture", 2D) = "white" {}
        _OutlineUVSpeedX ("Outline UV Speed X", Range(-5, 5)) = 0.0
        _OutlineUVSpeedY ("Outline UV Speed Y", Range(-5, 5)) = 0.0
        _OutlineWidth ("Outline Thickness", Range(0, 1)) = 0.1
        _OutlineSoftness ("Outline Softness", Range(0,1)) = 0

        _WeightNormal ("Weight Normal", float) = 0
        _WeightBold ("Weight Bold", float) = 0.5

        _ShaderFlags ("Flags", float) = 0
        _ScaleRatioA ("Scale RatioA", float) = 1
        _ScaleRatioB ("Scale RatioB", float) = 1
        _ScaleRatioC ("Scale RatioC", float) = 1

        _MainTex ("Font Atlas", 2D) = "white" {}
        _TextureWidth ("Texture Width", float) = 512
        _TextureHeight ("Texture Height", float) = 512
        _GradientScale ("Gradient Scale", float) = 5.0
        _ScaleX ("Scale X", float) = 1.0
        _ScaleY ("Scale Y", float) = 1.0
        _PerspectiveFilter ("Perspective Correction", Range(0, 1)) = 0.875
        _Sharpness ("Sharpness", Range(-1,1)) = 0

        _VertexOffsetX ("Vertex OffsetX", float) = 0
        _VertexOffsetY ("Vertex OffsetY", float) = 0

        _ClipRect ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
        _MaskSoftnessX ("Mask SoftnessX", float) = 0
        _MaskSoftnessY ("Mask SoftnessY", float) = 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _CullMode ("Cull Mode", Float) = 0
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull [_CullMode]
        ZWrite Off
        Lighting Off
        Fog { Mode Off }
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        // Compose the face and outline in one pass so CanvasRenderer cannot drop the face pass.
        Pass
        {
            Name "LineBoilOutline"

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex VertShader
            #pragma fragment PixShader
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            #include "LineBoil_Outline.hlsl"
            ENDCG
        }
    }

    Fallback "TextMeshPro/Mobile/Distance Field"
    CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
