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
        public int soundCooldown = 0; // Initialize a cooldown timer
        public int existenceTimer = 0;
        public int TileCollisions = 0;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = WidthDim + ScepterClassStats.SizeModifier;
            Projectile.height = HeightDim + ScepterClassStats.SizeModifier;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 9000; // 10 seconds max lifespan
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

        int trailLength = 10; // Adjust for desired effect
		public override bool PreDraw(ref Color lightColor)
			{
				// Set lightColor to a reddish hue and adjust its transparency based on the projectile's time left
				lightColor = ThemeColor;
				if (Projectile.timeLeft < 30)
				{
					lightColor *= ((float)Projectile.timeLeft / 30f); // Fade out glow as projectile nears expiration
				}

				// Prepare for sprite drawing
				SpriteBatch spriteBatch = Main.spriteBatch;
				Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
				DTUtils Utility = new DTUtils();


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
                /*
                var Trail = DTAssetLib.Trail(2).Value;
				Vector2 trailOrigin = new Vector2(Trail.Width / 2, Trail.Height / 2);

				for (int i = 0; i < trailLength && i < Projectile.oldPos.Length; i++)
					{
						float fade = (float)(trailLength - i) / trailLength;

						// Make sure transparency blending is correct
						Color trailColor = lightColor * fade * 0.3f;
						trailColor.A = (byte)(fade * 100); // Instead of setting it to 0

						Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition;
						float scaleFactor = 0.3f; // Adjust the factor to make it smaller
						Main.EntitySpriteDraw(Trail, drawPosition, null, trailColor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, trailOrigin, (Projectile.scale * fade) * scaleFactor, SpriteEffects.None, 0);
					}

                Opus.ReturnToDefaultDrawing(spriteBatch);
                */

                // Build a vertex triangle-strip ribbon for a smooth Zenith-like trail, and feed the shader
                // Start an immediate-mode batch for shader parameter setting (required by many shaders)
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

                // Attempt to get the vanilla Zenith/trail shader. Replace this key with the runtime-discovered key if needed.
                string shaderKey = "ZenithTrailKey"; // <-- replace this with the key you find at runtime
                if (GameShaders.Misc.TryGetValue(shaderKey, out var shaderData))
                {
                    shaderData.UseColor(ThemeColor.ToVector3());
                    // Bind a trail texture to the graphics device if the shader samples a texture
                    Texture2D trailTexture = DTAssetLib.Trail(2).Value;
                    Main.graphics.GraphicsDevice.Textures[0] = trailTexture;

                    // Apply shader so it affects subsequent primitive draws
                    shaderData.Apply(null);

                    // Collect non-zero old positions into a list (closest first)
                    var points = new System.Collections.Generic.List<Vector2>();
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                    {
                        if (Projectile.oldPos[i] != Vector2.Zero)
                            points.Add(Projectile.oldPos[i] + Projectile.Size / 2f);
                    }

                    if (points.Count >= 2)
                    {
                        GraphicsDevice gd = Main.graphics.GraphicsDevice;
                        // Ensure the shader's texture is bound to slot 0 for the primitive draw
                        gd.Textures[0] = DTAssetLib.Trail(2).Value;

                        int count = points.Count;
                        // Two vertices per point (left and right edge), ordered for TriangleStrip
                        var verts = new VertexPositionColorTexture[count * 2];

                        float maxWidth = 18f * Projectile.scale; // width at the base of the trail
                        float minWidth = 2f * Projectile.scale;  // width at the tail

                        for (int i = 0; i < count; i++)
                        {
                            float t = i / (float)(count - 1);
                            // Interpolate width so ribbon tapers
                            float width = MathHelper.Lerp(maxWidth, minWidth, t);

                            Vector2 dir;
                            if (i < count - 1)
                                dir = points[i + 1] - points[i];
                            else
                                dir = points[i] - points[i - 1];

                            if (dir == Vector2.Zero)
                                dir = new Vector2(0, -1);

                            Vector2 normal = Vector2.Normalize(new Vector2(-dir.Y, dir.X));
                            Vector2 left = points[i] + normal * width;
                            Vector2 right = points[i] - normal * width;

                            Color col = ThemeColor * (1f - t) * 1.0f;
                            col.A = (byte)(255 * (1f - t));

                            verts[i * 2] = new VertexPositionColorTexture(new Vector3(left, 0f), col, new Vector2(t, 0f));
                            verts[i * 2 + 1] = new VertexPositionColorTexture(new Vector3(right, 0f), col, new Vector2(t, 1f));
                        }

                        // Draw the triangle strip (primitiveCount = verts.Length - 2)
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, verts, 0, verts.Length - 2);
                    }
                }

                // Return to default batch / blending
                Opus.ReturnToDefaultDrawing(spriteBatch);


                // Zenith trail

                Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
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
                hitbox.Width / 2f - Width / 2f,
                -hitbox.Height / 2f + Height / 2f
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

        public virtual void DefaultBehaviour()
        {
            // Decrease the cooldown timer on each tick
            if (soundCooldown > 0)
            {
                soundCooldown--;
            }

            // Play the sound every 30 ticks
            if (soundCooldown <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item169);
                soundCooldown = 30; // Reset the cooldown to 30 ticks
            }

            Player player = Main.player[Projectile.owner];

            if (Projectile.Distance(player.Center) < 25) // 8 pixels radius
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }
            
            // Always spinning
            Projectile.rotation += 0.4f * Projectile.direction;

              // Generate flying dust effect
            if (Main.rand.NextBool(3)) // 33% chance per tick
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType, Projectile.velocity * 0.2f, 100, DustColor, 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }

            if (Projectile.Distance(player.Center) < 4) // 8 pixels radius
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
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



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundStyle Break = new SoundStyle("DestroyerTest/Assets/Audio/TO_Break") with
            {
                PitchVariance = 0.5f
            };
            // Play impact sound and spawn tile hit effects
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

            return false; // Prevents the projectile from being destroyed on collision
        }

    }
}

