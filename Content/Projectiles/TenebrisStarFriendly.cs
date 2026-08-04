using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class TenebrisStarFriendly : ModProjectile, IHomingProjectile
    {

        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 15;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 2800;

        bool IHomingProjectile.CanHome => DelayTimer >= 10;

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 160;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.TenebrisGradient;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(6).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, lightColor * 0.5f, trailOffset);

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(14).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, lightColor, trailOffset);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White with { A = 0 }, true, 0f, 0.9f, 0.9f);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 10 && Projectile.ManualCanHitFriendly(target);
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            DelayTimer++;

            Projectile.rotation += Projectile.direction * 0.07f;

            if (Main.rand.NextBool(10) && !DTOptimizationsConfig.instance.DisableExcessParticles)
            {
                TenebrousCloudParticle Cloud = new();
                Cloud.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.1f, ColorLib.TenebrisGradient * 0.6f, 0.8f, 0.2f, 120);
                ParticleEngine.BehindProjectiles.Add(Cloud);
            }

            Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

            if (DelayTimer < 20 || DelayTimer > 180)
            {
                return;
            }

            float maxDetectRadius = 2800f;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[0];
            if (Projectile.owner > -1)
            {
                player = Main.player[Projectile.owner];
            }

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
            ShimmeringFlames.ShimmerBurn(target);
            if (player.TryGetModPlayer<TenebrisMagicPlayer>(out var magicPlayer))
            {
                if (magicPlayer.Active)
                {
                    player.statMana += (int)(damageDone / 10);
                    player.ManaEffect((int)(damageDone / 10));
                    for (int u = 0; u < 16; u++)
                    {
                        Dust.NewDustPerfect(player.Center, DustID.FireworksRGB, Main.rand.NextVector2CircularEdge(6, 6), 0, ColorLib.TenebrisGradient);
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
            }
        }

    }

    public class TenebrisStarFriendly_NoHoming : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 160;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.TenebrisGradient;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(6).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, lightColor * 0.5f, trailOffset);

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(14).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, lightColor, trailOffset);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White with { A = 0 }, true, 0f, 0.9f, 0.9f);

            return false;
        }




        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            Projectile.rotation += Projectile.direction * 0.07f;

            if (Main.rand.NextBool(10) && !DTOptimizationsConfig.instance.DisableExcessParticles)
            {
                TenebrousCloudParticle Cloud = new();
                Cloud.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.06f, ColorLib.TenebrisGradient * 0.6f, 1f, 0.2f, 120);
                ParticleEngine.BehindProjectiles.Add(Cloud);
            }

            Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[0];
            if (Projectile.owner > -1)
            {
                player = Main.player[Projectile.owner];
            }

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
            ShimmeringFlames.ShimmerBurn(target);
            if (player.TryGetModPlayer<TenebrisMagicPlayer>(out var magicPlayer))
            {
                if (magicPlayer.Active)
                {
                    player.statMana += (int)(damageDone / 10);
                    player.ManaEffect((int)(damageDone / 10));
                    for (int u = 0; u < 16; u++)
                    {
                        Dust.NewDustPerfect(player.Center, DustID.FireworksRGB, Main.rand.NextVector2CircularEdge(6, 6), 0, ColorLib.TenebrisGradient);
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TintableDustLighted, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.TenebrisGradient, 2f);
            }
        }

    }
}