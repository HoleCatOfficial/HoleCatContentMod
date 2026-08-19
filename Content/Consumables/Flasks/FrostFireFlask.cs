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
	public class FrostFireFlask : BaseFlask
    {
        public override Color[] DrinkColors => new Color[2] { Color.SkyBlue, Color.DeepSkyBlue };

        public override int BuffType => ModContent.BuffType<WeaponImbueFrostburn>();

        public override Vector2 Dimensions => new Vector2(22, 22);

        public override int Rarity => ItemRarityID.Blue;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.Shiverthorn, 2)
                .AddTile(TileID.ImbuingStation)
                .Register();
        }
    }
}