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
using System.IO.Pipelines;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using OpusLib; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Scepter
{
	public class InfectedScepter : ScepterItem
	{
		public override int Width => 34;
        public override int Height => 34;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 40;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();

            // Assign projectile types
            ShootID = -1;
            ThrowID = ModContent.ProjectileType<InfectedScepterThrown>();

            // Optional: change sounds
            ShootSound = new SoundStyle("DestroyerTest/Assets/Audio/StellarBow/StellarBowArrowImpact", 4) { MaxInstances = 0, PitchVariance = 0.5f };
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.useTime = 180;
            Item.useAnimation = 180;
        }

        public override bool? UseItem(Player player)
        {
            if(player.altFunctionUse != 2)
            {
                Opus.RingProjectileOutward(ModContent.ProjectileType<InfectedCrystalCF>(), 6, Main.MouseWorld, 20, Item.damage / 2, 10, 4, RandomOffset: true);
                Opus.RingProjectileOutward(ModContent.ProjectileType<InfectedCrystalIchor>(), 6, Main.MouseWorld, 20, Item.damage / 2, 10, 4, RandomOffset: true);
            }
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.altFunctionUse != 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    
    }
} 