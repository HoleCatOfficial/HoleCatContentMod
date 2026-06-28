using DestroyerTest.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PotionFlowers
{
    public class RadiantRose : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack.Add(Type);


        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 64;
            Item.maxStack = 1;
            Item.value = 1000;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<PotionFlowerPlayer>(out PotionFlowerPlayer flower))
            {
                flower.RadiantRose = true;
            }

        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.JungleRose, 1)
                .AddIngredient(ItemID.HealingPotion, 10)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddIngredient(ItemID.FallenStar, 6)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.JungleRose, 1)
                .AddIngredient(ItemID.HealingPotion, 10)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddIngredient(ItemID.FallenStar, 6)
                .Register();
        }
	}
}