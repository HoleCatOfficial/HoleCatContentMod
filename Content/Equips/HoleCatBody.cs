using DestroyerTest.Common;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Body)]
    public class HoleCatBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.isDevItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<DevRarity>();
            Item.defense = 55;
        }
    }
}