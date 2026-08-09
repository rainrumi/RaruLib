#include "UnityCG.cginc"
#include "UnityUI.cginc"
#include "../../../../../../TextMesh Pro/Shaders/TMPro_Properties.cginc"

struct vertex_t
{
    UNITY_VERTEX_INPUT_INSTANCE_ID
    float4 position : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
    float4 texcoord0 : TEXCOORD0;
    float2 texcoord1 : TEXCOORD1;
};

struct pixel_t
{
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
    float4 position : SV_POSITION;
    fixed4 faceColor : COLOR;
    fixed4 outlineColor : COLOR1;
    float2 atlas : TEXCOORD0;
    half3 param : TEXCOORD1;
    half4 mask : TEXCOORD2;
    float2 outlineUV : TEXCOORD3;
};

float _Amount;
float _Factor;
float _fps;

float4 _FaceTex_ST;
float4 _OutlineTex_ST;
float _UIMaskSoftnessX;
float _UIMaskSoftnessY;
int _UIVertexColorAlwaysGammaSpace;

float Random(float2 p)
{
    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
}

float4 ApplyLineBoil(float4 position, float4 sourcePosition)
{
    float switchDuration = 1 / _fps * 2;
    float switchTime = 1 / _fps;
    float currentSwitchDuration = _Time.y % switchDuration;
    float switchValue = step(switchTime, currentSwitchDuration);
    float noiseFactor = (Random((sourcePosition + switchValue).xy) * 2 - 1) * _Factor * _Amount;
    position.xy += noiseFactor;

    return position;
}

pixel_t VertShader(vertex_t input)
{
    pixel_t output;

    UNITY_INITIALIZE_OUTPUT(pixel_t, output);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    float bold = step(input.texcoord0.w, 0);

    float4 vert = input.position;
    vert.x += _VertexOffsetX;
    vert.y += _VertexOffsetY;

    float4 vPosition = UnityObjectToClipPos(vert);
    float2 pixelSize = vPosition.w;
    pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

    float scale = rsqrt(dot(pixelSize, pixelSize));
    scale *= abs(input.texcoord0.w) * _GradientScale * (_Sharpness + 1);
    if (UNITY_MATRIX_P[3][3] == 0)
    {
        scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));
    }

    float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
    weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

    scale /= 1 + (_OutlineSoftness * _ScaleRatioA * scale);
    float bias = (0.5 - weight) * scale - 0.5;
    float outline = _OutlineWidth * _ScaleRatioA * 0.5 * scale;

    if (_UIVertexColorAlwaysGammaSpace && !IsGammaSpace())
    {
        input.color.rgb = UIGammaToLinear(input.color.rgb);
    }

    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);

    output.position = ApplyLineBoil(vPosition, input.position);
    output.atlas = input.texcoord0.xy;
    output.param = half3(scale, bias, outline);
    const half2 maskSoftness = half2(max(_UIMaskSoftnessX, _MaskSoftnessX), max(_UIMaskSoftnessY, _MaskSoftnessY));
    output.mask = half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * maskSoftness + pixelSize.xy));

    // _FaceColor stays white as the material fallback, so TMP's per-vertex color remains authoritative.
    output.faceColor = input.color * _FaceColor;
    output.faceColor.rgb *= output.faceColor.a;

    output.outlineColor = _OutlineColor;
    output.outlineColor.a *= input.color.a;
    output.outlineColor.rgb *= output.outlineColor.a;
    output.outlineUV = TRANSFORM_TEX(input.texcoord1, _OutlineTex);

    return output;
}

fixed4 PixShader(pixel_t input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);

    half distance = tex2D(_MainTex, input.atlas).a * input.param.x;
    half faceCoverage = saturate(distance - input.param.y);
    half expandedCoverage = saturate(distance - (input.param.y - input.param.z));
    half outlineCoverage = saturate(expandedCoverage - faceCoverage);

    fixed4 color = input.faceColor * faceCoverage;
    color += input.outlineColor
        * tex2D(_OutlineTex, input.outlineUV + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y)
        * outlineCoverage;

#if UNITY_UI_CLIP_RECT
    half2 mask = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
    color *= mask.x * mask.y;
#endif

#if UNITY_UI_ALPHACLIP
    clip(color.a - 0.001);
#endif

    return color;
}
