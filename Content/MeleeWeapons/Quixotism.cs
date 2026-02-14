using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Projectiles;
  
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Projectiles.Weapon.Melee.Quixotism;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class Quixotism : ModItem
	{
		public int attackType = 0;
		public int comboExpireTimer = 0;
        public int[] hitCount = new int[2];
        public bool Powered = false;
        public float PowerOpacity = 0f;

		public override void SetDefaults()
		{
			Item.width = 72;
			Item.height = 72;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 50;
			Item.knockBack = 8f;
			Item.crit = 26;

			Item.value = Item.buyPrice(gold: 16);
			Item.rare = ModContent.RarityType<VesperRarity>();
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<QuixotismSwing>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
			attackType = (attackType + 1) % 2;
			comboExpireTimer = 0;
			return false;
		}

		public override void UpdateInventory(Player player)
		{
			if (comboExpireTimer++ >= 120)
            {
				attackType = 0;
                hitCount[0] = 0;
                hitCount[1] = 0;
                PowerOpacity = 0f;
            }
		}

		public override bool MeleePrefix() 
        {
			return true;
		}		
        
        public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient<Vesper>(16)
            .AddTile(TileID.Anvils)
			.Register();
		}	
	}
}
