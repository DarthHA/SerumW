using Microsoft.Xna.Framework;
using SerumW.Buffs;
using SerumW.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SerumW
{
    public class ClawNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;


        private Vector2? OldVel = null;
        public override bool PreAI(NPC npc)
        {
            if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().IsWarping)
            {
                if (OldVel == null)
                {
                    OldVel = npc.velocity;
                    npc.velocity = Vector2.Zero;
                }
                npc.frameCounter = 0;
                npc.position = npc.oldPosition;
                return false;
            }
            if (IsControlled2(npc))
            {
                if (OldVel == null)
                {
                    OldVel = npc.velocity;
                    npc.velocity = Vector2.Zero;
                }
                npc.frameCounter = 0;
                npc.position = npc.oldPosition;
                return false;
            }
            if (IsStunned(npc))
            {
                if (OldVel == null)
                {
                    OldVel = npc.velocity;
                    npc.velocity = Vector2.Zero;
                }
                npc.frameCounter = 0;
                npc.position = npc.oldPosition;
                return false;
            }
            if (OldVel != null)
            {
                npc.velocity = (Vector2)OldVel;
                OldVel = null;
            }
            return true;
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (npc.HasBuff(ModContent.BuffType<SlowEX>()))
            {
                npc.position -= npc.velocity * 0.5f;
            }
        }

        public override void PostAI(NPC npc)
        {
            if (npc.HasBuff(ModContent.BuffType<IchorEX>()))
            {
                npc.ichor = true;
                npc.betsysCurse = true;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff(ModContent.BuffType<CursedInferoEX>()))
            {
                npc.buffImmune[BuffID.CursedInferno] = false;
                npc.AddBuff(BuffID.CursedInferno, 4);
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= (int)(npc.lifeMax * 0.004f) * 2;
                damage = (int)(npc.lifeMax * 0.004f);
            }

            if (npc.HasBuff(ModContent.BuffType<VenomEX>()))
            {
                npc.buffImmune[BuffID.Venom] = false;
                npc.venom = true;
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= (int)(npc.lifeMax * 0.003f) * 2;
                damage = (int)(npc.lifeMax * 0.003f);
            }

            if (npc.HasBuff(ModContent.BuffType<OnFireEX>()))
            {
                npc.buffImmune[BuffID.OnFire] = false;
                npc.onFire = true;
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= (int)(npc.lifeMax * 0.0075f) * 2;
                damage = (int)(npc.lifeMax * 0.0075f);
            }
        }

        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (npc.HasBuff(ModContent.BuffType<WeakEX>()))
            {
                damage = (int)(damage * 0.1f);
            }
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (npc.HasBuff(ModContent.BuffType<WeakEX>()))
            {
                damage = (int)(damage * 0.1f);
            }
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (IsControlled(npc))
            {
                return false;
            }
            return true;
        }


        public override bool CheckActive(NPC npc)
        {
            if (IsControlled(npc))
            {
                if (npc.timeLeft < 10)
                {
                    npc.timeLeft = 10;
                }
                return false;
            }
            return true;
        }


        public override bool CheckDead(NPC npc)
        {
            if (IsControlled(npc))
            {
                if (npc.life <= 0)
                {
                    npc.life = 1;
                }
                return false;
            }
            return true;
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            return !IsControlled(npc);
        }

        public bool IsControlled(NPC npc)
        {
            if (npc.realLife == -1)
            {
                if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().SelectedTarget == npc.whoAmI)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().SelectedTarget == npc.realLife)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public bool IsControlled2(NPC npc)
        {
            if (npc.realLife == -1)
            {
                foreach(Projectile proj in Main.projectile)
                {
                    if(proj.active && proj.type == ModContent.ProjectileType<DamageProj>())
                    {
                        if (proj.ai[0] == npc.whoAmI)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;

            }
            else
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<DamageProj>())
                    {
                        if (proj.ai[0] == npc.realLife)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
        }

        public bool IsStunned(NPC npc)
        {
            if (npc.HasBuff(ModContent.BuffType<StunnedEX>())) return true;
            if (npc.realLife != -1)
            {
                NPC owner = Main.npc[npc.realLife];
                if (owner.active)
                {
                    if (owner.HasBuff(ModContent.BuffType<StunnedEX>()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().IsWarping)
            {
                return false;
            }
            return default;
        }

        
    }
}