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
    public class CurseProjectile : ModProjectile, IHomingProjectile, IDrawPixelated
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

        float IHomingProjectile.HomingTurnSpeed => 4.7f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1.01f;

        float IHomingProjectile.HomingMaxAccel => 0f;

        float IHomingProjectile.DetectRadius => 2000f;

        bool IHomingProjectile.CanHome => Timer > 120;

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
            Projectile.hide = true;

            var ct = Enum.GetValues<curseType>();

            CurseType = ct[Main.rand.Next(3)];
        }

        int scroll = 0;
        public PixelLayer PixelLayer => PixelLayer.AbovePlayer;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {

            Texture2D texture = DTAssetLib.CurseSigilRing.Value;
            Texture2D SparkTex = DTAssetLib.MiscSparkle144.Value;
            Vector2 origin = texture.Size() / 2f;
            Vector2 SparkOrigin = SparkTex.Size() / 2f;

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            DTTrail.DrawTrailPixelated(spriteBatch, BlendState.AlphaBlend, DTAssetLib.Streak(Trailtype(), true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 16, Col() with { A = 0 }, scroll, 1);

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(SparkTex, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, MathHelper.PiOver2, SparkOrigin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(SparkTex, Projectile.Center - Main.screenPosition, null, Col() with { A = 0 }, MathHelper.PiOver2, SparkOrigin, Projectile.scale * 1.4f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Col() with { A = 0 }, rot, origin, 0.2f * Projectile.scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
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
            scroll -= 20;
            Timer++;

            rot += 0.1f;

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
    }
}