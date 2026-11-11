using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class StellarFireSlash : SpinningSlash
    {
        public int UpDown => (int)Projectile.ai[0];
        public override string Texture => "DestroyerTest/Content/Extras/144Slash";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = initTime = 60;
            Blending = true;
            themeColor = ColorLib.StellarFireGradient(stellarFireTime);
            DustType = DustID.FireworksRGB;
            DustUsesColor = true;
            DustScale = 0.5f;
        }

        public float stellarFireTime;
        public override bool PreAI()
        {
            stellarFireTime = (Projectile.timeLeft / (float)initTime).Inverse();
            themeColor = ColorLib.StellarFireGradient(stellarFireTime);
            Projectile.velocity *= 0.93f;


            ConstitutionParticle FX1 = new();
            FX1.Initialize(Projectile.Center + new Vector2(Main.rand.Next(15, 100), 0).RotatedBy(Projectile.rotation), (Projectile.rotation + (MathHelper.PiOver2 * UpDown * Projectile.direction)).ToRotationVector2(), Main.rand.NextFloat(0.7f, 1f), 30);
            ParticleEngine.BehindProjectiles.Add(FX1);


            return true;
        }

        public override void Rotation()
        {
            if (UpDown == 1)
            {
                Projectile.rotation += 0.8f * Projectile.direction;
            }
            if (UpDown == -1)
            {
                Projectile.rotation -= 0.8f * Projectile.direction;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GalantineBurn>(), 600);
        }
    }
}