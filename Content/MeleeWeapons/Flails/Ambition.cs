using DestroyerTest.Content.Projectiles.Weapon.Melee.Flail;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons.Flails
{
	public class Ambition : ModItem
	{
		public override void SetStaticDefaults() 
        {
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.knockBack = 5.5f;
			Item.width = 66;
			Item.height = 46;
			Item.damage = 13;
			Item.noUseGraphic = true; 
			Item.shoot = ModContent.ProjectileType<AmbitionProjectile>();
			Item.shootSpeed = 12f;
			Item.UseSound = SoundID.Item1;
			Item.rare = ModContent.RarityType<VesperRarity>();
			Item.value = Item.sellPrice(gold: 1, silver: 50);
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.channel = true;
			Item.noMelee = true;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Vesper>(14)
                .AddIngredient(ItemID.Chain, 6)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}