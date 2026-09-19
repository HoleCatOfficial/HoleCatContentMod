using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Common
{
    public abstract class OneTimeConsumable : ModItem
    {
        public bool Consumed = false;

        public abstract void OnConsume(Player player);

        public override bool ConsumeItem(Player player)
        {
            OnConsume(player);
            Consumed = true;
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            return !Consumed;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Consumed"] = Consumed;
        }

        public override void LoadData(TagCompound tag)
        {
            Consumed = tag.GetBool("Consumed");
        }



        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine Info = new TooltipLine(Mod, "AlreadyConsumed", Language.GetTextValue("Mods.DestroyerTest.OneTimeConsumableInfo"));

            if (Consumed)
            {
                tooltips.Add(Info);
            } 
        }
    }
}
