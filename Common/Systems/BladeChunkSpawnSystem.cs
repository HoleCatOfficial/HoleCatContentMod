using DestroyerTest.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Systems
{
    public class BladeChunkSpawnSystem : ModSystem
    {
        int Count = 0;
        public override void PostUpdateTime()
        {
            if (Main.dayTime)
            {
                Count = 0;
            }
            if (Main.rand.NextBool(7000) && !Main.dayTime && !Main.dedServ && !Main.CurrentFrameFlags.AnyActiveBossNPC && Count < 4 && !Main.pumpkinMoon && !Main.snowMoon && Main.LocalPlayer.ZoneOverworldHeight)
            {
                Main.NewText("Something zips down from the skies...");
                Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), Main.LocalPlayer.Center + new Vector2(Main.rand.Next(-600, 600), -2000), Vector2.Zero, ModContent.ProjectileType<BladeChunkProjectile>(), 50, 10);
                Count++;
            }
        }
    }
}
