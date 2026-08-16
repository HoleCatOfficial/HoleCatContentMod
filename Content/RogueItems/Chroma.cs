using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{
	public class Chroma : ModItem
	{
		public override void SetDefaults() 
		{

			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(silver: 5);
			Item.maxStack = 1;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 30;
			Item.useTime = 30;
            Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/Chroma_Throw") {
					Volume = 1.0f, 
					Pitch = 0.0f, 
					PitchVariance = 0.5f, 
				}; 
			Item.autoReuse = true;
			Item.consumable = false;	
			Item.damage = 180;
			Item.knockBack = 15f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Throwing;

            Item.shootSpeed = 25f;
			Item.shoot = ModContent.ProjectileType<Chroma_Projectile>();
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient(ItemID.Diamond, 8)
                .AddIngredient(ItemID.FragmentStardust, 5)
                .AddIngredient(ItemID.FragmentVortex, 5)
                .AddIngredient(ItemID.FragmentSolar, 5)
                .AddIngredient(ItemID.FragmentNebula, 5)
                .AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}