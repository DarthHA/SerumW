using Terraria;
using Terraria.ModLoader;

namespace SerumW
{
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

	public class WTile : GlobalTile
    {
        public override bool CanExplode(int i, int j, int type)
        {
			return !Main.LocalPlayer.GetModPlayer<ClawPlayer>().IsWarping;
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
			return !Main.LocalPlayer.GetModPlayer<ClawPlayer>().IsWarping;
		}

        public override bool CanPlace(int i, int j, int type)
        {
			return !Main.LocalPlayer.GetModPlayer<ClawPlayer>().IsWarping;
		}

        public override bool Dangersense(int i, int j, int type, Player player)
        {
			return !Main.LocalPlayer.GetModPlayer<ClawPlayer>().IsWarping;
		}

        public override bool PreHitWire(int i, int j, int type)
        {
			return !Main.LocalPlayer.GetModPlayer<ClawPlayer>().IsWarping;
		}
    }

	public class WGItem : GlobalItem
    {
        public override bool CanPickup(Item item, Player player)
        {
			return !player.GetModPlayer<ClawPlayer>().IsWarping;
		}
    }
}