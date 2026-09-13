using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
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

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class FleshBombFriendly : ModProjectile
    {
        public SoundStyle BombPlant = SoundID.Item50;
        public SoundStyle BombBlow = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/FleshBombExplode") with { PitchVariance = 1.0f, MaxInstances = 0 };


        public override void SetDefaults()
        {
            Projectile.width = 24; // The width of projectile hitbox
            Projectile.height = 24; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 30; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
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
            Projectile.rotation += Main.rand.NextFloat(-1f, 1.1f) * 0.1f;
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 0, default, 1.0f);
            if (Projectile.timeLeft == 1)
            {
                Projectile.Resize(200, 200);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(BombBlow, Projectile.Center);

            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Red, 0.1f, 0.05f, 1.7f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion);
            SimpleExplosionParticle Explosion2 = new();
            Explosion2.Prepare(Projectile.Center, Vector2.Zero, Color.Red * 0.5f, 0.1f, 0.05f, 1f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Explosion2);

            Opus.RadialSpreadDustRandom(DustID.Blood, 10, Projectile.Center, 0, default, 1f, 8);
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 10, Projectile.Center, 0, Color.Red, 1f, 8);
            Opus.RadialSpreadDustRandom(DustID.Blood, 6, Projectile.Center, 0, default, 0.75f, 6);
        }
    }
}
