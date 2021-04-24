using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SerumW.Buffs;
using SerumW.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static SerumW.VertexInfo;

namespace SerumW.Projectiles
{
    public class HoloBoard : ModProjectile
    {

        const int width = 80;
        const int height = 112;
        public override void SetDefaults()            //396 566 = 40 56
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.timeLeft = 9999;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.timeLeft = 9999;
            projectile.ai[0]++;

            projectile.Opacity = Utils.Clamp(projectile.ai[0] / 60f, 0, 1);
            Player owner = Main.player[projectile.owner];
            if (!owner.active || owner.dead)
            {
                projectile.Kill();
                return;
            }
            if (!owner.HasBuff(ModContent.BuffType<SerumBuff>()))
            {
                projectile.Kill();
                return;
            }
            if (owner.GetModPlayer<ClawPlayer>().IsWarping)
            {
                projectile.Kill();
                return;
            }
            if (projectile.ai[0] == 1)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Hologram"), owner.Center);
            }
            projectile.Center = owner.Center + new Vector2(owner.direction * 45, 0);
            projectile.direction = owner.direction;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.ai[1]++;
            DrawMain();
            DrawLine();
            Player owner = Main.player[projectile.owner];
            if (owner.active && !owner.dead)
            {
                if (owner.HeldItem.type == ModContent.ItemType<SerumWItem>() && !owner.GetModPlayer<ClawPlayer>().IsWarping)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    Texture2D Aim = mod.GetTexture("Images/Aim");
                    float alpha = Utils.Clamp(projectile.ai[1] / 30f, 0f, 1f);
                    float rot = projectile.ai[1] / 240 * MathHelper.TwoPi;
                    float scale = (float)Math.Sin(projectile.ai[1] / 120 * MathHelper.Pi) * 0.1f + 0.95f;
                    spriteBatch.Draw(Aim, Main.MouseWorld - Main.screenPosition, null, Color.White * alpha, rot, Aim.Size() / 2, scale * 0.5f, SpriteEffects.None, 0);
                }
            }
            return false;
        }


        public void DrawLine()
        {
            List<CustomVertexInfo> triangleList;
            Vector2 DrawPos = projectile.Center;
            Player owner = Main.player[projectile.owner];
            if (owner.direction <= 0)
            {
                triangleList = new List<CustomVertexInfo>
                {
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2), Color.Cyan,
                new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,height / 2), Color.Cyan,
                new Vector3(0, 1, 1)),
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2), Color.Cyan,
                new Vector3(1, 0, 1)),

                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2), Color.Cyan,
                new Vector3(1, 0, 1)),
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,height / 2), Color.Cyan,
                new Vector3(1, 1, 1)),
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,height / 2), Color.Cyan,
                new Vector3(0, 1, 1))
                };
            }
            else
            {
                triangleList = new List<CustomVertexInfo>
                {
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2),  Color.Cyan,
                new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,height / 2),  Color.Cyan,
                new Vector3(0, 1, 1)),
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2),  Color.Cyan,
                new Vector3(1, 0, 1)),

                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2),  Color.Cyan,
                new Vector3(1, 0, 1)),
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,height / 2),  Color.Cyan,
                new Vector3(1, 1, 1)),
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,height / 2), Color.Cyan,
                new Vector3(0, 1, 1))
                };
            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

            var screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
            var screenSize = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
            var projection = Matrix.CreateOrthographicOffCenter(0, screenSize.X, screenSize.Y, 0, 0, 1);
            var screenPos = screenCenter - screenSize / 2f;
            var model = Matrix.CreateTranslation(new Vector3(-screenPos.X, -screenPos.Y, 0));

            // 把变换和所需信息丢给shader
            SerumW.HoloEffect.Parameters["alpha"].SetValue(projectile.Opacity);

            SerumW.HoloEffect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = mod.GetTexture("Images/HoloLine");
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            SerumW.HoloEffect.CurrentTechnique.Passes["HoloEffectLine"].Apply();


            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

            Main.graphics.GraphicsDevice.RasterizerState = originalState;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

        }

        public void DrawMain()
        {
            List<CustomVertexInfo> triangleList;
            Vector2 DrawPos = projectile.Center;
            Player owner = Main.player[projectile.owner];
            if (owner.direction <= 0)
            {
                triangleList = new List<CustomVertexInfo>
                {
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2), Color.White,
                new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,height / 2), Color.White,
                new Vector3(0, 1, 1)),
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2), Color.White,
                new Vector3(1, 0, 1)),

                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2), Color.White,
                new Vector3(1, 0, 1)),
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,height / 2), Color.White,
                new Vector3(1, 1, 1)),
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,height / 2), Color.White,
                new Vector3(0, 1, 1))
                };
            }
            else
            {
                triangleList = new List<CustomVertexInfo>
                {
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2),  Color.White,
                new Vector3(0, 0f, 1)),
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,height / 2),  Color.White,
                new Vector3(0, 1, 1)),
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2),  Color.White,
                new Vector3(1, 0, 1)),

                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,-height / 2),  Color.White,
                new Vector3(1, 0, 1)),
                    new CustomVertexInfo(DrawPos + (-MathHelper.Pi / 4).ToRotationVector2() * width / 2 + new Vector2(0,height / 2),  Color.White,
                new Vector3(1, 1, 1)),
                    new CustomVertexInfo(DrawPos + (MathHelper.Pi / 4 * 3).ToRotationVector2() * width / 2 + new Vector2(0,height / 2), Color.Cyan,
                new Vector3(0, 1, 1))
                };
            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;

            var screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
            var screenSize = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
            var projection = Matrix.CreateOrthographicOffCenter(0, screenSize.X, screenSize.Y, 0, 0, 1);
            var screenPos = screenCenter - screenSize / 2f;
            var model = Matrix.CreateTranslation(new Vector3(-screenPos.X, -screenPos.Y, 0));

            // 把变换和所需信息丢给shader
            SerumW.HoloEffect.Parameters["alpha"].SetValue(projectile.Opacity);

            SerumW.HoloEffect.Parameters["uTransform"].SetValue(model * projection);
            Main.graphics.GraphicsDevice.Textures[0] = mod.GetTexture("Images/HoloMain");
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            SerumW.HoloEffect.CurrentTechnique.Passes["HoloEffectMain"].Apply();


            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

            Main.graphics.GraphicsDevice.RasterizerState = originalState;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

        }


    }
}
