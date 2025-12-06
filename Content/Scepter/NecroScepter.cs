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
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter; // Add this line if CT3_Swing is in the Projectiles namespace

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
            ShootDMG = 22;
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
                // Calculate the speed of the projectile
                float speed = velocity.Length();

                // Define the angles based on the player's facing direction
                float angle1, angle2;
                if (player.direction == -1) { // Facing left
                    angle1 = 225f;
                    angle2 = 140f;
                } else { // Facing right
                    angle1 = -45f;
                    angle2 = -315f;
                }

                // Calculate the velocity for the first projectile
                Vector2 velocity1 = new Vector2(speed * (float)Math.Cos(MathHelper.ToRadians(angle1)), speed * (float)Math.Sin(MathHelper.ToRadians(angle1)));

                // Calculate the velocity for the second projectile
                Vector2 velocity2 = new Vector2(speed * (float)Math.Cos(MathHelper.ToRadians(angle2)), speed * (float)Math.Sin(MathHelper.ToRadians(angle2)));

                // Fire the first projectile (SoulOfLight_Projectile)
                Projectile.NewProjectile(source, position, velocity1, ModContent.ProjectileType<HomingBone>(), damage, knockback, player.whoAmI);

                // Fire the second projectile (SoulOfNight_Projectile)
                Projectile.NewProjectile(source, position, velocity2, ModContent.ProjectileType<HomingBone>(), damage, knockback, player.whoAmI);
                }

			return true; // Allow firing if no other projectiles exist
		}


		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.Bone, 18)
				.AddIngredient<LifeEcho>(12)
                .AddTile(TileID.Anvils)
				.AddCondition(Condition.DownedSkeletron)
				.Register();
		}

    }
} 