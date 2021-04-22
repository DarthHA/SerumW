using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Graphics.Capture;

namespace SerumW
{

    internal class WCapture
	{

		public static Texture2D GetRenderedTexture(Vector2 Pos,bool Light)
		{
			if (Light)
			{
				WLight.Light = true;
			}
			WCapture wCapture = new WCapture(Pos);
			wCapture.Capture();
			Texture2D originalTex = wCapture.DrawTick();
			Texture2D resultTex = new Texture2D(Main.instance.GraphicsDevice, originalTex.Width, originalTex.Height);
			Color[] data = new Color[originalTex.Width * originalTex.Height];
			originalTex.GetData(data);
			resultTex.SetData(data);
			wCapture.Dispose();
			WLight.Light = false;
			return resultTex;
		}

		public WCapture(Vector2 Pos)
		{
			Point point = (Pos + Main.GameViewMatrix.Translation).ToTileCoordinates();
			Point point2 = (Pos + Main.GameViewMatrix.Translation + new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom).ToTileCoordinates();
			_activeSettings.Area = new Rectangle(point.X, point.Y, point2.X - point.X + 1, point2.Y - point.Y + 1);
			//_activeSettings.Area = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
			_activeSettings.CaptureBackground = true;
			_activeSettings.Biome = GetCaptureBiome();
			_activeSettings.CaptureMech = false;
			_activeSettings.CaptureEntities = true;
			_activeSettings.OutputName = "";
			_activeSettings.UseScaling = false;
			try
			{
				_frameBuffer = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, Main.instance.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
			}
			catch
			{
				Main.CaptureModeDisabled = true;
				return;
			}
		}



		~WCapture()
		{
			Dispose();
		}


		private void Capture()
		{
			Main.GlobalTimerPaused = true;
			Monitor.Enter(_captureLock);
            Rectangle area = _activeSettings.Area;
			float num = 1f;

			//_tilesProcessed = 0f;
			//_totalTiles = area.Width * area.Height;
			for (int i = area.X; i < area.X + area.Width; i += 126)
			{
				for (int j = area.Y; j < area.Y + area.Height; j += 126)
				{
					int num3 = Math.Min(128, area.X + area.Width - i);
					int num4 = Math.Min(128, area.Y + area.Height - j);
					int width = (int)Math.Floor(num * (num3 << 4));
					int height = (int)Math.Floor(num * (num4 << 4));
					int x = (int)Math.Floor(num * (i - area.X << 4));
					int y = (int)Math.Floor(num * (j - area.Y << 4));
					_renderQueue.Enqueue(new CaptureChunk(new Rectangle(i, j, num3, num4), new Microsoft.Xna.Framework.Rectangle(x, y, width, height)));
				}
			}
			Monitor.Exit(_captureLock);
		}


		private Texture2D DrawTick()
		{
			Monitor.Enter(_captureLock);
			if (_activeSettings == null)
			{
				return null;
			}
			if (_renderQueue.Count > 0)
			{
                CaptureChunk captureChunk = _renderQueue.Dequeue();
				Main.instance.GraphicsDevice.SetRenderTarget(_frameBuffer);
				Main.instance.GraphicsDevice.Clear(Color.Transparent);
				Main.instance.DrawCapture(captureChunk.Area, _activeSettings);

				Main.instance.GraphicsDevice.SetRenderTarget(null);


				//其中ScaledArea是Tex的长和宽
				

				//_tilesProcessed += captureChunk.Area.Width * captureChunk.Area.Height;
			}
			if (_renderQueue.Count == 0)
			{
				FinishCapture();
			}
			Monitor.Exit(_captureLock);

			return _frameBuffer;
		}

		


		private void FinishCapture()
		{
			Main.GlobalTimerPaused = false;
			CaptureInterface.EndCamera();
			if (_scaledFrameBuffer != null)
			{
				_scaledFrameBuffer.Dispose();
				_scaledFrameBuffer = null;
			}
			_activeSettings = null;
		}


		private void Dispose()
		{
			Monitor.Enter(_captureLock);
			if (_isDisposed)
			{
				return;
			}
			
			_frameBuffer.Dispose();
			if (_scaledFrameBuffer != null)
			{
				_scaledFrameBuffer.Dispose();
				_scaledFrameBuffer = null;
			}
			_isDisposed = true;
			Monitor.Exit(_captureLock);
		}


		private CaptureBiome GetCaptureBiome()
		{
			int BackgroundIndex = -1, BackgroundIndex2 = -1, WaterStyle;

			for (int i = 0; i < Main.bgAlpha.Length; i++)
            {
				if (Main.bgAlpha[i] == 1)
                {
					BackgroundIndex = i;
					break;
                }
            }
			for (int i = 0; i < Main.bgAlpha2.Length; i++)
			{
				if (Main.bgAlpha2[i] == 1)
				{
					BackgroundIndex2 = i;
					break;
				}
			}


			if(Main.bgAlpha2[0] == 1f)
            {
				BackgroundIndex2 = 6;
			}

            if (Main.holyTiles >= 400)
            {
				BackgroundIndex = 6;
			}
			WaterStyle = Main.waterStyle;

			return new CaptureBiome(BackgroundIndex, BackgroundIndex2, WaterStyle);

		}



		private RenderTarget2D _frameBuffer;

		private RenderTarget2D _scaledFrameBuffer;

		private readonly object _captureLock = new object();


		private bool _isDisposed;


		private CaptureSettings _activeSettings = new CaptureSettings();


		private Queue<CaptureChunk> _renderQueue = new Queue<CaptureChunk>();



		// Token: 0x020003A5 RID: 933
		private class CaptureChunk
		{
			public CaptureChunk(Rectangle area, Rectangle scaledArea)
			{
				Area = area;
				ScaledArea = scaledArea;
			}

			public readonly Rectangle Area;

			public readonly Rectangle ScaledArea;
		}
	}
}
