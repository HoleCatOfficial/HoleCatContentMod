using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.fire;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class HomingShadowflame : ModProjectile
    {
        private NPC NPCTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D T = TextureAssets.Projectile[Type].Value;

            int frameHeight = T.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                T.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(T.Width / 2f, frameHeight / 2f);

            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawGlowOnProj(Projectile, Color.DarkMagenta, true);

            Vector2 drawOrigin = new Vector2(T.Width * 0.5f, T.Height * 0.5f);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2;
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(T, drawPos, frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(T, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 20 && !target.friendly;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

 

        public override void AI()
        {
            AnimateProjectile();

            DelayTimer++;
            Projectile.rotation = 0.03f * Projectile.velocity.X;

            Fire fire = new Fire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), Color.Indigo, 0.3f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);

            Fire fireX = new Fire();
            fireX.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), DTColorUtils.Pastel(Color.Purple, 0.4f), 0.2f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fireX);

            if (Main.rand.NextBool(4))
            {
                Fire fire2 = new Fire();
                fire2.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), Color.DarkMagenta, 0.3f, 100, FireDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(fire2);

                Fire fire3 = new Fire();
                fire3.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), DTColorUtils.Pastel(Color.HotPink, 0.5f), 0.15f, 100, FireDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(fire3);
            }

            Lighting.AddLight(Projectile.Center, Color.DarkMagenta.ToVector3() * 0.2f);

            Lighting.AddLight(Projectile.Center, Color.DarkMagenta.ToVector3() * 0.2f);

            if (DelayTimer < 20)
            {
                return;
            }

            float maxDetectRadius = 1400f;

            if (NPCTarget == null)
            {
                NPCTarget = FindClosestNPC(maxDetectRadius);
            }


            if (NPCTarget != null && !IsValidNPC(NPCTarget))
            {
                NPCTarget = null;
            }


            if (NPCTarget == null)
                return;

            float targetAngle = Projectile.AngleTo(NPCTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * Projectile.velocity.Length();

            float speed = Projectile.velocity.Length();
            float desiredSpeed = 35f;
            float acceleration = 0.3f;
            if (speed < desiredSpeed)
                speed += acceleration;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;

        }
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            {
                if (IsValidNPC(target))
                {

                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidNPC(NPC target)
        {
            return target.CanBeChasedBy();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Opus.RadialSpreadDust(DustID.FireworksRGB, 12, Projectile.Center, 0, Color.DarkMagenta, 1f, 1.5f, RandomOffset: true);
        }
    }
}