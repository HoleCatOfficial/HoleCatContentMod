using BreadLibrary.Core.Graphics;
using BreadLibrary.Core.Graphics.PixelationShit;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee.Quixotism
{
    public class QuixotismSwing : BaseBroadswordProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 72;
            Projectile.height = 72;
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.Woosh;


        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            Player Owner = Main.player[Projectile.owner];
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            SwordLine = new Line(Owner.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);

            if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (Q.Powered)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), pt[Main.rand.Next(30)], SwordLine.GetLineRotation.ToRotationVector2() * 2, new Color(255, 219, 6), 1.5f);
                        Dust.NewDustPerfect(pt[Main.rand.Next(30)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, new Color(255, 219, 6), 2f);
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDustPerfect(pt[Main.rand.Next(30)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, Color.White, 2f);
                    }
                }
            }
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            Player Owner = Main.player[Projectile.owner];
            if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (!Q.Powered)
                {
                    Q.hitCount[0]++;

                    if (Q.hitCount[0] >= 8)
                    {
                        SoundEngine.PlaySound(DTAssetLib.Charge.Quixotism, npc.Center);
                        Q.Powered = true;
                        Q.hitCount[0] = 0;
                        Q.hitCount[1] = 0;
                        Q.comboExpireTimer = 120;
                    }
                }
                else
                {
                    Q.hitCount[1]++;
                    Q.comboExpireTimer = 120;

                    SoundEngine.PlaySound(DTAssetLib.SwordSounds.Slam, npc.Center);
                    Opus.RadialDustRandomDir(ModContent.DustType<ColorableNeonDust>(), 7, npc.Center, 0, new Color(255, 219, 6), 1f, 3);
                    npc.AddBuff(ModContent.BuffType<SoulInferno>(), 120);

                    PRTLoader.NewParticle(PRTLoader.GetParticleID<QuixoticParticle>(), Main.rand.NextVector2FromRectangle(npc.Hitbox), Vector2.Zero, default, 1f);

                    Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), npc.Center, Vector2.Zero, new Color(255, 219, 6) * 0.5f, 0.01f, 0.4f);

                    if (Q.hitCount[1] >= 2)
                    {
                        Q.Powered = false;
                        Q.hitCount[1] = 0;
                        Q.hitCount[0] = 0;
                    }
                }
            }
        }

        public override void DrawUnderBlade()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D powertexture = DTAssetLib.QuixotismPowerAura.Value;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, texture.Height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(texture.Width, texture.Height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }



            if (player.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (Q.Powered)
                {
                    if (Q.PowerOpacity < 1f)
                    {
                        Q.PowerOpacity += 0.02f;
                    }
                }
                if (!Q.Powered)
                {
                    if (Q.PowerOpacity > 0f)
                    {
                        Q.PowerOpacity -= 0.02f;
                    }
                }

                Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (new Color(255, 219, 6) * Q.PowerOpacity) * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale * 1.5f, effects, 0);
                Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            }
        }
    }

    /*
    public class QuixotismSwing1 : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 72;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            
            
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;


        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public enum State
        {
            SwingDown,
            SwingUp,
            Wait
        }

        public State CurrentState;
        public Vector2 targetAngle = Vector2.Zero;
        public int AITimer = 0;
        public float UpPoint = 0f;
        public float DownPoint = 0f;

        public Vector2 swordTip;
        public Line SwordLine;
        public override void AI()
        {
            AITimer++;
            if (Owner.controlUseItem)
            {
                swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
                Projectile.scale = Owner.GetAdjustedItemScale(Owner.HeldItem);

                SwordLine = new Line(Owner.Center, swordTip);
                Vector2[] pt = SwordLine.GetPointsAlongLine(30);

                if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
                {
                    if (Q.Powered)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), pt[Main.rand.Next(30)], SwordLine.GetLineRotation.ToRotationVector2() * 2, new Color(255, 219, 6), 1.5f);
                            Dust.NewDustPerfect(pt[Main.rand.Next(30)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, new Color(255, 219, 6), 2f);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Dust.NewDustPerfect(pt[Main.rand.Next(30)], ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 2, 0, Color.White, 2f);
                        }
                    }
                }

                if (CurrentState == State.Wait)
                {
                    targetAngle = (Main.MouseWorld - Owner.MountedCenter);
                }

                //Projectile.rotation = targetAngle.ToRotation();
                UpPoint = targetAngle.ToRotation() - MathHelper.ToRadians(135f);
                DownPoint = targetAngle.ToRotation() + MathHelper.ToRadians(135f);
            }

            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed || !Owner.controlUseItem)
            {
                Projectile.Kill();
                return;
            }

            SetSwordPosition();
            ControlRotation();
        }

        bool SetPos = false;
        bool f1 = false;
        public int LastSwing = -1;
        public int WaitTimer = 10;
        public void ControlRotation()
        {
            float speedFactor = Owner.GetAttackSpeed(DamageClass.Melee);
            float t = 0.15f * speedFactor;

            switch (CurrentState)
            {
                case State.SwingUp:
                    {
                        if (!SetPos)
                        {
                            Projectile.rotation = DownPoint;
                            WaitTimer = (int)(10 * Owner.GetAttackSpeed(DamageClass.Melee));
                            SetPos = true;
                        }
                        else
                        {
                            if (!f1)
                            {
                                if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
                                {
                                    if (Q.Powered)
                                    {
                                        SoundEngine.PlaySound(DTAssetLib.SwordSounds.StandardSwing);
                                    }
                                }
                                SoundEngine.PlaySound(DTAssetLib.SwordSounds.Woosh with { Pitch = -0.75f, PitchVariance = 0.3f }, Projectile.Center);
                                f1 = true;
                            }


                            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, UpPoint, t);
                            if (Math.Abs(Projectile.rotation - UpPoint) < 0.07)
                            {
                                LastSwing = -1;
                                CurrentState = State.Wait;
                            }
                        }

                        break;
                    }
                case State.SwingDown:
                    {
                        if (!SetPos)
                        {
                            Projectile.rotation = UpPoint;
                            WaitTimer = (int)(10 * Owner.GetAttackSpeed(DamageClass.Melee));
                            SetPos = true;
                        }
                        else
                        {
                            if (!f1)
                            {
                                if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
                                {
                                    if (Q.Powered)
                                    {
                                        SoundEngine.PlaySound(DTAssetLib.SwordSounds.StandardSwing);
                                    }
                                }
                                SoundEngine.PlaySound(DTAssetLib.SwordSounds.Woosh with { Pitch = -0.75f, PitchVariance = 0.3f }, Projectile.Center);
                                f1 = true;
                            }
                            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, DownPoint, t);
                            if (Math.Abs(Projectile.rotation - DownPoint) < 0.07)
                            {
                                LastSwing = 1;
                                CurrentState = State.Wait;
                            }
                        }
                        break;
                    }
                case State.Wait:
                    {
                        if (WaitTimer > 0)
                        {
                            SetPos = false;
                            f1 = false;
                            WaitTimer--;
                        }
                        else
                        {
                            if (LastSwing == -1)
                            {
                                CurrentState = State.SwingDown;
                            }
                            if (LastSwing == 1)
                            {
                                CurrentState = State.SwingUp;
                            }
                        }
                        break;
                    }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D powertexture = DTAssetLib.QuixotismPowerAura.Value;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, texture.Height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(texture.Width, texture.Height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }



            if (player.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (Q.Powered)
                {
                    if (Q.PowerOpacity < 1f)
                    {
                        Q.PowerOpacity += 0.02f;
                    }
                }
                if (!Q.Powered)
                {
                    if (Q.PowerOpacity > 0f)
                    {
                        Q.PowerOpacity -= 0.02f;
                    }
                }

                Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, (new Color(255, 219, 6) * Q.PowerOpacity) * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale * 1.5f, effects, 0);
                Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }


        public override bool? CanHitNPC(NPC target)
        {
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (!Q.Powered)
                {
                    Q.hitCount[0]++;

                    if (Q.hitCount[0] >= 8)
                    {
                        SoundEngine.PlaySound(DTAssetLib.Charge.Quixotism, target.Center);
                        Q.Powered = true;
                        Q.hitCount[0] = 0;
                        Q.hitCount[1] = 0;
                        Q.comboExpireTimer = 120;
                    }
                }
                else
                {
                    Q.hitCount[1]++;
                    Q.comboExpireTimer = 120;

                    SoundEngine.PlaySound(DTAssetLib.SwordSounds.Slam, target.Center);
                    Opus.RadialDustRandomDir(ModContent.DustType<ColorableNeonDust>(), 7, target.Center, 0, new Color(255, 219, 6), 1f, 3);
                    target.AddBuff(ModContent.BuffType<SoulInferno>(), 80);

                    PRTLoader.NewParticle(PRTLoader.GetParticleID<QuixoticParticle>(), Main.rand.NextVector2FromRectangle(target.Hitbox), Vector2.Zero, default, 1f);

                    Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), target.Center, Vector2.Zero, new Color(255, 219, 6) * 0.5f, 0.01f, 0.4f);

                    if (Q.hitCount[1] >= 2)
                    {
                        Q.Powered = false;
                        Q.hitCount[1] = 0;
                        Q.hitCount[0] = 0;
                    }
                }
            }
        }



        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Owner.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
            {
                if (Q.Powered)
                {
                    modifiers.SourceDamage *= 2f;
                }
            }
            modifiers.HitDirectionOverride = (int?)(target.position.Y + 15);
        }


        public void SetSwordPosition()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);

            if (Owner.gravDir == -1f)
            {
                Projectile.rotation = 0f - Projectile.rotation;
                armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
            }

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition;
            Projectile.scale = 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem);

            Owner.heldProj = Projectile.whoAmI;
        }
    }
    */
}