using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static SerumW.VertexInfo;

namespace SerumW.Projectiles
{
    public class TPTrail : ModProjectile
    {
        public Vector2 RelaBegin = new Vector2(Main.screenWidth, Main.screenHeight) / 2 + new Vector2(-200, -200);

        public Vector2 RelaEnd = new Vector2(Main.screenWidth, Main.screenHeight) / 2 + new Vector2(200, 200);

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.timeLeft = 15;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            float alpha = (1 - projectile.ai[0] / 15) * 1.5f;
            projectile.Opacity = Utils.Clamp(alpha, 0, 1);
            projectile.Center = Main.LocalPlayer.Center;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int height = (int)(100 * projectile.scale);
            List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>
            {
                new CustomVertexInfo(Main.screenPosition + RelaBegin + new Vector2(0, -height / 2), Color.White,
        new Vector3(0, 0f, 1)),
                new CustomVertexInfo(Main.screenPosition + RelaBegin + new Vector2(0, height / 2), Color.White,
        new Vector3(0, 1, 1)),
                new CustomVertexInfo(Main.screenPosition + RelaEnd + new Vector2(0, -height / 2), Color.White,
        new Vector3(1, 0, 1)),

                new CustomVertexInfo(Main.screenPosition + RelaEnd + new Vector2(0, -height / 2), Color.White,
        new Vector3(1, 0, 1)),
                new CustomVertexInfo(Main.screenPosition + RelaEnd + new Vector2(0, height / 2), Color.White,
        new Vector3(1, 1, 1)),
                new CustomVertexInfo(Main.screenPosition + RelaBegin + new Vector2(0, height / 2), Color.White,
        new Vector3(0, 1, 1))
            };


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

            var screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
            var screenSize = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
            var projection = Matrix.CreateOrthographicOffCenter(0, screenSize.X, screenSize.Y, 0, 0, 1);
            var screenPos = screenCenter - screenSize / 2f;
            var model = Matrix.CreateTranslation(new Vector3(-screenPos.X, -screenPos.Y, 0));

            // 把变换和所需信息丢给shader
            SerumW.TPTrailEffect.Parameters["alpha"].SetValue(projectile.Opacity);

            SerumW.TPTrailEffect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = Main.projectileTexture[projectile.type];
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            SerumW.TPTrailEffect.CurrentTechnique.Passes[0].Apply();


            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

            Main.graphics.GraphicsDevice.RasterizerState = originalState;
            spriteBatch.End();
            spriteBatch.Begin();

            return false;
        }



        

    }
}
