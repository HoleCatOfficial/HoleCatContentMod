using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common
{
    public abstract class OneTimeConsumable : ModItem
    {
        public abstract bool Consumed();

        public abstract void OnConsume(Player player);

        public override bool? UseItem(Player player)
        {

            return true;
        }

        public override bool ConsumeItem(Player player)
        {

            return true;
        }

        public override void OnConsumeItem(Player player)
        {
            OnConsume(player);
            
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine Info = new TooltipLine(Mod, "AlreadyConsumed", Language.GetTextValue("Mods.DestroyerTest.Extras.OneTimeConsumableInfo")) { OverrideColor = Colors.CoinGold };

            if (Consumed())
            {
                tooltips.Add(Info);
            }
        }
    }
}
