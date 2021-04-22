using System.Collections.Generic;
using Terraria;


namespace SerumW
{
    public static class PlayerStatus
    {
        public enum Biome
        {
            Purity,
            Snow,
            Desert,
            Corrupt,
            Crimson,
            Jungle,
            Dungeon,
            Hallow,
            Space,
            Hell
        };
        /*
            纯净：威力 * 1.2
            雪原：减速50%
            沙漠：降低90%接触伤害
            腐化：咒火EX,0.3%
            猩红：灵液EX 双减甲
            丛林/蘑菇：中毒EX 0.2%
            地牢：20%几率威力 * 3
            神圣：击晕
            太空：溅射伤害 25%
            地狱/陨石：着火EX  0.75&
        */

        public static void GetBiome(this Player player, List<Biome> biomes)
        {
            bool purity = false;
            if (player.ZoneSnow)
            {
                purity = true;
                biomes.Add(Biome.Snow);
            }
            if (player.ZoneDesert || player.ZoneUndergroundDesert)
            {
                purity = false;
                biomes.Add(Biome.Desert);
            }
            if (player.ZoneCorrupt)
            {
                purity = false;
                biomes.Add(Biome.Corrupt);
            }
            if (player.ZoneCrimson)
            {
                purity = false;
                biomes.Add(Biome.Crimson);
            }
            if (player.ZoneJungle || player.ZoneGlowshroom)
            {
                purity = false;
                biomes.Add(Biome.Jungle);
            }
            if (player.ZoneDungeon)
            {
                purity = false;
                biomes.Add(Biome.Dungeon);
            }
            if (player.ZoneHoly)
            {
                purity = false;
                biomes.Add(Biome.Hallow);
            }
            if (player.ZoneSkyHeight)
            {
                purity = false;
                biomes.Add(Biome.Space);
            }
            if(player.ZoneUnderworldHeight || player.ZoneMeteor)
            {
                purity = false;
                biomes.Add(Biome.Hell);
            }
            if (purity)
            {
                biomes.Add(Biome.Purity);
            }
        }
    }
}