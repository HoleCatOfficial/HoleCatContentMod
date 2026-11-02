using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.HellSet
{
    [AutoloadEquip(EquipType.Body)]
    public class HellPlatemail : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 28;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.09f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellArmorScrap>(9)
                .AddIngredient(ItemID.Ectoplasm, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
