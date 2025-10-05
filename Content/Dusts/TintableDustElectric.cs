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
            dust.noGravity = true;
            dust.noLight = false;
            LightColor = dust.color;
			dust.color = new Color(
				MathHelper.Clamp(dust.color.R + 100, 0, 255),
				MathHelper.Clamp(dust.color.G + 100, 0, 255),
				MathHelper.Clamp(dust.color.B + 100, 0, 255),
				dust.color.A
			);
        }

		public override bool Update(Dust dust) {
			float num2 = dust.scale;
			if ((double) num2 > 1.0)
			num2 = 1f;
			if (!dust.noLight)
			Lighting.AddLight((int) ((double) dust.position.X / 16.0), (int) ((double) dust.position.Y / 16.0), num2 * 0.2f, num2 * 0.7f, num2 * 1f);
			if (dust.noGravity)
			{
			dust.velocity *= 0.93f;
			if ((double) dust.fadeIn == 0.0)
				dust.scale += 1f / 400f;
			}
			dust.velocity *= new Vector2(0.97f, 0.99f);
			if (dust.customData != null && dust.customData is Player)
			{
			Player customData = (Player) dust.customData;
			dust.position += customData.position - customData.oldPosition;
			}
			dust.scale -= 0.01f;

			return false;
		}

	}
}