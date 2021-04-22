using Terraria.Localization;

namespace SerumW.Buffs
{
    public class WeakEX : BaseBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Weak");
            DisplayName.AddTranslation(GameCulture.Chinese, "虚♂弱");
            base.SetDefaults();
        }
    }
}