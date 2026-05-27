using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Dusts;
using InnoVault.PRT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class TenebrisCorruption : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            return DTFlags.TenebrisCanSpawnInWorldEvilBiome && player.ZoneCorrupt;
        }

        public override int Music => MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/TenebrisCorruption");

        public override SceneEffectPriority Priority =>  SceneEffectPriority.BiomeHigh;
        public override float GetWeight(Player player)
        {
            return 1f;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                player.ZoneCorrupt = true;
                Rectangle Screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, (int)Main.screenWidth, (int)Main.screenHeight);
                if (Main.rand.NextBool(10))
                {
                    Dust d = Dust.NewDustDirect(Main.screenPosition, Main.screenWidth, Main.screenHeight, ModContent.DustType<TenebrisDarkmatterDust>(), Main.rand.NextFloat(-2, 2), -10, 0, default, 2f);
                    d.noGravity = true;
                    d.noLight = true;
                    d.noLightEmittence = true;
                }
            }
        }
    }
}
