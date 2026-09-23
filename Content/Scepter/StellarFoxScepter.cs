using System;
using System.IO.Pipelines;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Remnants;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Scepter
{
	public class StellarFoxScepter : ScepterItem
	{

		public override int Width => 54;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Constitution>();
            }
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 56;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PaleFuchsiaRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<StellarFox>();
            ThrowID = ModContent.ProjectileType<StellarFoxScepterThrown>();

            // Optional: change sounds
            ShootSound = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/SwiftSwing1") { MaxInstances = 0, PitchVariance = 0.4f };
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.shootSpeed = 2f;
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.RemnantsIsLoaded)
            {
                CreateRecipe()
                    .AddIngredient<ConstitutionArtifact>(3)
                    .Register();
            }
        }

    }
} 