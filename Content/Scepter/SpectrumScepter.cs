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
    public class SpectrumScepter : ScepterItem
    {
        public override int Width => 46;
        public override int Height => 46;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            ShootDMG = 130;
            ShootCrit = 2;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();

            ShootID = ModContent.ProjectileType<SpectrumBolt>();
            ThrowID = ModContent.ProjectileType<SpectrumScepterThrown>();

            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();


        }
    }
}