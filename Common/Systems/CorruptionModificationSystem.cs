using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace DestroyerTest.Common.Systems
{
    public class CorruptionModificationSystem : ModSystem
    {
        public static LocalizedText ConversionMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            ConversionMessage = Language.GetText("Mods.DestroyerTest.WorldGen.TenebrisCorruption");
        }

        //Adding this back in for testing purposes.
        public static bool JustPressed(Keys key)
        {
            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }

        public static bool Gen = false;
        public override void PostUpdateWorld()
        {
            
            if (!Gen && DTFlags.TenebrisCanSpawnInWorldEvilBiome)
            {
                Generation();
                Gen = true;
            }
            

            /*
            if (JustPressed(Keys.F))
            {
                Generation();
            }
            */

        }

        public void Generation()
        {
            Main.NewText(ConversionMessage.Value, ColorLib.TenebrisGradient);

            //600 attempts. Will pick a more appropriate number later.
            for (int i = 0; i < Main.maxTilesX * 2; i++)
            {
                Point P = WorldGen.RandomWorldPoint();

                Tile T = Framing.GetTileSafely(P);
                if (T.HasTile && T.TileType == TileID.Ebonstone)
                {
                    WorldGen.OreRunner(P.X, P.Y, 32, 12, (ushort)ModContent.TileType<Tile_ShadeParticleBlock>());
                }
            }
        }

        public override void ClearWorld()
        {
            Gen = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("Gen", Gen);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("Gen"));
            {
                Gen = tag.GetBool("Gen");
            }
        }
    }
}
