using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SerumW.Buffs;
using SerumW.Projectiles;
using SerumW.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.UI;
using static SerumW.ClawPlayer;

namespace SerumW
{
    public class SerumW : Mod
	{

        public static SerumW Instance;

        //特效
        public static Effect SpaceBlur;
        public static Effect CustomDrawBlur;
        public static Effect GradBlur;
        public static Effect SpaceCrashBlur;
        public static Effect SpaceInBlur;
        public static Effect TPTrailEffect;
        public static Effect UIEffect;
        public static Effect ClawSlashBlur;
        public static Effect HoloEffect;

        private UserInterface _ClawUIInterface;   //UI
        internal ClawUI _ClawUI;
        public static float Progress = 0;

        public static bool IsCapturing = false;             //确定函数执行时机
        public static bool PlayerDrawed = false;            //确保玩家每帧绘制一次次


        public static Texture2D[] SavedBG = new Texture2D[TPCount];          //背景贴图

        public static SerumWConfig config;

        public SerumW()
        {
            Instance = this;
        }


        public override void Load()
        {
            SpaceBlur = GetEffect("Effects/Content/SpaceBlur");
            CustomDrawBlur = GetEffect("Effects/Content/CustomDrawBlur");
            GradBlur = GetEffect("Effects/Content/GradBlur");
            SpaceCrashBlur = GetEffect("Effects/Content/SpaceCrashBlur");
            SpaceInBlur = GetEffect("Effects/Content/SpaceInBlur");
            TPTrailEffect = GetEffect("Effects/Content/TPTrailEffect");
            UIEffect = GetEffect("Effects/Content/UIEffect");
            ClawSlashBlur = GetEffect("Effects/Content/ClawSlashBlur");
            HoloEffect = GetEffect("Effects/Content/HoloEffect");

            _ClawUI = new ClawUI();
            _ClawUIInterface = new UserInterface();
            _ClawUIInterface.SetState(_ClawUI);


            Filters.Scene["SerumW:WarpEffect"] = new Filter(
            new WarpScreenShaderData(new Ref<Effect>(GetEffect("Effects/Content/WarpEffect")), "WarpEffect"), EffectPriority.Medium);
            Filters.Scene["SerumW:WarpEffect"].Load();

            On.Terraria.Main.drawWaters += new On.Terraria.Main.hook_drawWaters(DrawWatersHook);
            On.Terraria.Main.DrawCapture += new On.Terraria.Main.hook_DrawCapture(DrawCaptureHook);
            On.Terraria.Main.DrawNPCs += new On.Terraria.Main.hook_DrawNPCs(DrawNPCsHook);
            On.Terraria.Main.DrawPlayer += new On.Terraria.Main.hook_DrawPlayer(DrawPlayerHook);
            On.Terraria.Main.DrawNPC += new On.Terraria.Main.hook_DrawNPC(DrawNPCHook);
            On.Terraria.Main.DrawItems += new On.Terraria.Main.hook_DrawItems(DrawItemsHook);
        }

        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int ClawUIIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1"));
            if (ClawUIIndex != -1)
            {
                layers.Insert(ClawUIIndex, new LegacyGameInterfaceLayer(
                    "SerumW: ClawUI",
                    delegate
                    {
                        _ClawUIInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }




        public override void Unload()
        {
            SpaceBlur = null;
            CustomDrawBlur = null;
            GradBlur = null;
            SpaceCrashBlur = null;
            SpaceInBlur = null;
            TPTrailEffect = null;
            UIEffect = null;
            ClawSlashBlur = null;
            HoloEffect = null;

            foreach(Texture2D tex in SavedBG)
            {
                if (tex != null && !tex.IsDisposed)
                {
                    tex.Dispose();
                }
            }
            Instance = null;
            Filters.Scene["SerumW:WarpEffect"].Deactivate();
        }


        #region Patch相关
        public static void DrawPlayerHook(On.Terraria.Main.orig_DrawPlayer orig, Main self, Player drawPlayer, Vector2 Position, float rotation, Vector2 rotationOrigin, float shadow = 0f)
        {
            if (Main.gameMenu)
            {
                orig.Invoke(self, drawPlayer, Position, rotation, rotationOrigin, shadow);
                return;
            }
            if(!IsCapturing || !drawPlayer.GetModPlayer<ClawPlayer>().IsWarping)
            {
                orig.Invoke(self, drawPlayer, Position, rotation, rotationOrigin, shadow);
            }

            if (drawPlayer == Main.LocalPlayer)
            {
                if (!PlayerDrawed || Main.gamePaused)
                {
                    PlayerDrawed = true;

                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<InjectCircle>())
                        {
                            (proj.modProjectile as InjectCircle).DrawFix();
                        }
                    }

                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<InjectDust>())
                        {
                            (proj.modProjectile as InjectDust).DrawFix();
                        }
                    }

                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<TPLight>())
                        {
                            (proj.modProjectile as TPLight).DrawFix();
                        }
                    }

                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<FlashHalf>())
                        {
                            (proj.modProjectile as FlashHalf).DrawFix();
                        }
                    }

                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<Flash>())
                        {
                            (proj.modProjectile as Flash).DrawFix();
                        }
                    }

                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<CrashTP>())
                        {
                            (proj.modProjectile as CrashTP).DrawFix();
                        }
                    }

                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<FlashHalfEnd>())
                        {
                            (proj.modProjectile as FlashHalfEnd).DrawFix();
                        }
                    }
                }
            }
        }



        public static void DrawCaptureHook(On.Terraria.Main.orig_DrawCapture orig, Main self, Rectangle Area, CaptureSettings captureSettings)
        {
            IsCapturing = true;
            orig.Invoke(self, Area, captureSettings);
            IsCapturing = false;
        }

        public static void DrawWatersHook(On.Terraria.Main.orig_drawWaters orig, Main self, bool bg, int styleOverride, bool allowUpdate)
        {
            if (!IsCapturing)
            {
                if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().warpingProgress == WarpingProgress.TPing)
                {
                    return;
                }
            }
            orig.Invoke(self, bg, styleOverride, allowUpdate);
        }


        public static void DrawNPCHook(On.Terraria.Main.orig_DrawNPC orig, Main self, int iNPCIndex, bool behindTiles)
        {
            int target = Main.LocalPlayer.GetModPlayer<ClawPlayer>().SelectedTarget;
            if (IsCapturing)
            {
                if (target != -1)
                {
                    if (iNPCIndex == target || Main.npc[iNPCIndex].realLife == target)
                    {
                        return;
                    }
                }
            }
            else
            {
                if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().warpingProgress == WarpingProgress.TPing)
                {
                    if (target != -1)
                    {
                        if (iNPCIndex != target && Main.npc[iNPCIndex].realLife != target)
                        {
                            return;
                        }
                    }
                }
            }
            orig.Invoke(self, iNPCIndex, behindTiles);
        }

        public static void DrawNPCsHook(On.Terraria.Main.orig_DrawNPCs orig, Main self,bool BehindTile)
        {
            if (!Main.gameMenu)
            {
                if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().warpingProgress == WarpingProgress.TPing)
                {
                    int index = Main.LocalPlayer.GetModPlayer<ClawPlayer>().WarpCountIndex;
                    Texture2D BorderTex = Instance.GetTexture("Images/Border");
                    if (index > 0)
                    {
                        Point Random = new Point(Main.rand.Next(-8, 8), Main.rand.Next(-8, 8));
                        if (SavedBG[index] != null)
                        {
                            if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().Timer < 7)
                            {
                                if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().Direction < 0)
                                {
                                    float k = Main.LocalPlayer.GetModPlayer<ClawPlayer>().Timer / 7f;

                                    Main.spriteBatch.Draw(SavedBG[index - 1],
                                        new Rectangle((int)(Main.screenWidth * k) + Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                        Color.White);
                                    Main.spriteBatch.Draw(BorderTex,
                                        new Rectangle((int)(Main.screenWidth * k) + Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                        Color.White);

                                    Main.spriteBatch.Draw(SavedBG[index],
                                        new Rectangle((int)(Main.screenWidth * (k - 1)) + Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                        Color.White);
                                    Main.spriteBatch.Draw(BorderTex,
                                        new Rectangle((int)(Main.screenWidth * (k - 1)) + Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                        Color.White);
                                }
                                else
                                {
                                    float k = 1 - Main.LocalPlayer.GetModPlayer<ClawPlayer>().Timer / 7f;

                                    Main.spriteBatch.Draw(SavedBG[index],
                                        new Rectangle((int)(Main.screenWidth * k) + Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                        Color.White);
                                    Main.spriteBatch.Draw(BorderTex,
                                        new Rectangle((int)(Main.screenWidth * k) + Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                        Color.White);

                                    Main.spriteBatch.Draw(SavedBG[index - 1],
                                        new Rectangle((int)(Main.screenWidth * (k - 1)) + Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                        Color.White);
                                    Main.spriteBatch.Draw(BorderTex,
                                        new Rectangle((int)(Main.screenWidth * (k - 1)) + Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                        Color.White);
                                }
                            }
                            else
                            {
                                Main.spriteBatch.Draw(SavedBG[index],
                                    new Rectangle(Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                    Color.White);
                                Main.spriteBatch.Draw(BorderTex,
                                    new Rectangle(Random.X, Random.Y, Main.screenWidth, Main.screenHeight),
                                    Color.White);
                            }

                        }
                    }
                }

            }
            orig.Invoke(self, !BehindTile);
            orig.Invoke(self,BehindTile);
        }

        public static void DrawItemsHook(On.Terraria.Main.orig_DrawItems orig,Main self)
        {
            if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().warpingProgress != WarpingProgress.TPing)
            {
                orig.Invoke(self);
            }
        }


        #endregion



        public static void SetCustomShader(float r = 1, float g = 1, float b = 1, float alpha = 1)
        {
            CustomDrawBlur.Parameters["r"].SetValue(r);
            CustomDrawBlur.Parameters["g"].SetValue(g);
            CustomDrawBlur.Parameters["b"].SetValue(b);
            CustomDrawBlur.Parameters["alpha"].SetValue(alpha);
            CustomDrawBlur.CurrentTechnique.Passes["CustomDrawBlur"].Apply();
        }

        public override void PreUpdateEntities()
        {
            PlayerDrawed = false;
            if (Main.LocalPlayer.GetModPlayer<ClawPlayer>().warpingProgress == WarpingProgress.TPing)
            {
                Progress += 0.025f;
                if (Progress > 1) Progress -= 1;
                if (!Filters.Scene["SerumW:WarpEffect"].IsActive())
                {
                    Filters.Scene["SerumW:WarpEffect"].GetShader().UseImage(Instance.GetTexture("Images/TPNoise"));
                    Filters.Scene["SerumW:WarpEffect"].GetShader().UseDirection(new Vector2(Main.LocalPlayer.GetModPlayer<ClawPlayer>().Direction, 0));
                    Filters.Scene["SerumW:WarpEffect"].GetShader().UseProgress(Progress);
                    Filters.Scene.Activate("SerumW:WarpEffect");
                }
                if (Filters.Scene["SerumW:WarpEffect"].IsActive())
                {
                    Filters.Scene["SerumW:WarpEffect"].GetShader().UseImage(Instance.GetTexture("Images/TPNoise"));
                    Filters.Scene["SerumW:WarpEffect"].GetShader().UseDirection(new Vector2(1, 0));
                    Filters.Scene["SerumW:WarpEffect"].GetShader().UseProgress(Progress);
                }
            }
            else
            {
                if (Filters.Scene["SerumW:WarpEffect"].IsActive())
                {
                    Filters.Scene.Deactivate("SerumW:WarpEffect");
                }
            }
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
            {
                return;
            }
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<SerumBuff>()) || Main.LocalPlayer.GetModPlayer<ClawPlayer>().IsWarping)
            {
                if (config.UseBGM)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/Head Eye Claw");
                    priority = MusicPriority.BossHigh;
                }
            }
        }


    }
}