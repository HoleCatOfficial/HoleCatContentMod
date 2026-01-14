using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.IO;

namespace DestroyerTest.Content.Projectiles.ParentClasses
{
    public abstract class UpDownSwingProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 9000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        private enum AttackType 
		{
			
			SwingDown,
			
			SwingUp,
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
				Progress = 0; 
			}
		}


        public Player Owner => Main.player[Projectile.owner];
        public int Progress;
        public virtual float spawnTime => 6f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		public virtual float execTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		public virtual float killTime => 6f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        public const float RotUp = -MathHelper.PiOver2;
        public const float RotDown = MathHelper.PiOver2;

        public bool Left {get; private set;}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Progress);
            writer.Write(Left);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Progress = reader.ReadInt32();
            Left = reader.ReadBoolean();
        }

        public virtual bool PreSwing()
        {
            return true;
        }

        private bool firsttickflag = false;
        public virtual void OnFirstTickSwing()
        {
            
        }
        public virtual void DuringSwing(bool FirstTickOnly)
        {
            if (FirstTickOnly)
            {
                if (!firsttickflag)
                {
                    OnFirstTickSwing();
                    firsttickflag = true;
                }
            }
        }

        public virtual void PostSwing()
        {
            
        }


        public virtual void OnDownSwing()
        {
            
        }

        public virtual void OnUpSwing()
        {
            
        }



        public void HandleSwing(float startRot, float endRot, float duration)
        {
            Progress++;

            float progress = Progress / duration;
            progress = MathHelper.Clamp(progress, 0f, 1f);

            float delta = MathHelper.WrapAngle(endRot - startRot);
            Projectile.rotation = (startRot + delta * progress) * Projectile.direction;

            if (Progress >= duration)
            {
                Progress = 0;
                AdvanceStage();
            }
        }

        public void AdvanceStage()
        {
            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    firsttickflag = false;
                    
                    CurrentStage = AttackStage.Execute;
                    break;

                case AttackStage.Execute:
                    
                    CurrentStage = AttackStage.Unwind;
                    break;

                case AttackStage.Unwind:
                    
                    if (Owner.controlUseTile)
                    {
                        // flip swing direction
                        CurrentAttack = CurrentAttack == AttackType.SwingDown
                            ? AttackType.SwingUp
                            : AttackType.SwingDown;

                        CurrentStage = AttackStage.Prepare;
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                    break;
            }
        }



        public override void AI()
        {
            float up = RotUp;
            float down = RotDown;

            if (CurrentAttack == AttackType.SwingUp)
            {
                (up, down) = (down, up);
            }

            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PreSwing();
                    HandleSwing(down, down, spawnTime);
                    break;

                case AttackStage.Execute:
                    DuringSwing(Progress == 0);
                    HandleSwing(up, down, execTime);
                    break;

                case AttackStage.Unwind:
                    PostSwing();
                    HandleSwing(down, down, killTime);
                    break;
            }
        }

        public void MainAnimation()
        {
            Vector2 pivot = Owner.MountedCenter;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = SpriteEffects.None;

            Left = Projectile.direction == -1 ? true : false;

            if (!Left)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Main.EntitySpriteDraw(tex, pivot - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, tex.Height), Projectile.scale, effects, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            MainAnimation();
            return false;
        }

        public virtual void DefaultBehaviour()
        {
            Player player = Main.player[Projectile.owner];
        }
    }
}

