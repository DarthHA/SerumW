using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class Flash : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 30;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.Opacity = 0;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.rotation = MathHelper.TwoPi * Main.rand.NextFloat();
            }
            if (projectile.timeLeft > 25)
            {
                projectile.Opacity = (30f - projectile.timeLeft) / 5f;
            }
            else
            {
                projectile.Opacity = projectile.timeLeft / 25f;
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

        public static void GenerateFlash(Vector2 Pos, float scale = 1, int extraUpdates = 0)
        {
            int protmp = Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<Flash>(), 0, 0, Main.myPlayer);
            Main.projectile[protmp].scale = scale;
            Main.projectile[protmp].extraUpdates = extraUpdates;
        }
    }
}