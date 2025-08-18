
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using InnoVault;
using InnoVault.PRT;
using log4net.Appender;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class MoltenScepterSwing : ModProjectile
	{
		public SoundStyle Swing = new SoundStyle("DestroyerTest/Assets/Audio/ConstitutionT3Slash") with { Volume = 1.0f, PitchVariance = 0.4f, Pitch = -1.0f, MaxInstances = 0 };
		public SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/StarHammerThrow") with { Volume = 2.0f, PitchVariance = 0.4f };

		private const float SWINGRANGE = 1.67f * (float)Math.PI;
		private const float SPINRANGE = 4.5f * (float)Math.PI;
		private const float WINDUP = 0.15f;
		private const float UNWIND = 0.4f;
		private const float SPINTIME = 2.0f;

		private enum AttackType 
		{
			Spin,
		}

		private enum AttackStage
		{
			Prepare,
			Execute,
			Unwind
		}

		private AttackType CurrentAttack
		{
			get => (AttackType)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
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

		private float prepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float execTime => 24f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

		private Player Owner => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 68; // Hitbox width of projectile
			Projectile.height = 68; // Hitbox height of projectile
			Projectile.friendly = true; // Projectile hits enemies
			Projectile.timeLeft = 10000; // Time it takes for projectile to expire
			Projectile.penetrate = -1; // Projectile pierces infinitely
			Projectile.tileCollide = false; // Projectile does not collide with tiles
			Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
			Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
			Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
			Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
		}

		public override void OnSpawn(IEntitySource source)
		{
			Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
			float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

			if (CurrentAttack == AttackType.Spin)
			{
				InitialAngle = (float)(-Math.PI / 2 - Math.PI * 1 / 3 * Projectile.spriteDirection); // For the spin, starting angle is designated based on direction of hit
			}
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			// Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
			writer.Write((sbyte)Projectile.spriteDirection);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Projectile.spriteDirection = reader.ReadSByte();
		}

		public override void AI()
		{


			// Extend use animation until projectile is killed
			Owner.itemAnimation = 2;
			Owner.itemTime = 2;

			// Kill the projectile if the player dies or gets crowd controlled
			if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
			{
				Projectile.Kill();
				return;
			}

			// AI depends on stage and attack
			// Note that these stages are to facilitate the scaling effect at the beginning and end
			// If this is not desirable for you, feel free to simplify
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
			// Draw the sword sprite itself
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

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

			return false;  // prevent default drawing
		}

		public override void PostDraw(Color lightColor)
		{
			// Draw the sword sprite itself
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

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);
		}


		// Find the start and end of the sword and use a line collider to check for collision with enemies
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
		}

		// Do a similar collision check for tiles
		public override void CutTiles()
		{
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
		}

		// We make it so that the projectile can only do damage in its release and unwind phases
		public override bool? CanDamage()
		{
			if (CurrentStage == AttackStage.Prepare)
				return false;
			return base.CanDamage();
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			// Make knockback go away from player
			modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;

			// If the NPC is hit by the spin attack, increase knockback slightly
			if (CurrentAttack == AttackType.Spin)
				modifiers.Knockback += 1;
		}

		// Function to easily set projectile and arm position
		public void SetSwordPosition()
		{
			Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress; // Set projectile rotation

			// Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
			Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

			armPosition.Y += Owner.gfxOffY;
			Projectile.Center = armPosition; // Set projectile to arm position
			Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers

			Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
		}

		// Function facilitating the taking out of the sword
		private void PrepareStrike()
		{
			Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime); // Calculates rotation from initial angle
			Size = MathHelper.SmoothStep(0, 1, Timer / prepTime); // Make sword slowly increase in size as we prepare to strike until it reaches max

			if (Timer >= prepTime)
			{
				CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
			}
		}


		public int SpinCount = 0;
		public int MinimumSpinCount = 1;
		public bool AuraActive = false;
		//private int AuraDustCooldown = 0;
		public bool HasWarned = false;
		public bool HasBoosted = false;


		// Function facilitating the first half of the swing
		private void ExecuteStrike()
		{
			Player player = Main.player[Projectile.owner];

			if (CurrentAttack != AttackType.Spin)
				return;

			float spinDuration = execTime * SPINTIME;

			// Update spin progress
			Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) * Timer / spinDuration);

			// Sound + immunity refresh
			if (Timer == (int)(spinDuration * 3 / 4))
			{
				SoundEngine.PlaySound(SoundID.Item1, player.Center);
				Projectile.ResetLocalNPCHitImmunity();

				if (SpinCount >= 5)
				{


					//SoundEngine.PlaySound(SoundID.Item67, player.Center);
					for (int i = 0; i < 4; i++)
					{
						Vector2 Direction = Main.rand.NextVector2CircularEdge(1f, 1f); // Random unit vector on circle edge
						Vector2 velocity = Direction * 24f; // 6f = desired projectile speed

						Projectile.NewProjectile(
							Entity.GetSource_FromThis(),
							Projectile.Center,
							velocity,
							ModContent.ProjectileType<LavaBlob>(),
							(int)(Projectile.damage * 0.5f),
							(int)(Projectile.knockBack * 0.5f),
							Projectile.owner
						);
					}
					
				}
			}

			// If this spin finishes
			if (Timer >= spinDuration)
			{
				SpinCount++;

				if (player.channel)
				{
					Timer = 0; // Start a new spin
				}
				else if (SpinCount >= MinimumSpinCount)
				{
					CurrentStage = AttackStage.Unwind;
				}
			}
		}





		// Function facilitating the latter half of the swing where the sword disappears
		private void UnwindStrike()
		{
			if (CurrentAttack == AttackType.Spin)
			{
				Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) + UNWIND / 2 * Timer / (hideTime * SPINTIME / 2));
				Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (hideTime * SPINTIME / 2));

				if (Timer >= hideTime * SPINTIME / 2)
				{
                    Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, Main.MouseWorld - Main.player[Projectile.owner].Center, ModContent.ProjectileType<MoltenScepterThrown>(), Projectile.damage, Projectile.knockBack, Main.player[Projectile.owner].whoAmI);
					Projectile.Kill();
				}
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Hit, target.Center);
            target.AddBuff(BuffID.OnFire3, 200);
		}
	}
}