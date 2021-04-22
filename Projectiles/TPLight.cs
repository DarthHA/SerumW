using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class TPLight : ModProjectile
    {
        private float alpha1 = 0;
        private float alpha2 = 0;



        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 100;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.Opacity = 0;
        }

        public override void AI()
        {

            if (projectile.ai[1] == 0)
            {
                Flash.GenerateFlash(projectile.Center, 0.5f, 4);
            }
            projectile.ai[1]++;
            if (projectile.ai[1] < 0)
            {
                alpha1 = 0;
                alpha2 = 0;
            }
            else if (projectile.ai[1] < 10)
            {
                alpha1 = (float)Math.Sin(projectile.ai[1] * MathHelper.Pi / 10f);
                alpha2 = 1;
            }
            else if (projectile.ai[1] < 30)
            {
                alpha1 = 0;
                alpha2 = (30 - projectile.ai[1]) / 20f;
            }
            else
            {
                projectile.Kill();
            }
            

            if (projectile.ai[0] == 1)
            {
                if(Main.player[projectile.owner].active && !Main.player[projectile.owner].dead)
                {
                    projectile.Center = Main.player[projectile.owner].Center;
                }
            }
        }

        public void DrawFix()
        {
            Texture2D tex1 = Main.projectileTexture[projectile.type];
            Texture2D tex2 = mod.GetTexture("Projectiles/TPNoise");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //SerumW.SetCustomShader(0.4f, 1.0f, 1.0f, alpha2);
            int i = projectile.timeLeft % 10;
            DrawDouble(projectile.Center, tex2, i, projectile.scale);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SerumW.SetCustomShader(0.4f, 1.0f, 1.0f, alpha1);
            Main.spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.Cyan * alpha1, 0, tex1.Size() / 2, projectile.scale * 4, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawFix();
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
            SerumW.GradBlur.Parameters["alpha"].SetValue(alpha2);
            SerumW.GradBlur.CurrentTechnique.Passes["GradBlur"].Apply();
            Rectangle rectangle1 = new Rectangle(0, 0, (int)(tex.Width * k), tex.Height);
            Rectangle rectangle2 = new Rectangle((int)(tex.Width * k), 0, tex.Width - (int)(tex.Width * k), tex.Height);
            Vector2 Pos1 = DrawPos - new Vector2(tex.Width, tex.Height) / 2 * scale;
            Vector2 Pos2 = Pos1 + new Vector2((1 - k) * tex.Width, 0) * scale;
            Main.spriteBatch.Draw(tex, Pos1 - Main.screenPosition, rectangle2, Color.Cyan * alpha2, 0, Vector2.Zero, scale, SpriteEffects.None, 0); //左边
            Main.spriteBatch.Draw(tex, Pos2 - Main.screenPosition, rectangle1, Color.Cyan * alpha2, 0, Vector2.Zero, scale, SpriteEffects.None, 0); //右边
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            base.DrawBehind(index, drawCacheProjsBehindNPCsAndTiles, drawCacheProjsBehindNPCs, drawCacheProjsBehindProjectiles, drawCacheProjsOverWiresUI);
        }

    }
}