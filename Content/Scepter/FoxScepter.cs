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
using DestroyerTest.Rarity.Scepter; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Scepter
{
	public class FoxScepter : ScepterItem
	{
		public override int Width => 54;
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
            ShootDMG = 7;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(gold: 4, silver: 80);
            Rarity = ModContent.RarityType<PearlRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<PipeBomb>();
            ThrowID = ModContent.ProjectileType<FoxScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
    }
} 