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
using System.IO.Pipelines; // Add this line if CT3_Swing is in the Projectiles namespace

namespace DestroyerTest.Content.Magic.ScepterSubclass
{
    public class FungalScepter : ScepterItem
    {
        public override int Width => 44;
        public override int Height => 44;

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
            ShootCrit = 5;
            ThrowCrit = 18;
            KB = 2;
            AdditiveValue = Item.sellPrice(gold: 4, silver: 80);
            Rarity = ItemRarityID.LightRed;

            // Assign projectile types
            ShootID = ModContent.ProjectileType<FungalScepterMushroom>();
            ThrowID = ModContent.ProjectileType<FungalScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
        
        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 56)
                .AddIngredient(ItemID.IronBar, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
} 