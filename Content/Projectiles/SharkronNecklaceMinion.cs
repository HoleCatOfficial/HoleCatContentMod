using System;
using System.Linq;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace DestroyerTest.Content.Projectiles
{
    public class SharkronNecklaceMinion : ModProjectile
    {
        private NPC target {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 18000;
        }

        public enum State
        {
            OrbitPlayer,
            FlyToNPC,
        };

        public State currentState = State.OrbitPlayer;
        private const float OrbitRadius = 80f;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float maxDetectRadius = 400f;

            if (target == null || !IsValidTarget(target))
                target = FindClosestNPC(maxDetectRadius);

            switch (currentState)
            {
                case State.OrbitPlayer:
                    if (target != null && IsValidTarget(target))
                    {
                        currentState = State.FlyToNPC;
                    }
                    else
                    {
                        IdleAI(player);
                    }
                    break;

                case State.FlyToNPC:
                    if (target == null || !IsValidTarget(target))
                    {
                        currentState = State.OrbitPlayer;
                    }
                    else
                    {
                        GoToTarget(target);
                    }
                    break;
            }
        }


        public void IdleAI(Player player)
        {
            Projectile.frame = 0;
            // Shared angular reference for all minions (keeps them synchronized)
            float baseAngle = (Main.GameUpdateCount * 0.03f) % MathHelper.TwoPi;

            // Determine this minion's angular offset among siblings
            var sameType = Main.projectile
                .Where(p => p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                .OrderBy(p => p.identity)
                .ToList();

            int index = sameType.IndexOf(Projectile);
            int total = sameType.Count;
            float spacing = MathHelper.TwoPi / Math.Max(1, total);

            float angle = baseAngle + index * spacing;
            Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * OrbitRadius;


            Vector2 desiredPosition = player.Center + offset;
            Projectile.velocity = (desiredPosition - Projectile.Center) * 0.1f;

            // Face the direction of orbit travel
            Projectile.rotation = Projectile.velocity.ToRotation();

            
        }


        public void GoToTarget(NPC target)
        {
            Projectile.frame = 1;
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(target.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(9)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public NPC FindClosestNPC(float maxDetectDistance) {
				NPC closestNPC = null;

				// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
				float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

				// Loop through all NPCs
				foreach (var target in Main.ActiveNPCs) {
					// Check if NPC able to be targeted. 
					if (IsValidTarget(target)) {
						// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
						float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

						// Check if it is within the radius
						if (sqrDistanceToTarget < sqrMaxDetectDistance) {
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
            
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 150, default(Color), 1.5f);
                Main.dust[dustIndex].velocity *= 1.4f;
            }
            int Gore1 = Mod.Find<ModGore>("SharkronGore1").Type;
            int Gore2 = Mod.Find<ModGore>("SharkronGore2").Type;
            int Gore3 = Mod.Find<ModGore>("SharkronGore3").Type;

            var entitySource = Projectile.GetSource_Death();
            Gore.NewGore(entitySource, Projectile.position, new Vector2(Projectile.velocity.X + Main.rand.Next(-2, 2), Main.rand.Next(-5, 0)), Gore1);
            Gore.NewGore(entitySource, Projectile.position, new Vector2(Projectile.velocity.X + Main.rand.Next(-2, 2), Main.rand.Next(-5, 0)), Gore2);
            Gore.NewGore(entitySource, Projectile.position, new Vector2(Projectile.velocity.X + Main.rand.Next(-2, 2), Main.rand.Next(-5, 0)), Gore3);
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BloodBlobKill") with {MaxInstances = 0}, Projectile.position);
        }
    }
}
