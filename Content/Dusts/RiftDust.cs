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
			dust.scale *= 1.11f;
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.995f;
			dust.rotation += dust.velocity.X * 0.15f;
			dust.scale *= 0.99f;

			float light = 0.001f * dust.scale;

			Lighting.AddLight(dust.position, ColorLib.Rift.R * light, ColorLib.Rift.G * light, ColorLib.Rift.B * light);

			if (dust.scale < 0.1f)
			{
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}

	}
}