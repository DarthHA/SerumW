using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SerumW.Buffs;
using System;
using System.Collections.Generic;
using System.Reflection;
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

			dmg = (int)StrikeNPC(target, dmg, 0, 0);
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
			if (Main.player[projectile.owner].killGuide && npc.type == NPCID.Guide && npc.active && !npc.dontTakeDamage) return true;
			if (Main.player[projectile.owner].killClothier && npc.type == NPCID.Clothier && npc.active && !npc.dontTakeDamage) return true;
			if (npc.type == NPCID.Angler && npc.active) return true;
			return npc.active && npc.lifeMax > 5 && !npc.dontTakeDamage && !npc.friendly && !npc.immortal;
		}



		public double StrikeNPC(NPC npc, int Damage, float knockBack, int hitDirection, bool crit = false, bool fromNet = false)
		{
			bool flag = Main.netMode == NetmodeID.SinglePlayer;
			var ReflectTarget = typeof(NPC).GetField("ignorePlayerInteractions", BindingFlags.NonPublic | BindingFlags.Static);

			if (flag && (int)ReflectTarget.GetValue(new NPC()) > 0)
			{
				ReflectTarget.SetValue(new NPC(), (int)ReflectTarget.GetValue(new NPC()) - 1);
				flag = false;
			}
			
			if (!npc.active || npc.life <= 0)
			{
				return 0.0;
			}
			double dmg = Damage;
			int def = npc.defense;
			if (npc.ichor)
			{
				def -= 20;
			}
			if (npc.betsysCurse)
			{
				def -= 40;
			}
			if (def < 0)
			{
				def = 0;
			}

			double OriginalDmg = dmg;
			NPCLoader.StrikeNPC(npc, ref dmg, def, ref knockBack, hitDirection, ref crit);
            if (dmg < OriginalDmg / 2)
            {
				dmg = OriginalDmg;
            }
			dmg = Main.CalculateDamage((int)dmg, def);
			if (crit)
			{
				dmg *= 2.0;
			}
			if (npc.takenDamageMultiplier > 1f)
			{
				dmg *= npc.takenDamageMultiplier;
			}
			
			if ((npc.takenDamageMultiplier > 1f || Damage != 9999) && npc.lifeMax > 1)
			{
				if (npc.friendly)
				{
					Color color = crit ? CombatText.DamagedFriendlyCrit : CombatText.DamagedFriendly;
					CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), color, (int)dmg, crit, false);
				}
				else
				{
					Color color2 = crit ? CombatText.DamagedHostileCrit : CombatText.DamagedHostile;
					if (fromNet)
					{
						color2 = crit ? CombatText.OthersDamagedHostileCrit : CombatText.OthersDamagedHostile;
					}
					CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height), color2, (int)dmg, crit, false);
				}
			}
			if (dmg >= 1.0)
			{
				if (flag)
				{
					npc.PlayerInteraction(Main.myPlayer);
				}
				npc.justHit = true;
				if (npc.townNPC)
				{
					bool flag2 = npc.aiStyle == 7 && (npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 16f || npc.ai[0] == 17f);
					if (flag2)
					{
						NPC npctmp = Main.npc[(int)npc.ai[2]];
						if (npctmp.active)
						{
							npctmp.ai[0] = 1f;
							npctmp.ai[1] = 300 + Main.rand.Next(300);
							npctmp.ai[2] = 0f;
							npctmp.localAI[3] = 0f;
							npctmp.direction = hitDirection;
							npctmp.netUpdate = true;
						}
					}
					npc.ai[0] = 1f;
					npc.ai[1] = 300 + Main.rand.Next(300);
					npc.ai[2] = 0f;
					npc.localAI[3] = 0f;
					npc.direction = hitDirection;
					npc.netUpdate = true;
				}
				if (npc.aiStyle == 8 && Main.netMode != 1)
				{
					if (npc.type == NPCID.RuneWizard)
					{
						npc.ai[0] = 450f;
					}
					else if (npc.type == NPCID.Necromancer || npc.type == NPCID.NecromancerArmored)
					{
						if (Main.rand.Next(2) == 0)
						{
							npc.ai[0] = 390f;
							npc.netUpdate = true;
						}
					}
					else if (npc.type == NPCID.DesertDjinn)
					{
						if (Main.rand.Next(3) != 0)
						{
							npc.ai[0] = 181f;
							npc.netUpdate = true;
						}
					}
					else
					{
						npc.ai[0] = 400f;
					}
					npc.TargetClosest(true);
				}
				if (npc.aiStyle == 97 && Main.netMode != 1)
				{
					npc.localAI[1] = 1f;
					npc.TargetClosest(true);
				}
				if (npc.type == NPCID.DetonatingBubble)
				{
					dmg = 0.0;
					npc.ai[0] = 1f;
					npc.ai[1] = 4f;
					npc.dontTakeDamage = true;
				}
				if (npc.type == NPCID.SantaNK1 && npc.life >= npc.lifeMax * 0.5 && npc.life - dmg < npc.lifeMax * 0.5)
				{
					Gore.NewGore(npc.position, npc.velocity, 517, 1f);
				}
				if (npc.type == NPCID.SpikedIceSlime)
				{
					npc.localAI[0] = 60f;
				}
				if (npc.type == NPCID.SlimeSpiked)
				{
					npc.localAI[0] = 60f;
				}
				if (npc.type == NPCID.SnowFlinx)
				{
					npc.localAI[0] = 1f;
				}
				if (!npc.immortal)
				{
					if (npc.realLife >= 0)
					{
						Main.npc[npc.realLife].life -= (int)dmg;
						npc.life = Main.npc[npc.realLife].life;
						npc.lifeMax = Main.npc[npc.realLife].lifeMax;
					}
					else
					{
						npc.life -= (int)dmg;
					}
				}
				if (knockBack > 0f && npc.knockBackResist > 0f)
				{
					float num3 = knockBack * npc.knockBackResist;
					if (num3 > 8f)
					{
						float num4 = num3 - 8f;
						num4 *= 0.9f;
						num3 = 8f + num4;
					}
					if (num3 > 10f)
					{
						float num5 = num3 - 10f;
						num5 *= 0.8f;
						num3 = 10f + num5;
					}
					if (num3 > 12f)
					{
						float num6 = num3 - 12f;
						num6 *= 0.7f;
						num3 = 12f + num6;
					}
					if (num3 > 14f)
					{
						float num7 = num3 - 14f;
						num7 *= 0.6f;
						num3 = 14f + num7;
					}
					if (num3 > 16f)
					{
						num3 = 16f;
					}
					if (crit)
					{
						num3 *= 1.4f;
					}
					int num8 = (int)dmg * 10;
					if (Main.expertMode)
					{
						num8 = (int)dmg * 15;
					}
					if (num8 > npc.lifeMax)
					{
						if (hitDirection < 0 && npc.velocity.X > -num3)
						{
							if (npc.velocity.X > 0f)
							{
								npc.velocity.X -= num3;
							}
							npc.velocity.X -= num3;
							if (npc.velocity.X < -num3)
							{
								npc.velocity.X = -num3;
							}
						}
						else if (hitDirection > 0 && npc.velocity.X < num3)
						{
							if (npc.velocity.X < 0f)
							{
								npc.velocity.X += num3;
							}
							npc.velocity.X += num3;
							if (npc.velocity.X > num3)
							{
								npc.velocity.X = num3;
							}
						}
						if (npc.type == NPCID.SnowFlinx)
						{
							num3 *= 1.5f;
						}
						if (!npc.noGravity)
						{
							num3 *= -0.75f;
						}
						else
						{
							num3 *= -0.5f;
						}
						if (npc.velocity.Y > num3)
						{
							npc.velocity.Y += num3;
							if (npc.velocity.Y < num3)
							{
								npc.velocity.Y = num3;
							}
						}
					}
					else
					{
						if (!npc.noGravity)
						{
							npc.velocity.Y = -num3 * 0.75f * npc.knockBackResist;
						}
						else
						{
							npc.velocity.Y = -num3 * 0.5f * npc.knockBackResist;
						}
						npc.velocity.X = num3 * hitDirection * npc.knockBackResist;
					}
				}
				if ((npc.type == NPCID.WallofFlesh || npc.type == NPCID.WallofFleshEye) && npc.life <= 0)
				{
					for (int i = 0; i < 200; i++)
					{
						if (Main.npc[i].active && (Main.npc[i].type == NPCID.WallofFlesh || Main.npc[i].type == NPCID.WallofFleshEye))
						{
							Main.npc[i].HitEffect(hitDirection, dmg);
						}
					}
				}
				else
				{
					npc.HitEffect(hitDirection, dmg);
				}
				if (npc.HitSound != null)
				{
					Main.PlaySound(npc.HitSound, npc.position);
				}
				if (npc.realLife >= 0)
				{
					Main.npc[npc.realLife].checkDead();
				}
				else
				{
					npc.checkDead();
				}
				return dmg;
			}
			return 0.0;
		}
	}
}