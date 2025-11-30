using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
	public class ConstitutionDust1 : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 1.0f;
			dust.noGravity = true;
			dust.noLight = false;
			dust.scale *= 1.11f;
            dust.color = Color.White;
		}

        private Color c1 = new Color(247, 233, 141);
        private Color c2 = new Color(207, 120, 90);
        private Color c3 = new Color(183, 61, 114);
        private Color c4 = new Color(143, 39, 120);
        private Color c5 = new Color(80, 38, 91);
        private Color c6 = new Color(33, 36, 37);
        private Color c7 = new Color(25, 33, 38);
        private Color c8 = new Color(18, 23, 24);
        private int time = 0;
        
		public override bool Update(Dust dust)
		{
            const int maxTime = 120;
            time++;

            float t = MathHelper.Clamp(time / (float)maxTime, 0f, 1f);

            dust.scale = MathHelper.Lerp(dust.scale, 0.01f, t);

            Color result =
                Color.Lerp(
                    Color.Lerp(
                        Color.Lerp(c1, c2, t),
                        Color.Lerp(c3, c4, t),
                    0.5f),
                    Color.Lerp(
                        Color.Lerp(c5, c6, t),
                        Color.Lerp(c7, c8, t),
                    0.5f),
                t);

            dust.color = result;

            if (time >= maxTime)
            {
                dust.active = false;
            }

            return false;
		}

	}
}