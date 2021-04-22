using Terraria.Localization;

namespace SerumW.Buffs
{
    public class OnFireEX : BaseBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("On Fire!");
            DisplayName.AddTranslation(GameCulture.Chinese, "着火了！");
            base.SetDefaults();
        }
    }
}