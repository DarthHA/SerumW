sampler uImage0 : register(s0);

float alpha;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float k = abs(coords.y - 0.5f) * 8;
    if (k > 1) k = 1;
    float4 c = lerp(float4(1, 1, 1, 1), float4(0.4f, 1, 1, 1), k);
    if (!any(color))
        return color;
    if (abs(coords.y - 0.5) * 2 > alpha) return float4(0, 0, 0, 0);
    return float4(c.r, c.g, c.b, color.r) * 0.75f;
}



technique Technique1
{
    pass SpaceCrashBlur
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
