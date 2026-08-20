using DestroyerTest.Common;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Buffs.Imbues;

namespace DestroyerTest.Content.Consumables.Flasks
{
	public class ComaceraticFlask : BaseFlask
	{
        public override Color[] DrinkColors => new Color[2] { ColorLib.LightRift2, ColorLib.Rift };

        public override int BuffType => ModContent.BuffType<WeaponImbueComaceraticBurn>();

        public override Vector2 Dimensions => new Vector2(22, 32);

        public override int Rarity => ModContent.RarityType<RiftRarity2>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Item_HeliciteCrystal>(8)
                .AddIngredient(ItemID.FragmentSolar, 3)
                .AddTile(TileID.ImbuingStation)
                .Register();
		}

	}
}