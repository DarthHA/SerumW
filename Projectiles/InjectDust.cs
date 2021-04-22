using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class InjectDust : ModProjectile
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
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.Opacity = 1;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            projectile.frame = (int)(projectile.ai[0] * 0.4);
            projectile.scale = (float)Math.Sqrt(projectile.ai[0] / 60);
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
            Rectangle rectangle = GetTexRect(projectile.frame);
            Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, rectangle, Color.Cyan, projectile.rotation, rectangle.Size() / 2, new Vector2(1, 0.4f) * projectile.scale * 0.6f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public Rectangle GetTexRect(int index)
        {
            index = Utils.Clamp(index, 0, 24);
            int dwidth = Main.projectileTexture[projectile.type].Width / 5;
            int dheight = Main.projectileTexture[projectile.type].Height / 5;
            int x = index % 5;
            int y = index / 5;
            return new Rectangle(x * dwidth, y * dheight, dwidth, dheight);
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}