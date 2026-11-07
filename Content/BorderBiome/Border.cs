using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.RiftBiome;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome
{
    public class Border : ModBiome
    {
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Mushroom;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/BorderPlaceholder");

        public override void OnInBiome(Player player)
        {
           
        }
        public override string BestiaryIcon => "DestroyerTest/Assets/Textures/BorderIcon";
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath;

        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<BorderTileCounts>().BlockCount > 100;

        
            return b1;
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh; 
        
    }
}
