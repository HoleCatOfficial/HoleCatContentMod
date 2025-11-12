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
    public class FrostScepter : ScepterItem
    {
        public override int Width => 58;
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
            ShootDMG = 8;
            ShootCrit = 14;
            ThrowCrit = 36;
            KB = 2;
            AdditiveValue = Item.sellPrice(gold: 4, silver: 80);
            Rarity = ModContent.RarityType<PearlRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<FrozenFireball>();
            ThrowID = ModContent.ProjectileType<FrostScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
        
        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.IceBlock, 15)
                .AddIngredient(ItemID.IronBar, 10)
                .AddIngredient(ItemID.BorealWood, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
} 