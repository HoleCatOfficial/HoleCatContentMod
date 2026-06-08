using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using System.IO;
using OpusLib;
using Terraria.Graphics.Shaders;
using Humanizer;
using System;
using ReLogic.Content;

namespace DestroyerTest.Content.Projectiles.ParentClasses
{
    public class ThrownScepter : ModProjectile
    {
        public Color ThemeColor { get; set; }
        public int WidthDim { get; set; }
        public int HeightDim { get; set; }
        public int DustType { get; set; }
        private Color _dustColor;
        public bool DustUsesColorOnDraw;
        public Color DustColor
        {
            get => _dustColor;
            set
            {
                if (DustUsesColorOnDraw)
                    _dustColor = value;
            }
        }

        public bool returning = false;
        public int flightTime = 0;
        public int HitCount = 0;
        public int soundCooldown = 0;
        public int existenceTimer = 0;
        public int TileCollisions = 0;
        public float TileCollideFXTimer = 0f;

        public bool ArmorSetHelper_AetherianShimmerEffects = false;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = WidthDim + ScepterClassStats.SizeModifier;
            Projectile.height = HeightDim + ScepterClassStats.SizeModifier;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 9000;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.tileCollide = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(returning);
            writer.Write(flightTime);
            writer.Write(HitCount);
            writer.Write(soundCooldown);
            writer.Write(existenceTimer);
            writer.Write(TileCollisions);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            returning = reader.ReadBoolean();
            flightTime = reader.ReadInt32();
            HitCount = reader.ReadInt32();
            soundCooldown = reader.ReadInt32();
            existenceTimer = reader.ReadInt32();
            TileCollisions = reader.ReadInt32();
        }

        public virtual Asset<Texture2D> GlowMask { get; set; } = null;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = ThemeColor;
			if (Projectile.timeLeft < 30)
			{
				lightColor *= ((float)Projectile.timeLeft / 30f); 
			}

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			DTUtils Utility = new DTUtils();

            if (!ArmorSetHelper_AetherianShimmerEffects)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

                if (returning)
                {
                    Main.EntitySpriteDraw(DTAssetLib.Cyclone(1).Value, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, DTAssetLib.Cyclone(1).Value.Size() / 2, 0.1f * Projectile.scale, SpriteEffects.None, 0);
                }
                else
                {
                    Main.EntitySpriteDraw(DTAssetLib.Cyclone(1).Value, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, DTAssetLib.Cyclone(1).Value.Size() / 2, 0.1f * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
                }
                        
                Main.EntitySpriteDraw(DTAssetLib.BloomRing.Value, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, DTAssetLib.BloomRing.Value.Size() / 2, 0.4f * Projectile.scale, SpriteEffects.None, 0);
                    
                Opus.ReturnToDefaultDrawing(spriteBatch);

                Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

                if (GlowMask != null)
                {
                    Main.EntitySpriteDraw(GlowMask.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, GlowMask.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            else
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

                if (returning)
                {
                    Main.EntitySpriteDraw(DTAssetLib.Cyclone(1).Value, Projectile.Center - Main.screenPosition, null, DTColorUtils.Pastel(Main.DiscoColor, 0.4f) * 0.4f, Projectile.rotation, DTAssetLib.Cyclone(1).Value.Size() / 2, 0.1f * Projectile.scale, SpriteEffects.None, 0);
                }
                else
                {
                    Main.EntitySpriteDraw(DTAssetLib.Cyclone(1).Value, Projectile.Center - Main.screenPosition, null, DTColorUtils.Pastel(Main.DiscoColor, 0.4f) * 0.4f, Projectile.rotation, DTAssetLib.Cyclone(1).Value.Size() / 2, 0.1f * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
                }
                        
                Main.EntitySpriteDraw(DTAssetLib.BloomRing.Value, Projectile.Center - Main.screenPosition, null, DTColorUtils.Pastel(Main.DiscoColor, 0.4f) * 0.4f, Projectile.rotation, DTAssetLib.BloomRing.Value.Size() / 2, 0.4f * Projectile.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, DTColorUtils.Pastel(Main.DiscoColor, 0.4f) * 0.6f, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);     
                    
                Opus.ReturnToDefaultDrawing(spriteBatch);
            }
            return false;
		}

        public override void AI()
        {
            DefaultBehaviour();
            EnchantmentVisuals();
        }

        public virtual Rectangle EnchantmentVisuals(int Width = 16, int Height = 16)
        {
            Rectangle hitbox = Projectile.Hitbox;
            Vector2 localOffset = new Vector2(
                (hitbox.Width / 2f) - (Width / 2f),
                -(hitbox.Height / 2f) + (Height / 2f)
            );
            Vector2 rotatedOffset = localOffset.RotatedBy(Projectile.rotation);

            Vector2 rectCenter = Projectile.Center + rotatedOffset;

            return new Rectangle(
                (int)(rectCenter.X - Width / 2f),
                (int)(rectCenter.Y - Height / 2f),
                Width,
                Height
            );
        }

        public event Action<ThrownScepter> OnReturnHook;
        public virtual void OnReturn()
        {
            OnReturnHook?.Invoke(this);
        }

        public bool OnReturnFlag = false;
        public virtual void DefaultBehaviour()
        {
            // Decrease the cooldown timer on each tick
            if (soundCooldown > 0)
            {
                soundCooldown--;
            }

            if (soundCooldown <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item169);
                soundCooldown = 30;
            }

            if (TileCollideFXTimer > 0)
            {
                TileCollideFXTimer -= 1f;
            }

            Player player = Main.player[Projectile.owner];

            if (Projectile.Distance(player.Center) < 25 || ArmorSetHelper_AetherianShimmerEffects)
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }
            
            Projectile.rotation += (Projectile.velocity.Length() * 0.03f) * Projectile.direction;

            if (Main.rand.NextBool(3) && !ArmorSetHelper_AetherianShimmerEffects)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType, Projectile.velocity * 0.2f, 100, DustColor, 1.2f);
                dust.noGravity = true;
            }
            else if (Main.rand.NextBool(3) && ArmorSetHelper_AetherianShimmerEffects)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, Projectile.velocity * 0.2f, 100, DTColorUtils.Pastel(Main.DiscoColor, 0.4f), 1.2f);
                dust.noGravity = true;
            }

            DTConfig config = ModContent.GetInstance<DTConfig>();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();

            if (!returning)
            {
                
                flightTime++;
                float returnDelayMultiplier = 1f + (ScepterClassStats.Range * 0.01f);
                int baseFlightTime = 60;
                if (flightTime >= baseFlightTime * returnDelayMultiplier)
                {
                    if (config.EnableDebugMessages)
                    {
                        Main.NewText($"Range: {ScepterClassStats.Range}, FlightTime: {flightTime}, Multiplier: {returnDelayMultiplier}");
                    }
                    returning = true;
                }
            }

            if (returning)
            {
                if (!OnReturnFlag)
                {
                    OnReturn();
                    OnReturnFlag = true;
                }
                ArmCatchAnimate(player);
                // InPhase: Smooth return using Lerp
                Vector2 returnDirection = player.Center - Projectile.Center;
                float speed = MathHelper.Lerp(Projectile.velocity.Length(), 15f, 0.8f); // Smooth acceleration
                Projectile.velocity = returnDirection.SafeNormalize(Vector2.Zero) * speed;

                // If close enough, remove the projectile
                if (Projectile.Distance(player.Center) < 8) // 8 pixels radius
                {
                    HitCount = 0;
                    existenceTimer = 0;
                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (!returning && TileCollisions >= 5)
            {
                for (int u = 0; u < 6; u++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustType, Main.rand.NextVector2CircularEdge(60, 60), 0, DustColor, 1.35f);
                }
            }
            returning = false;
        }

        public void ArmCatchAnimate(Player player)
        {
            // Calculate the direction vector from the player to the projectile
            Vector2 directionToProjectile = Projectile.Center - player.Center;

            // Normalize the direction vector to get a unit vector
            directionToProjectile.Normalize();

            // Calculate the angle between the player's direction and the direction to the projectile
            float angleDifference = MathHelper.WrapAngle(directionToProjectile.ToRotation() - player.direction * MathHelper.PiOver2);

            // Adjust arm rotation based on the player's facing direction
            if (player.direction == 1)
            {
                // Player is facing right, so we use the angle difference as is
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angleDifference);
            }
            else if (player.direction == -1)
            {
                // Player is facing left, so flip the angle by pi (180 degrees) to reach the opposite direction
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angleDifference + MathHelper.Pi);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 120);
            SoundEngine.PlaySound(SoundID.Item175, Projectile.position);
            HitCount += 1;
            returning = true; // Immediately start returning when hitting something
        }

        /// <summary>
        /// Use this for any tile collision effects. Always return base at the end so that the timer works right.
        /// </summary>
        public void TileCollideEffects()
        {
            if (TileCollideFXTimer <= 0f)
            {
                //Blah Blah run here.
                TileCollideFXTimer = ModContent.GetInstance<DTConfig>().ScepterTileCollsionsCooldown;
            }
        }

        private void CommonTileCollideEffects(ref Vector2 oldVelocity)
        {
            SoundStyle Break = new SoundStyle("DestroyerTest/Assets/Audio/TO_Break") with
            {
                PitchVariance = 0.5f
            };

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
            TileCollisions += 1;
            if (TileCollisions > 5)
            {
                SoundEngine.PlaySound(Break);
                Projectile.Kill();
                HitCount = 0;
                existenceTimer = 0;
                TileCollisions = 0;
            }


            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType, oldVelocity.X * 0.5f, oldVelocity.Y * 0.5f, 0, DustColor, 1.5f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }

            // Activate return phase
            returning = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!ArmorSetHelper_AetherianShimmerEffects)
            {
                CommonTileCollideEffects(ref oldVelocity);
                
                TileCollideEffects();
            }

            return false; // Prevents the projectile from being destroyed on collision
        }

    }
}

