using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{
	public class TrueRiftMaker : ModItem
	{
		public override void SetStaticDefaults() {
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shootSpeed = 30f;
			Item.shoot = ModContent.ProjectileType<TrueRiftMakerThrown>();
			Item.width = 128;
			Item.height = 128;
			Item.maxStack = 1;
			Item.UseSound = SoundID.Item71;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 0, 20, 0);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.damage = 200;
			Item.autoReuse = true;
			Item.DamageType = ModContent.GetInstance<DTRogueClass>();
        }

		public override void AddRecipes() {
			CreateRecipe()
            .AddIngredient<RiftMaker>()
            .AddIngredient(ItemID.DayBreak, 1)
            .AddIngredient(ItemID.FragmentSolar, 16)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
		}
	}
}