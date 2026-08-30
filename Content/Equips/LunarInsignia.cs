
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
using DestroyerTest.Common;

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
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(600, 9.2f, 2.75f, true);
            DTUtils.NoUpgradeStack[Type] = true;
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

			if (DTCrossMod.CalamityIsLoaded)
			{
				DTCrossMod.CalamityMod.Call("ToggleInfiniteFlight", true);
			}
        }

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
			ascentWhenFalling = 1f;
			ascentWhenRising = 1f;
			maxCanAscendMultiplier = 1.3f;
			maxAscentMultiplier = 1.3f;
			constantAscend = 0.09f;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.EmpressFlightBooster)
				.AddIngredient(ItemID.LunarBar, 6)
				.AddIngredient<Tenebris>(8)
				.AddTile(TileID.LunarCraftingStation)
				.SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
				.Register();
		}
	}
}