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
using System.Collections.Generic;
using DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots;
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Scepter
{
	public class ElementalScepter : ScepterItem
	{
		public override int Width => 48;
        public override int Height => 48;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 145;
            ShootCrit = 30;
            ThrowCrit = 14;
            KB = 4;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<ShadowShot>();
            ThrowID = ModContent.ProjectileType<ShadowScepterThrown>();

            // Optional: change sounds
            ShootSound = new SoundStyle("DestroyerTest/Assets/Audio/HopeScabbardTele") { PitchVariance = 0.2f, MaxInstances = 0 };
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.shootSpeed = 1.5f;
            Item.useTime = 60;
            Item.useAnimation = 180;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            List<int> Options = new List<int>
            {
                ModContent.ProjectileType<CursedShot>(),
                ModContent.ProjectileType<FireShot>(),
                ModContent.ProjectileType<GalantineShot>(),
                ModContent.ProjectileType<IceShot>(),
                ModContent.ProjectileType<LightShot>(),
                ModContent.ProjectileType<RiftShot>(),
                ModContent.ProjectileType<ShadowFireShot>(),
                ModContent.ProjectileType<VenomShot>()
            };

            type = Options[Main.rand.Next(Options.Count)];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrostScepter>()
                .AddIngredient<ShadowScepter>()
                .AddIngredient<EmberCane>()
                .AddIngredient<StellarFoxScepter>()
                .AddIngredient<InfectedScepter>()
                .AddIngredient<PrismaticScepter>()
                .AddIngredient<HeliciteScepter>()
                .AddIngredient<Vesper>(16)
                .AddIngredient(ItemID.GoldBar, 18)
                .AddIngredient<LifeEcho>(100)
            .Register();
        }
    }
} 