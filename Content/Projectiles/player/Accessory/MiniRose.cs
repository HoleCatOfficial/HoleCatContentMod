using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using InnoVault.PRT;
using DestroyerTest.Content.Particles.TitaniumShard;
using DestroyerTest.Content.Particles;
using DestroyerTest.Common;
using OpusLib;
using ReLogic.Utilities;
using Terraria.Audio;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Build.Evaluation;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class MiniRose : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 1200;
            MaxRadius = 250;
            if (Main.expertMode)
            {
                MaxRadius = 300;
            }
            if (Main.masterMode)
            {
                MaxRadius = 350;
            }
        }

        public override void PostDraw(Color lightColor)
        {
            if (oldCenter.Count > 11)
            {
                Main.EntitySpriteDraw(DTAssetLib.MiniRoseFragment(1).Value, oldCenter[5] - Main.screenPosition, null, Color.White, 0f, DTAssetLib.MiniRoseFragment(1).Value.Size() / 2, 1f, SpriteEffects.None);
                Main.EntitySpriteDraw(DTAssetLib.MiniRoseFragment(2).Value, oldCenter[10] - Main.screenPosition, null, Color.White, 0f, DTAssetLib.MiniRoseFragment(2).Value.Size() / 2, 1f, SpriteEffects.None);
            }
        }


        public int MaxRadius;
        public float Radius = 0f;
        public List<Vector2> oldCenter = new List<Vector2>();

        public SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/SpiritAura", 4) 
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        public float PitchVal = -0.7f;

        public override void AI()
        {
            int dustAmount = 6;

            if (PitchVal < 0 && Projectile.timeLeft > 180)
            {
                PitchVal += 0.01f;
            }

            if (PitchVal > -0.7f && Projectile.timeLeft <= 180)
            {
                PitchVal -= 0.01f;
            }

            if (Radius < MaxRadius && Projectile.timeLeft > 180)
            {
                Radius += 1f;
            }

            if (Radius > 0 && Projectile.timeLeft <= 180)
            {
                Radius -= 1f;
            }
            
            Vector2[] p = Opus.GetEquidistantOrbitVectors(dustAmount, Projectile.Center, 1f, Radius);

            foreach(Vector2 dustPos in p)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), dustPos, Vector2.Zero, ColorLib.CursedFlames * 0.5f, 2.0f);
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], dustPos, Vector2.Zero, ColorLib.CursedFlames * 0.75f, 1.0f, 75, ai2: 2);
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], dustPos, Vector2.Zero, ColorLib.CursedFlames, 0.75f, 60, ai2: 2);
            }

            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f, 0, default, 1f);
            d.noGravity = true;
            for (int c = 0; c < 2; c++)
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Radius, Radius), DustID.CursedTorch, Vector2.Zero, 0, default, 1f);
                d2.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, TorchID.Cursed);

            float radiusSq = Radius * Radius;
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && Vector2.DistanceSquared(player.Center, Projectile.Center) <= radiusSq)
                {
                    player.AddBuff(ModContent.BuffType<MiniRoseBoost>(), 60);
                }
            }

            Projectile.Center += new Vector2(0, Opus.Sine(-1f, 1f, 0.01f));

            oldCenter.Insert(0, Projectile.Center);

            if (oldCenter.Count > 20)
            {
                oldCenter.RemoveAt(19);
            }

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound)) {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    soundInstance.Position = Projectile.Center;
                    soundInstance.Pitch = PitchVal;
                    return tracker.IsActiveAndInGame();
                });
            }
            else
            {
                activeSound.Position = Projectile.Center;
                activeSound.Pitch = PitchVal;
            }
        }
    }
}