using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace SerumW
{
    public class WarpScreenShaderData : ScreenShaderData
    {
        public WarpScreenShaderData(string passName) : base(passName)
        {
        }
        public WarpScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName)
        {
        }
        public override void Apply()
        {
            base.Apply();
        }
    }
}