using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common.Interfaces
{
    public interface IHomingProjectile
    {
        /// <summary>
        /// Whether or not this projectile tracks NPCs.
        /// </summary>
        public bool TracksNPCs { get; }

        /// <summary>
        /// Whether or not this projectile tracks Players.
        /// </summary>
        public bool TracksPlayers { get; }

        /// <summary>
        /// How many radians the projectile can turn in a tick when tracking a target.
        /// </summary>
        public float HomingTurnSpeed { get; }

        /// <summary>
        /// Whether or not velocity can increase when tracking a target.
        /// </summary>
        public bool UsesHomingAcceleration { get; }

        /// <summary>
        /// If the projectile accelerates while homing (see UsesHomingAcceleration), this is the maximum speed it can reach.
        /// </summary>
        public float HomingMaxAccel { get; }

        /// <summary>
        /// The radius within which the target will be tracked.
        /// </summary>
        public float DetectRadius { get; }

        public bool CanHome { get; }


    }

    public class HomingGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        NPC TrackingNPC;
        Player TrackingPlayer;
        public bool IsValidNPCTarget(NPC target)
        {
            return target.CanBeChasedBy() && !target.friendly && target.active;
        }
        public NPC FindClosestNPC(float maxDetectDistance, Projectile projectile)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            {
                if (IsValidNPCTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidPlayerTarget(Player target)
        {
            return target.active && !target.dead && !target.invis;
        }
        public Player FindClosestPlayer(float maxDetectDistance, Projectile projectile)
        {
            Player closestPlayer = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActivePlayers)
            {
                if (IsValidPlayerTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestPlayer = target;
                    }
                }
            }

            return closestPlayer;
        }

        public override void AI(Projectile projectile)
        {
            if (projectile.ModProjectile is IHomingProjectile homing)
            {
                if (!homing.CanHome)
                    return;

                if (homing.TracksNPCs)
                {
                    if (TrackingNPC == null)
                        TrackingNPC = FindClosestNPC(homing.DetectRadius, projectile);

                    if (TrackingNPC != null && !IsValidNPCTarget(TrackingNPC))
                        TrackingNPC = null;

                    if (TrackingNPC != null)
                    {
                        ApplyHoming(projectile, TrackingNPC.Center, homing);
                    }
                }

                if (homing.TracksPlayers)
                {
                    if (TrackingPlayer == null)
                        TrackingPlayer = FindClosestPlayer(homing.DetectRadius, projectile);

                    if (TrackingPlayer != null && !IsValidPlayerTarget(TrackingPlayer))
                        TrackingPlayer = null;

                    if (TrackingPlayer != null)
                    {
                        ApplyHoming(projectile, TrackingPlayer.Center, homing);
                    }
                }

            }
        }

        void ApplyHoming(Projectile projectile, Vector2 targetCenter, IHomingProjectile homing)
        {
            float length = projectile.velocity.Length();
            float targetAngle = projectile.AngleTo(targetCenter);
            projectile.velocity = projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(homing.HomingTurnSpeed)).ToRotationVector2() * length;
            
            if (homing.UsesHomingAcceleration)
            {
                if (length < homing.HomingMaxAccel)
                {
                    projectile.velocity *= 1.04f;
                }
            }
        }
    }
}
