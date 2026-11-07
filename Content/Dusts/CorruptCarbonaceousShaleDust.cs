using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
    public class CorruptCarbonaceousShaleDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = true;
        }

        public override bool Update(Dust dust)
        {
            return true;
        }

    }
}