using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class SpectrumScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Main.DiscoColor;
            WidthDim = 46;
            HeightDim = 46;
            DustType = DustID.SpectreStaff;
            DustColor = Main.DiscoColor;


            base.SetDefaults();
        }

        public override void PostAI()
        {
            base.PostAI();


        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);


            if (hit.Crit)
            {

                SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/HopeScabbardTele") { PitchVariance = 0.5f, MaxInstances = 0 }, target.Center);

                Opus.RingSpreadProjectile(ModContent.ProjectileType<BlessedNodeCrystalFriendly>(), 4, target.Center, 200, Projectile.damage, 4, -18, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }

        }
    }
}

