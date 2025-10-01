using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
	public class TintableDustElectric : ModDust
	{
        public Color LightColor;
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.6f;
            dust.noGravity = true;
            dust.noLight = false;
            dust.scale *= 1.2f;
            LightColor = dust.color;
			dust.color = new Color(
				MathHelper.Clamp(dust.color.R + 100, 0, 255),
				MathHelper.Clamp(dust.color.G + 100, 0, 255),
				MathHelper.Clamp(dust.color.B + 100, 0, 255),
				dust.color.A
			);
        }

		public override bool Update(Dust dust) {
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.25f;
			dust.scale *= 0.90f;

			float light = 0.15f * dust.scale;

			Lighting.AddLight(dust.position, LightColor.ToVector3() * light);

			if (dust.scale < 0.1f) {
				dust.active = false;
			}

			return false;
		}

	}
}