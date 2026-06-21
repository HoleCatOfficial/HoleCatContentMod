using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
    public class TrackingFireSlash : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public Player HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.player[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }

        public Color MainColor = Color.White;
        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.FireSwing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.StellarFireGradientLooping(), Projectile.rotation, DTAssetLib.FireSwing.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.FireSwingHighlight.Value, Projectile.Center - Main.screenPosition, null, DTColorUtils.Pastel(ColorLib.StellarFireGradientLooping(), 0.7f), Projectile.rotation, DTAssetLib.FireSwingHighlight.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        public float TextureRotationOffset = 0f;

        public int Lifetime = 1200;
        public int Time = 0;

        public bool StartKill = false;
        public void UpdateLerpTime()
        {
            Time++;

            if (Time > Lifetime)
            {
                StartKill = true;
            }
        }
        public float LifetimeCompletion
        {
            get
            {
                if (Lifetime <= 0)
                {
                    return 0f;
                }

                return (float)Time / (float)Lifetime;
            }
        }

        float WarnScale = 0f;
        public Vector2 ToPlayer;
        public override void AI()
        {
            UpdateLerpTime();
            MainColor = ColorLib.StellarFireGradient(LifetimeCompletion);

            if (HomingTarget != null)
            {
                ToPlayer = Projectile.Center - HomingTarget.Center;
            }
            TextureRotationOffset -= 0.5f;
            Projectile.rotation += 0.4f;

            if (Projectile.timeLeft < 120)
            {
                Projectile.Opacity -= 0.08f;
            }

            float maxDetectRadius = 2400f;

            if (HomingTarget == null)
            {
                HomingTarget = FindPlayer(maxDetectRadius);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(4)).ToRotationVector2() * length;
            
        }

        public Player FindPlayer(float maxDetectDistance)
        {
            Player ClosestTarget = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var targetplayer in Main.player)
            {
                if (IsValidTarget(targetplayer))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(targetplayer.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        ClosestTarget = targetplayer;
                    }
                }
            }

            return ClosestTarget;
        }

        public bool IsValidTarget(Player target)
        {
            return target.active == true && target.statLife > 5;
        }



        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DescendantInferno>(), 600);
        }


        public override void OnKill(int timeLeft)
        {

        }
    }
}
