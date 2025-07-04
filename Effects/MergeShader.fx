sampler2D inputTexture : register(s0);

float threshold;

float4 MainPS(float2 uv : TEXCOORD0) : COLOR
{
    float alpha = tex2D(inputTexture, uv).a;
    float merged = step(threshold, alpha);
    return float4(1.0, 1.0, 1.0, merged);
}

technique Technique1
{
    pass MergePass
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}
