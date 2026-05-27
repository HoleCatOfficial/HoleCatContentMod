using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
    public class LilliesDashDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 1.0f;
            dust.noGravity = false;
            dust.noLight = false;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.995f;
            dust.rotation += dust.velocity.X * 0.15f;
            dust.scale *= 0.99f;

            float light = 0.001f * dust.scale;

            Lighting.AddLight(dust.position, Color.MediumPurple.R * light, Color.MediumPurple.G * light, Color.MediumPurple.B * light);

            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }

            return false;
        }

    }
}