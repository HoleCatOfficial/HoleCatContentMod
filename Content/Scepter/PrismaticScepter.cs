using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace DestroyerTest.Content.Scepter
{
    public class PrismaticScepter : ScepterItem
    {
        public override int Width => 62;
        public override int Height => 62;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 46;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 2;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<CerisePinkRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<LightShot>();
            ThrowID = ModContent.ProjectileType<PrismaticScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.DD2_EtherianPortalSpawnEnemy;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.shootSpeed = 2;
        }
    }
}