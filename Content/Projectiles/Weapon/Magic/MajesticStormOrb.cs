using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class MajesticStormOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
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

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                Color.White,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }
        private void AnimateProjectile() 
        {
            if (++Projectile.frameCounter >= 10) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            AnimateProjectile();
            Lighting.AddLight(Projectile.Center, new Color(29, 226, 186).ToVector3() * 0.6f);

            Projectile.ai[0]++;

            PointGlowPreMultiplied Glow2 = new();
            Glow2.Initialize(Projectile.Center + Main.rand.NextVector2CircularEdge(220f, 220f), Vector2.Zero, new Color(29, 226, 186) with { A = 0 } * 0.2f, 1f, 20);
            ParticleEngine.BehindProjectiles.Add(Glow2);

            PixelParticle Pixel2 = new();
            Pixel2.Initialize(Projectile.Center + Main.rand.NextVector2CircularEdge(220f, 220f), Main.rand.NextVector2Circular(1f, 1f), new Color(29, 226, 186) with { A = 0 }, 2f);
            ParticleEngine.BehindProjectiles.Add(Pixel2);


            if (Projectile.ai[0] % 15 == 0)
            {
                foreach (NPC target in Main.ActiveNPCs)
                {
                    if (target.Center.Distance(Projectile.Center) < 200f && !target.friendly)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, target.Center);

                        Vector2 outer = target.Center;
                        var curve = DTUtils.EasyBezier(Projectile.Center, Projectile.Center.DirectionTo(outer).RotatedByRandom(3f), outer, outer.DirectionFrom(Projectile.Center).RotatedByRandom(3f), 0.5f);
                        var points = curve.GetEvenlySpacedPoints(15);
                        for (int i = 0; i < points.Count; i++)
                        {
                            PointGlowPreMultiplied ArcGlow = new();
                            ArcGlow.Initialize(Projectile.Center, Main.rand.NextVector2Circular(4f, 4f), new Color(29, 226, 186) with { A = 0 } * 0.05f, 2.5f);
                            ParticleEngine.BehindProjectiles.Add(ArcGlow);

                            PixelParticle Arc = new();
                            Arc.Initialize(points[i], Vector2.Zero, new Color(29, 226, 186) with { A = 0 }, 2f, 60);
                            ParticleEngine.BehindProjectiles.Add(Arc);


                        }

                        target.AddBuff(BuffID.Electrified, 300);
                        int Damage = (int)(Projectile.damage);
                        Damage = Utils.Clamp(Damage, 10, 100);
                        target.SimpleStrikeNPC(Damage, Math.Sign((target.Center - Projectile.Center).X), false, 1f, DamageClass.Default);

                        if (!target.boss)
                        {
                            target.velocity *= 0.1f;
                        }
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.Zap with { PitchVariance = 0.2f, MaxInstances = 0 });
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MajesticStormExplosion>(), Projectile.damage, 12, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 20 * 60);
        }
	}
}