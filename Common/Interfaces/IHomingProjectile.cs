using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
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
        /// The amount that the projectile velocity is multiplied by when accelerating.
        /// </summary>
        public float HomingAccelAmount { get; }

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

    /*
    __/\\\________/\\\______/\\\_        
     _\/\\\_____/\\\//___/\\\\\\\_       
      _\/\\\__/\\\//_____\/////\\\_      
       _\/\\\\\\//\\\_________\/\\\_     
        _\/\\\//_\//\\\________\/\\\_    
         _\/\\\____\//\\\_______\/\\\_   
          _\/\\\_____\//\\\______\/\\\_  
           _\/\\\______\//\\\_____\/\\\_ 
            _\///________\///______\///_  
    https://symboldb.org/font-generator/big/
    */
    public class HomingGlobal : GlobalProjectile
    {
        //There is very rarely a time where you don't want to set this to true.
        public override bool InstancePerEntity => true;

        //Most of the code for finding and following a target is lifted from ExampleHomingProjectile
        //https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Projectiles/ExampleHomingProjectile.cs
        //You will find far more verbose and helpful documentation there.

        //Some liberties were taken of course.

        public NPC TrackingNPC;
        public Player TrackingPlayer;
        public bool IsValidNPCTarget(NPC target)
        {
            //Can only chase generally chaseable enemies that:
            //Are Hostile
            //Are Active in the game world
            //Aren't town NPCs
            //Aren't projectiles, such as Vile Spit and Prophecy's End.
            return target.CanBeChasedBy() && !target.friendly && target.active && !target.isLikeATownNPC && !NPCID.Sets.ProjectileNPC[target.type];
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
            //Can only chase players that:
            //Are Active in the game world
            //Are alive
            //Are not invisible (one of the liberties I took)
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
            //This is a special kind of cast. A very useful way of accessing an inherited item.
            //IHomingProjectile is only meant to be used with ModProjectiles for this reason.

            //IHomingProjectile, when applied to a ModProjectile, makes it count as one, as is the case with most interfaces.
            //See Quixotism's Swing Projectile for a more common application of this kind of cast. https://github.com/HoleCatOfficial/HoleCatContentMod/blob/main/Content/Projectiles/Weapon/Melee/Quixotism/QuixotismSwing.cs
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
            //AngleTo is similar to DirectionTo, but returns the float angle instead of the vector pointing to the destination.
            float targetAngle = projectile.AngleTo(targetCenter);
            //The velocity calculation works as follows:
            //The current angle of travel for the projectile is accessed (velocity.ToRotation())
            //It is then added or subtracted from in order to steer the projectile towards its target (AngleTowards(float targetAngle, float maxChange))
            //The updated steered angle for this tick is then converted back into a Vector value (ToRotationVector2())
            //But theres one step left to do. The resulting vector of ToRotationVector2 is normalized to the lowest stable value. It has to be set back to speed (* length).
            projectile.velocity = projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(homing.HomingTurnSpeed)).ToRotationVector2() * length;

            //If accelleration is allowed, accelerate after.
            if (homing.UsesHomingAcceleration)
            {
                if (length < homing.HomingMaxAccel)
                {
                    projectile.velocity *= homing.HomingAccelAmount;
                }
            }
        }
    }
}
