sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color1 = tex2D(uImage0, coords);
    float4 color2 = tex2D(uImage1, coords);
    if (!any(color1))
        return color1;

    if (!any(color2))
        return color2;

    return float4(0.4f, 1, 1, 1) * color1.r * color2.a;
}

technique Technique1
{
    pass UIEffect
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }

}
