using DestroyerTest.Common;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public class HeliouricFlask : BaseFlask
    {
        public override Color[] DrinkColors => [ColorLib.Rift, ColorLib.DarkRift3, ColorLib.LightRift2];

        public override int BuffType => ModContent.BuffType<WeaponImbueHeliouricShock>();

        public override Vector2 Dimensions => new Vector2(22, 32);

        public override int Rarity => ModContent.RarityType<RiftRarity1>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Living_Shadow>(10)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }
}