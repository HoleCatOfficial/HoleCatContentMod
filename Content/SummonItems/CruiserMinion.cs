
﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;

namespace DestroyerTest.Content.SummonItems
{
	// This Example show how to implement simple homing projectile
	// Can be tested with ExampleCustomAmmoGun
	public class CruiserMinionHead : ModProjectile
	{
		// Store the target NPC using Projectile.ai[0]
		private NPC HomingTarget
		{
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set
			{
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public override void SetDefaults()
		{
			Projectile.width = 42; // The width of projectile hitbox
			Projectile.height = 40; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Summon; // What type of damage does this projectile affect?
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.localNPCHitCooldown = 10;
			Projectile.penetrate = -1;
			Projectile.minion = true;
			Projectile.minionSlots = 1f;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}

		// Custom AI

		private int SegmentCount = 9;

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target

			// First, we find a homing target if we don't have one
			if (HomingTarget == null)
			{
				HomingTarget = FindClosestNPC(maxDetectRadius);
			}

			// If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
			if (HomingTarget != null && !IsValidTarget(HomingTarget))
			{
				HomingTarget = null;
			}

			// If we don't have a target, don't adjust trajectory
			if (HomingTarget == null)
				return;

			// If found, we rotate the projectile velocity in the direction of the target.
			// We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
			float length = Projectile.velocity.Length();
			float targetAngle = Projectile.AngleTo(HomingTarget.Center);
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(30)).ToRotationVector2() * length;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			// Create a list to hold references to the body segments
			List<Projectile> bodySegments = new List<Projectile>();

			for (int seg = 0; seg < SegmentCount; seg++)
			{
				Projectile body = Projectile.NewProjectileDirect(
					Entity.GetSource_FromThis(),
					Projectile.oldPos[3],
					Projectile.velocity,
					ModContent.ProjectileType<CruiserMinionBody>(),
					Projectile.damage,
					Projectile.knockBack
				);
				bodySegments.Add(body);
			}

			// Spawn the tail from the last body segment, if any
			if (bodySegments.Count > 0)
			{
				Projectile lastBody = bodySegments[bodySegments.Count - 1];
				Projectile.NewProjectileDirect(
					Entity.GetSource_FromThis(),
					lastBody.oldPos[3],
					lastBody.velocity,
					ModContent.ProjectileType<CruiserMinionTail>(),
					Projectile.damage,
					Projectile.knockBack
				);
			}

			if (HomingTarget == null)
			{
				float radius = 50f; // Distance from center
				float speed = 0.05f; // Rotation speed (radians per tick)
				float angle = Main.GameUpdateCount * speed; // Angle increases over time

				Vector2 center = Projectile.Center; // The point to orbit around
				Vector2 offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
				Projectile.position = center + offset - new Vector2(Projectile.width / 2, Projectile.height / 2);
			}
		}

		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null
		public NPC FindClosestNPC(float maxDetectDistance)
		{
			NPC closestNPC = null;

			// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			// Loop through all NPCs
			foreach (var target in Main.ActiveNPCs)
			{
				// Check if NPC able to be targeted. 
				if (IsValidTarget(target))
				{
					// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					// Check if it is within the radius
					if (sqrDistanceToTarget < sqrMaxDetectDistance)
					{
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			return closestNPC;
		}

		public bool IsValidTarget(NPC target)
		{
			// This method checks that the NPC is:
			// 1. active (alive)
			// 2. chaseable (e.g. not a cultist archer)
			// 3. max life bigger than 5 (e.g. not a critter)
			// 4. can take damage (e.g. moonlord core after all it's parts are downed)
			// 5. hostile (!friendly)
			// 6. not immortal (e.g. not a target dummy)
			// 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
			return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.ShadowFlame, 60);
		}
	}

        internal class CruiserMinionBody : ModProjectile
		{
		public override void SetDefaults()
		{
			Projectile.width = 44; // The width of projectile hitbox
			Projectile.height = 32; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Summon; // What type of damage does this projectile affect?
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.localNPCHitCooldown = 10;
			Projectile.penetrate = -1;
			Projectile.minion = true;
			Projectile.minionSlots = 1f;
			Projectile.tileCollide = false;

			}

			public override void AI()
			{
				Player owner = Main.player[Projectile.owner];
				if (!owner.active || owner.dead || !owner.HasBuff(ModContent.BuffType<NihilistBuff>()))
				{
					Projectile.Kill();
					return;
				}
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;


			}

		}
    internal class CruiserMinionTail : ModProjectile
    {
		public override void SetDefaults()
		{
			Projectile.width = 24; // The width of projectile hitbox
			Projectile.height = 38; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Summon; // What type of damage does this projectile affect?
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.localNPCHitCooldown = 10;
			Projectile.penetrate = -1;
			Projectile.minion = true;
			Projectile.minionSlots = 1f;
			Projectile.tileCollide = false;

        }

		public override void AI() {
			Player owner = Main.player[Projectile.owner];
			if (!owner.active || owner.dead || !owner.HasBuff(ModContent.BuffType<NihilistBuff>())) {
				Projectile.Kill();
				return;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			
		}

    }
}
