using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SerumW
{
    public class SerumWConfig : ModConfig
    {
        public override bool Autoload(ref string name)
        {
            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                name = "配置";
            }
            else
            {
                name = "Config";
            }
            return true;
        }
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        [Label("$Mods.SerumW.UseBGM")]
        public bool UseBGM;

        


        public override ModConfig Clone()
        {
            var clone = (SerumWConfig)base.Clone();
            return clone;
        }

        public override void OnLoaded()
        {
            SerumW.config = this;
            ModTranslation modTranslation = SerumW.Instance.CreateTranslation("UseBGM");
            modTranslation.SetDefault("Plays BGM when Serum W attack is ready");
            modTranslation.AddTranslation(GameCulture.Chinese, "血清-W攻击就绪时播放bgm");
            SerumW.Instance.AddTranslation(modTranslation);
        }


        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string messageline)
        {
            string message = "";
            string messagech = "";

            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                messageline = messagech;
            }
            else
            {
                messageline = message;
            }

            if (whoAmI == 0)
            {
                //message = "Changes accepted!";
                //messagech = "设置改动成功!";
                return true;
            }
            if (whoAmI != 0)
            {
                //message = "You have no rights to change config.";
                //messagech = "你没有设置改动权限.";
                return false;
            }
            return false;
        }
    }
}