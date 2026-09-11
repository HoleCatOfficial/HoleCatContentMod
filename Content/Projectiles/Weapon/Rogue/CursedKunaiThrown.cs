using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class CursedKunaiThrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f);
            dust.noGravity = true;
            dust.scale = 2f;

            Fire fire = new();
            fire.PrepareFire(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), -Projectile.velocity * 0.2f, DTUtils.RandomDirection(2), 0.01f, ColorLib.CursedFlames * 0.5f, 0.3f, 30, FireDrawMode.Additive, PixelLayer.AboveTiles);
            ParticleEngine.BehindProjectiles.Add(fire);

            if (LifeTime < 30)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            }
            else
            {
                if (Main.GameUpdateCount % 7 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 0 }, Projectile.Center);
                }

                Projectile.velocity.Y += 1.6f;
                Projectile.rotation += 0.5f * Projectile.direction;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 300);

            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { Pitch = 0.7f, PitchVariance = 0.7f, MaxInstances = 0 }, Projectile.Center);

            float X = -Projectile.oldVelocity.X * 0.5f;
            X = MathHelper.Clamp(X, -80f, 80f);
            float Y = -Projectile.oldVelocity.Y * 0.8f;
            Y = MathHelper.Clamp(Y, -40f, 40f);

            Projectile.velocity = new Vector2(X, Y);
            Projectile.ai[0] = 30;
        }

        public int tileHit = 0;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(DTAssetLib.Charge.MetalTinkLight with { Pitch = -0.7f, PitchVariance = 0.7f, MaxInstances = 0, Volume = 0.4f }, Projectile.Center);

            float X = -oldVelocity.X * 0.5f;
            X = MathHelper.Clamp(X, -80f, 80f);
            float Y = -oldVelocity.Y * 0.8f;
            Y = MathHelper.Clamp(Y, -40f, 40f);

            Projectile.velocity = new Vector2(X, Y);
            tileHit++;
            return tileHit > 4;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            
        }
    }
}
