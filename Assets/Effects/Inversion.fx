sampler uImage0 : register(s0); // The contents of the screen.
sampler uImage1 : register(s1); // Up to three extra textures you can use for various purposes (for instance as an overlay).
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition; // The position of the camera.
float2 uTargetPosition; // The "target" of the shader, what this actually means tends to vary per shader.
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect; // Doesn't seem to be used, but included for parity.
float2 uZoom;
float uRadius;
float uGlowStrength;
float2 uCenter;

float4 InversionPass(float2 uv : TEXCOORD0) : COLOR
{
    float4 dust = tex2D(uImage1, uv);
    float4 screen = tex2D(uImage0, uv);

    float mask = dust.a;

    float3 inverted = 1.0 - screen.rgb;

    float3 result = lerp(screen.rgb, inverted, mask);

    return float4(result, screen.a);
}

Technique Inversion
{
    pass InversionPass
    {
        PixelShader = compile ps_2_0 InversionPass();
    }
}