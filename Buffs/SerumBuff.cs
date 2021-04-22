using Microsoft.Xna.Framework;
using SerumW.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SerumW.Buffs
{
    public class SerumBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Serum-W");
            DisplayName.AddTranslation(GameCulture.Chinese, "血清-W");
            Description.SetDefault("You feel you can tear space with your bare hands");
            Description.AddTranslation(GameCulture.Chinese, "你感觉你可以空手撕裂时空");
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = true;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Cyan;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool flag = false;
            foreach(Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<HoloBoard>() && proj.owner == player.whoAmI)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<HoloBoard>(), 0, 0, player.whoAmI);
            }
        }

    }


}