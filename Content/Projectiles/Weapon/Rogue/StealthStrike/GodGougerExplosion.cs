using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using OpusLib;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue.StealthStrike
{
    public class GodGougerExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 12;
        }
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        private void AnimateProjectile()
        {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 60 / 13)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.Kill();
                }
            }
        }

        public bool DidTheThing = false;
        public override void AI()
        {
            AnimateProjectile();
            if (DidTheThing == false)
            {
                

                SoundEngine.PlaySound(DTAssetLib.Impacts.ExplosiveImpactBig, Projectile.Center);
                Opus.RadialSpreadDustRandom(DustID.TintableDustLighted, 8, Projectile.Center, 0, Color.Pink, 1f, 2.4f);
                Opus.RadialSpreadDustRandom(DustID.TintableDustLighted, 8, Projectile.Center, 0, Color.PaleTurquoise, 1f, 2.4f);
                DidTheThing = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Chilled, 600);
        }
    }
}
