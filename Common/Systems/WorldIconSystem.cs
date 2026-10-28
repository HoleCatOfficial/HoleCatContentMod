using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour.HookGen;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common.Systems
{
    //Approach modeled after Wrath of the Gods. I would not have been able to figure this out myself.
    public class WorldIconSystem : ModSystem
    {
        public override void OnModLoad()
        {

            
        }

        public override void SetStaticDefaults()
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
                    if (tag.ContainsKey("TenebrisCanSpawnInWorldEvilBiome") && tag.ContainsKey("AvatarWorld"))
                    {
                        return DTAssetLib.TenebrisCorruptionWorldIcon_WOTG;
                    }

                    if (tag.ContainsKey("TenebrisCanSpawnInWorldEvilBiome") && (self.Data.WorldGenModsRecorded && self.Data.TryGetModVersionGeneratedWith("AAModClassic", out _)))
                    {
                        return DTAssetLib.TenebrisCorruptionWorldIcon_AAClassic;
                    }

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