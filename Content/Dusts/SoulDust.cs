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


		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.995f;
			dust.rotation += dust.velocity.X * 0.15f;
			dust.scale *= 0.9f;

			float light = 0.005f * dust.scale;

			Lighting.AddLight(dust.position, ColorLib.Soul.ToVector3() * 0.5f);

			if (dust.scale < 0.05f)
			{
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}

	}
}