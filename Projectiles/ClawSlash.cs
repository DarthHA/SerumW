using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace SerumW.Projectiles
{
    class ClawSlash : ModProjectile
    {
        float widthScale = 1;
        float LengthScale = 1;
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
            projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.Opacity = projectile.timeLeft / 30f;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawFix();
            return false;
        }

        public void DrawFix()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D tex = Main.projectileTexture[projectile.type];
            SerumW.ClawSlashBlur.Parameters["alpha"].SetValue(projectile.Opacity * 2f);
            SerumW.ClawSlashBlur.CurrentTechnique.Passes["ClawSlashBlur"].Apply();
            Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity * 1.5f, projectile.rotation, tex.Size() / 2, new Vector2(projectile.scale * 2f * LengthScale, widthScale), SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public static void GenClawSlash(Vector2 Pos, float rot, float len, float width)
        {
            int protmp = Projectile.NewProjectile(Pos, rot.ToRotationVector2(), ModContent.ProjectileType<ClawSlash>(), 0, 0, Main.myPlayer);
            (Main.projectile[protmp].modProjectile as ClawSlash).LengthScale = len;
            (Main.projectile[protmp].modProjectile as ClawSlash).widthScale = width;
        }
    }
}