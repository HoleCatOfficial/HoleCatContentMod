using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.RiftArsenal;
using GlowmaskHelper.Content;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class RiftHypersabreSwing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 11;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 324;
            Projectile.height = 168;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.netImportant = true;
            Projectile.hide = true;
        }

        private SpriteEffects FX = SpriteEffects.None;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D G = ModContent.Request<Texture2D>($"{Texture}_Glow").Value;

            int frameHeight = t.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                t.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(t.Width / 2f, frameHeight / 2f);


            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, FX, 0f);
            Main.EntitySpriteDraw(G, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, FX, 0f);
            return false;
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

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        List<int> AttackFramesFront = new List<int>
        {
            1,
            2,
            3,
            8,
            9,
            10
        };
        List<int> AttackFramesBack = new List<int>
        {
            6,
            7
        };

        private bool IsOnAttackFrame(NPC target)
        {
            Player player = Main.player[Projectile.owner];

            int playerDir = player.direction;
            int targetDir = Math.Sign(target.Center.X - player.Center.X);

            bool facingTarget = playerDir == targetDir;

            if (facingTarget && AttackFramesFront.Contains(Projectile.frame))
                return true;

            if (!facingTarget && AttackFramesBack.Contains(Projectile.frame))
                return true;

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return IsOnAttackFrame(target);
        }

        private SoundStyle Slash = new SoundStyle("DestroyerTest/Assets/Audio/Rift_Katana_Slash") { PitchVariance = 0.2f };
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<RiftHypersabre>() && player.controlUseItem)
            {
                AnimateProjectile();

                if (Projectile.frame == 0 || Projectile.frame == 5)
                {
                    SoundEngine.PlaySound(Slash, Projectile.Center);
                    //SoundEngine.PlaySound(SoundID.Item132, Projectile.Center);
                }

                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();

                Projectile.Center = mountedCenter;

                Projectile.rotation = toCursor.ToRotation();

                /*
                if (player.direction == -1)
                {
                    Projectile.spriteDirection = -1;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                }
                
                */
                FX = toCursor.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

                if (Projectile.ai[0]++ % 60 == 0 && F > 2)
                {
                    F--;
                }
                

                /*
                Vector2 dustDirection = toCursor;
                Vector2 dustSpawn = Projectile.Center + dustDirection * Projectile.width * 0.5f;

                Vector2 randomSpawn = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));
                int dustIndex = Dust.NewDust(randomSpawn, 0, 0, DustID.FireworksRGB, dustDirection.X * 4f, dustDirection.Y * 4f, 100, ColorLib.Rift, 1.2f);
                Main.dust[dustIndex].noGravity = true;
                */

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
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), target.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.2f), choice * Main.rand.NextFloat(0.5f, 0.8f), 1f);
            }

            
                
        }

    }
}