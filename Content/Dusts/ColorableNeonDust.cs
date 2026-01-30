using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
	public class ColorableNeonDust : ModDust
	{
		public override void OnSpawn(Dust dust) {
			dust.noGravity = true;
			dust.noLight = false;
		}

        public override bool PreDraw(Dust dust)
        {
            Asset<Texture2D> Dusttex = ModContent.Request<Texture2D>(Texture);
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.spriteBatch.Draw(Dusttex.Value, dust.position - Main.screenPosition, dust.frame, dust.color, dust.rotation, dust.frame.Size() / 2f, dust.scale, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return false;
        }

		public override bool Update(Dust dust) 
        {
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.15f;
			dust.scale *= 0.9f;

			float light = 0.05f * dust.scale;

			Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.3f);

			if (dust.scale < 0.1f) {
				dust.active = false;
			}

			return true;
		}
	}
}