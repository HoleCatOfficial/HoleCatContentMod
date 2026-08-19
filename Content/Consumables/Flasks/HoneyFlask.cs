using DestroyerTest.Common;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public class HoneyFlask : BaseFlask
    {
        public override Color[] DrinkColors => [Color.Gold, Color.Goldenrod, Color.DarkGoldenrod];

        public override int BuffType => ModContent.BuffType<WeaponImbueHoney>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ItemRarityID.Yellow;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.BottledHoney)
                .AddIngredient(ItemID.Stinger)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }
}