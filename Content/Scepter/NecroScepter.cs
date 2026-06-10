using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
 
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;  

namespace DestroyerTest.Content.Scepter
{
	public class NecroScepter : ScepterItem
	{
		public override int Width => 56;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 41;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PaleFuchsiaRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<HomingBone>();
            ThrowID = ModContent.ProjectileType<NecroScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.2f), ModContent.ProjectileType<HomingBone>(), damage, knockback, player.whoAmI);

                Projectile.NewProjectile(source, position, velocity.RotatedBy(0.2f), ModContent.ProjectileType<HomingBone>(), damage, knockback, player.whoAmI);
            }

			return true;
		}


		public override void AddRecipes() 
        {
			CreateRecipe()
				.AddIngredient(ItemID.Bone, 18)
				.AddIngredient<LifeEcho>(12)
                .AddTile(TileID.Anvils)
				.AddCondition(Condition.DownedSkeletron)
				.Register();
		}

    }
} 