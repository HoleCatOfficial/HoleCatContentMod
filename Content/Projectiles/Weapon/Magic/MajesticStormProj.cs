using System;
using System.Collections.Generic;
using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
 


namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class MajesticStormProj : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
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

        public float r = 0;
        public override void PostDraw(Color lightColor)
        {
            r += 0.1f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.ZapTrail.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 50f * TextureScale, new Color(29, 226, 186), r * -0.2f, 2);



            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, Projectile.Center - Main.screenPosition, null, new Color(29, 226, 186) with { A = 0 }, 0, DTAssetLib.ShineFlare.Value.Size() / 2, 0.5f * TextureScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, Projectile.Center - Main.screenPosition, null, new Color(29, 226, 186) with { A = 0 }, 0, DTAssetLib.ShineFlare.Value.Size() / 2, 0.4f * TextureScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0, DTAssetLib.ShineFlare.Value.Size() / 2, 0.3f * TextureScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, r * 2, DTAssetLib.FeatheredCircle.Value.Size() / 2, 0.7f * TextureScale, SpriteEffects.None);


            Main.EntitySpriteDraw(DTAssetLib.CurseSigilRing.Value, Projectile.Center - Main.screenPosition, null, new Color(29, 226, 186) with { A = 0 }, r, DTAssetLib.CurseSigilRing.Value.Size() / 2, 0.2f * TextureScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.CurseSigilRing.Value, Projectile.Center - Main.screenPosition, null, new Color(29, 226, 186) with { A = 0 }, r * -1.5f, DTAssetLib.CurseSigilRing.Value.Size() / 2, 0.25f * TextureScale, SpriteEffects.None);
        }

       
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

        int KillTimer = 0;
        int KillIndex = -1;

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
                Projectile.SmoothMoveToPoint(Main.MouseWorld, 20, 200f);

                if (PitchVal < 0)
                {
                    PitchVal += 0.1f;
                }

                if (player.channel)
                {
                    if (Main.rand.NextBool(10) && ownedOrbs.Count <= 40)
                    {
                        Vector2 OuterPos = Projectile.OldCenter()[5];
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
                if (TextureScale > 0f)
                {
                    TextureScale -= 0.05f;
                    Projectile.velocity *= 0.9f;
                }
                else
                {
                    KillTimer++;

                    if (KillTimer % 3 == 0)
                    {
                        while (KillIndex < ownedOrbs.Count - 1)
                        {
                            KillIndex++;

                            if (ownedOrbs[KillIndex].active)
                            {
                                ownedOrbs[KillIndex].Kill();
                                ownedOrbs[KillIndex].active = false;
                                break;
                            }
                        }

                        if (KillIndex >= ownedOrbs.Count - 1)
                        {
                            Projectile.Kill();
                        }
                    }
                }

            }

        }

        public void UpdateOwnedSubProj()
        {
           
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(20, 20);
        }
        

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/VoltaicFenzimImpact") { PitchVariance = 0.2f, MaxInstances = 0 });
            BloomRingSharp Ring = new BloomRingSharp();
            Ring.Prepare(Projectile.Center, Vector2.Zero, new Color(29, 226, 186) * 0.1f, 0.02f, 0.01f, 0.6f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Ring);

            for (int u = 0; u < 15; u++)
            {
                Vector2 outer = Projectile.Center + Main.rand.NextVector2CircularEdge(220f, 220f);
                var curve = DTUtils.EasyBezier(Projectile.Center, Projectile.Center.DirectionTo(outer).RotatedByRandom(3f), outer, outer.DirectionFrom(Projectile.Center).RotatedByRandom(3f), 0.5f);
                var points = curve.GetEvenlySpacedPoints(15);
                for (int i = 0; i < points.Count; i++)
                {
                    PointGlowPreMultiplied ArcGlow = new();
                    ArcGlow.Initialize(points[i], Main.rand.NextVector2Circular(0.2f, 0.2f), new Color(29, 226, 186) with { A = 0 } * 0.05f, 2.5f);
                    ParticleEngine.BehindProjectiles.Add(ArcGlow);

                    PixelParticle Arc = new();
                    Arc.Initialize(points[i], Vector2.Zero, new Color(29, 226, 186) with { A = 0 }, 2f, 60);
                    ParticleEngine.BehindProjectiles.Add(Arc);
                }


            }
        }
    }
}