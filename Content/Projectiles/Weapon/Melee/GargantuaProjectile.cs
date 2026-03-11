
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using InnoVault;
using InnoVault.PRT;
using log4net.Appender;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
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
	public class GargantuaProjectile : ModProjectile
	{
        public SoundStyle Swing = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/HeavySwing", 3) with { Volume = 1.0f, PitchVariance = 0.2f, MaxInstances = 0 };
        public SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/DreamHit", 3) with { PitchVariance = 0.4f, MaxInstances = 0 };
        private enum AttackStage
        {
            Prepare,
            Execute,
            Unwind
        }

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
            Projectile.width = 122;
            Projectile.height = 122;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Hit);
            Player player = Main.player[Projectile.owner];
            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();
           
            int splatterdir = target.position.X > Owner.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), target.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), Color.Red * Main.rand.NextFloat(0.01f, 0.3f), 1f);
            }

            PRTLoader.NewParticle(PRTLoader.GetParticleID<GargantuaParticle>(), target.Center, Vector2.Zero, (Color)default, 1f);
			Opus.RadialSpreadParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], 10, target.Center, 0.4f, Color.Red, 2f, 3, RandomOffset: true);
            Opus.RadialProjectileRandomDir(ModContent.ProjectileType<GargantuaStar>(), 2, target.Center, (int)(Projectile.damage * 0.2f), (int)(Projectile.knockBack * 0.5f), 14f, friendly: true);

			if (hit.Crit)
			{
                ScreenShake.screenshakeMagnitude = 4;
                ScreenShake.screenshakeTimer = 20;
                for (int t = 0; t < 2; t++)
				{
					Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, new Vector2(20f * splatterdir, 0).RotatedByRandom(0.1f), ModContent.ProjectileType<GoliathPhantom>(), (int)(Projectile.damage * 0.2f), 4, Projectile.owner);
				}
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

                float speedRatio = Math.Min(1f, SPINSPEED / 0.36f);
                int soundInterval = (int)MathHelper.Lerp(200, 20, speedRatio);

                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], swordTip, Vector2.Zero, new Color(255, 0, 0, 0.5f), 4f, 40, ai2: 1);

                STimer++;
                if (STimer % soundInterval == 0)
                {
                    SoundEngine.PlaySound(Swing with { PitchVariance = 1f });
                }

				/*

                if (FullRevolutions > 5 && STimer % 40 == 0 && SPINSPEED >= 0.36f)
                {
					;
                }
                if (FullRevolutions > 15 && STimer % 40 == 0 && SPINSPEED >= 0.36f)
                {
                    Opus.RadialProjectileRandomDir(ModContent.ProjectileType<GoliathPhantom>(), 4, Projectile.Center, (int)(Projectile.damage * 0.65f), (int)(Projectile.knockBack * 0.5f), 36f, friendly: true);
                }
				*/
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
}