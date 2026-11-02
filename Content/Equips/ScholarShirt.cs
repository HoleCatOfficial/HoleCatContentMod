using DestroyerTest.Rarity;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting a X_Body.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
    public class ScholarShirt : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ModContent.RarityType<HeliciteRarity>(); // The rarity of the item
            Item.vanity = true;
        }
    }
}