
using System.Linq;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Wings)]
	public class LunarInsignia : ModItem
	{

		public override void SetStaticDefaults() {
			// These wings use the same values as the solar wings
			// Fly time: 180 ticks = 3 seconds
			// Fly speed: 9
			// Acceleration multiplier: 2.5
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(600, 9.7f, 2.75f, true);
            
		}

		public override void SetDefaults() {
			Item.width = 80;
			Item.height = 56;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.empressBrooch = true;
            player.wingTime = player.wingTimeMax;
        }

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
			ascentWhenFalling = 1f;
			ascentWhenRising = 0.7f;
			maxCanAscendMultiplier = 5f;
			maxAscentMultiplier = 5f;
			constantAscend = 0.16f;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.EmpressFlightBooster)
				.AddIngredient<Tenebris>(8)
				.AddTile(TileID.LunarCraftingStation)
				.SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
				.Register();
		}
	}
}