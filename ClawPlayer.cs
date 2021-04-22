using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SerumW.Buffs;
using SerumW.Items;
using SerumW.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static SerumW.PlayerStatus;

namespace SerumW
{

    public class ClawPlayer : ModPlayer
	{

		public const int TPCount = 10;            //瞬移次数总数

		/// <summary>
		/// 传送状态
		/// </summary>
		public enum WarpingProgress
		{
			Default,                  //待机
			TeleportToEnemy,          //瞬移抓取
			BreakSpace,              //产生空间裂缝
			TPing,                    //传送
			EndSlash,                //尾杀
			EndPose                 //尾杀Pose
		}

		public Texture2D[] SavedBG
        {
            get
            {
				return SerumW.SavedBG;
            }
            set
            {
				SerumW.SavedBG = value;
            }
        }

		public bool IsWarping
		{
            get
            {
				return warpingProgress != WarpingProgress.Default;
            }
		}

		public WarpingProgress warpingProgress = WarpingProgress.Default;


		/// <summary>
		/// 传送计数
		/// </summary>
		public int WarpCountIndex = 0;
		/// <summary>
		/// 计时器
		/// </summary>
		public int Timer = 0;

		/// <summary>
		/// 传送点屏幕坐标
		/// </summary>
		public Vector2[] WarpingScreenPos = new Vector2[TPCount];

		/// <summary>
		/// 传送点玩家坐标
		/// </summary>
		public Vector2[] WarpingPlayerPos = new Vector2[TPCount];


		/// <summary>
		/// 锁定目标
		/// </summary>
		public int SelectedTarget;

		/// <summary>
		/// 攻击方向
		/// </summary>
		public int Direction = 1;


		/// <summary>
		/// 攻击冷却
		/// </summary>
		public int WarpingCD = 0;

		/// <summary>
		/// NPC原坐标
		/// </summary>
		public Vector2 OriginalNPCPos = Vector2.Zero;

		/// <summary>
		/// 玩家经过的生态群落
		/// </summary>
		public List<Biome> CrossBiomes = new List<Biome>();

		/// <summary>
		/// 血清基础伤害
		/// </summary>
		public int WeaponDamage = 28;

        public override void UpdateDead()
        {
			WarpingCD = 0;
			warpingProgress = WarpingProgress.Default;
			WarpCountIndex = 0;
			Timer = 0;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
			return !IsWarping;
        }
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
			return !IsWarping;
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
			return !IsWarping;
		}
        public override void SetControls()
        {
            if (IsWarping)
            {
				player.controlDown = false;
				player.controlHook = false;
				player.controlJump = false;
				player.controlLeft = false;
				player.controlMount = false;
				player.controlRight = false;
				player.controlUp = false;
				player.controlThrow = false;
            }
        }
        public override void ModifyScreenPosition()
        {
			if (warpingProgress == WarpingProgress.TeleportToEnemy)
			{
				Main.screenPosition = MoveScreen.CurrentPos;
				if (MoveScreen.Distance > 150)
				{
					MoveScreen.Distance *= 0.8f;
				}
                else
                {
					MoveScreen.Distance *= 0.6f;
                }
				if (MoveScreen.Distance < 5) MoveScreen.Distance = 0;
				if (MoveScreen.Distance != 0)
				{
					MoveScreen.CurrentPos = MoveScreen.TargetPos + Vector2.Normalize(MoveScreen.CurrentPos - MoveScreen.TargetPos) * MoveScreen.Distance;
				}
                else
                {
					MoveScreen.CurrentPos = MoveScreen.TargetPos;
				}
			}
            else if (warpingProgress == WarpingProgress.BreakSpace)
            {
				Main.screenPosition = MoveScreen.TargetPos;
			}
			else if (warpingProgress == WarpingProgress.TPing)
            {
				Main.screenPosition = WarpingScreenPos[WarpCountIndex];
            }
            else
            {
				foreach(Projectile proj in Main.projectile)
                {
					if(proj.active && proj.type == ModContent.ProjectileType<DamageProj>())
                    {
						Main.screenPosition += new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-6, 6));
                    }
                }
            }
		}

        public override void PreUpdateMovement()
        {
            if (IsWarping)
            {
				player.velocity = Vector2.Zero;
			}
        }

        public override void PostUpdateMiscEffects()
        {
			if (!IsWarping)
			{
				if (WarpingCD > 0)
				{
					player.AddBuff(ModContent.BuffType<SerumCooldown>(), WarpingCD);
					WarpingCD--;
				}
			}
            else
            {
				WarpingCD = 300;
            }

			if(IsWarping)             //锁定缩放
            {
				Main.GameViewMatrix.Zoom = new Vector2(1, 1);
				Main.CaptureModeDisabled = true;
			}

			if (IsWarping)
			{
				//Main.NewText(Timer);
				if (!Main.npc[SelectedTarget].active || SelectedTarget == -1)   //紧急结束
                {
					QuickStop();
					return;
                }
				player.direction = Direction;
				float rot = Direction < 0 ? MathHelper.Pi : 0;
				player.itemRotation = (float)Math.Atan2(rot.ToRotationVector2().Y * Direction, rot.ToRotationVector2().X * Direction);
				player.itemAnimation = 2;
				player.itemTime = 2;
				player.gravControl = false;
				player.gravControl2 = false;
				player.gravDir = 1;
				player.channel = true;
				player.mount.Dismount(player);

                if (warpingProgress == WarpingProgress.TeleportToEnemy)         //传送到敌人
                {
					Timer++;
					if (Timer == 1)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Teleport"), player.Center);
						Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TPLight>(), 0, 0, player.whoAmI);
						player.GetModPlayer<ClawPlayer>().Teleport(Main.npc[SelectedTarget].Center - new Vector2(Direction, 0) * (5 + Main.npc[SelectedTarget].width / 2));
						int protmp = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TPLight>(), 0, 0, player.whoAmI, 1);
						Main.projectile[protmp].ai[1] = -3;
					}
                    if (Timer == 10)
                    {
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Grab"), player.Center);
					}
                    if (Timer > 30)
                    {
						Timer = 0;
						warpingProgress = WarpingProgress.BreakSpace;
                    }

				}
                if (warpingProgress == WarpingProgress.BreakSpace)          //打开空间
				{
					Timer++;
					if (Timer == 1)
					{
						Projectile.NewProjectile(Main.npc[SelectedTarget].Center + new Vector2(Direction, 0) * 60, new Vector2(Direction, 0), ModContent.ProjectileType<SpaceCrash>(), 0, 0, player.whoAmI);
					}
                    if (Timer == 40)
                    {
						player.Center += new Vector2(Direction, 0) * 40;
						Main.npc[SelectedTarget].Center += new Vector2(Direction, 0) * 40;
						Main.npc[SelectedTarget].oldPosition += new Vector2(Direction, 0) * 40;
					}
                    if (Timer >= 65)
                    {
						warpingProgress = WarpingProgress.TPing;
					}
				}

				if (warpingProgress == WarpingProgress.TPing)            //传送时
				{
					Timer++;
                    if (Timer < 7)           //还在这一个传送
                    {
						int index = WarpCountIndex - 1;
						if (index < 0) index = 0;
						player.Center = Main.screenPosition + (WarpingPlayerPos[index] - WarpingScreenPos[index]);
					}
					else                    //下一个传送
					{
						player.Center = Main.screenPosition + (WarpingPlayerPos[WarpCountIndex] - WarpingScreenPos[WarpCountIndex]);
					}
					if (Timer == 6)          //传送门特效和轨迹特效
					{
						Vector2 Pos = Main.screenPosition + (WarpingPlayerPos[WarpCountIndex] - WarpingScreenPos[WarpCountIndex]);
						int protmp = Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<CrashTP>(), 0, 0, player.whoAmI);
						(Main.projectile[protmp].modProjectile as CrashTP).RelaPos = WarpingPlayerPos[WarpCountIndex] - WarpingScreenPos[WarpCountIndex];
						//Flash.GenerateFlash(player.Center, 2, 1);
						if (WarpCountIndex > 0)
						{
							int index = WarpCountIndex - 1;
							protmp = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TPTrail>(), 0, 0, player.whoAmI);
							(Main.projectile[protmp].modProjectile as TPTrail).RelaBegin = WarpingPlayerPos[index] - WarpingScreenPos[index];
							(Main.projectile[protmp].modProjectile as TPTrail).RelaEnd = WarpingPlayerPos[WarpCountIndex] - WarpingScreenPos[WarpCountIndex];
						}

						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Teleport"), player.Center);

						if (WarpCountIndex == TPCount - 1)        //最后一次传送时
						{
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Broken"), player.Center);
							Pos = Main.screenPosition + (WarpingPlayerPos[WarpCountIndex] - WarpingScreenPos[WarpCountIndex]);
							Flash.GenerateFlash(Pos, 2.5f);
							protmp = Projectile.NewProjectile(Pos, new Vector2(-Direction, 0), ModContent.ProjectileType<SpaceCrashEnd>(), 0, 0, player.whoAmI);
							Main.projectile[protmp].ai[0] = -224;           //精确控时
						}
					}

                    if (Timer == 7)
                    {
						player.GetBiome(CrossBiomes);
                    }

					Main.npc[SelectedTarget].Center = player.Center + new Vector2(Direction, 0) * (5 + Main.npc[SelectedTarget].width / 2);
					Main.npc[SelectedTarget].oldPosition = Main.npc[SelectedTarget].position;
					
					if (Timer >= 20)
					{
						Timer = 0;
						WarpCountIndex++;
						//Main.NewText(WarpCountIndex);
						if (WarpCountIndex >= TPCount)
						{
							WarpCountIndex = 0;
							warpingProgress = WarpingProgress.EndSlash;
						}
					}
				}

				if (warpingProgress == WarpingProgress.EndSlash)
                {
					Timer++;
                    if (Timer == 60)
                    {
						Main.npc[SelectedTarget].Center += new Vector2(Direction * 20, 0);
						Main.npc[SelectedTarget].oldPosition += new Vector2(Direction * 60, 0);
						player.Center += new Vector2(Direction * (250 + Main.npc[SelectedTarget].width / 2), 0);
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/CutScene"), player.Center);
					}
					if (Timer > 120 && Timer < 160)
                    {
                        if (Timer % 3 == 1)
                        {
							//Main.NewText("a");
							Vector2 Pos = Main.screenPosition + new Vector2(Main.rand.Next(Main.screenWidth), Main.rand.Next(Main.screenHeight));
							float randRotation = MathHelper.TwoPi * Main.rand.NextFloat();
							float randLength = Main.rand.NextFloat() + 1f;
							float randWidth = Main.rand.NextFloat() * 2 + 0.5f;
							ClawSlash.GenClawSlash(Pos, randRotation, randLength * GetRanFloat(1.3f), randWidth);
							ClawSlash.GenClawSlash(Pos + (randRotation + MathHelper.Pi / 2).ToRotationVector2() * 80 * randWidth, randRotation, randLength * GetRanFloat(1.2f), randWidth);
							ClawSlash.GenClawSlash(Pos + (randRotation - MathHelper.Pi / 2).ToRotationVector2() * 80 * randWidth, randRotation, randLength * GetRanFloat(1.2f), randWidth);
						}
                    }
                    if (Timer > 200)
                    {
						Timer = 0;
						warpingProgress = WarpingProgress.EndPose;
                    }
                }

				if (warpingProgress == WarpingProgress.EndPose)        //伤害
                {
					Timer++;

                    if (Timer == 40)
                    {
						int protmp=Projectile.NewProjectile(Main.npc[SelectedTarget].Center, Vector2.Zero, ModContent.ProjectileType<DamageProj>(), WeaponDamage, 0, player.whoAmI, SelectedTarget);
						foreach (Biome b in CrossBiomes)
						{
							(Main.projectile[protmp].modProjectile as DamageProj).Effects.Add(b);
						}
						QuickStop();
					}
					/*
					if (Timer > 40 && Timer < 60)
                    {
                        if (Timer % 3 == 1)
                        {
							Vector2 Pos = Main.npc[SelectedTarget].Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20));
							float randRotation = MathHelper.TwoPi * Main.rand.NextFloat();
							float randLength = Main.rand.NextFloat() * 0.8f + 0.1f;
							float randWidth = Main.rand.NextFloat() * 0.75f + 0.25f;
							ClawSlash.GenClawSlash(Pos, randRotation, randLength * GetRanFloat(1.25f), randWidth);
							ClawSlash.GenClawSlash(Pos + (randRotation + MathHelper.Pi / 2).ToRotationVector2() * 20 * randWidth, randRotation * GetRanFloat(1.25f), randLength, randWidth);
							ClawSlash.GenClawSlash(Pos + (randRotation - MathHelper.Pi / 2).ToRotationVector2() * 20 * randWidth, randRotation * GetRanFloat(1.25f), randLength, randWidth);
						}
                    }
					if (Timer > 80)
                    {
						QuickStop();
                    }
					*/
                }
			}

        }



		/// <summary>
		/// 启动攻击
		/// </summary>
		/// <param name="target"></param>
		public void InitWarp(int target)
		{
			Main.GameViewMatrix.Zoom = new Vector2(1, 1);   //锁定缩放
			Main.CaptureModeDisabled = true;
			SelectedTarget = target;
			Direction = Math.Sign(Main.npc[target].Center.X - player.Center.X + 0.01f);
			warpingProgress = WarpingProgress.TeleportToEnemy;
			Timer = 0;
			MoveScreen.CurrentPos = Main.screenPosition;
			MoveScreen.TargetPos = Main.npc[target].Center - new Vector2(Direction, 0) * (5 + Main.npc[target].width / 2) - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
			MoveScreen.Distance = Vector2.Distance(MoveScreen.TargetPos, MoveScreen.CurrentPos);

            if (player.HeldItem.type == ModContent.ItemType<SerumWItem>())
            {
				WeaponDamage = player.HeldItem.damage;
            }
			
			WarpCountIndex = 0;
			OriginalNPCPos = Main.npc[target].Center;

			WarpingPlayerPos[0] = player.Center;
			WarpingScreenPos[0] = Main.screenPosition;
			WarpingPlayerPos[TPCount - 1] = MoveScreen.TargetPos + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
			WarpingScreenPos[TPCount - 1] = MoveScreen.TargetPos;

			for (int i = 1; i < TPCount - 1; i++)
			{
				WarpingPlayerPos[i] = GetRandomPos();
				Vector2 ScreenPos = WarpingPlayerPos[i] - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
				WarpingScreenPos[i] = ScreenPos + new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-250, 250));
			}

			for (int i = 0; i < TPCount; i++)
			{
				bool Light = false;
				if (i > 0 && i < TPCount - 1) Light = true;
				SerumW.SavedBG[i] = WCapture.GetRenderedTexture(WarpingScreenPos[i], Light);
			}
			
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void QuickStop()
        {
			SelectedTarget = -1;
			warpingProgress = WarpingProgress.Default;
			Timer = 0;
			MoveScreen.CurrentPos = Main.screenPosition;
			MoveScreen.TargetPos = Main.screenPosition;
			MoveScreen.Distance = 0;

			
			WarpCountIndex = 0;
			OriginalNPCPos = player.Center;
			for (int i = 0; i < TPCount; i++)
			{
				WarpingPlayerPos[i] = player.Center;
				WarpingScreenPos[i] = Main.screenPosition;
			}

			foreach(Texture2D tex in SavedBG)
            {
                if (tex != null)
                {
                    if (!tex.IsDisposed)
                    {
						tex.Dispose();
                    }
                }
            }

			CrossBiomes.Clear();

			player.immune = true;
			player.immuneTime = 150;
			for (int i = 0; i < player.hurtCooldowns.Length; i++)
            {
				player.hurtCooldowns[i] = 150;
            }
			player.immuneNoBlink = true;

			Main.CaptureModeDisabled = false;

            if (player.HeldItem.type == ModContent.ItemType<SerumWItem>())
            {
				player.HeldItem.stack--;
                if (player.HeldItem.stack <= 0)
                {
					player.HeldItem.TurnToAir();
                }
            }
		}


		public Vector2 GetRandomPos()
        {
			bool flag = false;
			int teleportStartX = 200;
			int teleportRangeX = Main.maxTilesX - 400;
			int teleportStartY = 200;
			int teleportRangeY = Main.maxTilesY - 400;
			Vector2 vector = TestTeleport(ref flag, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY);
			if (flag)
			{
				return vector;
			}
			return Main.LocalPlayer.Center;
		}

		/*
		private void SelectedTeleport(Vector2 Pos)
		{
			Vector2 vector2 = Pos;
			Teleport(vector2);
			player.velocity = Vector2.Zero;
			if (Main.netMode == NetmodeID.Server)
			{
				RemoteClient.CheckSection(player.whoAmI, player.position, 1);
				NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, vector2.X, vector2.Y, 3, 0, 0);
			}

		}
		*/

		public void Teleport(Vector2 newPos)
		{
			try
			{
				player.grappling[0] = -1;
				player.grapCount = 0;
				foreach(Projectile hook in Main.projectile)
                {
					if (hook.active && hook.owner == player.whoAmI && hook.aiStyle == 7)
                    {
						hook.Kill();
                    }
                }
				
				//float num = MathHelper.Clamp(1f - player.teleportTime * 0.99f, 0.01f, 1f);
				//Main.TeleportEffect(player.getRect(), Style, extraInfo2, num);
				//float num2 = Vector2.Distance(player.position, newPos);
				//PressurePlateHelper.UpdatePlayerPosition(player);
				player.Center = newPos;
				player.fallStart = (int)(player.position.Y / 16f);
				if (player.whoAmI == Main.myPlayer)
				{
					/*
					if (num2 < new Vector2(Main.screenWidth, Main.screenHeight).Length() / 2f + 100f)
					{
						int time = 0;
						if (Style == 1)
						{
							time = 10;
						}
						Main.SetCameraLerp(0.1f, time);
					}
					*/

					//Main.BlackFadeIn = 255;
					//Lighting.BlackOut();
					Main.screenLastPosition = Main.screenPosition;
					Main.screenPosition.X = player.position.X + player.width / 2 - Main.screenWidth / 2;
					Main.screenPosition.Y = player.position.Y + player.height / 2 - Main.screenHeight / 2;
					Main.quickBG = 10;


					if (Main.mapTime < 5)
					{
						Main.mapTime = 5;
					}
					Main.maxQ = true;
					Main.renderNow = true;
				}
				
				
				//PressurePlateHelper.UpdatePlayerPosition(player);
				for (int j = 0; j < 3; j++)
				{
					player.UpdateSocialShadow();
				}
				player.oldPosition = player.position + player.BlehOldPositionFixer;
				//Main.TeleportEffect(player.getRect(), Style, extraInfo2, num);
				//player.teleportTime = 1f;
				//player.teleportStyle = Style;
			}
			catch
			{
			}
		}

		private Vector2 TestTeleport(ref bool canSpawn, int teleportStartX, int teleportRangeX, int teleportStartY, int teleportRangeY)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int width = player.width;
			Vector2 vector = new Vector2(num2, num3) * 16f + new Vector2(-width / 2f + 8f, -player.height);
			while (!canSpawn && num < 1000)
			{
				num++;
				num2 = teleportStartX + Main.rand.Next(teleportRangeX);
				num3 = teleportStartY + Main.rand.Next(teleportRangeY);
				vector = new Vector2(num2, num3) * 16f + new Vector2(-width / 2f + 8f, -player.height);
				if (!Collision.SolidCollision(vector, width, player.height))
				{
					if (Main.tile[num2, num3] == null)
					{
						Main.tile[num2, num3] = new Tile();
					}
					if ((Main.tile[num2, num3].wall != 87 || num3 <= Main.worldSurface || NPC.downedPlantBoss) && (!Main.wallDungeon[(int)Main.tile[num2, num3].wall] || (double)num3 <= Main.worldSurface || NPC.downedBoss3))
					{
						int i = 0;
						while (i < 100)
						{
							if (Main.tile[num2, num3 + i] == null)
							{
								Main.tile[num2, num3 + i] = new Tile();
							}
							Tile tile = Main.tile[num2, num3 + i];
							vector = new Vector2(num2, num3 + i) * 16f + new Vector2(-width / 2f + 8f, -player.height);
                            bool flag = !Collision.SolidCollision(vector, width, player.height);

							if (flag)
							{
								i++;
							}
							else
							{
								if (tile.active() && !tile.inActive() && Main.tileSolid[(int)tile.type])
								{
									break;
								}
								i++;
							}
						}
						if (!Collision.LavaCollision(vector, width, player.height) && Collision.HurtTiles(vector, player.velocity, width, player.height, false).Y <= 0f)
						{
							Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
							if (Collision.SolidCollision(vector, width, player.height) && i < 99)
							{
								Vector2 vector3 = Vector2.UnitX * 16f;
								if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
								{
									vector3 = -Vector2.UnitX * 16f;
									if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
									{
										vector3 = Vector2.UnitY * 16f;
										if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
										{
											vector3 = -Vector2.UnitY * 16f;
											if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
											{
												canSpawn = true;
                                                break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return vector;
		}

		public float GetRanFloat(float a)
        {
			float b = (float)Math.Abs(a - 1);
			return 1 + Main.rand.NextFloat() * b;
        }
    }
}