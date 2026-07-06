using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common.Systems
{
    //Thanks Lucille for exposing which hook to detour!
    public class WorldIconSystem : ModSystem
    {
        public override void OnModLoad()
        {

            Main.QueueMainThreadAction(() =>
            {
                On_AWorldListItem.GetIcon += UseCustomIcons;
            });
        }

        private static Asset<Texture2D> UseCustomIcons(On_AWorldListItem.orig_GetIcon orig, AWorldListItem self)
        {
            if (self.Data.TryGetHeaderData<WorldIconSystem>(out TagCompound tag))
            {

                if (tag.ContainsKey("TenebrisCanSpawnInWorldEvilBiome"))
                {
                    return DTAssetLib.TenebrisCorruptionWorldIcon;
                }
            }
            return orig(self);
        }

        //I havent heard of this method before. Probably due to its niche use case.
        public override void SaveWorldHeader(TagCompound tag)
        {
            if (DTFlags.TenebrisCanSpawnInWorldEvilBiome)
            {
                tag["TenebrisCanSpawnInWorldEvilBiome"] = true;
            }
        }
    }
}