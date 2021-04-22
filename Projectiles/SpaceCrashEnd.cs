using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    public class SpaceCrashEnd : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 99999;
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
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlashHalfEnd>(), 0, 0, projectile.owner, 0, projectile.whoAmI);
            }
            if (projectile.ai[0] > 40)
            {
                projectile.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects SP = projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SerumW.SpaceBlur.GraphicsDevice.Textures[1] = mod.GetTexture("Images/Noise");
            SerumW.SpaceBlur.CurrentTechnique.Passes["SpaceBlur"].Apply();
            spriteBatch.Draw(mod.GetTexture("Projectiles/SpaceCrash3"), projectile.Center - Main.screenPosition, null, Color.White, 0, mod.GetTexture("Projectiles/SpaceCrash3").Size() / 2, projectile.scale * 1.5f, SP, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D tex2 = mod.GetTexture("Projectiles/SpaceCrash2");
            Texture2D texfield = mod.GetTexture("Projectiles/SpaceField");
            SpriteEffects SP2 = SP == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 DrawPos = projectile.Center;
            float alpha = projectile.ai[0] / 30f + 0.2f;
            alpha = Utils.Clamp(alpha, 0, 1);
            if (alpha > 1) alpha = 1;
            spriteBatch.Draw(texfield, DrawPos - Main.screenPosition, null, Color.LightCyan * alpha * 0.9f, 0, texfield.Size() / 2, projectile.scale * 2f, SP2, 0);
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