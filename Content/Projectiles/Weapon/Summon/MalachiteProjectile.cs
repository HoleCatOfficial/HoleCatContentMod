using System;
using System.Collections.Generic;
using System.Linq;
using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.AmmoProjectiles;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Fargowiltas.UI.StatSheetUI;
using static FargowiltasSouls.FargoSoulsSets;
namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{

    public class MalachiteProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;

            ProjectileID.Sets.TrailCacheLength[Type] = 40;
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
            Projectile.localNPCHitCooldown = 35;
        }

        public struct MalachiteTrailDrawer
        {
            public const int TotalIllusions = 1;
            public const int FramesPerImportantTrail = 60;
            private static VertexStrip _vertexStrip = new VertexStrip();
            public Color ColorStart;
            public Color ColorEnd;

            public void Draw(Projectile proj)
            {
                _ = proj.ai[1];
                MiscShaderData miscShaderData = GameShaders.Misc["EmpressBlade"];
                int num = 1;
                int num2 = 0;
                int num3 = 0;
                float w = -0.6f;
                miscShaderData.UseShaderSpecificData(new Vector4(num, num2, num3, w));
                miscShaderData.Apply();
                _vertexStrip.PrepareStrip(proj.oldPos, proj.oldRot.Select(n => { n += -(MathHelper.PiOver2 + MathHelper.PiOver4) * proj.direction; return n; }).ToArray(), StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f, proj.oldPos.Length, includeBacksides: true);
                _vertexStrip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }

            private Color StripColors(float progressOnStrip)
            {
                Color result = Color.Lerp(ColorStart, ColorEnd, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip, clamped: true));
                result.A /= 2;
                return result;
            }

            private float StripWidth(float progressOnStrip) => 36f;
        }

        float shadrot = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            MalachiteTrailDrawer drawer = new();
            drawer.ColorStart = Color.Aquamarine;
            drawer.ColorEnd = Color.DarkBlue;


            if (Projectile.ai[1] == 2 && (Projectile.ai[2] > 30 && Projectile.ai[2] < 40))
            {
                //drawer.Draw(Projectile);

                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    float fade = MathHelper.Lerp(0.5f, 0f, (float)i / (float)Projectile.oldPos.Length);

                    if (i != 0)
                    {
                        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.OldCenter()[i] - Main.screenPosition, null, Color.White with { A = 0 } * Projectile.Opacity * fade, Projectile.oldRot[i] - (MathHelper.PiOver4 + (Projectile.direction == 1 ? 0 : MathHelper.PiOver2)), TextureAssets.Projectile[Type].Value.Size() / 2, Projectile.scale, Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                    }
                }
            }

            shadrot += 0.1f;
            for (int i = 0; i < 4; i++)
            {
                DrawData data = DTUtils.CenteredDraw(Projectile, Color.White with { A = 0} * 0.5f ) with { position = (Projectile.Center + new Vector2(2f, 0).RotatedBy((MathHelper.PiOver2 * i) + shadrot)) - Main.screenPosition };
                Main.EntitySpriteDraw(data);
            }
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, lightColor));

            //Utils.DrawRectangle(Main.spriteBatch, visualHitbox.TopLeft(), visualHitbox.BottomRight(), Color.Green, Color.Green, 2f);
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;

        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] != 0 &&  Projectile.ManualCanHitFriendly(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact with { Volume = 1f }, target.Center);
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/DamascusSlash") { Pitch = 1.5f, PitchVariance = 0.2f, Volume = 0.8f }, target.Center);
            

            for (int i = 0; i < 5; i++)
            {
                Spark Trail = new();
                Trail.PrepareSpark(target.Center, Projectile.velocity.RotatedByRandom(0.1f) * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Color.Aquamarine, 0.4f, false, 20, SparkDrawMode.Additive, 6f);
                ParticleEngine.BehindProjectiles.Add(Trail);

            }

            if (Projectile.ai[1] == 2 && (Projectile.ai[2] > 30 && Projectile.ai[2] < 40))
            {
                for (int i = 0; i < 4; i++)
                {
                    Spark Trail = new();
                    Trail.PrepareSpark(target.Center, Projectile.velocity.RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2).RotatedByRandom(0.1f) * Main.rand.NextFloat(0.02f, 0.25f), Projectile.velocity.ToRotation(), Color.Aquamarine, 0.3f, false, 20, SparkDrawMode.Additive, 5f);
                    ParticleEngine.BehindProjectiles.Add(Trail);
                }
            }

        }

        Player Owner => Main.player[Projectile.owner];
        Rectangle visualHitbox => Utils.CenteredRectangle(Projectile.Center, new Vector2(58, 58));
        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Lighting.AddLight(Projectile.Center, Color.MediumAquamarine.ToVector3());

            if (Owner.dead || !Owner.active)
            {
                Owner.ClearBuff(ModContent.BuffType<MalachiteBuff>());
            }

            if (Owner.HasBuff(ModContent.BuffType<MalachiteBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            if(Projectile.Center.Distance(Owner.Center) > 4000)
            {
                Projectile.Center = Owner.Center;
            }


            if (Main.rand.NextBool(10))
            {
                StarParticle star = new();
                star.Initialize(Main.rand.NextVector2FromRectangle(visualHitbox), Projectile.velocity * 0.02f, Color.MediumAquamarine, 0.1f, Main.rand.NextFloat(0.1f, 0.1f), 60, 2);
                ParticleEngine.Particles.Add(star);
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
            

            MalachiteOrganizer organizer = Owner.GetModPlayer<MalachiteOrganizer>();
            List<Projectile> siblings = organizer.Siblings;

            int myIndex = siblings.IndexOf(Projectile);

            if (myIndex < 0)
                return;

            float phase = myIndex * 0.6f;
            float bob = MathF.Sin(Main.GlobalTimeWrappedHourly * 2f + phase) * 5f;

            Projectile.rotation = -MathHelper.PiOver4 + ((-0.1f * myIndex * Owner.direction) + (0.03f * bob * Owner.direction));

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
                Projectile.ai[1] = Main.rand.Next(1, 3);
            }

            // A stab
            if (Projectile.ai[1] == 1)
            {
                
                Projectile.ai[2]++;
                if (Projectile.ai[2] < 30)
                {
                    
                    if (Projectile.ai[2] == 1)
                    {
                        off = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                    Projectile.rotation = Projectile.Center.DirectionTo(target.Center).ToRotation() + MathHelper.PiOver4;
                    Projectile.SmoothMoveToPoint(target.Center + new Vector2(200, 0).RotatedBy(off - MathHelper.PiOver2), 30f, 50);
                    RememberedDirection = Projectile.Center.DirectionTo(target.Center);
                }
                if (Projectile.ai[2] > 30 && Projectile.ai[2] < 45)
                {
                    if (Projectile.ai[2] == 31)
                    {
                        SoundEngine.PlaySound(DTAssetLib.SwordSounds.MediumHeavySwing with { Pitch = 1.5f, PitchVariance = 0.2f, MaxInstances = 0, Volume = 0.7f }, Projectile.Center);
                    }
                  

                    Projectile.velocity = RememberedDirection * 30f;

                    Spark Trail = new();
                    Trail.PrepareSpark(Projectile.Center, Projectile.velocity * 0.001f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Color.Aquamarine, 0.4f, false, 20, SparkDrawMode.Additive, 4f);
                    ParticleEngine.BehindProjectiles.Add(Trail);

                }
                if (Projectile.ai[2] > 45)
                {
                    Projectile.ai[2] = 0;

                    Projectile.ai[1] = Main.rand.Next(1, 3);
                }
            }

            //A linear slash
            if (Projectile.ai[1] == 2)
            {
                Projectile.ai[2]++;
                if (Projectile.ai[2] < 30)
                {
                   
                    if (Projectile.ai[2] == 1)
                    {
                        side = Main.rand.NextBool();
                        dis = Main.rand.Next(180, 200);
                        off = Main.rand.NextFloat(MathHelper.TwoPi);

                    }

                    Projectile.rotation = -MathHelper.PiOver4;
                    Projectile.SmoothMoveToPoint(target.Center + new Vector2(dis * (side ? -1 : 1), 0), 30f, 50);
                    RememberedDirection = Projectile.Center.DirectionTo(target.Center);
                }
                if (Projectile.ai[2] > 30 && Projectile.ai[2] < 40)
                {
                    if (Projectile.ai[2] == 31)
                    {
                        Projectile.velocity *= 0;
                        SoundEngine.PlaySound(DTAssetLib.SwordSounds.ConSwing with { Pitch = 1.5f, PitchVariance = 0.2f, MaxInstances = 0, Volume = 0.7f }, Projectile.Center);
                    }
                    Projectile.velocity = RememberedDirection * 50f;


                }
                if (Projectile.ai[2] > 40 && Projectile.ai[2] < 60)
                {
                    Projectile.velocity *= 0.73f;
                }
                if (Projectile.ai[2] > 60)
                {
                    Projectile.ai[2] = 0;
                    Projectile.ai[1] = Main.rand.Next(1, 3);
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

    public class MalachiteOrganizer : ModPlayer
    {
        public List<Vector2> SiblingPositions = new();
        public List<Projectile> Siblings => Main.projectile.Where(n => n.type == ModContent.ProjectileType<MalachiteProjectile>() && n.owner == Player.whoAmI && n.ai[0] == 0 && n.active).ToList();
        public int Level = 1;
        public override void PostUpdateBuffs()
        {

            int siblingCount = Siblings.Count;

            
        }
    }
}