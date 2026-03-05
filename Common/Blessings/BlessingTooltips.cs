using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Blessings
{
    public class BlessingTooltips : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public static List<int> OfferingItems = new List<int>()
        {
            ItemID.LifeCrystal,
            ItemID.BottledHoney,
            ItemID.HermesBoots
        };

        public static List<int> ModifierHerbs = new List<int>
        {
            ItemID.Daybloom,
            ItemID.Moonglow,
            ItemID.Blinkroot,
            ItemID.Deathweed,
            ItemID.Waterleaf,
            ItemID.Fireblossom,
            ItemID.Shiverthorn
        };

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (OfferingItems.Contains(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "OfferingItemLine", Language.GetTextValue("Mods.DestroyerTest.Blessings.OfferingItemTooltip")) { OverrideColor = ColorLib.Soul});
            }
            if (ModifierHerbs.Contains(item.type))
            {
                tooltips.Add(new TooltipLine(Mod, "HerbLine", Language.GetTextValue("Mods.DestroyerTest.Blessings.HerbTooltip")) { OverrideColor = ColorLib.Soul });
            }
        }
    }
}
