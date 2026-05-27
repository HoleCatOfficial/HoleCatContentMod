using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class SoulOfNight_Projectile : ModProjectile, IHomingProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 600;
            Projectile.frame = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, frame, ColorLib.SoulOfNightColor, Projectile.rotation, origin, Projectile.scale * 1.4f, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }

        public bool ExplodesWithPattern = false;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 8f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.08f;

        float IHomingProjectile.HomingMaxAccel => 30f;

        float IHomingProjectile.DetectRadius => 2400;

        bool IHomingProjectile.CanHome => Projectile.ai[0] >= 40;

        public void DeathPrep(float Threshold = 600)
        {
            if (Projectile.timeLeft > Threshold)
            {
                return;
            }

            if (Projectile.timeLeft <= Threshold)
            {
                if (Projectile.velocity.Length() > 0.01f)
                {
                    Projectile.velocity *= 0.999f;
                }

                if (Projectile.timeLeft < 10)
                {
                    ExplodesWithPattern = true;
                }
            }
        }

        public override void AI()
        {
            AnimateProjectile();
            DeathPrep();

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 20)
            {
                Projectile.velocity *= 0.92f;
            }

            if (Main.rand.NextBool(3))
            {
                Dust Trail = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SoulOfNightDust>(), Projectile.velocity * 0.2f, 0, default, 2f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() * 0.05f;

        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<NightInferno>(), 300);
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarBurst2") with { MaxInstances = 0 });

            var P = Polar.GenerateCurvedStar(6, 5, 200f, Projectile.Center, 6, 0.6f, 0f);

            foreach (Vector2 p in P)
            {
                Vector2 D = p - Projectile.Center;
                //D.Normalize();
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SoulOfNightDust>(), D * 0.01f, 0, default, 3f);
            }
            
        }
    }
}