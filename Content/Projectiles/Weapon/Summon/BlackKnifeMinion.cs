using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Verlet;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.SummonItems;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class BlackKnifeMinion : ModProjectile
    {
        public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/BlackKnifeTP") with { };
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 60;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (Projectile.ai[1] == 2 && (Projectile.ai[2] > 30 && Projectile.ai[2] < 40))
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    float fade = MathHelper.Lerp(0.5f, 0f, (float)i / (float)Projectile.oldPos.Length);

                    if (i != 0)
                    {
                        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.OldCenter()[i] - Main.screenPosition, null, ColorLib.TenebrisGradient with { A = 0 } * Projectile.Opacity * fade, Projectile.oldRot[i] - (MathHelper.PiOver4 + (Projectile.direction == 1 ? 0 : MathHelper.PiOver2)), TextureAssets.Projectile[Type].Value.Size() / 2, Projectile.scale, Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                    }
                }
            }



            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, ColorLib.TenebrisGradient));
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;

        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] != 0 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact with { Volume = 1.2f }, target.Center);
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/DamascusSlash") { Pitch = 1.5f, PitchVariance = 0.2f, Volume = 1.2f }, target.Center);


            for (int i = 0; i < 5; i++)
            {
                Spark Trail = new();
                Trail.PrepareSpark(target.Center, Projectile.velocity.RotatedByRandom(0.1f) * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.TenebrisGradient, 0.4f, false, 20, SparkDrawMode.Additive, 6f);
                ParticleEngine.BehindProjectiles.Add(Trail);
            }

            if (Projectile.ai[1] == 2 && (Projectile.ai[2] > 30 && Projectile.ai[2] < 40))
            {
                for (int i = 0; i < 4; i++)
                {
                    Spark Trail = new();
                    Trail.PrepareSpark(target.Center, Projectile.velocity.RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2).RotatedByRandom(0.1f) * 0.25f, Projectile.velocity.ToRotation(), ColorLib.TenebrisGradient, 0.3f, false, 20, SparkDrawMode.Additive, 5f);
                    ParticleEngine.BehindProjectiles.Add(Trail);
                }
            }

        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3());

            if (Owner.dead || !Owner.active)
            {
                Owner.ClearBuff(ModContent.BuffType<BlackKnifeBuff>());
            }

            if (Owner.HasBuff(ModContent.BuffType<BlackKnifeBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            int targetindex = Projectile.AutoTarget();

            if (targetindex == -1)
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1] = 0;
                Idle();
                return;
            }

            NPC Target = Main.npc[targetindex];
            Projectile.ai[0] = 1;
            Attack(Target);
        }

        public void Idle()
        {


            BlackKnifeOrganizer organizer = Owner.GetModPlayer<BlackKnifeOrganizer>();
            List<Projectile> siblings = organizer.Siblings;

            int myIndex = siblings.IndexOf(Projectile);

            if (myIndex < 0)
                return;

            float phase = myIndex * 1.1f;
            float bob = MathF.Sin(Main.GlobalTimeWrappedHourly * 2f + phase) * 5f;

            Projectile.rotation = -MathHelper.PiOver4 + (-0.1f * myIndex * Owner.direction);

            Vector2 targetPosition = Owner.MountedCenter + new Vector2((-10 * Owner.direction) + (-10 * myIndex * Owner.direction), bob);

            if (Projectile.Distance(targetPosition) < 2000)
            {
                Projectile.SmoothMoveToPoint(targetPosition, 30f, 90);
            }
            else
            {
                Projectile.Center = targetPosition;
            }


        }

        bool f1 = false;
        float off = 0;
        bool side = false;
        int dis = 0;
        Vector2 RememberedDirection = Vector2.Zero;

        BezierCurve CurveSlash;
        Vector2[] CurveSlashPoints;

        BezierCurve TrueSlash;
        int curveslashtime = 0;
        Vector2 RememberedCenter;
        Vector2 mid;
        Vector2 end;
        public void Attack(NPC target)
        {
            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = Main.rand.Next(1, 4);
            }

            // A stab
            if (Projectile.ai[1] == 1)
            {

                Projectile.ai[2]++;
                if (Projectile.ai[2] < 20)
                {

                    if (Projectile.ai[2] == 1)
                    {
                        off = Main.rand.NextFloat(MathHelper.TwoPi);
                    }

                    Projectile.rotation = Projectile.Center.DirectionTo(target.Center).ToRotation() + MathHelper.PiOver4;
                    Projectile.SmoothMoveToPoint(target.Center + new Vector2(200, 0).RotatedBy(off - MathHelper.PiOver2), 40f, 40);
                    RememberedDirection = Projectile.Center.DirectionTo(target.Center);
                }
                if (Projectile.ai[2] > 20 && Projectile.ai[2] < 30)
                {
                    if (Projectile.ai[2] == 21)
                    {
                        SoundEngine.PlaySound(DTAssetLib.SwordSounds.StandardSwing with { Pitch = 0.1f, PitchVariance = 0.2f, MaxInstances = 0 });
                    }


                    Projectile.velocity = RememberedDirection * 30f;

                    Spark Trail = new();
                    Trail.PrepareSpark(Projectile.Center, Projectile.velocity * 0.001f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, ColorLib.TenebrisGradient, 0.4f, false, 20, SparkDrawMode.Additive, 4f);
                    ParticleEngine.BehindProjectiles.Add(Trail);

                }
                if (Projectile.ai[2] > 30)
                {
                    Projectile.ai[2] = 0;

                    Projectile.ai[1] = Main.rand.Next(1, 4);
                }
            }

            //A linear slash
            if (Projectile.ai[1] == 2)
            {
                Projectile.ai[2]++;
                if (Projectile.ai[2] < 20)
                {

                    if (Projectile.ai[2] == 1)
                    {
                        side = Main.rand.NextBool();
                        dis = Main.rand.Next(180, 200);
                        off = Main.rand.NextFloat(MathHelper.TwoPi);

                    }

                    Projectile.rotation = -MathHelper.PiOver4;
                    Projectile.SmoothMoveToPoint(target.Center + new Vector2(dis * (side ? -1 : 1), 0), 70f, 50);
                    RememberedDirection = Projectile.Center.DirectionTo(target.Center);
                }
                if (Projectile.ai[2] > 20 && Projectile.ai[2] < 30)
                {
                    if (Projectile.ai[2] == 21)
                    {

                        SoundEngine.PlaySound(DTAssetLib.SwordSounds.StandardSwing with { PitchVariance = 0.2f, MaxInstances = 0 });
                    }
                    Projectile.velocity = RememberedDirection * 50f;


                }
                if (Projectile.ai[2] > 30 && Projectile.ai[2] < 50)
                {
                    Projectile.velocity *= 0.5f;
                }
                if (Projectile.ai[2] > 50)
                {
                    Projectile.ai[2] = 0;
                    Projectile.ai[1] = Main.rand.Next(1, 4);
                }
            }

            // Projectile
            if (Projectile.ai[1] == 3)
            {

                Projectile.ai[2]++;
                if (Projectile.ai[2] < 20)
                {

                    off += 0.1f;


                    Projectile.rotation = Projectile.Center.DirectionTo(target.Center).ToRotation() + MathHelper.PiOver4;
                    Projectile.SmoothMoveToPoint(target.Center + new Vector2(300, 0).RotatedBy(off - MathHelper.PiOver2), 30f, 50);

                }
                if (Projectile.ai[2] > 20 && Projectile.ai[2] < 30)
                {
                    if (Projectile.ai[2] == 21)
                    {
                        SoundEngine.PlaySound(DTAssetLib.Impacts.MagicBeep with { Pitch = 0.4f, PitchVariance = 0.2f, MaxInstances = 0 });

                        Projectile.velocity += Projectile.DirectionFrom(target.Center) * 6f;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(target.Center) * 15f, ModContent.ProjectileType<TenebrisStarFriendly>(), Projectile.damage, 3, Projectile.owner);
                    }

                    Projectile.SmoothMoveToPoint(target.Center + new Vector2(300, 0).RotatedBy(off - MathHelper.PiOver2), 30f, 50);
                }
                if (Projectile.ai[2] > 30)
                {
                    Projectile.ai[2] = 0;

                    Projectile.ai[1] = Main.rand.Next(1, 4);
                }
            }
        }

        public static bool TryGetCircleFromPoints(Vector2 a, Vector2 b, Vector2 c, out Vector2 center, out float radius)
        {
            Vector2 ab = b - a;
            Vector2 ac = c - a;

            float d = 2f * (ab.X * ac.Y - ab.Y * ac.X);

            center = Vector2.Zero;
            radius = 0f;

            if (Math.Abs(d) < 0.001f)
                return false; // Points are collinear or nearly so.

            float abSq = ab.LengthSquared();
            float acSq = ac.LengthSquared();

            center = a + new Vector2(
                (ac.Y * abSq - ab.Y * acSq) / d,
                (ab.X * acSq - ac.X * abSq) / d
            );

            radius = Vector2.Distance(center, a);
            return true;
        }
    }

    public class BlackKnifeOrganizer : ModPlayer
    {
        public List<Vector2> SiblingPositions = new();
        public List<Projectile> Siblings => Main.projectile.Where(n => n.type == ModContent.ProjectileType<BlackKnifeMinion>() && n.owner == Player.whoAmI && n.ai[0] == 0 && n.active).ToList();
        public override void PostUpdateBuffs()
        {

            int siblingCount = Siblings.Count;


        }
    }
}

