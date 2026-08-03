using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.RiftArsenal;
 
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class RiftClaymoreSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 11;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 380;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.hide = true;
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
        }

        int F = 6;
        private void AnimateProjectile() 
        {
            if (++Projectile.frameCounter >= F) 
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public bool Back = false;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (!Back)
            {
                if (behindProjectiles.Contains(index))
                {
                    behindProjectiles.Remove(index);
                }
                overPlayers.Add(index);
            }
            else
            {
                if (overPlayers.Contains(index))
                {
                    overPlayers.Remove(index);
                }
                behindProjectiles.Add(index);
            }
        }

        List<int> AttackFramesFront = new List<int>
        { 
            0, 
            1, 
            2, 
            3,
            4,
            5,
            6
        };
        List<int> AttackFramesBack = new List<int>
        {
            7,
            8,
            9,
            10
        };

        List<int> AttackFramesFrontHit = new List<int>
        {
            4,
            5,
            6,
            7,
            8,
        };
        List<int> AttackFramesBackHit = new List<int>
        {
            0,
            1,
            2,
            10
        };

        private bool IsOnAttackFrame(NPC target)
        {
            Player player = Main.player[Projectile.owner];

            int playerDir = player.direction;
            int targetDir = Math.Sign(target.Center.X - player.Center.X);

            bool facingTarget = playerDir == targetDir;

            if (facingTarget && AttackFramesFrontHit.Contains(Projectile.frame))
                return true;

            if (!facingTarget && AttackFramesBackHit.Contains(Projectile.frame))
                return true;

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return IsOnAttackFrame(target) && Projectile.ManualCanHitFriendly(target);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.Center;

            Projectile.direction = player.direction;
            Projectile.spriteDirection = Projectile.direction;

            if (player.HeldItem.type == ModContent.ItemType<RiftClaymore>() && player.controlUseItem)
            {
                player.ChangeDir(Projectile.direction);
                player.SetDummyItemTime(2);

                AnimateProjectile();

                if (AttackFramesFront.Contains(Projectile.frame))
                {
                    Back = false;
                }
                if (AttackFramesBack.Contains(Projectile.frame))
                {
                    Back = true;
                }

                if (Projectile.frame == 0 || Projectile.frame == 5)
                {
                    SoundEngine.PlaySound(DTAssetLib.SwordSounds.BigBasicSwing, Projectile.Center);
                }

                if (Projectile.ai[0]++ % 60 == 0 && F > 1)
                {
                    F--;
                }
                if (F <= 1)
                {
                    if (Math.Abs(player.velocity.Y) < 4)
                    {
                        player.velocity.Y -= 0.5f;
                    }
                }

                Projectile.timeLeft = 10;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();
            ScreenShake.screenshakeMagnitude = 4;
            ScreenShake.screenshakeTimer = 10;

            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { MaxInstances = 0 }, Projectile.position);
            List<Color> RiftLightColors = new List<Color>
            {
                ColorLib.Rift,
                ColorLib.LightRift1,
                ColorLib.LightRift2,
                ColorLib.LightRift3,
                ColorLib.LightRift4,
                Color.White
            };

            int splatterdir = target.position.X > player.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                Color choice = RiftLightColors[Main.rand.Next(RiftLightColors.Count)];
                Spark Spark = new Spark();
                Spark.PrepareSpark(target.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, choice, 0.75f, false, 30, SparkDrawMode.NonPremultiplied);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            var modPlayer = player.GetModPlayer<LivingShadowPlayer>();
            if (modPlayer.LivingShadowCurrent > 0)
            {
                hit.SourceDamage = (int)(hit.SourceDamage * 1.5f);
                target.AddBuff(ModContent.BuffType<HeliouricShock>(), 240);
            }
        }
    }
}