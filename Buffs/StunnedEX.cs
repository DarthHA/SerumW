using Terraria.Localization;

namespace SerumW.Buffs
{
    public class StunnedEX : BaseBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Stunned");
            DisplayName.AddTranslation(GameCulture.Chinese, "击晕");
            base.SetDefaults();
        }
    }
}