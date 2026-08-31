using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.Audio;
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
			Item.shootSpeed = 20f;
			Item.shoot = ModContent.ProjectileType<TrueRiftMakerThrown>();
			Item.width = 200;
			Item.height = 200;
			Item.maxStack = 1;
			Item.UseSound = new SoundStyle(DTAssetLib.AudioPath + "/Riftmaker/TrueRiftmakerThrow");
			Item.useAnimation = 120;
			Item.useTime = 120;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 0, 20, 0);
			Item.rare = ModContent.RarityType<RiftRarity2>();
			Item.damage = 200;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Throwing;
        }

		public override void AddRecipes() {
			CreateRecipe()
            .AddIngredient<RiftMaker>()
            .AddIngredient(ItemID.FragmentSolar, 16)
            .AddIngredient<PhantasmalRemnant>(2)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
		}
	}
}