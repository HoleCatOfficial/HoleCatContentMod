using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class StunBomb : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public SoundStyle BombPlant = SoundID.NPCHit43;
        public SoundStyle BombBlow = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/FleshBombExplode") with { PitchVariance = 1.0f, MaxInstances = 0 };


        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24; 
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 120; 
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 100;
        }

        public override bool CanHitPlayer(Player target)
        {


            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(BombPlant, Projectile.Center);
        }

        public override void AI()
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            Projectile.velocity *= 0.99f;
            HeatseekerSilohSpark spark = new();
            spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), 0f, ColorLib.Soul, 1f, false, 30, SparkDrawMode.Additive, 2f);
            ParticleEngine.Particles.Add(spark);

            if (Projectile.timeLeft == 1)
            {
                Projectile.Resize(200, 200);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Dazed, 300);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(BombBlow, Projectile.Center);

            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Soul2, 0.1f, 0.05f, 1.7f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion);


            for (int i = 0; i < 20; i++)
            {
                HeatseekerSilohSpark spark = new();
                spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(10f, 10f), 0f, ColorLib.Soul, 1f, false, 60, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark);
            }
        }
    }
}