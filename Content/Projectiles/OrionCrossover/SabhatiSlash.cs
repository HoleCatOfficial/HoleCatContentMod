using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.OrionCrossover
{
    public class SabhatiSlash : SpinningSlash
    {
        public int UpDown => (int)Projectile.ai[0];
        public override string Texture => "DestroyerTest/Content/Extras/144Slash";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Blending = true;
            themeColor = ColorLib.StellarFireGradient(stellarFireTime);
            DustType = DustID.FireworksRGB;
            DustUsesColor = true;
            DustScale = 0.5f;
        }

        public float stellarFireTime;
        public override bool PreAI()
        {
            stellarFireTime += 0.025f;
            themeColor = ColorLib.StellarFireGradient(stellarFireTime);

            StellarPointGlow Glow = new StellarPointGlow();
            Glow.Initialize(Main.rand.NextVector2FromRectangle(Utils.CenteredRectangle(Projectile.Center, new Vector2(Projectile.width * Projectile.scale, Projectile.height * Projectile.scale))), Projectile.velocity * 0.2f, default, 1f);
            ParticleEngine.BehindProjectiles.Add(Glow);
            return true;
        }

        public override void Rotation()
        {
            if (UpDown == 0)
            {
                Projectile.rotation += 0.8f * Projectile.direction;
            }
            else
            {
                Projectile.rotation -= 0.8f * Projectile.direction;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<DescendantInferno>(), 600);
        }
    }
}