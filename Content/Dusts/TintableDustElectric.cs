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
			// Store the original color to use for lighting
			LightColor = dust.color;

			// Make a pastel (whitened) version of the original color for drawing
			// Lerp toward white to produce a pastel look. Keep original alpha.
			Color pastel = Color.Lerp(LightColor, Color.White, 0.6f);
			pastel.A = LightColor.A;
			dust.color = pastel;
		}

		public override bool Update(Dust dust) {
			float num2 = dust.scale;
			if (num2 > 1f)
				num2 = 1f;

			// Emit light using the original (stored) color, normalized to 0..1 and scaled by size
			Vector3 lightVec = new Vector3(LightColor.R / 255f, LightColor.G / 255f, LightColor.B / 255f) * num2;
			if (!dust.noLight)
				Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), lightVec.X, lightVec.Y, lightVec.Z);

			if (dust.noGravity)
			{
				dust.velocity *= 0.93f;
				if (dust.fadeIn == 0f)
					dust.scale += 1f / 400f;
			}
			dust.velocity *= new Vector2(0.97f, 0.99f);
			if (dust.customData != null && dust.customData is Player)
			{
				Player customData = (Player)dust.customData;
				dust.position += customData.position - customData.oldPosition;
			}
			dust.scale -= 0.01f;

			if (dust.scale < 0.01f)
			{
				dust.active = false;
			}

			return false;
		}

	}
}