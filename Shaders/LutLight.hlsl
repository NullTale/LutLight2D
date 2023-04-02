#ifndef LUT_LIGHT_INCLUDED
#define LUT_LIGHT_INCLUDED

//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
//#include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Fullscreen.hlsl"
//#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

SamplerState _point_clamp_sampler;

real3 GetLinearToSRGB(real3 c)
{
#if _USE_FAST_SRGB_LINEAR_CONVERSION
    return FastLinearToSRGB(c);
#else
    return LinearToSRGB(c);
#endif
}

real3 GetSRGBToLinear(real3 c)
{
#if _USE_FAST_SRGB_LINEAR_CONVERSION
    return FastSRGBToLinear(c);
#else
    return SRGBToLinear(c);
#endif
}

/*real3 FastLinearToSRGB(real3 c)
{
    return saturate(1.055 * PositivePow(c, 0.416666667) - 0.055);
}
real3 LinearToSRGB(real3 c)
{
    real3 sRGBLo = c * 12.92;
    real3 sRGBHi = (PositivePow(c, real3(1.0/2.4, 1.0/2.4, 1.0/2.4)) * 1.055) - 0.055;
    real3 sRGB   = (c <= 0.0031308) ? sRGBLo : sRGBHi;
    return sRGB;
}
real3 FastSRGBToLinear(real3 c)
{
    return c * (c * (c * 0.305306011 + 0.682171111) + 0.012522878);
}
real3 SRGBToLinear(real3 c)
{
#if defined(UNITY_COLORSPACE_GAMMA) && REAL_IS_HALF
    c = min(c, 100.0); // Make sure not to exceed HALF_MAX after the pow() below
#endif
    real3 linearRGBLo  = c / 12.92;
    real3 linearRGBHi  = PositivePow((c + 0.055) / 1.055, real3(2.4, 2.4, 2.4));
    real3 linearRGB    = (c <= 0.04045) ? linearRGBLo : linearRGBHi;
    return linearRGB;
}*/
//#if defined(UNITY_COLORSPACE_GAMMA)
//#define GRADES 7.

#if defined(_LUT_SIZE_X16)
#define LUT_SIZE 16.
#define LUT_SIZE_MINUS (16. - 1.)
#endif
#if defined(_LUT_SIZE_X32)
#define LUT_SIZE 32.
#define LUT_SIZE_MINUS (32. - 1.)
#endif
#if defined(_LUT_SIZE_X64)
#define LUT_SIZE 64.
#define LUT_SIZE_MINUS (64. - 1.)
#endif


void lut_pix_float(in float3 col, in float lum, out float4 result)
{
    // sample the texture
#if !defined(UNITY_COLORSPACE_GAMMA)
    float3 uvw = GetLinearToSRGB(col);
#else
    float3 uvw = col;
#endif
    
    float2 uv;
    
    uv.y = (uvw.y * (LUT_SIZE_MINUS / LUT_SIZE) + .5 * (1. / LUT_SIZE)) * (1. / _Grades) + floor(lum * _Grades) / _Grades;
    uv.x = uvw.x * (LUT_SIZE_MINUS / (LUT_SIZE * LUT_SIZE)) + .5 * (1. / (LUT_SIZE * LUT_SIZE)) + floor(uvw.z * LUT_SIZE) / LUT_SIZE;

    float4 lutColor = _Lut.Sample(_point_clamp_sampler, uv);
    
#if !defined(UNITY_COLORSPACE_GAMMA)
    lutColor = float4(GetSRGBToLinear(lutColor.xyz), lutColor.w);
#endif

    result = lutColor;
}

void lut_pix_smooth_float(in float3 col, in float lum, out float4 result)
{
    // sample the texture
#if !defined(UNITY_COLORSPACE_GAMMA)
    float3 uvw = GetLinearToSRGB(col);
#else
    float3 uvw = col;
#endif
    
    float2 uv;
    
    uv.y = (uvw.y * (LUT_SIZE_MINUS / LUT_SIZE) + .5 * (1. / LUT_SIZE)) * (1. / _Grades) + floor(lum * _Grades) / _Grades;
    uv.x = uvw.x * (LUT_SIZE_MINUS / (LUT_SIZE * LUT_SIZE)) + .5 * (1. / (LUT_SIZE * LUT_SIZE)) + floor(uvw.z * LUT_SIZE) / LUT_SIZE;

    float4 lutSource = _Lut.Sample(_point_clamp_sampler, uv);
    
    uv.y = (uvw.y * (LUT_SIZE_MINUS / LUT_SIZE) + .5 * (1. / LUT_SIZE)) * (1. / _Grades) + clamp((floor(lum * _Grades) - 1), 0, _Grades - 1) / _Grades;
    uv.x = uvw.x * (LUT_SIZE_MINUS / (LUT_SIZE * LUT_SIZE)) + .5 * (1. / (LUT_SIZE * LUT_SIZE)) + floor(uvw.z * LUT_SIZE) / LUT_SIZE;
    
    float4 lutDest = _Lut.Sample(_point_clamp_sampler, uv);

    float4 lutColor = lerp(lutSource, lutDest, 1. - frac(lum * _Grades));
    
#if !defined(UNITY_COLORSPACE_GAMMA)
    lutColor = float4(GetSRGBToLinear(lutColor.xyz), lutColor.w);
#endif

    result = lutColor;
}

/*void uv_snap_float(in float2 uv, in float texel, out float2 result)
{
    // from uv to world, snap to texels, back to uv
    float2 world = (uv - .5) * unity_OrthoParams.xy + _WorldSpaceCameraPos.xy;
    world  = world - world % texel;
    result = (world - _WorldSpaceCameraPos.xy) / unity_OrthoParams.xy + .5;
}*/


void uv_screen_float(in float3 world, out float2 result)
{
    // from uv to world, snap to texels, back to uv
    float3 screen = mul(UNITY_MATRIX_MVP, float4(world, 1)).xyw;
    screen.y *= _ProjectionParams.x;
    result = screen.xy / screen.z  * 0.5 + 0.5;
}

void uv_snap_float(in float3 world, in float texel, out float2 result)
{
    // from uv to world, snap to texels, back to uv
    float3 screen = mul(UNITY_MATRIX_MVP, float4(world - world % texel, 1)).xyw;
    screen.y *= _ProjectionParams.x;
    result = screen.xy / screen.z  * 0.5 + 0.5;
}

void lum_float(in float3 col, out float val)
{
    //col = GetSRGBToLinear(col);
    // https://en.wikipedia.org/wiki/Grayscale
    //float3 gray = col * float3(0.299, 0.587, 0.114);        // rec601
    //float3 gray = sqrt( 0.299*col.r*col.r + 0.587*col.g*col.g + 0.114*col.b*col.b );
    //float3 gray = col * float3(0.2126, 0.7152, 0.0722);
    //float3 gray = col * float3(0.333, 0.333, 0.333);
    //lum = gray.x + gray.y + gray.z;
    //lum = col;
    val = Luminance(col);
}

void max_float(in float3 col, out float val)
{
    val = max(max(col.x, col.y), col.z);
}

void avg_float(in float3 col, out float val)
{
    val = (col.x + col.y + col.z) / 3.;
}

#endif // LUT_LIGHT_INCLUDED