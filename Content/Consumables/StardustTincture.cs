using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class StardustTincture : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;

            ItemID.Sets.DrinkParticleColors[Type] = [
                new Color(196, 247, 255),
                new Color(0, 174, 238),
                new Color(0, 106, 185)
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
            Item.buffType = ModContent.BuffType<StardustSummonBoost>();
            Item.buffTime = (60 * 60) * 8;
            Item.value = Item.sellPrice(0, 2, 5);
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SummoningPotion)
                .AddIngredient(ItemID.Ale)
                .AddIngredient(ItemID.FragmentStardust, 8)
                .AddIngredient<Dyrn>(10)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}