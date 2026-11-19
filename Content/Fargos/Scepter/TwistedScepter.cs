using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Fargos;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Rarity.Scepter;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Fargos.Scepter
{
    [ExtendsFromMod(DTCrossMod.FargosSoulsName)]
    [JITWhenModsEnabled(DTCrossMod.FargosSoulsName)]
    public class TwistedScepter : ScepterItem
    {
        public override int Width => 28;
        public override int Height => 28;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GildedSceptre>();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 29;
            ShootCrit = 2;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PearlRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<TwistedSpirit>();
            ThrowID = ModContent.ProjectileType<TwistedScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
    }
}