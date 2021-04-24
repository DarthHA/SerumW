using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SerumW.Buffs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static SerumW.PlayerStatus;

namespace SerumW.Projectiles
{
    public class DamageProj : ModProjectile
    {
        public List<Biome> Effects = new List<Biome>();

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.timeLeft = 100;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 0 || projectile.ai[0] > 200)
            {
                projectile.Kill();
                return;
            }

            projectile.localAI[1]++;
            if (projectile.localAI[1] > 40)
            {
                projectile.Kill();
                return;
            }
            NPC target = Main.npc[(int)projectile.ai[0]];

            if (target.active)
            {
                projectile.Center = target.Center;
                if (projectile.localAI[1] <= 20)
                {
                    if (projectile.localAI[1] % 4 == 1)
                    {
                        Vector2 Pos = target.Center + new Vector2(Main.rand.Next(-24, 24), Main.rand.Next(-24, 24));
                        float randRotation = MathHelper.TwoPi * Main.rand.NextFloat();
                        float randLength = Main.rand.NextFloat() * 0.8f + 0.1f;
                        float randWidth = Main.rand.NextFloat() * 0.75f + 0.25f;
                        ClawSlash.GenClawSlash(Pos, randRotation, randLength * GetRanFloat(1.25f), randWidth);
                        ClawSlash.GenClawSlash(Pos + (randRotation + MathHelper.Pi / 2).ToRotationVector2() * 25 * randWidth, randRotation * GetRanFloat(1.25f), randLength, randWidth);
                        ClawSlash.GenClawSlash(Pos + (randRotation - MathHelper.Pi / 2).ToRotationVector2() * 25 * randWidth, randRotation * GetRanFloat(1.25f), randLength, randWidth);
                    }
                }
                if (projectile.localAI[1] == 1)
                {
                    Player owner = Main.player[projectile.owner];
                    if (owner.active)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Final"), owner.Center);
                    }
                }

                if (projectile.localAI[1] == 20)
                {
                    Player owner = Main.player[projectile.owner];

                    if (owner.active) 
                    {
                        ApplyDamage(owner, target);
                    } 
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public float GetRanFloat(float a)
        {
            float b = (float)Math.Abs(a - 1);
            return 1 + Main.rand.NextFloat() * b;
        }


        private void ApplyDamage(Player owner, NPC target)
        {
            int BaseDmg = projectile.damage * 2500;
            float SubDmg = 0.25f;
            float Range = 200;
            float bonus = owner.meleeDamage + owner.rangedDamage + owner.magicDamage + owner.minionDamage + owner.thrownDamage - 4;
            int dmg = (int)(BaseDmg * bonus);
            if (dmg < BaseDmg) dmg = BaseDmg;
            if (Effects.Contains(Biome.Space))
            {
                dmg = (int)(dmg * 1.2f);
            }
            if (Effects.Contains(Biome.Dungeon))
            {
                if (Main.rand.NextFloat() <= 0.05f)
                {
                    dmg = Utils.Clamp(dmg, 0, int.MaxValue / 101);
                    dmg *= 100;
                }
            }

            //造成伤害,对周围敌人造成25%溅射伤害，不带效果

            dmg = (int)target.StrikeNPC(dmg, 0, 0);
            owner.addDPS(dmg);
            foreach (NPC subtarget in Main.npc)
            {
                if (CanBeChasedBy(subtarget) && subtarget.Distance(target.Center) < Range && subtarget.whoAmI != target.whoAmI)
                {
                    int subdmg = (int)subtarget.StrikeNPC((int)(dmg * SubDmg), 0, 0);
                    owner.addDPS(subdmg);
                }
            }


            if (Effects.Contains(Biome.Snow))
            {
                target.buffImmune[ModContent.BuffType<SlowEX>()] = false;
                target.AddBuff(ModContent.BuffType<SlowEX>(), 600);
            }
            if (Effects.Contains(Biome.Desert))
            {
                target.buffImmune[ModContent.BuffType<WeakEX>()] = false;
                target.AddBuff(ModContent.BuffType<WeakEX>(), 600);
            }
            if (Effects.Contains(Biome.Corrupt))
            {
                target.buffImmune[ModContent.BuffType<CursedInferoEX>()] = false;
                target.AddBuff(ModContent.BuffType<CursedInferoEX>(), 600);
            }
            if (Effects.Contains(Biome.Crimson))
            {
                target.buffImmune[ModContent.BuffType<IchorEX>()] = false;
                target.AddBuff(ModContent.BuffType<IchorEX>(), 600);
            }
            if (Effects.Contains(Biome.Jungle))
            {
                target.buffImmune[ModContent.BuffType<VenomEX>()] = false;
                target.AddBuff(ModContent.BuffType<VenomEX>(), 1000);
            }
            if (Effects.Contains(Biome.Hallow))
            {
                target.buffImmune[ModContent.BuffType<StunnedEX>()] = false;
                target.AddBuff(ModContent.BuffType<StunnedEX>(), 180);
            }
            if (Effects.Contains(Biome.Hell))
            {
                target.buffImmune[ModContent.BuffType<OnFireEX>()] = false;
                target.AddBuff(ModContent.BuffType<OnFireEX>(), 300);
            }

        }

        public bool CanBeChasedBy(NPC npc)
        {
            if (npc.type == NPCID.Angler) return true;
            return npc.active && npc.lifeMax > 5 && !npc.dontTakeDamage && !npc.friendly && !npc.immortal;
        }
    }
}