using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
	public class SoulDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 1.0f;
			dust.noGravity = true;
			dust.noLight = false;
			dust.scale *= 1.11f;
		}

        public override bool PreDraw(Dust dust)
        {
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();
			Texture2D DustTexture = TextureAssets.Dust.Value;
			var GlowTex = DTAssetLib.PointGlow.Value;

			Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Main.EntitySpriteDraw(
				GlowTex,
				dust.position - Main.screenPosition,
				dust.frame,
				ColorLib.Soul3,
				dust.rotation,
				new Vector2(dust.frame.Width / 2, dust.frame.Height / 2),
				dust.scale,
				SpriteEffects.None,
				0
			);

			Main.EntitySpriteDraw(
				DustTexture,
				dust.position - Main.screenPosition,
				dust.frame,
				ColorLib.Soul,
				dust.rotation,
				new Vector2(dust.frame.Width / 2, dust.frame.Height / 2),
				dust.scale,
				SpriteEffects.None,
				0
			);
			Utility.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }


		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.995f;
			dust.rotation += dust.velocity.X * 0.15f;
			dust.scale *= 0.9f;

			float light = 0.005f * dust.scale;

			Lighting.AddLight(dust.position, ColorLib.Soul.ToVector3() * 0.5f);

			if (dust.scale < 0.75f)
			{
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}

	}
}