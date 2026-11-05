using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class StockyPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;

            ItemID.Sets.DrinkParticleColors[Type] = [
                Color.DodgerBlue,
                Color.SeaGreen,
                Color.DarkBlue
            ];
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item163;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
            Item.buffType = ModContent.BuffType<Stocky>(); // Specify an existing buff to be applied when used.
            Item.buffTime = 18000; // The amount of time the buff declared in Item.buffType will last in ticks. 5400 / 60 is 90, so this buff will last 90 seconds.
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.LifeforcePotion)
                .AddIngredient(ItemID.RegenerationPotion)
                .AddIngredient<Dyrn>(5)
                .AddTile(TileID.Bottles)
                .Register();

        
        }
    }
}