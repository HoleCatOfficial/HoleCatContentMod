using DestroyerTest.Common;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{

    [AutoloadEquip(EquipType.Legs)]
    public class HoleCatLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.isDevItem[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<DevRarity>();
            Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {

        }
    }
}