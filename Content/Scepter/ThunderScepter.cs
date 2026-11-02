using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Rarity.Scepter;
using OpusLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Scepter
{
    public class ThunderScepter : ScepterItem
    {
        public override int Width => 36;
        public override int Height => 34;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            Opus.ItemChannel_RightChannel_LeftNot.Add(Type);
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 10;
            ShootCrit = 2;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PearlRarity>();

            // Assign projectile types
            ShootID = ProjectileID.ThunderStaffShot;
            ThrowID = ModContent.ProjectileType<ThunderScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item25;
            ThrowSound = SoundID.Item169;

            ChannelingDuringShoot = false;
            ChannelingDuringThrow = true;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
    }
}