using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class HolyScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Red;
            WidthDim = 46;
            HeightDim = 46;
            DustType = DustID.RedTorch;
            base.SetDefaults();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Main.myPlayer];  // Accessing the current player
            SoundEngine.PlaySound(SoundID.Item113, Projectile.position);
            HitCount += 1;
            //PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), target.Center, Vector2.Zero, Color.Red, 0.025f);
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}

