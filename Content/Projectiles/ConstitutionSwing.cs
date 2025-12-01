using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{

	public class ConstitutionSwing : ModProjectile
	{
        public SoundStyle Swing = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/MagicSwing", 3){ PitchVariance = 0.2f, MaxInstances = 0 };
		private const float SWINGRANGE = 1.67f * (float)Math.PI; 
		private const float FIRSTHALFSWING = 0.45f;
		private const float WINDUP = 0.05f;
		private const float UNWIND = 0.4f;

		private enum AttackType 
		{
			
			SwingDown,
			
			SwingUp,
		}

		private enum AttackStage 
		{
			Prepare,
			Execute,
			Unwind
		}

		
		private AttackType CurrentAttack {
			get => (AttackType)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}

		private AttackStage CurrentStage {
			get => (AttackStage)Projectile.localAI[0];
			set {
				Projectile.localAI[0] = (float)value;
				Timer = 0; 
			}
		}

		
		private ref float InitialAngle => ref Projectile.ai[1];
		private ref float Timer => ref Projectile.ai[2];
		private ref float Progress => ref Projectile.localAI[1];
		private ref float Size => ref Projectile.localAI[2];
		private float prepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float execTime => 6f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

		public override string Texture => "DestroyerTest/Content/MeleeWeapons/SwordLineage/Constitution";
		private Player Owner => Main.player[Projectile.owner];

		public override void SetStaticDefaults() {
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.timeLeft = 10000;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee; 
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}

		public override void OnSpawn(IEntitySource source) {
			Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

            if (CurrentAttack == AttackType.SwingUp) {
                if (Projectile.spriteDirection == 1) {
                   
                    targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 1 / 6, (float)Math.PI * 2 / 3);
                }
                else {
                    if (targetAngle < 0) {
                        targetAngle += 2 * (float)Math.PI;
                    }
                    targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 4 / 3, (float)Math.PI * 5 / 3);
                }
                InitialAngle = targetAngle + FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Inverse: add instead of subtract
            }
            else {
                if (Projectile.spriteDirection == 1) {
				
					targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
				}
				else {
					if (targetAngle < 0) {
						targetAngle += 2 * (float)Math.PI; 
					}

					targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
				}

				InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Otherwise, we calculate the angle
			}
		}

		public override void SendExtraAI(BinaryWriter writer) {
		
			writer.Write((sbyte)Projectile.spriteDirection);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
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
                    PrepareStrike();
                    break;
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                default:
                    UnwindStrike();
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
            Texture2D texture = TextureAssets.Projectile[Type].Value;

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

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, default, Color.White * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);
         
            return false;
        }

		
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
		}

			public override void CutTiles() {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
		}

	
		public override bool? CanDamage() {
			if (CurrentStage == AttackStage.Prepare)
				return false;
			return base.CanDamage();
		}
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];
			
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (int?)(target.position.Y + 15);
        }


		public void SetSwordPosition()
		{

			Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;


			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
			Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);

			armPosition.Y += Owner.gfxOffY;
			Projectile.Center = armPosition;
			Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem);

			Owner.heldProj = Projectile.whoAmI;
			
			
		}

		
		private void PrepareStrike() {
           
			Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime); 
			Size = MathHelper.SmoothStep(0, 1, Timer / prepTime); 

			if (Timer >= prepTime) {
				SoundEngine.PlaySound(SoundID.Item1); 
				CurrentStage = AttackStage.Execute; 
			}
		}

		private bool Sound1 = false;
        private bool Sound2 = false;
		public Vector2 swordTip;
        private void ExecuteStrike() {
			swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Player player = Main.player[Projectile.owner];

            if (CurrentAttack == AttackType.SwingDown) {
				
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / execTime);
				if (!Sound1)
				{
                	SoundEngine.PlaySound(Swing, player.Center);
					Sound1 = true;
				}

				Vector2 Velocity = (Main.MouseWorld - Projectile.Center).ToRotation().ToRotationVector2() * 8f;

                if (Timer == execTime / 2)
				{
                	Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Velocity, ModContent.ProjectileType<StellarFireSlash>(), Projectile.damage / 3, 2, Main.player[Projectile.owner].whoAmI, ai0: 0);
				}


				Lighting.AddLight(player.Center, ColorLib.StellarColor.ToVector3());

                if (Timer >= execTime) {
                    CurrentStage = AttackStage.Unwind;
                }
            }
            else if (CurrentAttack == AttackType.SwingUp) {
				if (player.direction == 1)
				{
					Progress = MathHelper.SmoothStep(SWINGRANGE, 0, (1f - UNWIND) * Timer / execTime);
				}
				if (player.direction == -1)
				{
					Progress = MathHelper.SmoothStep(SWINGRANGE, 2.0f, (1f - UNWIND) * Timer / execTime);
				}

                if (!Sound2)
				{
                	SoundEngine.PlaySound(Swing, player.Center);
					Sound2 = true;
				}

				Vector2 Velocity = (Main.MouseWorld - Projectile.Center).ToRotation().ToRotationVector2() * 8f;

				if (Timer == execTime / 2)
				{
                	Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Velocity, ModContent.ProjectileType<StellarFireSlash>(), Projectile.damage / 3, 2, Main.player[Projectile.owner].whoAmI, ai0: 1);
				}

               
                Lighting.AddLight(player.Center, ColorLib.StellarColor.ToVector3());

                if (Timer >= execTime) {
                    CurrentStage = AttackStage.Unwind;
                }
            }
        }
		
		private void UnwindStrike() {
            Player player = Main.player[Projectile.owner];
			if (CurrentAttack == AttackType.SwingDown)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) + UNWIND * Timer / hideTime);
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);

                if (Timer >= hideTime)
                {
					Sound1 = false;
					Sound2 = false;
                    Projectile.Kill();
                }
            }
            else if (CurrentAttack == AttackType.SwingUp)
            {
				if (player.direction == 1)
				{
					Progress = MathHelper.SmoothStep(SWINGRANGE, 0, (1f - UNWIND) + UNWIND * Timer / hideTime);
				}
				if (player.direction == -1)
				{
					Progress = MathHelper.SmoothStep(SWINGRANGE, 1.6f, (1f - UNWIND) + UNWIND * Timer / hideTime);
				}
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);

                if (Timer >= hideTime)
                {
					Sound1 = false;
					Sound2 = false;
                    Projectile.Kill();
                }
            }
		}
		public override void OnKill(int timeLeft)
        {
        }
	}

	

}