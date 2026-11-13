using System;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class GhostBoaHoldout : ModProjectile
    {
        Asset<Texture2D> Glowmask;

        public override void SetStaticDefaults()
        {
            Glowmask = ModContent.Request<Texture2D>(Texture + "_Glow");
        }
        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.timeLeft = 60;
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        float glowScale = 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            Glowmask = ModContent.Request<Texture2D>(Texture + "_Glow");

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, texture.Height);
                effects = SpriteEffects.None;
                rotationOffset = MathHelper.ToRadians(45f);
            }
            else
            {
                origin = new Vector2(0, 0);
                effects = SpriteEffects.FlipVertically;
                rotationOffset = MathHelper.ToRadians(-45f);
            }


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, (Projectile.rotation + rotationOffset), origin, Projectile.scale, effects, 0);

            Main.EntitySpriteDraw(Glowmask.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation + rotationOffset), origin, Projectile.scale, effects, 0);

            DrawShine();


            //Utils.DrawBorderString(Main.spriteBatch, time.ToString(), Projectile.position - Main.screenPosition, Color.Red);

            return false;
        }

        float rot = 0f;
        void DrawShine()
        {
            rot += 0.1f;
            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, PT - Main.screenPosition, null, Color.DeepSkyBlue with { A = 0 }, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.5f, 0.8f, 0.4f) * glowScale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, PT - Main.screenPosition, null, Color.DeepSkyBlue with { A = 0 }, rot, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.3f, 0.6f, 0.4f) * glowScale, SpriteEffects.None);

            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, PT - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.4f, 0.7f, 0.4f) * glowScale, SpriteEffects.None);
        }
    
        public override void OnSpawn(IEntitySource source)
        {
            Vector2 Dir = Main.MouseWorld - Projectile.Center;
            Dir.Normalize();

            var SpawnPT = Dir * 50;

            Projectile.rotation = Dir.ToRotation();
            Projectile.ai[0] = 0;
        }

        SlotId ChargeSlot;
        public SoundStyle charge = new SoundStyle(DTAssetLib.AudioFolder.ChargeSounds + "/GhostBoaCharge") { MaxInstances = 0, PitchVariance = 0.3f, PauseBehavior = PauseBehavior.PauseWithGame };
        ActiveSound chargeInstance;
        float ChargeProgress = 0f;
        int time = 0;
        int Maxtime = 270;

        public Vector2 PT;

        int firetime = 0;
        int fireAmount = 1;


        public override void AI()
        {
            ChargeProgress = (float)time / (float)Maxtime;

            fireAmount = (int)MathHelper.Lerp(1f, 5f, ChargeProgress);
            
            Vector2 Dir = Main.MouseWorld - Projectile.Center;
            Dir.Normalize();

            PT = Projectile.Center + new Vector2(80, 0).RotatedBy(Projectile.rotation);

            float TargetRot = Dir.ToRotation();

            Projectile.rotation = Projectile.rotation.AngleLerp(TargetRot, 0.1f);



            Projectile.Center = Owner.Center;
            Owner.heldProj = Projectile.whoAmI;
            Projectile.spriteDirection = Math.Sign((Main.MouseWorld - Owner.MountedCenter).X);


            glowScale = MathHelper.Lerp(0f, 1f, ChargeProgress);

            Lighting.AddLight(PT, Color.DeepSkyBlue.ToVector3() * glowScale);

            if (Owner.controlUseItem && !Owner.dead && Owner.HeldItem.type == ModContent.ItemType<GhostBoa>() && Owner.CheckMana((int)(4 * (0.01f * Owner.manaRegen)), true, false))
            {
                if (time < Maxtime)
                {
                    time++;
                }
                firetime = 0;
                Projectile.ai[1] = 0;

                Owner.SetDummyItemTime(10);
                Projectile.timeLeft = 10;

                if (time == 1)
                {

                    if (!SoundEngine.TryGetActiveSound(ChargeSlot, out var activeSound))
                    {
                        var tracker = new ProjectileAudioTracker(Projectile);
                        ChargeSlot = SoundEngine.PlaySound(charge, Projectile.Center, soundInstance =>
                        {
                            soundInstance.Position = PT;
                            return tracker.IsActiveAndInGame();
                        });
                    }
                    else
                    {
                        activeSound.Position = PT;
                    }
                }

                if (time == Maxtime - 1)
                {
                    SoundEngine.PlaySound(SoundID.Zombie53, PT);
                }

                if (time < Maxtime)
                {
                    float ArcStart = Projectile.rotation - MathHelper.PiOver4;
                    float ArcEnd = Projectile.rotation + MathHelper.PiOver4;
                    float angle = Main.rand.NextFloat(ArcStart, ArcEnd);
                    Vector2 Outer = PT + new Vector2(90, 0).RotatedBy(angle);
                    Vector2 inward = Outer.DirectionTo(PT);

                    PointGlowPreMultiplied particle = new();
                    particle.Initialize(Outer, inward, Color.DeepSkyBlue, 0.5f);
                    ParticleEngine.Particles.Add(particle);

                }
                firetime = 5;
            }
            else
            {
                firetime++;

                if (SoundEngine.TryGetActiveSound(ChargeSlot, out var activeSound))
                {
                    activeSound.Stop();
                }

                if (Projectile.ai[1] < fireAmount)
                {
                    Owner.SetDummyItemTime(10);
                    Projectile.timeLeft = 10;
                }

                if (firetime % 5 == 0)
                {
                    Projectile.ai[1]++;

                    
                    SoundEngine.PlaySound(DTAssetLib.Impacts.HellWeaponImpact, PT);
                    SoundEngine.PlaySound(DTAssetLib.Impacts.WindHit, PT);

                    if (fireAmount >= 5)
                    {
                        SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/DAHit") with { MaxInstances = 0, PitchVariance = 0.5f, Pitch = -1f }, PT);
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        PointGlowPreMultiplied particle1 = new();
                        particle1.Initialize(PT, Main.rand.NextVector2Circular(2f, 2f), Color.DeepSkyBlue, 0.5f);
                        ParticleEngine.Particles.Add(particle1);

                        float ArcStart = Projectile.rotation - MathHelper.PiOver4;
                        float ArcEnd = Projectile.rotation + MathHelper.PiOver4;
                        float angle = Main.rand.NextFloat(ArcStart, ArcEnd);
                        Vector2 Outer = PT + new Vector2(90, 0).RotatedBy(angle);
                        Vector2 outward = Outer.DirectionFrom(PT);

                        PointGlowPreMultiplied particle2 = new();
                        particle2.Initialize(PT, outward * Main.rand.NextFloat(1f, 2f), Color.DeepSkyBlue, 0.5f);
                        ParticleEngine.Particles.Add(particle2);
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), PT, Dir.RotatedByRandom(0.1f) * 2f, ModContent.ProjectileType<SpectralCharge1>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                    if (fireAmount >= 5)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), PT, Dir.RotatedByRandom(0.5f) * 10f, ProjectileID.SpectreWrath, Projectile.damage / 4, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
                
            }
            
        }




        Player Owner => Main.player[Projectile.owner];

        public void SetPosition()
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
            Projectile.scale = 1f * Owner.GetAdjustedItemScale(Owner.HeldItem);

            Owner.heldProj = Projectile.whoAmI;
        }


    }
}
