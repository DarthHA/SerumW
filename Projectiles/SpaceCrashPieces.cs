using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class SpaceCrashPieces : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 35;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                projectile.frame = Main.rand.Next(4);
                projectile.rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                projectile.scale = Main.rand.NextFloat() * 0.4f + 0.8f;
            }
            if (projectile.timeLeft > 20)
            {
                projectile.Opacity = 1;
            }
            else
            {
                projectile.Opacity = projectile.timeLeft / 15f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SpriteEffects SP = SpriteEffects.None;

            SerumW.SetCustomShader(1, 1, 1, projectile.Opacity);
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle rectangle = new Rectangle(0, tex.Height / 4 * projectile.frame, tex.Width, tex.Height / 4);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, rectangle, Color.White * projectile.Opacity, projectile.rotation, rectangle.Size() / 2, projectile.scale * 0.3f, SP, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }


    }
}