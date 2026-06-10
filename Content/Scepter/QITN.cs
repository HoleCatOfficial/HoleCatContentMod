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
using DestroyerTest.Content.Resources;

namespace DestroyerTest.Content.Scepter
{
    public class QITN : ScepterItem
    {
        public override int Width => 56;
        public override int Height => 56;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            ShootDMG = 4;
            ShootCrit = 2;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PearlRarity>();

            ShootID = ModContent.ProjectileType<FrigidEcho>();
            ThrowID = ModContent.ProjectileType<QITNThrown>();

            ShootSound = new SoundStyle(DTAssetLib.AudioPath + "/MiniRoseSummon") { Pitch = -0.6f, PitchVariance = 0.4f, MaxInstances = 0 };
            ThrowSound = SoundID.Item169;

            base.SetDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LifeEcho>(16)
                .AddIngredient(ItemID.Wood, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}