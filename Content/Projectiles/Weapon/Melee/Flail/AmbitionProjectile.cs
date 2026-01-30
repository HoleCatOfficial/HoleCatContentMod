using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee.Flail
{
    //See https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Projectiles/ExampleAdvancedFlailProjectile.cs for the base code of this projectile, as all comments were removed.
	public class AmbitionProjectile : ModProjectile
	{
		private const string ChainTexturePath = $"DestroyerTest/Content/Projectiles/Weapon/Melee/Flail/AmbitionChain";

		private static Asset<Texture2D> chainTexture;

		private enum AIState
		{
			Spinning,
			LaunchingForward,
			Retracting,
			UnusedState,
			ForcedRetracting,
			Ricochet,
			Dropping
		}

		private AIState CurrentAIState {
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}
		public ref float StateTimer => ref Projectile.ai[1];
		public ref float CollisionCounter => ref Projectile.localAI[0];
		public ref float SpinningStateTimer => ref Projectile.localAI[1];

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Type] = 6;
			ProjectileID.Sets.TrailingMode[Type] = 2;

			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.netImportant = true;
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
		}

        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 150;
        private void CacheTrail()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

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
        }

        SlotId LoopSlot;
        public SoundStyle Loop = DTAssetLib.FlailSpin with
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };
        
        public int Charge = 0;
		public override void AI() {
			Player player = Main.player[Projectile.owner];

			if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f) {
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner && Main.mapFullscreen) {
				Projectile.Kill();
				return;
			}

            CacheTrail();

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound) && CurrentAIState == AIState.Spinning) {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    soundInstance.Position = Projectile.Center;
                    if (Charge > 120)
                    {
                        if (soundInstance.Pitch < 1)
                        {
                            soundInstance.Pitch += 0.05f;   
                        }
                    }
                    return tracker.IsActiveAndInGame() && CurrentAIState == AIState.Spinning;
                });
                
            }

            //...Except for these, as the comments make it easier for me to come back and make tweaks to the behaviour.
			Vector2 mountedCenter = player.MountedCenter;
			bool doFastThrowDust = false;
			bool shouldOwnerHitCheck = false;
			int launchTimeLimit = 20;  // How much time the projectile can go before retracting (speed and shootTimer will set the flail's range)
			float launchSpeed = 20f; // How fast the projectile can move
			float maxLaunchLength = 1000f; // How far the projectile's chain can stretch before being forced to retract when in launched state
			float retractAcceleration = 3f; // How quickly the projectile will accelerate back towards the player while retracting
			float maxRetractSpeed = 30f; // The max speed the projectile will have while retracting
			float forcedRetractAcceleration = 6f; // How quickly the projectile will accelerate back towards the player while being forced to retract
			float maxForcedRetractSpeed = 30f; // The max speed the projectile will have while being forced to retract
			float unusedRetractAcceleration = 1f;
			float unusedMaxRetractSpeed = 14f;
			int unusedChainLength = 60;
			int defaultHitCooldown = 10; // How often your flail hits when resting on the ground, or retracting
			int spinHitCooldown = 20; // How often your flail hits when spinning
			int movingHitCooldown = 10; // How often your flail hits when moving
			int ricochetTimeLimit = launchTimeLimit + 5;

			// Scaling these speeds and accelerations by the players melee speed makes the weapon more responsive if the player boosts it or general weapon speed
			float meleeSpeedMultiplier = player.GetTotalAttackSpeed(DamageClass.Melee);
			launchSpeed *= meleeSpeedMultiplier;
			unusedRetractAcceleration *= meleeSpeedMultiplier;
			unusedMaxRetractSpeed *= meleeSpeedMultiplier;
			retractAcceleration *= meleeSpeedMultiplier;
			maxRetractSpeed *= meleeSpeedMultiplier;
			forcedRetractAcceleration *= meleeSpeedMultiplier;
			maxForcedRetractSpeed *= meleeSpeedMultiplier;
			float launchRange = launchSpeed * launchTimeLimit;
			float maxDroppedRange = launchRange + 160f;
			Projectile.localNPCHitCooldown = defaultHitCooldown;

			switch (CurrentAIState) {
				case AIState.Spinning: {
						shouldOwnerHitCheck = true;
						if (Projectile.owner == Main.myPlayer) {
							Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
							player.ChangeDir((unitVectorTowardsMouse.X > 0f).ToDirectionInt());
                            Charge++;
							if (!player.channel)
							{
								CurrentAIState = AIState.LaunchingForward;
								StateTimer = 0f;
								Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
								Projectile.Center = mountedCenter;
								Projectile.netUpdate = true;
								Projectile.ResetLocalNPCHitImmunity();
								Projectile.localNPCHitCooldown = movingHitCooldown;
                                SoundEngine.PlaySound(DTAssetLib.FlailThrow, Projectile.Center);
								break;
							}
						}
						SpinningStateTimer += 1f;
						//10f controls how fast the projectile visually spins around the player
						Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

						offsetFromPlayer.Y *= 0.8f;
						if (offsetFromPlayer.Y * player.gravDir > 0f) {
							offsetFromPlayer.Y *= 0.5f;
						}
						Projectile.Center = mountedCenter + offsetFromPlayer * 30f + new Vector2(0, player.gfxOffY);
						Projectile.velocity = Vector2.Zero;
						Projectile.localNPCHitCooldown = spinHitCooldown;
						break;
					}
				case AIState.LaunchingForward: {
						doFastThrowDust = true;
						bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
						shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
						if (player.controlUseItem)
						{
							CurrentAIState = AIState.Dropping;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
							
							
							break;
						}
						if (shouldSwitchToRetracting) {
							CurrentAIState = AIState.Retracting;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.3f;
                            int amt = 3;
                            if (Charge > 120)
                            {
                                amt = 7;
                                DTUtils.GenericSparkleEffect(Projectile.Center);
                                Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, Color.White * 0.5f, 0.01f, 0.1f);
                                SoundEngine.PlaySound(DTAssetLib.Impacts.AmbitionChargeBurst, Projectile.Center);
                            }
                            
                            for (int p = 0; p < amt; p++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.4f), ModContent.ProjectileType<AmbitionSpark>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            }
                        }
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						Projectile.localNPCHitCooldown = movingHitCooldown;
						break;
					}
				case AIState.Retracting: {
						Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= maxRetractSpeed) {
							Projectile.Kill(); // Kill the projectile once it is close enough to the player
							return;
						}
						if (player.controlUseItem) // If the player clicks, transition to the Dropping state
						{
							CurrentAIState = AIState.Dropping;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
						}
						else {
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
							player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						}
						break;
					}
				// Projectile.ai[0] == 3; This case is actually unused, but maybe a Terraria update will add it back in, or maybe it is useless, so I left it here.
				case AIState.UnusedState: {
						if (!player.controlUseItem) {
							CurrentAIState = AIState.ForcedRetracting; // Move to super retracting mode if the player taps
							StateTimer = 0f;
							Projectile.netUpdate = true;
							break;
						}
						float currentChainLength = Projectile.Distance(mountedCenter);
						Projectile.tileCollide = StateTimer == 1f;
						bool flag3 = currentChainLength <= launchRange;
						if (flag3 != Projectile.tileCollide) {
							Projectile.tileCollide = flag3;
							StateTimer = Projectile.tileCollide ? 1 : 0;
							Projectile.netUpdate = true;
						}
						if (currentChainLength > unusedChainLength) {

							if (currentChainLength >= launchRange) {
								Projectile.velocity *= 0.5f;
								Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedMaxRetractSpeed);
							}
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedRetractAcceleration);
						}
						else {
							if (Projectile.velocity.Length() < 6f) {
								Projectile.velocity.X *= 0.96f;
								Projectile.velocity.Y += 0.2f;
							}
							if (player.velocity.X == 0f) {
								Projectile.velocity.X *= 0.96f;
							}
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						break;
					}
				case AIState.ForcedRetracting: {
						Projectile.tileCollide = false;
						Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed) {
							Projectile.Kill(); // Kill the projectile once it is close enough to the player
							return;
						}
						Projectile.velocity *= 0.98f;
						Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
						Vector2 target = Projectile.Center + Projectile.velocity;
						Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
						if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f) {
							Projectile.Kill(); // Kill projectile if it will pass the player
							return;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						break;
					}
				case AIState.Ricochet:
					if (StateTimer++ >= ricochetTimeLimit) {
						CurrentAIState = AIState.Dropping;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else {
						Projectile.localNPCHitCooldown = movingHitCooldown;
						Projectile.velocity.Y += 0.6f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					}
					break;
				case AIState.Dropping:
					if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange) {
						CurrentAIState = AIState.ForcedRetracting;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else {
						Projectile.velocity.Y += 0.8f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					}
					break;
			}

			// This is where Flower Pow launches projectiles. Decompile Terraria to view that code.

			Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
			Projectile.spriteDirection = Projectile.direction;
			Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary.

			// This rotation code is unique to this flail, since the sprite isn't rotationally symmetric and has tip.
            /*
			bool freeRotation = CurrentAIState == AIState.Ricochet || CurrentAIState == AIState.Dropping;
			if (freeRotation) {
				if (Projectile.velocity.Length() > 1f)
					Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
				else
					Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			}
			else {
				Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
				Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;
			}
            */

			
			if (Projectile.velocity.Length() > 1f)
            {
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
            }
			else
            {
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
            }
			

			Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2); // Add a delay so the player can't button mash the flail
			player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
			if (Projectile.Center.X < mountedCenter.X) {
				player.itemRotation += (float)Math.PI;
			}
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

			// Spawning dust. We spawn dust more often when in the LaunchingForward state
			int dustRate = 7;
			if (doFastThrowDust)
            {
				dustRate = 1;
            }

			if (Main.rand.NextBool(dustRate))
            {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ColorableNeonDust>(), 0f, 0f, 150, Color.White, 2f);
		    }

            if (Charge > 120)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ColorableNeonDust>(), 0f, 0f, 150, Color.HotPink, 2.6f);
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity) {
			int defaultLocalNPCHitCooldown = 10;
			int impactIntensity = 0;
			Vector2 velocity = Projectile.velocity;
			float bounceFactor = 0.2f;
			if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet) {
				bounceFactor = 0.4f;
			}

			if (CurrentAIState == AIState.Dropping) {
				bounceFactor = 0f;
			}

			if (oldVelocity.X != Projectile.velocity.X) {
				if (Math.Abs(oldVelocity.X) > 4f) {
					impactIntensity = 1;
				}

				Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
				CollisionCounter += 1f;
			}

			if (oldVelocity.Y != Projectile.velocity.Y) {
				if (Math.Abs(oldVelocity.Y) > 4f) {
					impactIntensity = 1;
				}

				Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
				CollisionCounter += 1f;
			}

			// If in the Launched state, spawn sparks
			if (CurrentAIState == AIState.LaunchingForward) {
				CurrentAIState = AIState.Ricochet;
				Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
				Projectile.netUpdate = true;
				Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
				Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
				impactIntensity = 2;
				Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
				Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
				Projectile.position -= velocity;
			}

			// Here the tiles spawn dust indicating they've been hit
			if (impactIntensity > 0) {
				Projectile.netUpdate = true;
				for (int i = 0; i < impactIntensity; i++) {
					Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
				}

				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}

			// Force retraction if stuck on tiles while retracting
			if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f) {
				CurrentAIState = AIState.ForcedRetracting;
				Projectile.netUpdate = true;
			}

			// tModLoader currently does not provide the wetVelocity parameter, this code should make the flail bounce back faster when colliding with tiles underwater.
			//if (Projectile.wet)
			//	wetVelocity = Projectile.velocity;

			return false;
		}

		public override bool? CanDamage() {
			// Flails in spin mode won't damage enemies within the first 12 ticks. Visually this delays the first hit until the player swings the flail around for a full spin before damaging anything.
			if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f) {
				return false;
			}
			return base.CanDamage();
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
			// Flails do special collision logic that serves to hit anything within an ellipse centered on the player when the flail is spinning around the player. For example, the projectile rotating around the player won't actually hit a bee if it is directly on the player usually, but this code ensures that the bee is hit. This code makes hitting enemies while spinning more consistent and not reliant of the actual position of the flail projectile.
			if (CurrentAIState == AIState.Spinning) {
				Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
				Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
				shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
				float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
				return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
			}
			// Regular collision logic happens otherwise.
			return base.Colliding(projHitbox, targetHitbox);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) 
        {
			// Flails do a few custom things, you'll want to keep these to have the same feel as vanilla flails.

			// Flails do 20% more damage while spinning
			if (CurrentAIState == AIState.Spinning) 
            {
				modifiers.SourceDamage *= 1.2f;
			}
			// Flails do 100% more damage while launched or retracting. This is the damage the item tooltip for flails aim to match, as this is the most common mode of attack. This is why the item has ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
			else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting) 
            {
				modifiers.SourceDamage *= 2f;
			}

			// The hitDirection is always set to hit away from the player, even if the flail damages the npc while returning
			modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

			// Knockback is only 25% as powerful when in spin mode
			if (CurrentAIState == AIState.Spinning) 
            {
				modifiers.Knockback *= 0.25f;
			}
			// Knockback is only 50% as powerful when in drop down mode
			else if (CurrentAIState == AIState.Dropping) 
            {
				modifiers.Knockback *= 0.5f;
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Opus.RadialDustRandomDir(ModContent.DustType<ColorableNeonDust>(), 10, Projectile.Center, 0, Color.White, 2f, 2f);
            if (Charge > 120)
            {
                damageDone = (int)(damageDone * 1.75f);
                Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), target.Center, Vector2.Zero, Color.White, 0.01f, 0.25f);
                BasePRT crack = PRTLoader.NewParticle(PRTLoader.GetParticleID<ImpactCracks>(), target.Center, Vector2.Zero, Color.HotPink, 1f);
                crack.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Opus.RadialDustRandomDir(ModContent.DustType<ColorableNeonDust>(), 5, Projectile.Center, 0, Color.HotPink, 2f, 3f);
                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Hit, Projectile.Center);
            }
        }

		// PreDraw is used to draw a chain and trail before the projectile is drawn normally.
		public override bool PreDraw(ref Color lightColor) 
        {
			Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

			// This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
			playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

			Rectangle? chainSourceRectangle = null;
			// Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
			float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap.

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
			if (chainSegmentLength == 0) 
            {
				chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
			}
			float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

			// This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
			while (chainLengthRemainingToDraw > 0f) 
            {
				// This code gets the lighting at the current tile coordinates
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				// Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
				// Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
				// This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

				

				// Here, we draw the chain texture at the coordinates
				Main.spriteBatch.Draw(chainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				// chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
				chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			// Add a motion trail when moving forward, like most flails do (don't add trail if already hit a tile)
			if (CurrentAIState == AIState.LaunchingForward) 
            {
				Texture2D projectileTexture = TextureAssets.Projectile[Type].Value;
				Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
				SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
				int afterimageCount = Math.Min(Projectile.oldPos.Length - 1, (int)StateTimer);
				for (int k = afterimageCount; k > 0; k--) {
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
					Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
				}
			}

            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float t = 1f - (i / (float)TrailPositions.Count); // fade toward tail
					Color b = lightColor * t;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 offset = dir.RotatedBy(MathHelper.ToRadians(90)) * 4;
                    Vector2 offset2 = dir.RotatedBy(MathHelper.ToRadians(-90)) * 4;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, t, b, 0);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
                    gd.Textures[0] = DTAssetLib.Square.Value;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2); 
				}
			}

			Opus.ReturnToDefaultDrawing(spriteBatch);
            
			return true;
		}
	}
}