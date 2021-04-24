using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class InjectCircle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 10;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.Opacity = 1;
        }

        public override void AI()
        {
            projectile.scale = (float)Math.Sqrt((10f - projectile.timeLeft) / 10f);
            projectile.frame = Utils.Clamp(4 - projectile.timeLeft / 2, 0, 4);
            if (Main.player[projectile.owner].active)
            {
                projectile.Center = Main.player[projectile.owner].Center;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public void DrawFix()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle rectangle = new Rectangle(0, tex.Height / 5 * projectile.frame, tex.Width, tex.Height / 5);
            Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, rectangle, Color.White, projectile.rotation, rectangle.Size() / 2, projectile.scale * 1.5f, SpriteEffects.None, 0);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}