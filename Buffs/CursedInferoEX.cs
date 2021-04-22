using Terraria.Localization;

namespace SerumW.Buffs
{
    public class CursedInferoEX : BaseBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Cursed Infero");
            DisplayName.AddTranslation(GameCulture.Chinese, "诅咒焰");
            base.SetDefaults();
        }
    }
}