using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SerumW.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SerumW.UI
{
    internal class ClawUI : UIState
    {
        private UIElement area;

        Line[] lines = new Line[3];


        public override void OnInitialize()
        {
            area = new UIElement();
            area.Left.Set(1, 1f);
            area.Top.Set(1, 0f);
            area.Width.Set(1, 0f);
            area.Height.Set(1, 0f);

            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            ClawPlayer modplayer = Main.LocalPlayer.GetModPlayer<ClawPlayer>();
            if (modplayer.warpingProgress == ClawPlayer.WarpingProgress.TPing ||
                modplayer.warpingProgress == ClawPlayer.WarpingProgress.EndSlash)
            {
                if(modplayer.warpingProgress == ClawPlayer.WarpingProgress.EndSlash)
                {
                    float alpha = Utils.Clamp(1 - modplayer.Timer / 30f, 0, 1);
                    Color black = new Color(alpha, alpha, alpha, 1 - alpha);
                    spriteBatch.Draw(Main.magicPixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), black);
                
                    
                }

                //面板UI特效
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                SerumW.UIEffect.GraphicsDevice.Textures[1] = SerumW.Instance.GetTexture("UI/UIBG");
                SerumW.UIEffect.CurrentTechnique.Passes["UIEffect"].Apply();
                spriteBatch.Draw(SerumW.Instance.GetTexture("UI/ClawUI"), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);


                //爪绘制
                foreach(Projectile proj in Main.projectile)
                {
                    if (proj.active)
                    {
                        if (proj.type == ModContent.ProjectileType<ClawSlash>())
                        {
                            (proj.modProjectile as ClawSlash).DrawFix();
                        }
                    }
                }


                //划线
                UpdateLine();
                foreach (Line line in lines)
                {
                    spriteBatch.Draw(Main.magicPixel, new Rectangle((int)line.Pos.X, (int)line.Pos.Y, (int)line.Length, (int)line.Width), Color.White);
                }

            }


            if (modplayer.warpingProgress == ClawPlayer.WarpingProgress.EndPose)
            {
                float alpha = Utils.Clamp(modplayer.Timer / 30f, 0, 1);
                Color black = Color.Lerp(new Color(0f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 0f), alpha);
                spriteBatch.Draw(Main.magicPixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), black);
            }
        }

        public void UpdateLine()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length < 20)
                {
                    bool flag = false;
                    while (!flag)
                    {
                        float len = Main.rand.Next(400) + 50;
                        float width = Main.rand.Next(4) + 2;
                        Vector2 Pos = new Vector2(Main.rand.Next(Main.screenWidth), Main.rand.Next(Main.screenHeight));
                        if (Pos.X + len < Main.screenWidth && Pos.Y + width < Main.screenHeight)
                        {
                            lines[i] = new Line(Pos, len, width);
                            flag = true;
                        }
                    }
                }
                else
                {
                    lines[i].Pos.X += lines[i].Length / 3f;
                    lines[i].Length *= 0.85f;

                }
            }
            /*
            while (i < lines.Length)
            {
                float len = Main.rand.Next(150) + 25;
                float width = Main.rand.Next(2) + 1;
                Vector2 Pos = new Vector2(Main.rand.Next(Main.screenWidth), Main.rand.Next(Main.screenHeight));
                if (Pos.X + len < Main.screenWidth && Pos.Y + width < Main.screenHeight)
                {
                    lines[i] = new Line(Pos, len, width);
                    i++;
                }
            }
            */
        }




        private struct Line
        {
            public Vector2 Pos;
            public float Length;
            public float Width;
            public Line(Vector2 vec, float len, float wid)
            {
                Pos = vec;
                Length = len;
                Width = wid;
            }
        }

    }
}
