sampler uImage0 : register(s0);

float alpha;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    return color * alpha;
}



technique Technique1
{
    pass ClawSlashBlur
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
