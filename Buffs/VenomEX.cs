using Terraria.Localization;

namespace SerumW.Buffs
{
    public class VenomEX : BaseBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Venom");
            DisplayName.AddTranslation(GameCulture.Chinese, "剧毒");
            base.SetDefaults();
        }
    }
}