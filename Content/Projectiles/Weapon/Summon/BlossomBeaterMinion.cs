using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Verlet;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.SummonItems;
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
using Terraria.DataStructures;
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

        public Line ToPlayer;
        public Line ToMouse;


        public Line ToPlayerInit;


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

        public float ScaleY;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D PT = TextureAssets.Projectile[Type].Value;
            var GT = Projectile.GetGlowTexture("DestroyerTest/Content/Projectiles/Weapon/Summon", "BlossomBeaterMinion");
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

            RenderRope(Main.screenPosition, Projectile.GetAlpha(lightColor));

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawProjectileShadowsRotating(Projectile, Opus.Sine(2f, 5.3f), ColorLib.CursedFlames, 0.06f);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            Main.EntitySpriteDraw(PT, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, PT.Size() / 2, Projectile.scale, FX);
            Main.EntitySpriteDraw(GT.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, GT.Value.Size() / 2, Projectile.scale, FX);
            return false;
        }

        private void RenderRope(Vector2 screenPos, Color drawColor)
        {

            var tex = DTAssetLib.BlossomBeaterRope.Value;


            int segmentCount = Vine.Positions.Length;
            for (var i = 0; i < segmentCount - 1; i++)
            {

                var start = Vine.Positions[i];
                var end = Vine.Positions[i + 1];

                Vector2 VinePos = (start + end) / 2;
                var DrawPos = VinePos - screenPos;

                var style = 0;

                

                if (i == Vine.Positions.Length - 3)
                {
                    style = 0;
                }

                if (i > Vine.Positions.Length - 3)
                {
                    style = 1;
                }

                var frame = tex.Frame(1, 1, style);

                var rotation = start.AngleTo(end);


                var t = 0f;

                if (segmentCount > 1)
                {
                    t = i / (float)(segmentCount - 1); // 0 at base, 1 at tip
                }
   

                // Vertical stretch based on actual distance to next segment and texture height
                var segmentDistance = start.Distance(end);
                var lengthFactor = 1f;
                float denom = Math.Max(1, frame.Height - 5);
                lengthFactor = segmentDistance / denom * 1.2f;

                // Combine into final stretch vector and apply a small global multiplier for visual tuning
                var stretch = new Vector2(lengthFactor, 1f) * 1.2f;
                var Origin = frame.Size() * 0.5f;

                if (i % 2 == 0)
                {
                    continue;
                }

                if (i == segmentCount - 2)
                {
                    stretch = Vector2.One;
                    Origin = new Vector2(frame.Width / 2, 2);
                }

                Vector2 V = Vine.Positions[i];
                Point O = new Point((int)(V.X), (int)(V.Y));
                Vector3 C = Lighting.GetSubLight(V);

                Main.EntitySpriteDraw(tex, DrawPos, frame, new Color(C), rotation, Origin, stretch, 0);
            }

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

        private VerletChain Vine;

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            RandP = player.MountedCenter + new Vector2(IdealDistanceFromPlayerExact, 0);

            ToPlayer = new Line(Projectile.Center, player.MountedCenter);

            Vector2 Handle = Projectile.Center + new Vector2(-8f, -1f).RotatedBy(Projectile.rotation);

            if (Vine == null)
            {
                Vine = new VerletChain(18, 6, Handle);

                Vector2[] pt = ToPlayer.GetPointsAlongLine(18);

                for (int k = 0; k < pt.Length - 1; k++)
                {
                    Vine.Positions[k] = pt[k];
                }
            }
        }

        
        
        public Vector2 RandP;

        float glowAMT = 1f;
        public bool CanFire = false;
        public override void AI()
        {

            Player player = Main.player[Projectile.owner];
            ToPlayer = new Line(Projectile.Center, player.MountedCenter);
            ToMouse = new Line(Projectile.Center, Main.MouseWorld);

            Vector2 Muzzle = Projectile.Center + (new Vector2(Projectile.width / 2, -4).RotatedBy(Projectile.rotation));
            Vector2 Handle = Projectile.Center + new Vector2(-14f, -1f).RotatedBy(Projectile.rotation);


            if (Vine != null)
            {
                Vine.Positions[^1] = player.MountedCenter;
                Vine.Simulate(Vector2.Zero, Handle, 1.5f, 1f);
            }
           
            
            Projectile.ai[1]++;

            if (glowAMT > 0f)
            {
                glowAMT -= 0.07f;
            }

            //CycleLine(ToPlayer);

            

            if (!CheckActive(player))
            {
                return;
            }

            CurrentDistance = Projectile.Center.Distance(player.MountedCenter);

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
                        Projectile.Center = player.MountedCenter;
                        break;
                    }
                case Condition.TooFarFromPlayer:
                    {
                        Vector2 targ1 = player.MountedCenter + new Vector2(IdealDistanceFromPlayerExact, 0).RotatedBy(ToPlayer.GetLineRotation + MathHelper.Pi);
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
                        
                        
                        if (/*Main.GameUpdateCount % 600 == 0*/ Main.rand.NextBool(120))
                        {
                            RandP = player.MountedCenter + new Vector2(IdealDistanceFromPlayerExact, 0).RotatedByRandom(MathHelper.TwoPi);
                            float Rot = (player.MountedCenter - RandP).ToRotation() - ToPlayer.GetLineRotation;
                            Vector2 IdealPos = player.MountedCenter + new Vector2(IdealDistanceFromPlayerExact, 0).RotatedBy(Rot);
                            Vector2 toIdeal = IdealPos - Projectile.Center;
                            toIdeal.Normalize();

                            Projectile.velocity += toIdeal * 10f;
                        }

                        


                        

                        

                        Projectile.velocity *= 0.995f;





                        break;
                    }
                case Condition.TooCloseToPlayer:
                    {
                        Vector2 targ1 = player.MountedCenter + new Vector2(IdealDistanceFromPlayerExact, 0).RotatedBy(ToPlayer.GetLineRotation + MathHelper.Pi);
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

            var FX = new BlossomBeaterFire();

            if (CanFire)
            {
                FX.Initiate(Muzzle, Projectile.rotation + MathHelper.PiOver2, DTColorUtils.Pastel(ColorLib.CursedFlames, 0.3f), 0.15f, 10);
                CanFire = false;
            }
            
            FX.position = Muzzle;
            ParticleEngine.BehindProjectiles.Add(FX);

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
                            glowAMT = 1f;
                            Lighting.AddLight(Muzzle, ColorLib.CursedFlames.ToVector3() * glowAMT);

                            CanFire = true;
                            

                            for (int i = 0; i < 4; i++)
                            {

                                Spark Spark = new Spark();
                                Spark.PrepareSpark(Muzzle, Vel.RotatedByRandom(0.1f), 0f, ColorLib.CursedFlames, 0.25f, false, 30, SparkDrawMode.Additive);
                                ParticleEngine.BehindProjectiles.Add(Spark);
                            }
                            Projectile.velocity += dir * -4f;
                            Projectile bullet = Projectile.NewProjectileDirect(Source, Muzzle, Vel, projToShoot, damage, knockBack, player.whoAmI);
                            bullet.ArmorPenetration = 8;

                            if (Main.rand.NextBool(5))
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

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
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
