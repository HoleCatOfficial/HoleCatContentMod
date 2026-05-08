using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using DestroyerTest.Common;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpusLib.Content.Helpers;
using Terraria.Audio;
using DestroyerTest.Content.MeleeWeapons.Flails;
using OpusLib;
using DestroyerTest.Content.Dusts;
using System;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using Terraria.GameContent;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee.Flail
{
    public class CoronaBreakerProjectile : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public string AssetRoot = "DestroyerTest/Content/Projectiles/Weapon/Melee/Flail";
        public static Asset<Texture2D> Head;
        public static Asset<Texture2D> Handle;
        public static Asset<Texture2D> Chain;
        public static Asset<Texture2D> Segment1;
        public static Asset<Texture2D> Segment2;
        public override void SetStaticDefaults()
        {
            Head = ModContent.Request<Texture2D>($"{AssetRoot}/CoronaBreakerHead");
            Handle = ModContent.Request<Texture2D>($"{AssetRoot}/CoronaBreakerHandle");
            Chain = ModContent.Request<Texture2D>($"{AssetRoot}/CoronaBreakerChain");
            Segment1 = ModContent.Request<Texture2D>($"{AssetRoot}/CoronaBreakerSegment1");
            Segment2 = ModContent.Request<Texture2D>($"{AssetRoot}/CoronaBreakerSegment2");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public void DrawChain()
        {
            //Linear wrapping of the chain texture for seamless drawing.
            Vector2 Start = (Body.Start + (Body.GetLineRotation.ToRotationVector2() * 20));
            Vector2 chainDirection = Body.End - Start;
            float chainDistance = chainDirection.Length();
            chainDirection.Normalize();

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            int chainSegments = (int)(chainDistance / Chain.Value.Height);
            for (int i = 0; i < chainSegments; i++)
            {
                Vector2 position = Start + chainDirection * (i * Chain.Value.Height);
                Main.EntitySpriteDraw(Chain.Value, position - Main.screenPosition, null, Color.White, chainDirection.ToRotation() + MathHelper.PiOver2, new Vector2(Chain.Value.Width / 2, 0), Projectile.scale, SpriteEffects.None);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDrawExtras()
        {
            DrawChain();
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawGlowOnProj(Projectile, ColorLib.Rift, false, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Main.EntitySpriteDraw(Handle.Value, (Body.Start + (Body.GetLineRotation.ToRotationVector2() * 20)) - Main.screenPosition, null, Color.White, Body.GetLineRotation + MathHelper.PiOver2, Handle.Value.Size() / 2, Projectile.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(Segment2.Value, BodyPoints[2] - Main.screenPosition, null, Color.White, Body.GetLineRotation + MathHelper.PiOver2, Segment2.Value.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(Segment1.Value, BodyPoints[1] - Main.screenPosition, null, Color.White, Body.GetLineRotation + MathHelper.PiOver2, Segment1.Value.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(Segment2.Value, BodyPoints[4] - Main.screenPosition, null, Color.White, Body.GetLineRotation + MathHelper.PiOver2, Segment2.Value.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(Segment1.Value, BodyPoints[3] - Main.screenPosition, null, Color.White, Body.GetLineRotation + MathHelper.PiOver2, Segment1.Value.Size() / 2, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(Head.Value, Projectile.Center - Main.screenPosition, null, Color.White, Body.GetLineRotation + MathHelper.PiOver2, Head.Value.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        

        public Player Owner;
        public Player ItemCheckOwner;
        public override bool PreAI()
        {
            ItemCheckOwner = Main.player[Projectile.owner];
            bool HasWeapon = (ItemCheckOwner.HeldItem.type == ModContent.ItemType<CoronaBreaker>());
            return HasWeapon;
        }

        public Line Body;
        public Vector2[] BodyPoints;

        public const float MaxExtendDistance = 400f;

        private float arcT;
        private float baseAngle;

        public int AttackType => (int)Projectile.ai[1];

        public override void AI()
        {
            Projectile.ai[2]++;
            Owner = Main.player[Projectile.owner];
            Body = new Line(Owner.MountedCenter, Projectile.Center);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            BodyPoints = Body.GetPointsAlongLine(5);

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Body.GetLineRotation - MathHelper.PiOver2);

            float Modifier = 0.02f * Owner.GetAttackSpeed(DamageClass.Melee);
            float Speed = 0.04f + Modifier;

            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, ColorLib.Rift, 1f);
            //PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Projectile.Center, Projectile.velocity * 0.05f, ColorLib.Rift, 0.5f, ai1: 2);

        
            foreach(Vector2 p in BodyPoints)
            {
                if (p != BodyPoints[0] && p != BodyPoints[5])
                {
                    Dust.NewDustPerfect(p, ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, ColorLib.Rift, 1f);
                }
            }

            if (arcT == 0f)
            {
                baseAngle = (Main.MouseWorld - Owner.Center).ToRotation();
            }

            arcT += Speed;
            arcT = MathHelper.Clamp(arcT, 0f, 1f);
            float radius = (float)Math.Sin(arcT * MathHelper.Pi) * MaxExtendDistance;

            float sweepAngle = MathHelper.ToRadians(90f);

            float sweepDir = AttackType == 2 ? -1f : 1f;

            float angle = baseAngle + sweepDir * MathHelper.Lerp(-sweepAngle / 2f, sweepAngle / 2f, arcT);

            Vector2 targetPos = Owner.Center + angle.ToRotationVector2() * radius;

            Projectile.velocity = (targetPos - Projectile.Center) * 0.35f;

            if (arcT >= 1f)
            {
                Projectile.Kill();
            }
        }

        public override void EmitEnchantmentVisualsAt(Vector2 boxPosition, int boxWidth, int boxHeight)
        {
            
            boxWidth = 16;
            boxHeight = 16;
            Vector2 Offset = new Vector2(boxWidth / 2, boxHeight / 2);
            
            if (Projectile.ai[2] > 10)
            {
                foreach(Vector2 p in BodyPoints)
                {
                    if (p != BodyPoints[0])
                    {
                        boxPosition = p - Offset;
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (Vector2 p in BodyPoints)
            {
                for (int i = 1; i < BodyPoints.Length; i++)
                {
                    Vector2 point1 = BodyPoints[i - 1];
                    Vector2 point2 = BodyPoints[i];
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), point1, point2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Strike = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Opus.RadialDustRandomDir(DustID.FireworksRGB, 10, target.Center, 0, ColorLib.Rift, 1.3f, 3);

            if (Strike)
            {
                return;
            }

            if (Owner.HeldItem.ModItem is CoronaBreaker c)
            {
                c.hitcount++;
                c.p += 0.3f;

                if (c.hitcount < 4)
                {
                    SoundEngine.PlaySound(DTAssetLib.Charge.RiftFlailTick with { MaxInstances = 0, Pitch = c.p}, Projectile.Center);
                }

                if (c.hitcount >= 4)
                {
                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<RiftStarFriendly>(), 3, Projectile.Center, Projectile.damage / 6, 2, 4, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                    SoundEngine.PlaySound(DTAssetLib.Charge.RiftFlailBurst with { MaxInstances = 0, PitchVariance = 0.4f }, Projectile.Center);
                    c.hitcount = 0;
                    c.p = 0f;
                }
            }
            Strike = true;
        }
    }
}