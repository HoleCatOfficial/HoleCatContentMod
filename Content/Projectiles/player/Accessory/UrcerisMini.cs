using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Equips.Cards.AstirDeck;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class UrcerisMini : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {

            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
  
        }

        SpriteEffects FX = SpriteEffects.None;
        float ROff = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            ArmorShaderData DumbassShader = GameShaders.Armor.GetSecondaryShader(Main.GetProjectileDesiredShader(Projectile), Main.player[Projectile.owner]);
            if (DumbassShader != null)
            {
                DumbassShader.Apply(Projectile);
            }


            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + ROff, projectileTexture.Size() / 2, Projectile.scale, FX, 0);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return null;
        }



        Player Owner => Main.player[Projectile.owner];
        public override void OnSpawn(IEntitySource source)
        {
            
        }
        public enum State
        {
            Idle,
            Stab,
            Spin
        }

        public State CurrentState;
        int TargetIndex = -1;

        Vector2 DirToTarget;
        NPC Target;
        int WaitTime1 = 60;
        public override void AI()
        {
            if (Main.rand.NextBool(15))
            {
                TenebrousCloudParticle P = new();
                P.Initialize(Projectile.Center, Projectile.velocity * 0.1f + new Vector2(0, Main.rand.NextFloat(1f)), Color.SkyBlue, 0.5f, 0.2f);
                ParticleEngine.BehindProjectiles.Add(P);
            }

            if (Main.rand.NextBool(5))
            {
                TenebrousCloudParticle P = new();
                P.Initialize(Projectile.Center, Projectile.velocity * 0.1f + new Vector2(0, Main.rand.NextFloat(1f)), Color.White, 0.5f, 0.1f);
                ParticleEngine.BehindProjectiles.Add(P);
            }


            if (Owner.TryGetModPlayer<UrcerisMiniPlayer>(out var Cool) && Cool.Active && !Owner.dead)
            {
                Projectile.timeLeft = 60;

                

                int chosen = -1;

                // #1 — Player whip target
                if (Owner.MinionAttackTargetNPC >= 0 &&
                    Owner.MinionAttackTargetNPC < Main.maxNPCs)
                {
                    NPC whipTarget = Main.npc[Owner.MinionAttackTargetNPC];
                    if (whipTarget.CanBeChasedBy())
                    {
                        chosen = Owner.MinionAttackTargetNPC;
                    }
                }

                // #2 — Bosses (if no whip target)
                if (chosen == -1)
                {
                    float bossDist = float.MaxValue;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy() && npc.boss)
                        {
                            float dist = Vector2.DistanceSquared(npc.Center, Owner.Center);
                            if (dist < bossDist)
                            {
                                bossDist = dist;
                                chosen = i;
                            }
                        }
                    }
                }

                // #3 — Closest to player
                if (chosen == -1)
                {
                    float closestDist = float.MaxValue;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy())
                        {
                            float dist = Vector2.DistanceSquared(npc.Center, Owner.Center);
                            if (dist < closestDist)
                            {
                                closestDist = dist;
                                chosen = i;
                            }
                        }
                    }
                }

                TargetIndex = chosen;

                if (TargetIndex > -1)
                {

                    Target = Main.npc[TargetIndex];
                }

                

                if (Target == null || !Target.active || TargetIndex <= -1)
                {
                    CurrentState = State.Idle;

                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 0;
                    Projectile.localAI[0] = 0;
                    Projectile.localAI[1] = 0;
                }

                if (Target != null)
                {
                    DirToTarget = Target.Center - Projectile.Center;
                    DirToTarget.Normalize();
                }

                if (Projectile.Distance(Owner.MountedCenter) > 2000)
                {
                    Projectile.Center = Owner.MountedCenter;
                }

                //LocalAI[0]: Orbit Speed
                Projectile.localAI[0] += 0.08f;

                switch (CurrentState)
                {
                    case State.Idle:
                        {
                            FX = Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                            ROff = Owner.direction == 1 ? 0f : MathHelper.PiOver2;

                            Projectile.SmoothMoveToPoint(Owner.MountedCenter - new Vector2(0, Opus.Sine(60, 80, 0.01f)), 10f);

                            if (Projectile.rotation != -MathHelper.PiOver4)
                            {
                                //Minus a negative is addition. If I remember anything from algebra, it's that.
                                if (Math.Abs(Projectile.rotation + MathHelper.PiOver4) > 0.1f)
                                {
                                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, -MathHelper.PiOver4, 0.08f);
                                }
                                else
                                {
                                    Projectile.rotation = -MathHelper.PiOver4;
                                }
                            }

                            if (Target != null && Target.active)
                            {
                                CurrentState = State.Stab;
                            }
                            break;
                        }
                    case State.Stab:
                        {
                            FX = SpriteEffects.None;
                            ROff = 0f;
                            //Update only before the dash
                            if (Projectile.localAI[1] != 1)
                            {
                                Projectile.rotation = DirToTarget.ToRotation() + MathHelper.PiOver4;
                            }

                            if (Projectile.ai[0] < WaitTime1)
                            {
                                if (Projectile.Distance(Target.Center) < 200)
                                {
                                    Projectile.ai[0]++;
                                }

                                

                                Vector2 Ideal = Target.Center + new Vector2(150, 0).RotatedBy(Projectile.localAI[0]);

                                Projectile.SmoothMoveToPoint(Ideal, 18);
                            }
                            else
                            {

                                //LocalAI[1]: Has Dashed.
                                if (Projectile.localAI[1] != 1)
                                {
                                    SoundEngine.PlaySound(DTAssetLib.StellarBow.EmpoweredShoot, Projectile.Center);

                                    Opus.RadialSpreadProjectile(ProjectileID.IceBolt, 8, Projectile.Center, Projectile.damage, 5, 8);
                                    Projectile.velocity = DirToTarget * 15;
                                    Projectile.localAI[1] = 1;
                                    
                                }
                                else
                                {
                                    Projectile.velocity *= 0.99f;
                                    Projectile.ai[1]++;
                                }

                                if (Projectile.ai[1] > 30)
                                {
                                    if (Target.active)
                                    {
                                        CurrentState = State.Spin;
                                    }
                                    else
                                    {
                                        CurrentState = State.Idle;
                                    }

                                    //Reset
                                    Projectile.ai[0] = 0;
                                    Projectile.ai[1] = 0;
                                    Projectile.localAI[0] = 0;
                                    Projectile.localAI[1] = 0;
                                }
                            }
                            break;
                        }
                    case State.Spin:
                        {


                            Projectile.rotation += 0.4f;

                            Vector2 Figure8 = new Vector2(Opus.Sine(-400, 400, 0.1f), Opus.Sine(300f, -300f, 0.2f));

                            if (Projectile.ai[0] < 180)
                            {
                                if (Projectile.ai[0] % 15 == 0)
                                {
                                    SoundEngine.PlaySound(DTAssetLib.SwordSounds.Woosh, Projectile.Center);
                                }
                                Projectile.SmoothMoveToPoint(Target.Center + Figure8, 15);
                                Projectile.ai[0]++;
                            }
                            else
                            {
                                if (Target.active)
                                {
                                    WaitTime1 = Main.rand.Next(30, 120);
                                    CurrentState = State.Stab;
                                    Projectile.ai[0] = 0;
                                }
                                else
                                {
                                    CurrentState = State.Idle;
                                    Projectile.ai[0] = 0;
                                }
                            }
                            break;
                        }
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for(int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BorealWood, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 50);
            }

            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Platinum, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 50);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 50);
            }
        }
    }
}