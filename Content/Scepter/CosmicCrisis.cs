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
    public class CosmicCrisis : ScepterItem
    {
        public override int Width => 40;
        public override int Height => 40;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 28;
            ShootCrit = 2;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PaleFuchsiaRarity>();
            

            // Assign projectile types
            ShootID = ModContent.ProjectileType<MoltenStar>();
            ThrowID = ModContent.ProjectileType<CosmicCrisisThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item156;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MeteoriteBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}