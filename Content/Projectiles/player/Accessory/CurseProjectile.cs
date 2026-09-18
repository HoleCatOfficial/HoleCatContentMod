using System;
using System.Formats.Tar;
using System.Linq;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class CurseProjectile : ModProjectile, IHomingProjectile
    {
        enum curseType
        {
            Hellfire,
            Shadowflame,
            SpiritDrift
        }

        curseType CurseType;

        public ref float DelayTimer => ref Projectile.ai[1];

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 6f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.2f;

        float IHomingProjectile.HomingMaxAccel => 20f;

        float IHomingProjectile.DetectRadius => 2000f;

        bool IHomingProjectile.CanHome => Timer > 90;

        int Timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 70;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;

            var ct = Enum.GetValues<curseType>();

            CurseType = ct[Main.rand.Next(3)];
        }

        int scroll = 0;
        float sclAMT = 0f;
        float Modifiier = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            sclAMT += 2f;
            Texture2D texture = DTAssetLib.CurseSigilRing.Value;
            Texture2D SparkTex = DTAssetLib.MiscSparkle144.Value;
            Vector2 origin = texture.Size() / 2f;
            Vector2 SparkOrigin = SparkTex.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Modifiier = Opus.Sine(1f, 1.6f, 0.9f);

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(Trailtype(), true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 12f, Col(), sclAMT);

            spriteBatch.Draw(SparkTex, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0f, SparkOrigin, new Vector2(0.4f, 1f) * Modifiier * 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(SparkTex, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, MathHelper.PiOver2, SparkOrigin, new Vector2(0.4f, 2f) * Modifiier * 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(SparkTex, Projectile.Center - Main.screenPosition, null, Col() with { A = 0 }, MathHelper.PiOver2, SparkOrigin, Projectile.scale * Modifiier * 1.4f, SpriteEffects.None, 0f);
            
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Col() with { A = 0 }, rot, origin, 0.3f * Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Col() with { A = 0 }, -rot, origin, 0.245f * Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(DTAssetLib.BloomRing.Value, Projectile.Center - Main.screenPosition, null, Col() with { A = 0 } * 0.5f, -rot, DTAssetLib.BloomRing.Value.Size() / 2, 0.6f * Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        int Trailtype()
        {
            switch (CurseType)
            {
                case curseType.Hellfire:
                    return 4;
                case curseType.Shadowflame:
                    return 2;
                case curseType.SpiritDrift:
                    return 8;
                default:
                    return 8;
            }
        }

        Color Col()
        {
            switch (CurseType)
            {
                case curseType.Hellfire:
                    return Color.OrangeRed;
                case curseType.Shadowflame:
                    return Color.Purple;
                case curseType.SpiritDrift:
                    return Color.CadetBlue;
                default:
                    return Color.White;
            }
        }

        float rot = 0f;
        public override void AI()
        {
           
            Timer++;

            if (Timer < 90)
            {
                Projectile.velocity *= 0.94f;
            }

            rot += 0.05f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    Projectile.oldPos[i] = Projectile.Center;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (CurseType)
            {
                case curseType.Hellfire:
                    {
                        target.AddBuff(BuffID.OnFire3, 300);
                        break;
                    }
                case curseType.Shadowflame:
                    {
                        target.AddBuff(BuffID.ShadowFlame, 300);
                        break;
                    }
                case curseType.SpiritDrift:
                    {
                        target.AddBuff(ModContent.BuffType<SpiritDrift>(), 300);
                        break;
                    }

            }
        }

        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, newColor: Col(), Scale: 2f);
                dust.noGravity = true;
            }
        }
    }

}