#ifndef LUT_LIGHT_INCLUDED
#define LUT_LIGHT_INCLUDED

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
    
    // get replacement color from the lut tables set
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

    // blended lut interpolation from the lut tables set
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

void uv_screen_float(in float3 world, out float2 result)
{
    // uv screen from world, to make lighting with an offset from the z position
    float3 screen = mul(UNITY_MATRIX_VP, float4(world, 1)).xyw;
    screen.y *= _ProjectionParams.x;
    result = screen.xy / screen.z  * 0.5 + 0.5;
}

void uv_snap_float(in float3 world, in float texel, out float2 result)
{
    // uv screen from world, but pixelated
    float3 screen = mul(UNITY_MATRIX_VP, float4(world - world % texel, 1)).xyw;
    screen.y *= _ProjectionParams.x;
    result = screen.xy / screen.z  * 0.5 + 0.5;
}

void lum_float(in float3 col, out float val)
{
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