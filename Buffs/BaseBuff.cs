using Terraria;
using Terraria.ModLoader;

namespace SerumW.Buffs
{
    public abstract class BaseBuff : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "SerumW/Buffs/PlaceHolder";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}