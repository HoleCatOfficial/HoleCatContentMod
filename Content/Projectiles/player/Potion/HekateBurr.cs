using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.DataStructures;
using DestroyerTest.Content.Buffs;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.player.Potion
{
    public class HekateBurr : ModProjectile
    {
        private const int ringLifetime = 300;
        private const float MaxDistFromPlayer = 100f;
        private const float InterpolationSpeed = 0.1f;
        public NPC AttachedNPC
        {
            get
            {
                if (attachedNPCIndex < 0 || attachedNPCIndex >= Main.maxNPCs)
                    return null;

                NPC npc = Main.npc[attachedNPCIndex];
                return npc.active ? npc : null;
            }
        }
        public int attachedNPCIndex = -1;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = ringLifetime;
            Projectile.frame = Main.rand.Next(3);
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
            return false;
        }

        public int RingIndex = -1;
        public int RingSize = 12;
        public bool IsAttached => attachedNPCIndex != -1;


        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            var modPlayer = player.GetModPlayer<HekateBurrPlayer>();

            RingIndex = modPlayer.GetFreeRingIndex();

            if (RingIndex < 0)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = Main.rand.NextFloat(MathHelper.Pi);

        }



        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Never attached yet
            if (attachedNPCIndex == -1)
            {
                UpdateRingPosition(player);
                return;
            }

            // Attached target died/despawned
            if (!Main.npc[attachedNPCIndex].active)
            {
                Projectile.Kill();
                return;
            }

            StickToNPC();
        }

        private void UpdateRingPosition(Player player)
        {
            Projectile.timeLeft = ringLifetime;
            float anglePer = MathF.Tau / RingSize;
            float targetAngle = anglePer * RingIndex;
            float radius = 20f;

            Vector2 targetPos =
                player.Center + targetAngle.ToRotationVector2() * radius;

            Vector2 toTarget = targetPos - Projectile.Center;

            if (toTarget.Length() > MaxDistFromPlayer)
            {
                Projectile.Center = targetPos;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    toTarget,
                    InterpolationSpeed
                );
            }
        }


        private void StickToNPC()
        {
            NPC npc = Main.npc[attachedNPCIndex];

            Projectile.Center = npc.Center;
            Projectile.velocity = Vector2.Zero;

            if (Projectile.timeLeft % 20 == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);

                if (!Main.hardMode)
                    npc.AddBuff(BuffID.Poisoned, 300);
                else
                    npc.AddBuff(BuffID.Venom, 300);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (attachedNPCIndex == -1)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.ModProjectile is HekateBurr burr)
                    {
                        if (burr.attachedNPCIndex != target.whoAmI)
                        {
                            attachedNPCIndex = target.whoAmI;
                        }
                    }
                }
                Projectile.timeLeft = ringLifetime; // start decay now
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item80, Projectile.Center);
            if (!Main.hardMode)
            {
                Opus.RadialSpreadDustRandom(DustID.Poisoned, 16, Projectile.Center, 100, default, 0.9f, 2f);
                Opus.RadialSpreadDustRandom(DustID.Poisoned, 9, Projectile.Center, 60, default, 1f, 3f);
                Opus.RadialSpreadDustRandom(DustID.Poisoned, 4, Projectile.Center, 0, default, 1.8f, 5f);
            }
            else
            {
                Opus.RadialSpreadDustRandom(DustID.Venom, 16, Projectile.Center, 100, default, 0.9f, 2f);
                Opus.RadialSpreadDustRandom(DustID.Venom, 9, Projectile.Center, 60, default, 1f, 3f);
                Opus.RadialSpreadDustRandom(DustID.Venom, 4, Projectile.Center, 0, default, 1.8f, 5f);
            }
        }

    }
}