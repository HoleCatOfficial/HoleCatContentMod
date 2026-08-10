using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class UnionFireball : ModProjectile, IHomingProjectile, IDrawPixelated
    {
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => TSpeed;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.1f;

        float IHomingProjectile.HomingMaxAccel => 30f;

        float IHomingProjectile.DetectRadius => 1200f;

        bool IHomingProjectile.CanHome => Timer >= 20;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        float RAMT = 0f;
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            RAMT = Main.rand.NextFloat(-0.005f, 0.005f);
        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveNPCs;
        bool IDrawPixelated.ShouldDrawPixelated => true;

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);
            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.Additive, DTAssetLib.Streak(9).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15f, ColorLib.Ichor, 0f, Projectile.OldCenter().Length);

            spriteBatch.ResetToDefault();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 }));
            return false;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }


        int Timer = 0;
        float TSpeed = 1f;
        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            AnimateProjectile();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;

            TSpeed = MathHelper.Lerp(1f, 9f, (float)Timer / 300f);

            if (Timer < 20)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(RAMT);
                Projectile.velocity *= 0.96f;
            }



            Dust D = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch);
            D.noGravity = true;

            LerpingFire Fire = new();
            Fire.PrepareFire(Projectile.Center, Projectile.velocity * 0.1f, Math.Sign(Projectile.velocity.X), 0.1f, ColorLib.Wretched3 * 0.5f, ColorLib.Ichor * 0.5f, 0.5f, 60, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(Fire);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Timer >= 20 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(ModContent.BuffType<Defilement>(), 600);


        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/FleshBombExplode") with { PitchVariance = 1.0f, MaxInstances = 0 }, Projectile.Center);
            
            for (int i = 0; i < 5; i++)
            {
                LerpingFire Fire = new();
                Fire.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), DTUtils.RandomDirection(2), 0.1f, ColorLib.Wretched3 * 0.5f, ColorLib.Ichor * 0.5f, 0.5f, 60, FireDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Fire);
            }

            BloomRingSharp Ring = new();
            Ring.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Ichor, 0.05f, 0.01f, 0.5f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring);
        }
    }
}
