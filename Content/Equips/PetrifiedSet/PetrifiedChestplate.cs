using DestroyerTest.Common;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PetrifiedSet
{
    [AutoloadEquip(EquipType.Body)]
    public class PetrifiedChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 26;
            Item.value = DTUtils.GetScepterArmorSellPricePerRarity(Item.rare);
            Item.rare = ModContent.RarityType<WineRarity>();
            Item.defense = 12;
        }


    }
}