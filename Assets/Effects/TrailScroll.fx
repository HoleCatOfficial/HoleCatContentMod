float4x4 WorldViewProjection;
float ScrollSpeed;
float TotalTime;
texture Texture0;

sampler TextureSampler = sampler_state {
    Texture = <Texture0>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput {
    float4 Position : POSITION0;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput {
    float4 Position : SV_POSITION;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VS_Main(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;

    float2 uv = input.TexCoord;
    uv.x += TotalTime * ScrollSpeed;  
    output.TexCoord = uv;

    return output;
}

float4 PS_Main(VertexShaderOutput input) : COLOR
{
    float4 tex = tex2D(TextureSampler, input.TexCoord);
    return tex * input.Color;
}

technique ScrollingUV
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS_Main();
        PixelShader  = compile ps_3_0 PS_Main();
    }
}
