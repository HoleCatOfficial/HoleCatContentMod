using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.RiftArsenal;
using GlowmaskHelper.Content;
using Terraria.GameContent;
using System.Collections.Generic;

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
            Projectile.localNPCHitCooldown = 15;
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

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 4)
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
            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { MaxInstances = 0 }, Projectile.position);
        }

    }
}