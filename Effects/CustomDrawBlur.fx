sampler uImage0 : register(s0);

float alpha;
float r;
float g;
float b;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    return float4(r, g, b, color.r) * alpha;
}



technique Technique1
{
    pass CustomDrawBlur
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
