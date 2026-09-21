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
	public class EnchantedScepter : ScepterItem
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
            ShootDMG = 25;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 6;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PaleFuchsiaRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<EnchantedShot>();
            ThrowID = ModContent.ProjectileType<EnchantedScepterThrown>();

            // Optional: change sounds
            ShootSound = new SoundStyle(DTAssetLib.AudioPath + "/HopeScabbardTele") { PitchVariance = 0.5f, MaxInstances = 0 };
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.useTime = 10;
            Item.useAnimation = 10;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                velocity = velocity.RotatedByRandom(0.8f);
            }
        }
    }
} 