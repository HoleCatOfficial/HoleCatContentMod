using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
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

	public class QuixotismSwing : ModProjectile
	{
        private const float SWINGRANGE = 1.67f * (float)Math.PI; 
		private const float FIRSTHALFSWING = 0.4f;
		private const float WINDUP = 0.0000001f;
		private const float UNWIND = 0.4f;

        private enum AttackType
        {

            SwingDown,
            SwingUp
        }

		private enum AttackStage 
		{
			Prepare,
			Execute,
			Unwind
		}

		
		private AttackType CurrentAttack {
			get => (AttackType)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}

		private AttackStage CurrentStage {
			get => (AttackStage)Projectile.localAI[0];
			set {
				Projectile.localAI[0] = (float)value;
				Timer = 0; 
			}
		}

		
		private ref float InitialAngle => ref Projectile.ai[1];
		private ref float Timer => ref Projectile.ai[2];
		private ref float Progress => ref Projectile.localAI[1];
		private ref float Size => ref Projectile.localAI[2];
		private float prepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float execTime => 10f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        

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
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee; 
		}

		public override void OnSpawn(IEntitySource source)
        {
			Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

            if (CurrentAttack == AttackType.SwingUp) {
                if (Projectile.spriteDirection == 1) {
                   
                    targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 1 / 6, (float)Math.PI * 2 / 3);
                }
                else {
                    if (targetAngle < 0) {
                        targetAngle += 2 * (float)Math.PI;
                    }
                    targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 4 / 3, (float)Math.PI * 5 / 3);
                }
                InitialAngle = targetAngle + FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Inverse: add instead of subtract
            }
            else {
                if (Projectile.spriteDirection == 1) {
				
					targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
				}
				else {
					if (targetAngle < 0) {
						targetAngle += 2 * (float)Math.PI; 
					}

					targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
				}

				InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Otherwise, we calculate the angle
			}
		}

		public override void SendExtraAI(BinaryWriter writer) {
		
			writer.Write((sbyte)Projectile.spriteDirection);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			Projectile.spriteDirection = reader.ReadSByte();
		}

		public override void AI()
        {
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;

         
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

         
            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PrepareStrike();
                    break;
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                default:
                    UnwindStrike();
                    break;
            }

            SetSwordPosition();
            Timer++;
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
                    if(Q.PowerOpacity < 1f)
                    {
                        Q.PowerOpacity += 0.02f;
                    }
                }
                if (!Q.Powered)
                {
                    if(Q.PowerOpacity > 0f)
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

		
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
		}

			public override void CutTiles() {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
		}

	
		public override bool? CanDamage() {
			if (CurrentStage == AttackStage.Prepare)
				return false;
			return base.CanDamage();
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
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
                    }
                }
                else
                {
                    Q.hitCount[1]++;
                    damageDone *= 2;

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
        
            modifiers.HitDirectionOverride = (int?)(target.position.Y + 15);
        }


		public void SetSwordPosition()
		{

            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;


            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);
            
            if (Owner.gravDir == -1f) {
				Projectile.rotation = 0f - Projectile.rotation;
				armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
			}

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition;
            Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem);

            Owner.heldProj = Projectile.whoAmI;
		}

		
		private void PrepareStrike() 
        {
            Player player = Main.player[Projectile.owner];
			Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime); 
			Vector2 Velocity = Main.MouseWorld - Projectile.Center;
            Size = 1;

			if (Timer >= prepTime)
			{
                if (player.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
                {
                    if (Q.Powered)
                    {
                        SoundEngine.PlaySound(DTAssetLib.SwordSounds.StandardSwing);
                    }
                }
				SoundEngine.PlaySound(SoundID.Item71);
				CurrentStage = AttackStage.Execute;
			}
		}

        public Vector2 swordTip;
        public Line SwordLine;
        private void ExecuteStrike()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            
            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);

            if (player.HeldItem.ModItem is MeleeWeapons.Quixotism Q)
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

            if (CurrentAttack == AttackType.SwingDown)
            {

                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / execTime);
               
				Vector2 Velocity = Main.MouseWorld - Projectile.Center;

                if (Timer >= execTime)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }
            else
            {
                if (player.direction == 1)
                {
                    Progress = MathHelper.SmoothStep(SWINGRANGE, 0, (1f - UNWIND) * Timer / execTime);
                }
                if (player.direction == -1)
                {
                    Progress = MathHelper.SmoothStep(SWINGRANGE, 2.0f, (1f - UNWIND) * Timer / execTime);
                }

                if (Timer >= execTime)
                {
					
                    CurrentStage = AttackStage.Unwind;
                }
            }
        }

        private void UnwindStrike()
        {
            Player player = Main.player[Projectile.owner];
            if (CurrentAttack == AttackType.SwingDown)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) + UNWIND * Timer / hideTime);
                Size = 1f - MathHelper.SmoothStep(0, 1, (Timer / hideTime) * 0.7f);
                Projectile.Opacity = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);

                if (Timer >= hideTime)
                {
                    Projectile.Kill();
                }
            }
            else if (CurrentAttack == AttackType.SwingUp)
            {
                if (player.direction == 1)
                {
                    Progress = MathHelper.SmoothStep(SWINGRANGE, 0, (1f - UNWIND) + UNWIND * Timer / hideTime);
                }
                if (player.direction == -1)
                {
                    Progress = MathHelper.SmoothStep(SWINGRANGE, 1.6f, (1f - UNWIND) + UNWIND * Timer / hideTime);
                }
                Size = 1f - MathHelper.SmoothStep(0, 1, (Timer / hideTime) * 0.7f);
                Projectile.Opacity = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);

                if (Timer >= hideTime)
                {
                    Projectile.Kill();
                }
            }
		}
	}
}