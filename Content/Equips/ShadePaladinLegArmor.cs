using DestroyerTest.Rarity;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Legs)]
    public class ShadePaladinLegArmor : ModItem
    {


        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ModContent.RarityType<ShimmeringRarity>();
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {

        }
    }
}