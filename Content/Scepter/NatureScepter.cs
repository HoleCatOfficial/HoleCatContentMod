using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
 
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;  

namespace DestroyerTest.Content.Scepter
{
	public class NatureScepter : ScepterItem
	{
		public override int Width => 56;
        public override int Height => 52;

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
            Rarity = ModContent.RarityType<PearlRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<JungleSporeCloud>();
            ThrowID = ModContent.ProjectileType<NatureScepterThrown>();

            // Optional: change sounds
            ShootSound = new SoundStyle(DTAssetLib.AudioPath + "/NatureScepterSpray") with { PitchVariance = 0.4f, MaxInstances = 0 };
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.useTime = 80;
            Item.useAnimation = 80;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                for (int i = 0; i < Main.rand.Next(3, 6); i++)
                {
                    Projectile.NewProjectile(source, position, (velocity * Main.rand.NextFloat(0.8f, 1.2f)).RotatedByRandom(0.6f), Item.shoot, damage, knockback, player.whoAmI);
                }
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.RichMahogany, 16)
				.AddIngredient(ItemID.JungleSpores, 12)
				.AddTile(TileID.WorkBenches)
				.Register();
		}


    }
} 