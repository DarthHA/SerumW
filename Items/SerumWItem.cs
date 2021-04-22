using Microsoft.Xna.Framework;
using SerumW.Buffs;
using SerumW.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SerumW.Items
{
    public class SerumWItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Serum W");
			DisplayName.AddTranslation(GameCulture.Chinese, "血清 - W");
			Tooltip.SetDefault(
				"Use this for the first time to activate serum, then select an enemy to launch a strong attack\n" +
				"1 minute duration\n" +
				"\'It contains a singularity technology\'");
			Tooltip.AddTranslation(GameCulture.Chinese, 
				"首次使用激活血清，然后选中一名敌人发动猛攻\n" +
				"1分钟持续时间\n" +
				"“奇点技术，震撼人心”");
		}

		public override void SetDefaults() 
		{
			item.damage = 28;
			item.noMelee = true;
			item.width = 39;
			item.height = 46;
			item.maxStack = 999;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.knockBack = 6;
			item.value = Item.sellPrice(1, 0, 0, 0);
			item.rare = ItemRarityID.Cyan;
			item.noUseGraphic = true;
			item.shoot = ModContent.ProjectileType<SpaceCrash>();
			item.shootSpeed = 12;
			item.autoReuse = false;
		}

        public override bool CanUseItem(Player player)
        {
			if(player.GetModPlayer<ClawPlayer>().WarpingCD != 0)
            {
				return false;
            }
            if (player.HasBuff(ModContent.BuffType<SerumBuff>()))
            {
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.noUseGraphic = true;
				item.useTime = 10;
				item.useAnimation = 10;
				item.autoReuse = true;
			}
            else
            {
				item.useStyle = ItemUseStyleID.HoldingUp;
				item.noUseGraphic = false;
				item.useTime = 90;
				item.useAnimation = 90;
				item.autoReuse = false;
            }
			return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (player.HasBuff(ModContent.BuffType<SerumBuff>()))
			{
				int targetNPC = -1;
				foreach (NPC target in Main.npc)
				{
					if (CanBeChasedBy(target))
					{
						if (Contains(target, Main.MouseWorld))
						{
							if (target.realLife != -1)
							{
								targetNPC = Main.npc[target.realLife].whoAmI;
								break;
							}
							else
							{
								targetNPC = target.whoAmI;
								break;
							}
						}
					}
				}

				if (targetNPC != -1)
				{
					foreach(Projectile proj in Main.projectile)          //一个细节Fix
                    {
						if (proj.active && proj.type == ModContent.ProjectileType<HoloBoard>() && proj.owner == player.whoAmI)
                        {
							proj.Kill();
                        }
                    }
					player.GetModPlayer<ClawPlayer>().InitWarp(targetNPC);
					player.ClearBuff(ModContent.BuffType<SerumBuff>());
				}
			}
            else
            {
				Projectile.NewProjectile(player.Center + new Vector2(0, 10), Vector2.Zero, ModContent.ProjectileType<InjectDust>(), 0, 0, player.whoAmI);
				Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<InjectCircle>(), 0, 0, player.whoAmI);
				player.AddBuff(ModContent.BuffType<SerumBuff>(), 60 * 60);
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Injection"), player.Center);
			}
			return false;
		}

		public bool CanBeChasedBy(NPC npc)
		{
			return npc.active && npc.lifeMax > 5 && !npc.dontTakeDamage && !npc.friendly && !npc.immortal;
		}


        private bool Contains(NPC npc, Vector2 Pos)
		{
			int width = npc.width < 16 ? 16 : npc.width;
			int height = npc.height < 16 ? 16 : npc.height;

			if (Pos.X > npc.Center.X - width / 2 && Pos.X < npc.Center.X + width / 2)
			{
				if (Pos.Y > npc.Center.Y - height / 2 && Pos.Y < npc.Center.Y + height / 2)
				{
					return true;
				}
			}
			return false;
		}
	}
}