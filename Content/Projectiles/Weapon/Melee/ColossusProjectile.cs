
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
 
 
using log4net.Appender;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class ColossusProjectile : BaseBroadswordProjectileFullSwing
    {
        public SoundStyle Hit = DTAssetLib.SwordSounds.Slam with { PitchVariance = 0.4f, MaxInstances = 0 };

        public override SoundStyle Swing => DTAssetLib.SwordSounds.BigBasicSwing with { Volume = 1.0f, PitchVariance = 0.2f, MaxInstances = 0 };
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 122;
            Projectile.height = 122;
            SweepColor = ColorLib.TenebrisMagenta;
            SwingSpeed = 0.10f;
            UsesDefaultSweepFX = true;
            SweepScale = 1.7f;
        }
        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Hit);
            Player player = Main.player[Projectile.owner];
            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();

            int splatterdir = npc.position.X > Owner.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, ColorLib.TenebrisMagenta, 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            ColossusParticle FX = new ColossusParticle();
            FX.Initiate(npc.Center);
            ParticleEngine.ShaderParticles.Add(FX);

            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<ColossusStar>(), 2, npc.Center, (int)(Projectile.damage * 0.18f), (int)(Projectile.knockBack * 0.5f), 14f);
            if (hit.Crit)
            {
                ScreenShake.screenshakeMagnitude = 8;
                ScreenShake.screenshakeTimer = 10;
                SoundEngine.PlaySound(DTAssetLib.EnergyWoosh with { PitchVariance = 0.4f });
                for (int t = 0; t < 2; t++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center, new Vector2(20f * splatterdir, 0).RotatedByRandom(0.1f), ModContent.ProjectileType<GargantuaPhantom>(), (int)(Projectile.damage * 0.17f), 4, Projectile.owner);
                }
            }
            else
            {
                ScreenShake.screenshakeMagnitude = 4;
                ScreenShake.screenshakeTimer = 10;
            }
        }

        public Vector2 swordTip;
        public Line SwordLine;

        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            SwordLine = new Line(Owner.Center, swordTip);

            ScaleMult = 1.6f;

            Vector2[] Pos = new Vector2[4]
            {
                Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * (Projectile.scale * 0.80f)),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * (Projectile.scale * 0.60f)),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * (Projectile.scale * 0.40f)),
                Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * (Projectile.scale * 0.20f)),
            };

            Fire[] fire = new Fire[5]
            {
                new Fire(),
                new Fire(),
                new Fire(),
                new Fire(),
                new Fire()
            };

            fire[0].PrepareFire(swordTip, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), ColorLib.TenebrisMagenta, 2f, 40, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[0]);

            fire[1].PrepareFire(Pos[0], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), ColorLib.TenebrisMagenta * 0.8f, 2f, 35, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[1]);

            fire[2].PrepareFire(Pos[1], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), ColorLib.TenebrisMagenta * 0.6f, 2f, 30, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[2]);

            fire[3].PrepareFire(Pos[2], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), ColorLib.TenebrisMagenta * 0.4f, 2f, 25, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[3]);

            fire[4].PrepareFire(Pos[3], Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.12f, 0.12f), ColorLib.TenebrisMagenta * 0.2f, 2f, 10, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
            ParticleEngine.BehindProjectiles.Add(fire[4]);
        }
    }

    /*
    public class ColossusProjectile : ModProjectile
    {
        public SoundStyle Swing = DTAssetLib.SwordSounds.BigBasicSwing with { Volume = 1.0f, PitchVariance = 0.2f, MaxInstances = 0 };
        public SoundStyle Hit = DTAssetLib.SwordSounds.Slam with { PitchVariance = 0.4f, MaxInstances = 0 };
        private enum AttackStage
        {
            Prepare,
            Execute,
            Unwind
        }

        public static int HitCooldownGlobal = 10;
        private int HitCooldown = 0;


        private AttackStage CurrentStage
        {
            get => (AttackStage)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (float)value;
                Timer = 0;
            }
        }

        private ref float InitialAngle => ref Projectile.ai[1];
        private ref float Timer => ref Projectile.ai[2];
        private ref float Progress => ref Projectile.localAI[1];
        private ref float Size => ref Projectile.localAI[2];

        private float prepTime => 8f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 20f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

        private Player Owner => Main.player[Projectile.owner];

        private bool CanContinueSwing(Player player)
        {
            if (player.dead || player.CCed || !player.active)
            {
                return false;
            }
            else
            {
                return player.controlUseItem;
            }
        }

        List<float> OldRotations = new List<float>();
        List<float> OldScales = new List<float>();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 10000; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>(); // Projectile is a melee projectile
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            if (HitCooldown > 0)
            {
                HitCooldown--;
            }
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    Prepare();
                    break;
                case AttackStage.Execute:
                    Execute();
                    break;
                default:
                    Unwind();
                    break;
            }

            SetSwordPosition();
            Timer++;
        }

        public float Scl = 3f;
        public float SlOpacity = 0f;

        public void DrawSlashFX()
        {
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                effects = SpriteEffects.None;
            }
            else
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.spriteBatch.Draw(DTAssetLib.FireSwing.Value, Projectile.Center - Main.screenPosition, null, (DTColorUtils.Darken(ColorLib.TenebrisMagenta, 0.75f) * Projectile.Opacity) * SlOpacity, Projectile.rotation - 0.2f, DTAssetLib.FireSwing.Value.Size() / 2, Scl * Projectile.scale, effects, 0);
            //Main.spriteBatch.Draw(DTAssetLib.FireSwingHighlight.Value, Projectile.Center - Main.screenPosition, null, ((Color.Red * 0.5f) * Projectile.Opacity) * SlOpacity, Projectile.rotation - 0.2f, DTAssetLib.FireSwingHighlight.Value.Size() / 2, Scl * Projectile.scale, effects, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            DrawSlashFX();
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return HitCooldown <= 0 && !target.friendly;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitCooldown = HitCooldownGlobal;
            SoundEngine.PlaySound(Hit);
            Player player = Main.player[Projectile.owner];
            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();
            
            int splatterdir = target.position.X > Owner.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(target.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, ColorLib.TenebrisMagenta, 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            ColossusParticle FX = new ColossusParticle();
            FX.Initiate(target.Center);
            ParticleEngine.ShaderParticles.Add(FX);

            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<ColossusStar>(), 2, target.Center, (int)(Projectile.damage * 0.2f), (int)(Projectile.knockBack * 0.5f), 14f);
            if (hit.Crit)
            {
                ScreenShake.screenshakeMagnitude = 8;
                ScreenShake.screenshakeTimer = 10;
                SoundEngine.PlaySound(DTAssetLib.EnergyWoosh with { PitchVariance = 0.4f });
                for (int t = 0; t < 2; t++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, new Vector2(20f * splatterdir, 0).RotatedByRandom(0.1f), ModContent.ProjectileType<GargantuaPhantom>(), (int)(Projectile.damage * 0.2f), 4, Projectile.owner);
                }
            }
            else
            {
                ScreenShake.screenshakeMagnitude = 4;
                ScreenShake.screenshakeTimer = 10;
            }
        }

        public void SetSwordPosition()
        {
            Projectile.rotation = (InitialAngle + Projectile.spriteDirection * Progress) * Owner.direction;


            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            // Adjust the position for reversed gravity.
            if (Owner.gravDir == -1f)
            {
                Projectile.rotation = 0f - Projectile.rotation;
                armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
            }

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
            Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers

            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
        }

        private void Prepare()
        {
            InitialAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();
            Progress = 0f;
            Size = 1f;

            if (Timer >= prepTime)
            {
                CurrentStage = AttackStage.Execute;
            }
        }

        private float SPINSPEED = 0.01f; // radians per tick
        private int STimer = 0;
        public Vector2 swordTip;
        public Line SwordLine;
        // Tracks the last rotation used to compute angular delta between ticks
        private float _lastRotation = 0f;
        // Accumulates signed angular change; when absolute value reaches TwoPi we count a full revolution
        private float _accumulatedRotation = 0f;
        // Number of full revolutions completed while channeling this projectile
        public int FullRevolutions = 0;
        private void Execute()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            SwordLine = new Line(Owner.Center, swordTip);
            Vector2[] p = SwordLine.GetPointsAlongLine(10);

            if (CanContinueSwing(Owner))
            {
                if (SPINSPEED < 0.36f)
                {
                    SPINSPEED += 0.008f;
                }
                else
                {
                    SlOpacity += 0.05f;
                }

                float speed = SPINSPEED * Owner.GetTotalAttackSpeed(Projectile.DamageType);
                Progress += speed * Projectile.spriteDirection;

                // Compute the rotation the sword will have this tick (matches SetSwordPosition logic)
                float newRotation = (InitialAngle + Projectile.spriteDirection * Progress) * Owner.direction;

                // Initialize last rotation on the first execute tick
                if (Timer == 0)
                {
                    _lastRotation = newRotation;
                }
                else
                {
                    // Compute shortest signed angular difference and accumulate it
                    float delta = MathHelper.WrapAngle(newRotation - _lastRotation);
                    _accumulatedRotation += delta;
                    _lastRotation = newRotation;

                    // If we've accumulated a full revolution (in either direction), increment counter
                    float absAccum = MathF.Abs(_accumulatedRotation);
                    if (absAccum >= MathHelper.TwoPi)
                    {
                        int completed = (int)(absAccum / MathHelper.TwoPi);
                        FullRevolutions += completed;
                        // remove the completed revolutions from the accumulator but preserve the remainder and sign
                        _accumulatedRotation -= MathF.Sign(_accumulatedRotation) * completed * MathHelper.TwoPi;
                    }
                }

                Size = 1f;

                float speedRatio = Math.Min(1f, SPINSPEED / 0.12f);
                int soundInterval = (int)MathHelper.Lerp(200, 40, speedRatio);

                Fire fire = new Fire();
                fire.PrepareFire(swordTip, Vector2.Zero, DTUtils.RandomDirection(2), Main.rand.NextFloat(-0.3f, 0.3f), ColorLib.TenebrisMagenta * 0.8f, 0.5f, 40, FireDrawMode.NonPremultiplied, PixelLayer.AboveProjectiles);
                ParticleEngine.BehindProjectiles.Add(fire);

                STimer++;
                if (STimer % soundInterval == 0)
                {
                    SoundEngine.PlaySound(Swing with { PitchVariance = 1f });
                }
            }
            else
            {
                CurrentStage = AttackStage.Unwind;
            }
        }

        private void Unwind()
        {
            float speed = SPINSPEED * Owner.GetTotalAttackSpeed(Projectile.DamageType);
            Progress += speed * Projectile.spriteDirection;
            Size = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);
            Projectile.Opacity = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);
            SlOpacity = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);

            if (Timer >= hideTime)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            OldRotations.Clear();
            OldScales.Clear();
        }
    }
    */
}