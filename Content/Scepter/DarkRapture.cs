using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Rarity.Scepter;

namespace DestroyerTest.Content.Scepter
{
    public class DarkRapture : ScepterItem
    {
        public override int Width => 56;
        public override int Height => 56;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 100;
            ShootCrit = 6;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();


            // Assign projectile types
            ShootID = ModContent.ProjectileType<ShimmeringSpark>();
            ThrowID = ModContent.ProjectileType<DarkRaptureThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item156;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tenebris>(12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}