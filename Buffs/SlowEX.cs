using Terraria.Localization;

namespace SerumW.Buffs
{
    public class SlowEX : BaseBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Slowness");
            DisplayName.AddTranslation(GameCulture.Chinese, "迟缓");
            base.SetDefaults();
        }
    }
}