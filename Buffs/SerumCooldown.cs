using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SerumW.Buffs
{
    public class SerumCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Serum Resistance");
            DisplayName.AddTranslation(GameCulture.Chinese, "血清抗性");
            Description.SetDefault("You can't use Serum W for now");
            Description.AddTranslation(GameCulture.Chinese,"你暂时不能使用血清-W");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            longerExpertDebuff = false;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Gray;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.GetModPlayer<ClawPlayer>().WarpingCD == 0 || player.GetModPlayer<ClawPlayer>().IsWarping)
            {
                if (player.buffTime[buffIndex] > 1)
                {
                    player.buffTime[buffIndex] = 1;
                }
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] = time;
            return true;
        }
    }


}