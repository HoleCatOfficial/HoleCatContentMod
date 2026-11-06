using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class FleshBomb : ModProjectile
    {
        public SoundStyle BombPlant = SoundID.Item50;
        public SoundStyle BombBlow = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/FleshBombExplode") with { PitchVariance = 1.0f, MaxInstances = 0 };

        public override void SetStaticDefaults()
        {
            DTUtils.OwnedByBossNPC[Type] = ModContent.NPCType<WyvernCorpseHead>();
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            Projectile.ArmorPenetration = 100;
        }

        float GlowOpacity = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/FleshBombOutline").Value;

            Main.EntitySpriteDraw(DTAssetLib.Circle.Value, Projectile.Center - Main.screenPosition, null, (Color.Red * GlowOpacity * Projectile.Opacity) * 0.5f, 0f, DTAssetLib.Circle.Size() / 2, DTAssetLib.Circle.Value.ScaleRingTextureToMatchRadius(100, 300), SpriteEffects.None);

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.Red * GlowOpacity * Projectile.Opacity, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None);
            

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            return false;
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

            
            if (Projectile.timeLeft < 20)
            {
                Vector2 dustPos = Projectile.Center;
                Rectangle DustRect = Utils.CenteredRectangle(dustPos, new Vector2(200, 200));
                Vector2 spawnPos = Main.rand.NextVector2FromRectangle(DustRect);

                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDustPerfect(spawnPos, DustID.FireworksRGB, spawnPos.DirectionTo(dustPos) * 8f, newColor: Color.Red);
                }

                GlowOpacity += 0.1f;

                Projectile.scale += 0.03f;
            }
            if (Projectile.timeLeft == 0)
            {
                Projectile.Opacity = 0;
                Projectile.Resize(200, 200);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(BombBlow, Projectile.Center);

            SimpleExplosionParticle Explosion = new();
            Explosion.Prepare(Projectile.Center, Vector2.Zero, Color.Red, 0.1f, 0.05f, 1.7f, BlendState.AlphaBlend);
            ParticleEngine.BehindProjectiles.Add(Explosion);
            SimpleExplosionParticle Explosion2 = new();
            Explosion2.Prepare(Projectile.Center, Vector2.Zero, Color.Red * 0.5f, 0.1f, 0.05f, 1f, BlendState.AlphaBlend);
            ParticleEngine.BehindProjectiles.Add(Explosion2);

            Opus.RadialSpreadDustRandom(DustID.Blood, 10, Projectile.Center, 0, default, 1f, 8);
            Opus.RadialSpreadDustRandom(DustID.FireworksRGB, 10, Projectile.Center, 0, Color.Red, 1f, 8);
            Opus.RadialSpreadDustRandom(DustID.Blood, 6, Projectile.Center, 0, default, 0.75f, 6);
        }
    }
}