using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System.Collections.Generic;
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
			dust.scale *= 1.11f;
		}

        public override bool PreDraw(Dust dust)
        {
            Asset<Texture2D> Dusttex = ModContent.Request<Texture2D>(Texture);
			//Main.spriteBatch.UseBlendState(BlendState.Additive);
		
            Main.spriteBatch.Draw(Dusttex.Value, dust.position - Main.screenPosition, dust.frame, dust.color, dust.rotation, dust.frame.Size() / 2f, dust.scale, SpriteEffects.None, 0f);
			//Main.spriteBatch.ResetToDefault();
            return false;
        }

        public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.995f;
			dust.rotation += dust.velocity.X * 0.5f;
			dust.scale *= 0.99f;

			

			float light = 0.001f * dust.scale;

			Lighting.AddLight(dust.position, ColorLib.Rift.R * light, ColorLib.Rift.G * light, ColorLib.Rift.B * light);

			if (dust.scale < 0.4f)
			{
				dust.active = false;
			}

			return false; 
		}

	}
}