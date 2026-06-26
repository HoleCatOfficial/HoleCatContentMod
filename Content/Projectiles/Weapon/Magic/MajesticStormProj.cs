using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using ReLogic.Utilities;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;


namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class MajesticStormProj : ModProjectile
    {
            
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DrawCrystalCore(spriteBatch, Projectile.Center);
        }
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center)
        {
            // Helper method from a utility mod.
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            for (int i = 0; i < TrailPositions.Count; i++)
            {
                float progress = i / (float)TrailLength;
                float scale = MathHelper.Lerp(1.5f, 0.0005f, progress);
                Color color = new Color(29, 226, 186);

                Main.EntitySpriteDraw(
                    DTAssetLib.FeatheredCircle.Value,
                    TrailPositions[i] - Main.screenPosition,
                    null,
                    color,
                    TextureRotationOffset,
                    DTAssetLib.FeatheredCircle.Value.Size() / 2f,
                    scale * TextureScale,
                    SpriteEffects.None,
                    0
                );
            }
            
            for (int i = 0; i < TrailPositions.Count; i++)
			{
				float progress = i / (float)TrailLength;
				float scale = MathHelper.Lerp(1f, 0.0005f, progress);
				Color color = Color.White;

				Main.EntitySpriteDraw(
					DTAssetLib.FeatheredCircle.Value,
					TrailPositions[i] - Main.screenPosition,
					null,
					color,
					Projectile.rotation,
					DTAssetLib.FeatheredCircle.Value.Size() / 2f,
					scale * TextureScale,
					SpriteEffects.None,
					0
				);
			}

            Main.spriteBatch.Draw(
                DTAssetLib.FeatheredCircle.Value,
                Center - Main.screenPosition,
                null,
                new Color(29, 226, 186),
                TextureRotationOffset,
                DTAssetLib.FeatheredCircle.Value.Size() / 2f,
                1.5f * TextureScale,
                SpriteEffects.None,
                1f
            );

            Main.spriteBatch.Draw(
                DTAssetLib.FeatheredCircle.Value,
                Center - Main.screenPosition,
                null,
                Color.White,
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                1f * TextureScale,
                SpriteEffects.None,
                1f
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        
        
        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
        private const int TrailLength = 40;
        public float TextureRotationOffset = 0f;
        public float TextureScale = 1f;
        public int Leeway = 15;

        SlotId LoopSlot;
        public SoundStyle Loop = DTAssetLib.ElectricLoopSound(3) with
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        public SoundStyle Killed = new SoundStyle("DestroyerTest/Assets/Audio/RiftCharge") 
        { 
            MaxInstances = 0,
            PitchVariance = 0.5f
        };
        public float PitchVal = -3;

        public int manaTimer = 0;

        public bool shouldActAsChanneling(Player player)
        {
            // Basic: must be channeling
            if (!player.channel || player.noItems || player.CCed)
                return false;

            int useTime = player.inventory[player.selectedItem].useTime;
            int manaCost = player.inventory[player.selectedItem].mana;

            // Increment the timer each call (AI calls this every tick)
            manaTimer++;

            // If we reached the mana drain point…
            if (manaTimer >= useTime)
            {
                manaTimer = 0;

                // Attempt to consume mana
                if (!player.CheckMana(manaCost, true, false))
                {
                    // Not enough mana → stop channeling immediately
                    return false;
                }
            }

            // If we are between drain intervals → continue normally
            return true;
        }

        public List<Projectile> ownedOrbs = new List<Projectile>();

        public override void AI()
        {
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            TextureRotationOffset -= 0.5f;
            Lighting.AddLight(Projectile.Center, new Color(29, 226, 186).ToVector3());
          

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, 0f, 0f, 0, new Color(29, 226, 186), 0.5f);
            }

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound)) {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    soundInstance.Position = Projectile.Center;
                    soundInstance.Pitch = PitchVal;
                    return tracker.IsActiveAndInGame();
                });
            }
            else
            {
                activeSound.Position = Projectile.Center;
                activeSound.Pitch = PitchVal;
            }

            UpdateOwnedSubProj();

            Player player = Main.player[Projectile.owner];
            /*
            if (player.channel)
            {
                if (PitchVal < 0)
                {
                    PitchVal += 0.1f;
                }
                Projectile.timeLeft = 100;
                Leeway = 60;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f, 0.1f);


                
            }
            else
            {
                // Leeway is used for Mana flower support, since the weapon is functionally useless if the channel is constantly breaking on you due to lack of mana.
                // Ideally, this should allow a window to return to the channeling behaviour, since it likely takes more than a few ticks for the mana flower to consume a mana potion.
                Leeway--;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f, 0.1f);
                if (Leeway <= 0)
                {
                    Projectile.Kill();
                }
            }
            */

            

            if (shouldActAsChanneling(player))
            {
                if (!player.channel)
                {
                    TextureScale -= 0.1f;
                    Leeway--;
                }

                Projectile.timeLeft = 100;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 12f, 0.1f);

                if (PitchVal < 0)
                {
                    PitchVal += 0.1f;
                }

                if (player.channel)
                {
                    if (Main.rand.NextBool(10) && ownedOrbs.Count <= 40)
                    {
                        Vector2 OuterPos = Projectile.Center + Main.rand.NextVector2CircularEdge(100, 100);
                        Projectile Orb = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), OuterPos, Vector2.Zero, ModContent.ProjectileType<MajesticStormOrb>(), Projectile.damage / 2, 4, Projectile.owner);
                        if (Orb.owner == Projectile.owner)
                        {
                            ownedOrbs.Add(Orb);
                        }
                    }
                    Leeway = 60;
                    TextureScale = 1f;
                }
            }
            else
            {
                Projectile.Kill();
            }

        }

        public void UpdateOwnedSubProj()
        {
            foreach (Projectile p in Main.projectile)
            {
                if (ownedOrbs.Contains(p) && !p.active)
                {
                    ownedOrbs.Remove(p);
                }
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(20, 20);
        }
        

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Killed, Projectile.Center);
            for (int u = 0; u < 15; u++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2CircularEdge(10, 10), 0, new Color(29, 226, 186), 2);
            }
        }
    }
}