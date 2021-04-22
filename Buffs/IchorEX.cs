using Terraria.Localization;

namespace SerumW.Buffs
{
    public class IchorEX : BaseBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Ichor");
            DisplayName.AddTranslation(GameCulture.Chinese, "灵液");
            base.SetDefaults();
        }
    }
}