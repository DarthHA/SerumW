sampler uImage0 : register(s0);
sampler uImage1 : register(s1);


float4 PixelShaderFunction1(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color1 = tex2D(uImage0, coords);
    if (!any(color1))
        return color1;
    float4 color2 = tex2D(uImage1, coords);
    if (!any(color2))
        return color1;
    return float4(1, 1, 1, color1.r) * color2 * float4(0.15f, 0.2f, 0.5f, 1) * 16;
}

technique Technique1 
{
    pass SpaceBlur
    {
        PixelShader = compile ps_2_0 PixelShaderFunction1();
    }

}
