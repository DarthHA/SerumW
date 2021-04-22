using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SerumW.Buffs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static SerumW.PlayerStatus;

namespace SerumW.Projectiles
{
    class DamageProj : ModProjectile
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
            int BaseDmg = projectile.damage * 1000;
            float bonus = owner.meleeDamage + owner.rangedDamage + owner.magicDamage + owner.minionDamage + owner.thrownDamage - 4;
            int dmg = (int)(BaseDmg * bonus);
            if (dmg < BaseDmg) dmg = BaseDmg;
            if (Effects.Contains(Biome.Purity))
            {
                dmg = (int)(dmg * 1.2f);
            }
            if (Effects.Contains(Biome.Dungeon))
            {
                if (Main.rand.NextFloat() <= 0.2f)
                {
                    dmg *= 3;
                }
            }

            target.StrikeNPC(dmg, 0, 0);

            if (Effects.Contains(Biome.Snow))
            {
                target.AddBuff(ModContent.BuffType<SlowEX>(), 600);
            }
            if (Effects.Contains(Biome.Desert))
            {
                target.AddBuff(ModContent.BuffType<WeakEX>(), 600);
            }
            if (Effects.Contains(Biome.Corrupt))
            {
                target.AddBuff(ModContent.BuffType<CursedInferoEX>(), 600);
            }
            if (Effects.Contains(Biome.Crimson))
            {
                target.AddBuff(ModContent.BuffType<IchorEX>(), 600);
            }
            if (Effects.Contains(Biome.Jungle))
            {
                target.AddBuff(ModContent.BuffType<VenomEX>(), 1000);
            }
            if (Effects.Contains(Biome.Hallow))
            {
                target.AddBuff(ModContent.BuffType<StunnedEX>(), 180);
            }

            if (Effects.Contains(Biome.Hell))
            {
                target.AddBuff(ModContent.BuffType<OnFireEX>(), 300);
            }

            if (Effects.Contains(Biome.Space))
            {
                foreach (NPC subtarget in Main.npc)
                {
                    if (subtarget.CanBeChasedBy() && subtarget.Distance(target.Center) < 300 && subtarget.whoAmI != target.whoAmI)
                    {
                        subtarget.StrikeNPC((int)(dmg * 0.25f), 0, 0);
                        owner.addDPS((int)(dmg * 0.25f));
                    }
                }
            }



            owner.addDPS(dmg);
        }
    }
}