using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class FlashHalfEnd : ModProjectile
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
            projectile.Opacity = 0;
        }

        public override void AI()
        {
            if (!Main.projectile[(int)projectile.ai[1]].active)
            {
                projectile.Kill();
                return;
            }
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.rotation = MathHelper.TwoPi * Main.rand.NextFloat();
            }
            projectile.localAI[1]++;
            if (projectile.localAI[1] < 30)
            {
                projectile.Opacity = projectile.localAI[1] / 40;
            }
            else
            {
                projectile.Opacity = 0.75f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //DrawFix();
            return false;
        }

        public void DrawFix()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SpriteEffects SP = projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, Color.Cyan * projectile.Opacity, projectile.rotation, Main.projectileTexture[projectile.type].Size() / 2, projectile.scale * 2, SP, 0);
            Texture2D tex2 = mod.GetTexture("Projectiles/Flash2");
            Main.spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex2.Size() / 2, projectile.scale, SP, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}