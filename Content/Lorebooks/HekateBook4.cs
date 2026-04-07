using System.Collections.Generic;
using DestroyerTest.Content.UI;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Lorebooks
{
    public class HekateBook4 : BaseLoreBook
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override bool? UseItem(Player player)
        {
            if (PageReader.Visible)
                return true;

            var pages = new List<string>()
            {
                Language.GetText("Mods.DestroyerTest.BookText.HekateBook4.Page1").Value,
                Language.GetText("Mods.DestroyerTest.BookText.HekateBook4.Page2").Value,
                Language.GetText("Mods.DestroyerTest.BookText.HekateBook4.Page3").Value
            };

            ModContent.GetInstance<TextReaderSystem>().OpenLocalizedBook("HekateBook4", 2000, pages.Count);
            return true;
        }
    }
}
