using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class CrashTP : ModProjectile
    {
        private float progress = 1;

        public Vector2 RelaPos = new Vector2(Main.screenWidth, Main.screenHeight) / 2;

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
            projectile.Opacity = 0;
            //projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.Center = Main.screenPosition + RelaPos;
            projectile.ai[1]++;
            if (projectile.ai[1] < 20)
            {
                float k = 1 - projectile.ai[1] / 20f;
                k *= k;
                progress = k;
            }
            else
            {
                progress = 0;
            }

        }

        public void DrawFix()
        {
            Texture2D tex1 = Main.projectileTexture[projectile.type];
            Texture2D tex2 = mod.GetTexture("Projectiles/CrashTP2");
            Texture2D tex3 = mod.GetTexture("Projectiles/TPLight");
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int i = projectile.timeLeft % 10;
            DrawDouble(projectile.Center, tex1, i, projectile.scale * 3f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SerumW.SetCustomShader(0.4f, 1.0f, 1.0f, 1.5f);
            Main.spriteBatch.Draw(tex3, projectile.Center - Main.screenPosition, null, Color.Cyan * 1.5f, 0, tex3.Size() / 2, new Vector2((float)Math.Pow(progress, 0.3f) / 2, 1) * 12, SpriteEffects.None, 0);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SerumW.SpaceInBlur.CurrentTechnique.Passes["SpaceInBlur"].Apply();
            Main.spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White, 0, tex2.Size() / 2, new Vector2(progress * 1.8f, 1) * 0.7f, SpriteEffects.None, 0);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //DrawFix();
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void DrawDouble(Vector2 DrawPos, Texture2D tex, int i, float scale)
        {
            float k = i / 10f;
            SerumW.GradBlur.Parameters["k"].SetValue(k);
            SerumW.GradBlur.Parameters["alpha"].SetValue(1);
            SerumW.GradBlur.CurrentTechnique.Passes["GradBlur"].Apply();
            Rectangle rectangle1 = new Rectangle(0, 0, (int)(tex.Width * k), tex.Height);
            Rectangle rectangle2 = new Rectangle((int)(tex.Width * k), 0, tex.Width - (int)(tex.Width * k), tex.Height);
            Vector2 Pos1 = DrawPos - new Vector2(tex.Width, tex.Height) / 2 * scale;
            Vector2 Pos2 = Pos1 + new Vector2((1 - k) * tex.Width, 0) * scale;
            Main.spriteBatch.Draw(tex, Pos1 - Main.screenPosition, rectangle2, Color.Cyan, 0, Vector2.Zero, scale, SpriteEffects.None, 0); //左边
            Main.spriteBatch.Draw(tex, Pos2 - Main.screenPosition, rectangle1, Color.Cyan, 0, Vector2.Zero, scale, SpriteEffects.None, 0); //右边
        }


    }
}