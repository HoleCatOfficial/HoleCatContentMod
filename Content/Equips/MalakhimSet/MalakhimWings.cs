
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources;

using DestroyerTest.Rarity;

namespace DestroyerTest.Content.Equips.MalakhimSet
{
	[AutoloadEquip(EquipType.Wings)]
	public class MalakhimWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			// These wings use the same values as the solar wings
			// Fly time: 180 ticks = 3 seconds
			// Fly speed: 9
			// Acceleration multiplier: 2.5
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(80, 5f, 1.4f);
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 34;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<VesperRarity>(); // The rarity of the item
			Item.accessory = true;
		}

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 3.0f; // Falling glide speed
			ascentWhenRising = 1.8f; // Rising speed
			maxCanAscendMultiplier = 1.45f;
			maxAscentMultiplier = 1.45f;
			constantAscend = 0.135f;
		}

		public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<Vesper>(25)
				.AddTile(TileID.Anvils)
				.Register();
        }

	}
}