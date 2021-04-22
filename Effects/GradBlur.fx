sampler uImage0 : register(s0);

float alpha;
float k;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    float k1;
    if (coords.x > k)
    {
        k1 = (coords.x - k) * 2;
    }
    else
    {
        k1 = (k - coords.x) * 2;
    }
    if (k1 > 1) k1 = 1;
    return float4(0.4, 1.0, 1.0, color.r) * k1 * alpha;
}



technique Technique1
{
    pass GradBlur
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
