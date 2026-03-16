using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.SummonItems;
using FargowiltasSouls;
using InnoVault.PRT;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class BlossomBeaterMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.minionSlots = 2f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D PT = TextureAssets.Projectile[Type].Value;
            SpriteEffects FX = SpriteEffects.None;

            float rot = Projectile.rotation;

            if (rot > MathHelper.PiOver2 || rot < -MathHelper.PiOver2)
            {
                FX = SpriteEffects.FlipVertically;
            }
            else
            {
                FX = SpriteEffects.None;
            }

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawProjectileShadowsRotating(Projectile, Opus.Sine(2f, 5.3f), ColorLib.CursedFlames, 0.06f);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            Main.EntitySpriteDraw(PT, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, PT.Size() / 2, Projectile.scale, FX);
            return false;
        }

        public int IdealDistancefromPlayerMin = 140;
        public int IdealDistanceFromPlayerExact = 150;
        public int IdealDistancefromPlayerMax = 160;
        public int DistancefromPlayerToTeleport = 1200;

        public float CurrentDistance = 0f;
        public int Buff = ModContent.BuffType<BlossomBeaterBuff>();

        public enum Condition
        {
            TeleportToPlayer,
            TooFarFromPlayer,
            TooCloseToPlayer,
            SweetSpot,
            Limbo
        }
        public Condition CurrentCondition;

        public Line ToPlayer;
        public Line ToMouse;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            ToPlayer = new Line(Projectile.Center, player.Center);
            ToMouse = new Line(Projectile.Center, Main.MouseWorld);
            Projectile.ai[1]++;

            CycleLine(ToPlayer);

            Vector2 Muzzle = Projectile.Center + (new Vector2(Projectile.width / 2, 10).RotatedBy(Projectile.rotation));

            if (!CheckActive(player))
            {
                return;
            }

            CurrentDistance = Projectile.Center.Distance(player.Center);

            if (CurrentDistance > DistancefromPlayerToTeleport)
            {
                CurrentCondition = Condition.TeleportToPlayer;
            }
            else if (CurrentDistance < DistancefromPlayerToTeleport && CurrentDistance > IdealDistancefromPlayerMax)
            {
                CurrentCondition = Condition.TooFarFromPlayer;
            }
            else if (CurrentDistance < IdealDistancefromPlayerMax && CurrentDistance > IdealDistancefromPlayerMin)
            {
                CurrentCondition = Condition.SweetSpot;
            }
            else if (CurrentDistance < IdealDistancefromPlayerMin)
            {
                CurrentCondition = Condition.TooCloseToPlayer;
            }
            else
            {
                CurrentCondition = Condition.Limbo;
            }

            switch (CurrentCondition)
            {
                case Condition.TeleportToPlayer:
                    {
                        Projectile.Center = player.Center;
                        break;
                    }
                case Condition.TooFarFromPlayer:
                    {
                        Vector2 targ1 = player.Center + new Vector2(IdealDistanceFromPlayerExact, 0).RotatedBy(ToPlayer.GetLineRotation + MathHelper.Pi);
                        Vector2 toTarget = targ1 - Projectile.Center;
                        float dist = toTarget.Length();

                        float maxSpeed = 20f;
                        float slowRadius = 200f;

                        float desiredSpeed = maxSpeed * MathHelper.Clamp(dist / slowRadius, 0f, 1f);

                        Vector2 desiredVelocity = Vector2.Normalize(toTarget) * desiredSpeed;

                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);
                        //Projectile.Center += Projectile.velocity;
                        break;
                    }
                case Condition.SweetSpot:
                    {
                        Projectile.velocity *= 0.995f;
                        if (Main.GameUpdateCount % 120 == 0)
                        {
                            Projectile.velocity += Main.rand.NextVector2Circular(5, 5);
                        }
                        break;
                    }
                case Condition.TooCloseToPlayer:
                    {
                        Vector2 targ1 = player.Center + new Vector2(IdealDistanceFromPlayerExact, 0).RotatedBy(ToPlayer.GetLineRotation + MathHelper.Pi);
                        Vector2 toTarget = targ1 - Projectile.Center;
                        float dist = toTarget.Length();

                        float maxSpeed = 20f;
                        float slowRadius = 200f;

                        float desiredSpeed = maxSpeed * MathHelper.Clamp(dist / slowRadius, 0f, 1f);

                        Vector2 desiredVelocity = Vector2.Normalize(toTarget) * desiredSpeed;

                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);

                        break;
                    }
                case Condition.Limbo:
                    {
                        CurrentCondition = Condition.TeleportToPlayer;
                        break;
                    }
            }


            SearchForTargets(player, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);

            Rotation(foundTarget, targetCenter);
            Spread();

            if (foundTarget)
            {

                if (CheckAmmoForConsumption(player, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item B))
                {
                    var Config = ModContent.GetInstance<DTConfig>();
                    if (Config.MinionAmmoReplace)
                    {
                        projToShoot = ProjectileID.CursedBullet;
                    }
                    else
                    {
                        projToShoot = player.FindAmmoDT(AmmoID.Bullet).shoot;
                    }
                    if (B != null)
                    {
                        var Source = player.GetSource_ItemUse_WithPotentialAmmo(B, usedAmmoItemId, "BlossomBeaterFire");
                        Vector2 dir = targetCenter - Projectile.Center;
                        dir.Normalize();
                        Vector2 Vel = dir * 8;

                        if (Projectile.ai[1] % 85 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item36, Projectile.Center);
                            for (int i = 0; i < 4; i++)
                            {
                                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Muzzle, Vel.RotatedByRandom(0.1f), ColorLib.CursedFlames, 0.25f);
                            }
                            Projectile.velocity += dir * -4f;
                            Projectile bullet = Projectile.NewProjectileDirect(Source, Muzzle, Vel, projToShoot, damage, knockBack, player.whoAmI);
                            bullet.ArmorPenetration = 8;

                            if (Main.rand.NextBool(3))
                            {
                                SoundEngine.PlaySound(DTAssetLib.Impacts.ExplosiveImpactSmall with { MaxInstances = 4, PitchVariance = 0.2f });
                                Projectile petal = Projectile.NewProjectileDirect(Source, Muzzle, Vel * 3f, ModContent.ProjectileType<BlossomBeaterPetal>(), (int)(damage * 2.5f), knockBack, player.whoAmI);
                            }
                        }
                    }
                }
            }
        }

        private bool CheckAmmoForConsumption(Player player, out int projToShoot, out float speed, out int damage, out float knockBack, out int usedAmmoItemId, out Item Beater)
        {
            foreach (Item i in player.inventory)
            {
                if (i.ModItem is BlossomBeater B)
                {
                    Beater = B.Item;
                    if (player.PickAmmo(B.Item, out projToShoot, out speed, out damage, out knockBack, out usedAmmoItemId))
                    {
                        return true;
                    }
                }
            }

            projToShoot = -1;
            speed = 0f;
            damage = 0;
            knockBack = 0f;
            usedAmmoItemId = -1;
            Beater = null;
            
            return false;
        }
        private void Rotation(bool foundTarget, Vector2 targetCenter)
        {
            if (foundTarget)
            {
                float IdealRot = (targetCenter - Projectile.Center).ToRotation(); ;
                float RotDiff = MathF.Atan2(MathF.Sin(IdealRot - Projectile.rotation), MathF.Cos(IdealRot - Projectile.rotation));
                if (Math.Abs(RotDiff) >= 0.1f)
                {
                    float currentRot = Projectile.rotation;

                    float rotDiff = MathF.Atan2(
                        MathF.Sin(IdealRot - currentRot),
                        MathF.Cos(IdealRot - currentRot)
                    );

                    currentRot += rotDiff * 0.1f;

                    Projectile.rotation = MathHelper.WrapAngle(currentRot);
                }
                else
                {
                    Projectile.rotation = IdealRot;
                }
            }
            else
            {
                float IdealRot = ToMouse.GetLineRotation;
                float RotDiff = MathF.Atan2(MathF.Sin(IdealRot - Projectile.rotation), MathF.Cos(IdealRot - Projectile.rotation));
                if (Math.Abs(RotDiff) >= 0.1f)
                {
                    float currentRot = Projectile.rotation;

                    float rotDiff = MathF.Atan2(
                        MathF.Sin(IdealRot - currentRot),
                        MathF.Cos(IdealRot - currentRot)
                    );

                    currentRot += rotDiff * 0.1f;

                    Projectile.rotation = MathHelper.WrapAngle(currentRot);
                }
                else
                {
                    Projectile.rotation = IdealRot;
                }
            }
        }


        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            distanceFromTarget = 1200f;
            targetCenter = Projectile.position;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            Projectile.friendly = foundTarget;
        }

        private void Spread()
        {
            
            foreach (Projectile proj in Main.projectile)
            {
                if (proj == Projectile)
                    continue;

                if (proj.type == Type && proj.active)
                {
                    Vector2 Dir = proj.Center - Projectile.Center;
                    Dir.Normalize();
                    float TooClose = 20f * 20f;
                    if (Projectile.Center.DistanceSQ(proj.Center) < TooClose)
                    {
                        Projectile.velocity += Dir * -1f;
                    }
                    if (Projectile.Center == proj.Center)
                    {
                        Projectile.velocity += Main.rand.NextVector2Circular(5, 5);
                    }
                }
            }
            
        }


        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(Buff);

                return false;
            }

            if (owner.HasBuff(Buff))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private float scroll;

        private void CycleLine(Line line)
        {
            Player player = Main.player[Projectile.owner];

            int C = 3;

            if (player.ownedProjectileCounts[Type] > 4)
            {
                C = 2;
            }
            Vector2[] basePoints = line.GetPointsAlongLine(C);
            int len = basePoints.Length;

            scroll += 0.05f;

            int baseIndex = (int)scroll % len;
            float t = scroll % 1f;

            for (int i = 0; i < len; i++)
            {
                int a = (baseIndex + i) % len;
                int b = (a + 1) % len;

                Vector2 pos = Vector2.Lerp(basePoints[a], basePoints[b], t);

                Dust T = Dust.NewDustPerfect(pos, DustID.CursedTorch, Vector2.Zero, 0, default, 0.8f);
                T.noGravity = true;
            }
        }
    }
}
