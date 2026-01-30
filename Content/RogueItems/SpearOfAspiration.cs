using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using Terraria.Audio;

namespace DestroyerTest.Content.RogueItems
{
	public class SpearOfAspiration : ModItem
	{
		public override void SetStaticDefaults() {
		}

		public override void SetDefaults() 
        {
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shootSpeed = 30f;
			Item.shoot = ModContent.ProjectileType<SpearOfAspirationThrown>();
			Item.width = 56;
			Item.height = 56;
			Item.maxStack = 1;
			Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/MCTrident", 2) with { MaxInstances = 0, PitchVariance = 0.2f };
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.value = Item.buyPrice(0, 0, 20, 0);
			Item.rare = ModContent.RarityType<VesperRarity>();
			Item.damage = 10;
			Item.autoReuse = true;
		}

		public override void AddRecipes() {
			CreateRecipe()
            .AddIngredient<Vesper>(6)
            .AddIngredient(ItemID.FallenStar, 2)
            .AddTile(TileID.Anvils)
            .Register();
		}
	}
}