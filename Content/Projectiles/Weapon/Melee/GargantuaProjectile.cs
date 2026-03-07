
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
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
	// ExampleCustomSwingSword is an example of a sword with a custom swing using a held projectile
	// This is great if you want to make melee weapons with complex swing behavior
	// Note that this projectile only covers 2 relatively simple swings, everything else is up to you
	// Aside from the custom animation, the custom collision code in Colliding is very important to this weapon
	public class GargantuaProjectile : ModProjectile
	{
        /*
		public SoundStyle Swing = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/HeavySwing", 3) with { Volume = 1.0f, PitchVariance = 0.2f, MaxInstances = 0 };
		public SoundStyle Hit = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/DreamHit", 3) with { PitchVariance = 0.4f, MaxInstances = 0 };
		// We define some constants that determine the swing range of the sword
		// Not that we use multipliers here since that simplifies the amount of tweaks for these interactions
		// You could change the values or even replace them entirely, but they are tweaked with looks in mind
		private const float SWINGRANGE = 1.67f * (float)Math.PI; // The angle a swing attack covers (300 deg)
		private const float SPINRANGE = 4.5f * (float)Math.PI; // The angle a spin attack covers (630 degrees)
		private const float WINDUP = 0.15f; // How far back the player's hand goes when winding their attack (in relation to swingRange)
		private const float UNWIND = 0.4f; // When should the sword start disappearing
		private const float SPINTIME = 2.0f; // How much longer a spin is than a swing

		private enum AttackType // Which attack is being performed
		{
			// Spins are swings that go full circle
			// They are slower and deal more knockback
			Spin,
		}

		private enum AttackStage // What stage of the attack is being executed, see functions found in AI for description
		{
			Prepare,
			Execute,
			Unwind
		}

		// These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
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
				Timer = 0; // reset the timer when the projectile switches states
			}
		}

		// Variables to keep track of during runtime
		private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
		private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
		private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle
		private ref float Size => ref Projectile.localAI[2]; // Size of sword

		// We define timing functions for each stage, taking into account melee attack speed
		// Note that you can change this to suit the need of your projectile
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
			Projectile.width = 122; // Hitbox width of projectile
			Projectile.height = 122; // Hitbox height of projectile
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

		public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
		private const int TrailLength = 40;
		public Vector2 swordTip;
		public Vector2 ToTip;
		public override void AI()
		{
			swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			ToTip = swordTip - Projectile.Center;


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

			Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : swordTip;
			Vector2 newPos  = swordTip;

			Vector2 TR_Dir = newPos - lastPos;
			float TR_Rot = TR_Dir.ToRotation();

			

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 8f;

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);

					TrailPositions.Insert(0, pos);
					TrailRotations.Insert(0, TR_Rot);
				}
			}
			else
			{
				TrailPositions.Insert(0, newPos);
				TrailRotations.Insert(0, TR_Rot);
			}

			while (TrailPositions.Count > TrailLength)
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			while (TrailRotations.Count > TrailLength)
				TrailRotations.RemoveAt(TrailRotations.Count - 1);

			Timer++;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Trail();
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

		public void Trail()
		{
			Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
					Color b = Color.Red * t;

					//Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					float rot = TrailRotations[i];
					Vector2 dir = rot.ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 150;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 1;

					ve.Add(new ColoredVertex(
						TrailPositions[i] - Main.screenPosition + offset,
						new Vector3(t, 1, 1),
						b));

					ve.Add(new ColoredVertex(
						TrailPositions[i] - Main.screenPosition + offset2,
						new Vector3(t, 0, 1),
						b));
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
					gd.Textures[0] = DTAssetLib.SwordSlash.Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
				}
			}
			Opus.ReturnToDefaultDrawing(Main.spriteBatch);
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
			Player player = Main.player[Projectile.owner];
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
		public bool Sound = false;


		// Function facilitating the first half of the swing
		private void ExecuteStrike()
		{
			Player player = Main.player[Projectile.owner];

			if (!Sound)
            {
				SoundEngine.PlaySound(Swing, player.Center);
				Sound = true;
            }

			
			Vector2 swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Vector2 sword1 = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() * Projectile.scale) - 8);
			Vector2 sword2 = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() * Projectile.scale) - 32);
			Vector2 sword3 = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() * Projectile.scale) - 64);
			Vector2 sword4 = Projectile.Center + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length() * Projectile.scale) - 84);

			

			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], swordTip, Vector2.Zero, new Color(255, 0, 0, 0.5f), 3.0f, 40, ai2: 1);
			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], sword1, Vector2.Zero, new Color(255, 0, 0, 0.5f) * 0.8f, 2.5f, 40, ai2: 1);
			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], sword2, Vector2.Zero, new Color(255, 0, 0,0.5f) * 0.6f, 2f, 40, ai2: 1);
			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], sword3, Vector2.Zero, new Color(255, 0, 0, 0.5f) * 0.4f, 1.5f, 40, ai2: 1);
			PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], sword4, Vector2.Zero, new Color(255, 0, 0, 0.5f) * 0.2f, 1f, 40, ai2: 1);
			
			

			//PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, ToTip * 0.05f, Color.Red, 3.0f, 40, ai2: 1);

			int rad = (int)(Projectile.Size.Length() * Projectile.scale);


			if (CurrentAttack != AttackType.Spin)
				return;

			float spinDuration = execTime * SPINTIME;

			// Update spin progress
			Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) * Timer / spinDuration);

			// Sound + immunity refresh
			if (Timer == (int)(spinDuration * 3 / 4))
			{
				
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
							ModContent.ProjectileType<GargantuaStar>(),
							(int)(Projectile.damage * 0.5f),
							(int)(Projectile.knockBack * 0.5f),
							Projectile.owner
						);
					}
					
				}

				if (SpinCount >= 15)
				{
					for (int i = 0; i < 4; i++)
					{
						Vector2 Direction = Main.rand.NextVector2CircularEdge(1f, 1f); // Random unit vector on circle edge
						Vector2 velocity = Direction * 36f; // 6f = desired projectile speed

						Projectile.NewProjectile(
							Entity.GetSource_FromThis(),
							Projectile.Center,
							velocity,
							ModContent.ProjectileType<GoliathPhantom>(),
							(int)(Projectile.damage * 0.75f),
							(int)(Projectile.knockBack * 0.5f),
							Projectile.owner
						);
					}
				}

				if (SpinCount >= 30)
				{
					if (player.HasBuff(ModContent.BuffType<GargantuaBoost>()) == false)
					{
						player.AddBuff(ModContent.BuffType<GargantuaBoost>(), 5 * 60);
					}
				}


			}

			// If this spin finishes
			if (Timer >= spinDuration)
			{
				SpinCount++;
				Sound = false;

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

		private void UnwindStrike()
		{
			if (CurrentAttack == AttackType.Spin)
			{
				Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) + UNWIND / 2 * Timer / (hideTime * SPINTIME / 2));
				Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (hideTime * SPINTIME / 2));

				if (Timer >= hideTime * SPINTIME / 2)
				{
					Projectile.Kill();
				}
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			SoundEngine.PlaySound(Hit, target.Center);
			for (int i = 0; i < 10; i++)
			{
				Vector2 ToTarget = Projectile.Center - target.Center;
				Vector2 Dir = ToTarget.ToRotation().ToRotationVector2() * -16;
				PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), target.Center, Dir.RotatedByRandom(1), Color.Red, 1f, 1);
			}
		}
		*/

        
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
            Projectile.height = 122; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 10000; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
            Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
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
            int splatterdir = target.position.X > Owner.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), target.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), Color.Red * Main.rand.NextFloat(0.5f, 1f), 1f);
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
					Opus.RadialProjectileRandomDir(ModContent.ProjectileType<GargantuaStar>(), 4, Projectile.Center, (int)(Projectile.damage * 0.5f), (int)(Projectile.knockBack * 0.5f), 24f, friendly: true);
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