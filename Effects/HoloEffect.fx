sampler uImage0 : register(s0);

float4x4 uTransform;
float alpha;

struct VSInput
{
	float2 Pos : POSITION0;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};

struct PSInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float3 Texcoord : TEXCOORD0;
};


float4 PixelShaderFunction1(PSInput input) : COLOR0
{
	float3 coord = input.Texcoord;
	float4 color = tex2D(uImage0, float2(coord.x,coord.y));
	if (1 - coord.y >= alpha) 
	{
		return float4(0, 0, 0, 0);
	}
	float a = (color.r + color.g + color.b) / 3;
	color = float4(color.r, color.g, color.b, a);
	return color;

}

float4 PixelShaderFunction2(PSInput input) : COLOR0
{
	float3 coord = input.Texcoord;
	float4 color = tex2D(uImage0, float2(coord.x,coord.y));
	if (1 - coord.y >= alpha)
	{
		return float4(0, 0, 0, 0);
	}
	float a = (color.r + color.g + color.b) / 3;
	color = float4(0.4f, 1, 1, a);
	return color;

}

PSInput VertexShaderFunction(VSInput input)
{
	PSInput output;
	output.Color = input.Color;
	output.Texcoord = input.Texcoord;
	output.Pos = mul(float4(input.Pos, 0, 1), uTransform);
	return output;
}


technique Technique1
{
	pass HoloEffectMain
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction1();
	}

	pass HoloEffectLine
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction2();
	}
}