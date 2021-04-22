sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float uOpacity;
float3 uSecondaryColor;
float uTime;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uImageOffset;
float uIntensity;
float uProgress;
float2 uDirection;
float2 uZoom;
float2 uImageSize0;
float2 uImageSize1;


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float x1 = uProgress + coords.x / 3;
	if (x1 > 1) x1 -= 1;

	float dx = 1 / uScreenResolution.x;

	float4 color2 = tex2D(uImage1, float2(x1, coords.y));
	float move = 75 * color2.r * dx;
	float4 color1 = tex2D(uImage0, float2(coords.x + move * uDirection.x, coords.y));
	return color1;
}

technique Technique1
{
	pass WarpEffect
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}