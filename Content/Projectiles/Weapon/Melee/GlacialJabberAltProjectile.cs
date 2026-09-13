using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Terraria.Audio;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class GlacialJabberAltProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Melee;
        }

 

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.MountedCenter;
            Projectile.rotation += 0.2f * player.direction;
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 15 == 0)
            {
                SoundEngine.PlaySound(DTAssetLib.SwordSounds.Woosh with { PitchVariance = 0.6f });
            }

            if (player.HeldItem.type == ModContent.ItemType<GlacialJabber>() && player.controlUseTile)
            {
                
                

                Projectile.timeLeft = 20;
                player.SetDummyItemTime(20);
            }
            else
            {

            }
        }

    }
}