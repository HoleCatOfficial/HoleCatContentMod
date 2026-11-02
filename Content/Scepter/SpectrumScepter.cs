using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Scepter
{
    public class SpectrumScepter : ScepterItem
    {
        public override int Width => 46;
        public override int Height => 46;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            if (ModLoader.HasMod("QoLCompendium"))
            {
                ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Purity>();
            }
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