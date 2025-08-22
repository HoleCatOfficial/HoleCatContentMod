using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
	public class RiftDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 1.0f;
			dust.noGravity = true;
			dust.noLight = false;
			dust.scale *= 1.5f;
		}

        public override bool PreDraw(Dust dust)
        {
			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();
			Texture2D DustTexture = TextureAssets.Dust.Value;
			Texture2D GlowTex = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SimpleParticle").Value;

			Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Main.EntitySpriteDraw(
				GlowTex,
				dust.position - Main.screenPosition,
				dust.frame,
				ColorLib.Rift,
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
				ColorLib.Rift,
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
			dust.scale *= 0.99f;

			float light = 0.005f * dust.scale;

			Lighting.AddLight(dust.position, ColorLib.Rift.R * light, ColorLib.Rift.G * light, ColorLib.Rift.B * light);

			if (dust.scale < 0.75f)
			{
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}

	}
}