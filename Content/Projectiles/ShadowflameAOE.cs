using System;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class ShadowflameAOE : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.alpha = 255;
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawTextureOnProj(DTAssetLib.Swirl, Projectile,  new Color(179, 54, 201), true, Projectile.rotation, 1f, 1f);
            Opus.DrawTextureOnProj(DTAssetLib.FireRing, Projectile,  new Color(204, 121, 219) * 0.85f, false, -Projectile.rotation, 0.125f, 0.125f);
            Opus.DrawTextureOnProj(DTAssetLib.FireRing, Projectile,  new Color(179, 54, 201) * 0.85f, false, Projectile.rotation, 0.1f, 0.1f);
            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/ShadowflameAuraLoop") 
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
            
        public override void AI()
        {

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound)) {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.position, soundInstance => {
                    soundInstance.Position = Projectile.position;
                    return tracker.IsActiveAndInGame();
                });
            }

            Projectile.rotation += 0.05f;

            Projectile.ai[0] += 0.12f;
            float angleBase = Projectile.ai[0];
            int dustCount = 7;
            float minR = 12f;
            float maxR = 90f;

            for (int i = 0; i < dustCount; i++)
            {
                float t = i / (float)dustCount;
                float angle = angleBase + t * MathHelper.TwoPi;
                float radius = MathHelper.Lerp(minR, maxR, t) + Main.rand.NextFloat(-8f, 8f);
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Vector2 spawnPos = Projectile.Center + offset;

                int d = Dust.NewDust(new Vector2(spawnPos.X - 4f, spawnPos.Y - 4f), 8, 8, DustID.Shadowflame, 0f, 0f, 100, default(Color), 1.2f);
                Dust dust = Main.dust[d];

                Vector2 dir = offset;
                if (dir == Vector2.Zero) dir = new Vector2(0f, -1f);
                dir.Normalize();

                float speed = 1.2f + t * 1.8f;
                Vector2 tangential = new Vector2(1f, 0f).RotatedBy(angle + MathHelper.PiOver2) * 0.6f;

                dust.velocity = dir * speed + tangential;
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
                dust.scale = 1f + Main.rand.NextFloat(0.2f, 0.8f);
                dust.rotation = angle;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(Loop, Projectile.Center);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.StopTrackedSounds();
        }
    }
}