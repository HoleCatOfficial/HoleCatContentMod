using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class NecroScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.White;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.Bone;
            base.SetDefaults();
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.position);

            base.OnHitNPC(target, hit, damageDone);
        }

    }

}

