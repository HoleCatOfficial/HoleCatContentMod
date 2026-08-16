using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue.StealthStrike;
using DestroyerTest.Content.SummonItems;
using GlowmaskHelper.Content;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Runtime.Intrinsics.X86;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.RGB;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    [AutoloadGlowmask]
    public class DreamDiscThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = DTAssetLib.DreamDiscHighlight.Value;
            float Opac = Opus.Sine(0f, 0.6f, 1f);

            

            SpriteBatch spriteBatch = Main.spriteBatch;

            SpriteEffects fx = SpriteEffects.None;

            if (Projectile.direction == -1)
            {
                fx = SpriteEffects.FlipHorizontally;
            }

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, Main.shader, Main.GameViewMatrix.TransformationMatrix);
            //Opus.DrawProjectileShadowsRotating(Projectile, 10, Color.White, 0.5f);
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Main.DiscoColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, fx, 0);
            }

            Main.EntitySpriteDraw(DTAssetLib.CircularSwingThin.Value, Projectile.Center - Main.screenPosition, null, Main.DiscoColor * Opac, Projectile.rotation, DTAssetLib.CircularSwingThin.Value.Size() / 2, 0.6f * Projectile.scale, fx);
            Main.EntitySpriteDraw(DTAssetLib.CircularSwingThin.Value, Projectile.Center - Main.screenPosition, null, Main.DiscoColor * Opac.Inverse(), Projectile.rotation + MathHelper.Pi, DTAssetLib.CircularSwingThin.Value.Size() / 2, 0.6f * Projectile.scale, fx);
            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, Projectile.scale, fx);
            return false;
        }
        Player Owner => Main.player[Projectile.owner];
        public float speedfactor => Owner.GetTotalAttackSpeed(DamageClass.Generic);

      

        public Vector2 a;
        public BezierCurve Path;


        public override void OnSpawn(IEntitySource source)
        {
            a = Main.MouseWorld - Projectile.Center;
            a.Normalize();
            Projectile.velocity = a * (20 + speedfactor);
        }

        public int timer = 0;
        public Vector2 toMouse;
        public Vector2 toOwner;
        public float offset = 0;
        public override void AI()
        {
            Projectile.rotation += 0.55f * Projectile.direction;
            
            offset += 0.4f;

            timer++;

            toMouse = Main.MouseWorld - Projectile.Center;
            toMouse.Normalize();

            toOwner = Owner.Center - Projectile.Center;
            toOwner.Normalize();

            Opus.RingSpreadDust(ModContent.DustType<ColorableNeonDust>(), 12, Projectile.Center, 10, 40, Main.DiscoColor, 1f, 3f, offset: offset);
     

            float interval = 6f / speedfactor;
            if (timer >= interval)
            {
                timer = 0;
                SoundEngine.PlaySound(SoundID.Item1 with { MaxInstances = 0, PitchVariance = 0.9f }, Projectile.Center);
            }

            if (Projectile.timeLeft > 180)
            {
                 
            }
            if ((Projectile.timeLeft > 60 && Projectile.timeLeft < 180) || hitcount > 10)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toMouse * 32, 0.05f);
            }
            if (Projectile.timeLeft < 60)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toOwner * 32, 0.05f);
                Projectile.scale *= 0.99f;

                if (Math.Abs(Projectile.Center.Distance(Owner.Center)) < 10)
                {
                    Projectile.Kill();
                }
            }
        }

        public int hitcount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitcount++;
            if (hitcount < 10)
            {
                Projectile.velocity *= 0.05f;
            }
            Projectile.timeLeft += 2;
            target.AddBuff(BuffID.Bleeding, 600);

            if (Projectile.StealthStrike(Owner))
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, target.Center);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<DreamDiscMini>(), 4, target.Center, Projectile.damage / 4, 8, 20, offset: Projectile.rotation);
            }

            SoundEngine.PlaySound(DTAssetLib.SwordSounds.ThinSlice with { MaxInstances = 0, PitchVariance = 0.9f }, Projectile.Center);
            SoundEngine.PlaySound(DTAssetLib.IdriGreatswordSlice(ChildSafety.Disabled), Projectile.Center);

            Vector2 li = target.Center - Projectile.Center;
            Rectangle Spawn = Utils.CenteredRectangle(target.Center + (li / 2), new Vector2(10, 28));


            Spark Spark = new Spark();
            Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Spawn), new Vector2(Main.rand.NextFloat(-0.03f, 0.03f), Main.rand.NextFloat(-4f, 4f)), 0f, Main.DiscoColor, 0.5f, false, 30, SparkDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(Spark);
        }

        public override void OnKill(int timeLeft)
        {
            Vector2[] Vels = Opus.RadialVectorOutwardRandom(10, Projectile.Center, 3f);

            for (int i = 0; i < 10; i++)
            {
                HallowedPallStar Star = new();
                Star.Initialize(Projectile.Center, Vels[i], Color.White, 1f);
                ParticleEngine.ShaderParticles.Add(Star);
            }
        }
    }
}