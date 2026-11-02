using DestroyerTest.Content.Buffs.Imbues;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables.Flasks
{
    public class MudFlask : BaseFlask
    {
        public override Color[] DrinkColors => [Color.Brown];

        public override int BuffType => ModContent.BuffType<WeaponImbueMud>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ItemRarityID.White;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.MudBlock, 4)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }
}