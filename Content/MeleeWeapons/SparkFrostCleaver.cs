  
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class SparkFrostCleaver : ModItem
	{
        public int attackType = 0;
        public int comboExpireTimer = 0;

        public override void SetDefaults() {
			Item.width = 162;
			Item.height = 162;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Green;
			Item.useTime = 120;
			Item.useAnimation = 120;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 70;
			Item.autoReuse = false;
			Item.damage = 95;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true; 
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<SparkFrostCleaverSwing>();
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
            }
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

		public override void AddRecipes() 
		{
			CreateRecipe()
                .AddIngredient(ItemID.WandofSparking)
				.AddIngredient(ItemID.WandofFrosting)
				.AddIngredient(ItemID.SoulofMight, 12)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}