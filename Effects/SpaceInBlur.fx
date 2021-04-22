sampler uImage0 : register(s0);


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    color.a -= 0.1f;
    if (color.a < 0) color.a = 0;
    float r1 = sqrt((coords.x - 0.5f) * (coords.x - 0.5f) + (coords.y - 0.5f) * (coords.y - 0.5f));
    r1 = sqrt(r1);
    float r2 = r1 + 0.5f;
    if (r1 > 0.5f) r1 = 0.5f;
    return lerp(float4(1, 1, 1, color.a * r2), float4(0, 0.65f, 1, color.a * r2), r1 * 2) * 4;
}

technique Technique1
{
    pass SpaceInBlur
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }

}
