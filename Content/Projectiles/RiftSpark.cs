using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;
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
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class RiftSpark : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 5f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.01f;

        float IHomingProjectile.HomingMaxAccel => 4f;

        float IHomingProjectile.DetectRadius => 600f;

        bool IHomingProjectile.CanHome => DelayTimer >= 30;

        public float DelayTimer;



        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.Rift;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();


            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(10).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 22, lightColor, trailOffset);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawGlowOnProj(Projectile, lightColor, true);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, lightColor * 0.75f, false, trailOffset, 0.2f, 0.6f);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, lightColor, false, 0f, 0.2f, 0.6f);
            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 30;
        }

        public float length = 0;
        public override void AI()
        {
            DelayTimer++;
            Projectile.ResetExcessTrailPoints();

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.Rift, 0.5f);

            Lighting.AddLight(Projectile.Center, ColorLib.Rift.ToVector3());
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);
            for (int u = 0; u < 6; u++)
            {
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 0, ColorLib.Rift, 0.3f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            
        }

    }

    public class RiftSpark_NoHoming : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public int DelayTimer;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.Rift;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(10).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 22, lightColor, trailOffset);
            Opus.DrawGlowOnProj(Projectile, lightColor, true);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, lightColor * 0.75f, false, trailOffset, 0.2f, 0.6f);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, lightColor, false, 0f, 0.2f, 0.6f);
            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 30;
        }

        public float length = 0;
        public override void AI()
        {
            DelayTimer++;

            Projectile.ResetExcessTrailPoints();

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.Rift, 0.5f);

            Lighting.AddLight(Projectile.Center, ColorLib.Rift.ToVector3());
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);
            for (int u = 0; u < 6; u++)
            {
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 0, ColorLib.Rift, 0.3f);
            }
        }

        public override void OnKill(int timeLeft)
        {

        }

    }



    public class RiftSparkHostile : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        private Player PLRTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.player[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public float DelayTimer;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.Rift;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(10).Value, TrailPositions, TrailRotations, 16, lightColor, trailOffset, 0.01f);
            Opus.DrawGlowOnProj(Projectile, lightColor, true);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, lightColor with { A = 0 } * 0.75f, false, trailOffset, 0.2f, 0.6f);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, lightColor with { A = 0 }, false, 0f, 0.2f, 0.6f);
            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.timeLeft <= 240;
        }
        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 400;
        public float length = 0;
        public override void AI()
        {


            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
            Vector2 newPos = Projectile.Center;

            float dist = Vector2.Distance(lastPos, newPos);
            float step = 1f; // how closely to sample. tweak this!

            if (dist > 0f)
            {
                int segments = (int)(dist / step);

                for (int i = 1; i <= segments; i++)
                {
                    Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
                    TrailPositions.Insert(0, pos);
                    TrailRotations.Insert(0, Projectile.rotation);
                }
            }
            else
            {
                TrailPositions.Insert(0, newPos);
                TrailRotations.Insert(0, Projectile.rotation);
            }


            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            if (Projectile.timeLeft < 285)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Opus.Sine(-0.25f, 0.25f, 0.35f));
            }

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.Rift, 0.5f);

            Lighting.AddLight(Projectile.Center, ColorLib.Rift.ToVector3());

            if (Projectile.timeLeft > 240)
            {
                length = Projectile.velocity.Length();
                return;
            }
            else
            {
                float maxDetectRadius = 2800f;


                if (PLRTarget == null)
                {
                    PLRTarget = FindClosestPlayer(maxDetectRadius);
                }


                if (PLRTarget != null && !IsValidPlayer(PLRTarget))
                {
                    PLRTarget = null;
                }


                if (PLRTarget == null)
                    return;


                if (Projectile.velocity.Length() < 40)
                {
                    length += 0.1f;
                }
                else
                {
                    length = Projectile.velocity.Length();
                }
                float targetAngle = Projectile.AngleTo(PLRTarget.Center);
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * length;

            }
        }

        public Player FindClosestPlayer(float maxDetectDistance)
        {
            Player closestPlayer = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.player)
            {
                if (IsValidPlayer(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestPlayer = target;
                    }
                }
            }

            return closestPlayer;
        }

        public bool IsValidPlayer(Player target)
        {
            return target.active == true && target.statLife > 1;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);
            for (int u = 0; u < 6; u++)
            {
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 0, ColorLib.Rift, 0.3f);
            }
        }

        public override void OnKill(int timeLeft)
        {

        }

    }

    public class RiftSparkHostile_NoHoming : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.Rift;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(10).Value, TrailPositions, TrailRotations, 16, lightColor, trailOffset, 0.01f);
            //Opus.DrawGlowOnProj(Projectile, lightColor, true);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, lightColor with { A = 0 } * 0.75f, false, trailOffset, 0.2f, 0.6f);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, lightColor with { A = 0 }, false, 0f, 0.2f, 0.6f);
            Opus.ReturnToDefaultDrawing(spriteBatch);

            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return Projectile.timeLeft <= 240;
        }

        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 400;
        public float length = 0;
        public override void AI()
        {


            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
            Vector2 newPos = Projectile.Center;

            float dist = Vector2.Distance(lastPos, newPos);
            float step = 1f; // how closely to sample. tweak this!

            if (dist > 0f)
            {
                int segments = (int)(dist / step);

                for (int i = 1; i <= segments; i++)
                {
                    Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
                    TrailPositions.Insert(0, pos);
                    TrailRotations.Insert(0, Projectile.rotation);
                }
            }
            else
            {
                TrailPositions.Insert(0, newPos);
                TrailRotations.Insert(0, Projectile.rotation);
            }


            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            if (Projectile.timeLeft < 285)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Opus.Sine(-0.25f, 0.25f, 0.35f));
            }

            Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1.1f), Main.rand.NextFloat(-1, 1.1f), 0, ColorLib.Rift, 0.5f);

            Lighting.AddLight(Projectile.Center, ColorLib.Rift.ToVector3());
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);
            for (int u = 0; u < 6; u++)
            {
                Dust.NewDust(Projectile.position, Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.FireworksRGB, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 0, ColorLib.Rift, 0.3f);
            }
        }

        public override void OnKill(int timeLeft)
        {

        }

    }
}
