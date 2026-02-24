using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common.Systems
{
    public class HardmodeNotificationSystem : ModSystem
    {
        public bool Flag1 = false;
        public int Delay = 180;

        public override void PostUpdateWorld()
        {
            if (Main.hardMode && !Flag1)
            {
                if (Delay > 0)
                {
                    Delay--;
                }
                if (Delay <= 0)
                {
                    SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Break);
                    Main.NewText("The Great Wyvern has been shot from the sky!", Color.Purple);
                    Flag1 = true;
                }
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (Flag1)
            {
                tag["Flag1"] = true;
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Flag1 = tag.ContainsKey("Flag1");
        }
        
    }
}