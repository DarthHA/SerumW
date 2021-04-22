using Terraria.ModLoader;

public class WLight : GlobalWall
{
	public static bool Light = false;
	public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
	{
		if (Light)
		{
			r = 1;
			g = 1;
			b = 1;
		}
	}
}