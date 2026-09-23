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
using OpusLib.Content.Helpers;
using Terraria.GameContent.ItemDropRules;

namespace DestroyerTest.Content.Scepter
{
    public class BloodScepter : ScepterItem
    {
        public override int Width => 54;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            OpusNPCDropHelper.DropsFromNPC[Type] = new NPCDropData(NPCID.BloodNautilus, ItemDropRule.Common(Type, 5));
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 100;
            ShootCrit = 2;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<WineRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<BloodShot>();
            ThrowID = ModContent.ProjectileType<BloodScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();

            Item.shootSpeed = 2f;
        }
    }
}