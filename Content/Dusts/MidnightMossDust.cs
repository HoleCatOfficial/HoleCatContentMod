using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
    public class MidnightMossDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(172, 23, 232) * 0.02f * dust.scale);
            return true;
        }

    }
}