using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class SpaceCrash : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 80;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCs.Add(index);
        }

        public override void AI()
        {
            if (!Main.player[projectile.owner].active || Main.player[projectile.owner].dead)
            {
                projectile.Kill();
                return;
            }
            projectile.direction = Math.Sign(projectile.velocity.X);
            projectile.ai[0]++;
            if (projectile.ai[0] == 1)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Broken"), Main.player[projectile.owner].Center);
                Flash.GenerateFlash(projectile.Center, 1.5f, 1);
            }
            if (projectile.ai[0] == 5)
            {
                for (int i = 0; i < 12; i++)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SpaceCrashTrail>(), 0, 0, projectile.owner);
                }
            }

            /*
            if (projectile.ai[0] == 10)
            {
                for (int i = 0; i < 14; i++)
                {
                    Vector2 RanPos = new Vector2(Main.rand.Next(-175, 175), Main.rand.Next(-250, 250));
                    Projectile.NewProjectile(projectile.Center + RanPos, Vector2.Zero, ModContent.ProjectileType<SpaceCrashPieces>(), 0, 0, projectile.owner);
                }
            }
            */
            if (projectile.ai[0] == 35)
            {
                Flash.GenerateFlash(projectile.Center, 2);
                FlashHalf.GenerateFlash(projectile.Center, projectile.whoAmI, 2);

                for (int i = 0; i < 14; i++)
                {
                    Vector2 RanPos = new Vector2(Main.rand.Next(-175, 175), Main.rand.Next(-250, 250));
                    Projectile.NewProjectile(projectile.Center + RanPos, Vector2.Zero, ModContent.ProjectileType<SpaceCrashPieces>(), 0, 0, projectile.owner);
                }
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects SP = projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (projectile.ai[0] >= 40)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                SerumW.SpaceBlur.GraphicsDevice.Textures[1] = mod.GetTexture("Images/Noise");
                SerumW.SpaceBlur.CurrentTechnique.Passes["SpaceBlur"].Apply();
                spriteBatch.Draw(mod.GetTexture("Projectiles/SpaceCrash3"), projectile.Center - Main.screenPosition, null, Color.White, 0, mod.GetTexture("Projectiles/SpaceCrash3").Size() / 2, projectile.scale * 1.5f, SP, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D tex1 = mod.GetTexture("Projectiles/SpaceCrash1");
            Texture2D tex2 = mod.GetTexture("Projectiles/SpaceCrash2");

            if (projectile.ai[0] >= 10 && projectile.ai[0] < 40)
            {
                Texture2D texfield = mod.GetTexture("Projectiles/SpaceField");
                SpriteEffects SP2 = SP == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 DrawPos = projectile.Center;// - new Vector2(Math.Sign(projectile.velocity.X), 0) * 10;
                float alpha = (projectile.ai[0] - 10f) / 30f;
                spriteBatch.Draw(texfield, DrawPos - Main.screenPosition, null, Color.LightCyan * alpha * 0.9f, 0, texfield.Size() / 2, projectile.scale * 2f, SP2, 0);
            }
            if (projectile.ai[0] < 40)
            {
                float alpha = 1;
                //if (projectile.ai[0] >= 10)
                //{
                    //alpha = (40f - projectile.ai[0]) / 30f;
                //}
                spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.White * alpha, 0, tex1.Size() / 2, projectile.scale * 1.5f, SP, 0);
            }
            spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White, 0, tex2.Size() / 2, projectile.scale * 1.5f, SP, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}