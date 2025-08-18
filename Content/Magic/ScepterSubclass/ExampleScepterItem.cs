using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Content.Magic.ScepterSubclass;
using DestroyerTest.Content.Projectiles;

namespace DestroyerTest.Content.Magic.ScepterSubclass
{
    /// <summary>
    /// Created simply as a template on overriding the ScepterItem class.
    /// </summary>
    public class ExampleScepterItem : ScepterItem
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
            ShootDMG = 18;
            ShootCrit = 6;
            ThrowCrit = 14;
            KB = 4;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ItemRarityID.LightRed;

            // Assign projectile types
            ShootID = ProjectileID.RubyBolt;
            ThrowID = ModContent.ProjectileType<EnchantedScepterThrown>();

            // Optional: change sounds
            ShootSound = SoundID.Item20;
            ThrowSound = SoundID.Item71;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
    }
}
