using System;
using System.Collections.Generic;
using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class TenebrisElementalMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            Main.projPet[Type] = true;
        }

        int Variant = 0;
        Color Col = Color.White;
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000; // stays alive indefinitely while buff is active
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.minionSlots = 1f;
            Projectile.frame = Variant = Main.rand.Next(3);

            switch (Variant)
            {
                case 0:
                    {
                        Col = ColorLib.TenebrisMagenta;
                        break;
                    }
                case 1:
                    {
                        Col = ColorLib.TenebrisBlue;
                        break;
                    }
                case 2:
                    {
                        Col = ColorLib.TenebrisBeige;
                        break;
                    }
            }

        }

        float shadrot = 0;

        float glowOpacity = 0f;


        public override bool PreDraw(ref Color lightColor)
        {

            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(14, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 30f, Col, shadrot * -0.5f);

            shadrot += 0.1f;
            for (int i = 0; i < 4; i++)
            {
                DrawData data = DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 } * 0.5f) with { position = (Projectile.Center + new Vector2(4f, 0).RotatedBy((MathHelper.PiOver2 * i) + shadrot)) - Main.screenPosition };
                Main.EntitySpriteDraw(data);

                DrawData data2 = DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 } * 0.15f) with { position = (Projectile.Center + new Vector2(8f, 8f).RotatedBy((MathHelper.PiOver2 * i) + shadrot)) - Main.screenPosition };
                Main.EntitySpriteDraw(data2);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White with { A = 0 } * glowOpacity));
            Opus.DrawTextureOnProj(DTAssetLib.SparkSmoothThin, Projectile, Col with { A = 0 } * glowOpacity, false, 0, 0.4f, 0.1f);
            Opus.DrawTextureOnProj(DTAssetLib.SparkSmoothThin, Projectile, Col with { A = 0 } * glowOpacity, false, MathHelper.PiOver2, 0.4f, 0.1f);
            return false;
        }


        Player Owner => Main.player[Projectile.owner];
        int ShootIndex = 0;
        bool fire = false;
        public override void AI()
        {

            if (!Valid(Owner))
            {
                Projectile.Kill();
                return;
            }

            // Always reset lifetime while valid
            Projectile.timeLeft = 2;

            var organizer = Owner.GetModPlayer<TEOrganizer>();
            List<Projectile> summons = organizer.Summons;
            int count = summons.Count;

            int myIndex = summons.IndexOf(Projectile);

            if (myIndex < 0)
                return;

            if (glowOpacity > 0)
            {
                glowOpacity -= 0.025f;
            }

            bool Alt = myIndex % 2 == 0;


            Vector2[] array = new Vector2[count];
            float num = MathF.PI * 2f / count;
            float num2 = (float)Main.GameUpdateCount * (Utils.Clamp(0.001f * count, 0.001f, 0.5f));

            for (int i = 0; i < count; i++)
            {
                bool alt = i % 2 == 0;

                float individualRadius = alt
                    ? Opus.Sine(50f, 200f, 0.005f * count)
                    : Opus.Sine(200f, 50f, 0.005f * count);

                float angle = num2 + num * i;

                array[i] = Owner.MountedCenter + new Vector2(individualRadius, 0f).RotatedBy(angle);
            }

            Vector2[] Positions = array;


            Vector2 targetPosition = Positions[myIndex];


            if (Projectile.Distance(targetPosition) < 2000)
            {
                Projectile.SmoothMoveToPoint(targetPosition, 30f, 30);
            }
            else
            {
                Projectile.Center = targetPosition;
            }


            int targetIndex = Projectile.AutoTarget(3200);

            if (targetIndex == -1)
            {
                return;
            }

            NPC Target = Main.npc[targetIndex];

            if (organizer.ShootTimer <= organizer.ShootStartup)
            { 
                return;
            }

            if (organizer.ShootIndex != myIndex)
            {
                fire = false;
                return;
            }


            if (!fire)
            {
                SoundEngine.PlaySound(SoundID.LiquidsWaterLava, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item100, Projectile.Center);
                SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/TenebrisBlock") { MaxInstances = 0, PitchVariance = 0.2f, Volume = 0.4f }, Projectile.Center);

                glowOpacity = 1f;

                for (int i = 0; i < 16; i++)
                {
                    Fire fire = new();
                    fire.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), DTUtils.RandomDirection(2), ColorLib.TenebrisGradient * 0.5f, 0.7f, 60, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
                    ParticleEngine.Particles.Add(fire);
                }

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.Center.DirectionTo(Target.Center).RotatedByRandom(1f) * 40f, ModContent.ProjectileType<TenebrisElementalMinionShot>(), Projectile.damage, 4, Projectile.owner);
                fire = true;
            }

        }

        public bool Valid(Player player)
        {
            return player != null && player.active && !player.dead && player.HasBuff<TenebrisCrystalMinionBuff>();
        }
      

    }

    public class TEOrganizer : ModPlayer
    {
        public List<Vector2> Positions = new();
        public List<Projectile> Summons => Main.projectile.Where(n => n.type == ModContent.ProjectileType<TenebrisElementalMinion>() && n.owner == Player.whoAmI && n.ai[0] == 0 && n.active).ToList();
        public int ShootIndex;
        public int ShootTimer;
        public int ShootInterval = 6;
        public int ShootStartup = 120;

        public override void PostUpdateBuffs()
        {

            int summonCount = Summons.Count;
            UpdateShooting(summonCount);

        }

        public void UpdateShooting(int count)
        {
            if (count <= 0)
            {
                ShootIndex = 0;
                ShootTimer = 0;
                return;
            }

            ShootTimer++;

            if (ShootTimer <= ShootStartup)
                return;

            if (ShootTimer% ShootInterval == 0)
            {
                ShootIndex++;

                if (ShootIndex >= count)
                    ShootIndex = 0;
            }
        }
    }
}
