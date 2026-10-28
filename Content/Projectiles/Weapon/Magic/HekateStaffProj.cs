using System;
using System.Collections.Generic;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.RiftArsenal;
 
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class HekateStaffProj : ModProjectile
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

            
            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(8, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 50f * TextureScale, new Color(184, 45, 117), r * -0.2f, 2);
            


            Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, Projectile.Center - Main.screenPosition, null, new Color(184, 45, 117) with { A = 0 }, r, DTAssetLib.Swirl.Value.Size() / 2, 0.5f * TextureScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, Projectile.Center - Main.screenPosition, null, new Color(184, 45, 117) with { A = 0 }, r * 1.5f, DTAssetLib.Swirl.Value.Size() / 2, 0.4f * TextureScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Swirl.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, r * 2, DTAssetLib.Swirl.Value.Size() / 2, 0.3f * TextureScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, r * 2, DTAssetLib.FeatheredCircle.Value.Size() / 2, 0.7f * TextureScale, SpriteEffects.None);


            Main.EntitySpriteDraw(DTAssetLib.CurseSigilRing.Value, Projectile.Center - Main.screenPosition, null, new Color(184, 45, 117) with { A = 0 }, r, DTAssetLib.CurseSigilRing.Value.Size() / 2, 0.2f * TextureScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.CurseSigilRing.Value, Projectile.Center - Main.screenPosition, null, new Color(184, 45, 117) with { A = 0 }, r * -1.5f, DTAssetLib.CurseSigilRing.Value.Size() / 2, 0.25f * TextureScale, SpriteEffects.None);
        }


        public int Leeway = 15;

        public float TextureScale = 1f;
        SlotId LoopSlot;
        public SoundStyle Loop = new SoundStyle("DestroyerTest/Assets/Audio/AuraLoop/ShadowflameAuraLoop") 
        { 
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
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

        public List<Projectile> ownedEmbers = new List<Projectile>();
        int KillTimer = 0;
        int KillIndex = -1;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Color(184, 45, 117).ToVector3());

            Projectile.ResetExcessTrailPoints();
            

           

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


                if (Main.rand.NextBool(10))
                {
                    Vector2 OuterPos = Projectile.Center + Main.rand.NextVector2CircularEdge(100, 100);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), OuterPos, Vector2.Zero, ModContent.ProjectileType<HekateStaffEmber>(), Projectile.damage / 10, 4, Projectile.owner);
                }
            }
            else
            {
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

                Projectile.timeLeft = 1200;
                Projectile.SmoothMoveToPoint(Main.MouseWorld, 20, 200f);

                if (PitchVal < 0)
                {
                    PitchVal += 0.1f;
                }

                if (player.channel)
                {
                    if (Main.rand.NextBool(10) && ownedEmbers.Count <= 30)
                    {
                        Vector2 OuterPos = Projectile.oldPos[5];
                        Projectile Ember = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), OuterPos, Vector2.Zero, ModContent.ProjectileType<HekateStaffEmber>(), Projectile.damage / 4, 4, Projectile.owner);

                        ownedEmbers.Add(Ember);
                        
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
                        while (KillIndex < ownedEmbers.Count - 1)
                        {
                            KillIndex++;

                            if (ownedEmbers[KillIndex].active)
                            {
                                ownedEmbers[KillIndex].Kill();
                                ownedEmbers[KillIndex].active = false;
                                break;
                            }
                        }

                        if (KillIndex >= ownedEmbers.Count - 1)
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
            SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HekateStaffExplosion>(), Projectile.damage, 12, Projectile.owner);
        }
    }
}