using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Entities;
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
using Terraria.DataStructures;
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
            Projectile.height = 324;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
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

            //Utils.DrawRect(Main.spriteBatch, AdjustedHitbox, ColorLib.Rift);
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
            return IsOnAttackFrame(target) && !target.friendly;
        }

        private SoundStyle Slash = new SoundStyle("DestroyerTest/Assets/Audio/Rift_Katana_Slash") { PitchVariance = 0.2f, Volume = 0.7f };
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<RiftHypersabre>() && player.controlUseItem)
            {
                player.SetDummyItemTime(2);
                AnimateProjectile();

                if (Projectile.frame == 0 || Projectile.frame == 5)
                {
                    SoundEngine.PlaySound(Slash, Projectile.Center);
                }

                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();

                Projectile.Center = mountedCenter;

                Projectile.rotation = toCursor.ToRotation();
                
                FX = toCursor.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

                if (Projectile.ai[0]++ % 60 == 0 && F > 2)
                {
                    F--;
                }

                Projectile.timeLeft = 10;

            }
            else
            {
                Projectile.Kill();
            }
        }

        public Rectangle AdjustedHitbox;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int AdjustedX = (int)Projectile.position.X;
            int AdjustedY = (int)Projectile.position.Y;
            //AdjustedHitbox = new Rectangle(AdjustedX, AdjustedY, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale));
            AdjustedHitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(Projectile.width * Projectile.scale, Projectile.height * Projectile.scale));
            return targetHitbox.Intersects(AdjustedHitbox);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();
            ScreenShake.screenshakeMagnitude = 4;
            ScreenShake.screenshakeTimer = 10;

            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { MaxInstances = 0 }, target.Center);
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
                Spark.PrepareSpark(target.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, choice, 0.75f, false, 30, SparkDrawMode.AlphaBlend);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            Vector2 d = player.Center - target.Center;
            d.Normalize();
            Vector2 d2 = new Vector2(d.X * 0.3f, d.Y);
            if (player.Center.Y <=  target.Center.Y && Math.Abs((player.Center.X - target.Center.X)) <= 50 && !DTConfig.instance.WeaponKickback)
            {
                player.velocity += d2 * 3;
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