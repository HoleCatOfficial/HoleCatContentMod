using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Scepter
{
	public class WondersWill : ScepterItem
	{
		public override int Width => 58;
        public override int Height => 60;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 34;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<WineRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<WondersWillPinkShot>();
            ThrowID = ModContent.ProjectileType<WondersWillThrownPink>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }




		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
            {
                // Check if another projectile of the same type is active
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.owner == player.whoAmI && (proj.type == ModContent.ProjectileType<WondersWillThrownPink>() || proj.type == ModContent.ProjectileType<WondersWillThrownTeal>()))
                    {
                        return false; // Prevent new projectile from being fired
                    }
                }

                if (player.altFunctionUse == 2)
                {
                    // Calculate the speed of the projectile
                    float speed = velocity.Length();

                    // Define the angles for the thrown projectiles (45 degrees up and down from 270)
                    float angle1, angle2;
                    if (player.direction == -1) { // Facing left
                        angle1 = 180f;  // 90 degrees to the left (horizontal spread)
                        angle2 = 0f;    // 90 degrees to the right (horizontal spread)
                    } else { // Facing right
                        angle1 = 180f;  // 90 degrees to the left (horizontal spread)
                        angle2 = 0f;    // 90 degrees to the right (horizontal spread)
                    }

                    // Calculate the velocity for the first thrown projectile
                    Vector2 velocity1 = new Vector2(speed * (float)Math.Cos(MathHelper.ToRadians(angle1)), speed * (float)Math.Sin(MathHelper.ToRadians(angle1)));

                    // Calculate the velocity for the second thrown projectile
                    Vector2 velocity2 = new Vector2(speed * (float)Math.Cos(MathHelper.ToRadians(angle2)), speed * (float)Math.Sin(MathHelper.ToRadians(angle2)));

                    // Fire the first thrown projectile (WondersWillThrownPink)
                    Projectile.NewProjectile(source, position, velocity1, ModContent.ProjectileType<WondersWillThrownPink>(), damage, knockback, player.whoAmI);

                    // Fire the second thrown projectile (WondersWillThrownTeal)
                    Projectile.NewProjectile(source, position, velocity2, ModContent.ProjectileType<WondersWillThrownTeal>(), damage, knockback, player.whoAmI);
                }

                if (player.altFunctionUse != 2)
                {
                    // Calculate the speed of the projectile
                    float speed = velocity.Length();

                    // Define the angles for the shot projectiles (15 degrees up and down from 270)
                    float angle1, angle2;
                    if (player.direction == -1) { // Facing left
                        angle1 = 180f;  // 90 degrees to the left (horizontal spread)
                        angle2 = 0f;    // 90 degrees to the right (horizontal spread)
                    } else { // Facing right
                        angle1 = 180f;  // 90 degrees to the left (horizontal spread)
                        angle2 = 0f;    // 90 degrees to the right (horizontal spread)
                    }

                    // Calculate the velocity for the first shot projectile
                    Vector2 velocity1 = new Vector2(speed * (float)Math.Cos(MathHelper.ToRadians(angle1)), speed * (float)Math.Sin(MathHelper.ToRadians(angle1)));

                    // Calculate the velocity for the second shot projectile
                    Vector2 velocity2 = new Vector2(speed * (float)Math.Cos(MathHelper.ToRadians(angle2)), speed * (float)Math.Sin(MathHelper.ToRadians(angle2)));

                    // Fire the first shot projectile (WondersWillPinkShot)
                    Projectile.NewProjectile(source, position, velocity1, ModContent.ProjectileType<WondersWillPinkShot>(), damage, knockback, player.whoAmI);

                    // Fire the second shot projectile (WondersWillTealShot)
                    Projectile.NewProjectile(source, position, velocity2, ModContent.ProjectileType<WondersWillTealShot>(), damage, knockback, player.whoAmI);
                }

                return true; // Allow firing if no other projectiles exist
            }


			public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<GoldPearl>(2)
				.AddIngredient(ItemID.MythrilBar, 10)
				.AddIngredient(ItemID.OrichalcumBar, 10)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}



    }
} 