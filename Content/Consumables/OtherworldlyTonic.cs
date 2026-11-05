using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class OtherworldlyTonic : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;

            ItemID.Sets.DrinkParticleColors[Type] = [
                Color.DarkGoldenrod,
                Color.Maroon,
                Color.DarkBlue
            ];
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 20;
            Item.height = 30;
            Item.buffType = ModContent.BuffType<Warped>();
            Item.buffTime = (60 * 60) * 8;
            Item.value = Item.sellPrice(0, 2, 5);
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.CrystalShard)
                .AddIngredient(ItemID.Ale)
                .AddIngredient<Dyrn>(10)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}