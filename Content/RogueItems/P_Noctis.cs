using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{
	public class P_Noctis : ModItem
	{
		public override void SetDefaults() 
		{
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(silver: 5);
			Item.maxStack = 1;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 30;
			Item.useTime = 30;
            Item.UseSound = new SoundStyle($"DestroyerTest/Assets/Audio/P_Noctis_Throw") {Volume = 1.0f, Pitch = 0.0f, PitchVariance = 0.2f }; 
			Item.autoReuse = true;
			Item.consumable = false;
            Item.crit = 32;
		
			Item.damage = 210;
			Item.knockBack = 16f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.DamageType = ModContent.GetInstance<DTRogueClass>();

            Item.shootSpeed = 25f;
			Item.shoot = ModContent.ProjectileType<P_Noctis_Projectile>();
			
		}
		
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.Bone, 200)
				.AddIngredient(ItemID.BoneFeather, 5)
				.AddIngredient(ItemID.IceFeather, 5)
				.AddIngredient(ItemID.FireFeather, 5)
				.AddIngredient(ItemID.GiantHarpyFeather, 5)
				.AddIngredient(ItemID.Feather, 5)
                .AddIngredient(ItemID.DayBreak, 1)
                .AddIngredient(ItemID.SoulofFright, 30)
				.AddTile(TileID.BoneWelder)
				.Register();
		}
	}
}