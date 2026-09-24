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
using OpusLib;
using DestroyerTest.Content.BossSummons;

namespace DestroyerTest.Content.Scepter
{
    public class CosmicPing : ScepterItem
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
            ShootDMG = 21;
            ShootCrit = 6;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 6);
            Rarity = ModContent.RarityType<PearlRarity>();


            // Assign projectile types
            ShootID = ModContent.ProjectileType<Ping>();
            ThrowID = ModContent.ProjectileType<CosmicPingThrown>();

            // Optional: change sounds
            ShootSound = SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 0f };
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);

            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                velocity = Vector2.Zero;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BladeChunk>(2)
                .AddIngredient(ItemID.FallenStar)
                .AddIngredient(ItemID.IronBar, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}