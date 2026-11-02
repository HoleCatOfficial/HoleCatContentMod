using DestroyerTest.Common;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PetrifiedSet
{
    [AutoloadEquip(EquipType.Legs)]
    public class PetrifiedGreaves : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = DTUtils.GetScepterArmorSellPricePerRarity(Item.rare);
            Item.rare = ModContent.RarityType<WineRarity>();
            Item.defense = 8;
        }

    }
}