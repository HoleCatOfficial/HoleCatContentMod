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

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class MiniRose : ModProjectile
    {


        public override void SetDefaults()
        {
            Projectile.width = 27;
            Projectile.height = 42;
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

        public int MaxRadius;
        public float Radius = 0f;

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

            if (PitchVal < 0)
            {
                PitchVal += 0.01f;
            }

            if (Radius < MaxRadius)
            {
                Radius += 0.5f;
            }
            
            Vector2[] p = Opus.GetEquidistantOrbitVectors(dustAmount, Projectile.Center, 0.6f, Radius);

            foreach(Vector2 dustPos in p)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), dustPos, Vector2.Zero, ColorLib.Soul3 * 0.85f, 2.0f, 75, ai2: 2);
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], dustPos, Vector2.Zero, ColorLib.Soul2 * 0.85f, 1.0f, 75, ai2: 2);
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], dustPos, Vector2.Zero, ColorLib.Soul, 0.75f, 60, ai2: 2);
            }

            float radiusSq = Radius * Radius;
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead && Vector2.DistanceSquared(player.Center, Projectile.Center) <= radiusSq)
                {
                    player.AddBuff(ModContent.BuffType<MiniRoseBoost>(), 60);
                }
            }

            Projectile.Center += new Vector2(0, Opus.Sine(-1f, 1f, 0.01f));

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