using System.Collections.Generic;
using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Blessings
{
    /// <summary>
    /// OBSOLETE
    /// The way blessings are currently implemented involves combinations of items.
    /// The items have this displayed in their tooltips.
    /// </summary>
    public class BlessingTooltips : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public static List<int> OfferingItems = new List<int>()
        {
            ItemID.LifeCrystal,
            ItemID.ManaCrystal,
            ItemID.NaturesGift,
            ItemID.BottledHoney,
            ItemID.Starfish,
            ItemID.ShadowScale,
            ItemID.TissueSample,
            ItemID.Ambrosia,
            ItemID.JungleSpores,
            ItemID.HellstoneBar,
            ModContent.ItemType<StellarMatter>()
        };

        //While originally all herbs were planned to be covered, I ultimately didn't have it in me to do all of them, especially considering how very boring it was to set up a new blessing.
        public static List<int> ModifierHerbs = new List<int>
        {
            ItemID.Daybloom,
            ItemID.Moonglow,
            //ItemID.Blinkroot,
            //ItemID.Deathweed,
            //ItemID.Waterleaf,
            //ItemID.Fireblossom,
            //ItemID.Shiverthorn
        };

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (OfferingItems.Contains(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "OfferingItemLine", Language.GetTextValue("Mods.DestroyerTest.Blessings.OfferingItemTooltip")) { OverrideColor = ColorLib.Soul });
            }
            if (ModifierHerbs.Contains(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "HerbLine", Language.GetTextValue("Mods.DestroyerTest.Blessings.HerbTooltip")) { OverrideColor = ColorLib.Soul });
            }
        }
    }
}
