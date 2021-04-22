using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class SpaceCrashTrail : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 20;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.alpha = 255;
            //projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                projectile.rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                projectile.scale = Main.rand.NextFloat() * 0.8f + 0.4f;
                //projectile.scale *= 1.5f;
            }
            projectile.Opacity = projectile.timeLeft / 20f;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            SerumW.SpaceCrashBlur.Parameters["alpha"].SetValue(projectile.Opacity);
            SerumW.SpaceCrashBlur.CurrentTechnique.Passes["SpaceCrashBlur"].Apply();
            Texture2D tex = Main.projectileTexture[projectile.type];
            Vector2 DrawPos = projectile.Center + tex.Width / 2f * projectile.rotation.ToRotationVector2() * projectile.scale;
            spriteBatch.Draw(tex, DrawPos - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }


    }
}