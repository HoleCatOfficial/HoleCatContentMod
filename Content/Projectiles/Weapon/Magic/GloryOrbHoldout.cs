using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Terraria.Audio;
using DestroyerTest.Content.Magic;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class GloryOrbHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2000;
            Projectile.netImportant = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects FX = SpriteEffects.None;
            if ((Projectile.direction == -1 || Projectile.spriteDirection == -1) && player.direction == 1)
            {
                FX = SpriteEffects.FlipVertically;
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, FX, 0);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

       
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<GloryOrb>() && player.controlUseItem == true)
            {
                Vector2 toCursor = Main.MouseWorld - Projectile.Center;
                toCursor.Normalize();
                Projectile.Center = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.ThreeQuarters, toCursor.ToRotation() - MathHelper.PiOver2);
                Projectile.rotation = toCursor.ToRotation() + MathHelper.PiOver2;

                if (player.direction == -1)
                {
                    Projectile.spriteDirection = -1;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                }
                Projectile.direction = toCursor.X > 0 ? 1 : -1;

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi);

                Projectile.ai[0]++;

                if (Projectile.ai[0] == 61)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasersCharge"), Projectile.Center);
                }

                if (Projectile.ai[0] > 120)
                {
                    if (player.CheckMana(200, true))
                    {
                        SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasers") with { PitchVariance = 0.5f }, Projectile.Center);
                        Projectile.ai[0] = 0;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, toCursor * 0.1f, ModContent.ProjectileType<BlessedLaserFriendly>(), Projectile.damage, 10, player.whoAmI, 0f, 0f);
                    }
                }
                else
                {
                    Opus.RingDustInwardRandomDir(DustID.AncientLight, 7, Projectile.Center + new Vector2(0, -20).RotatedBy(Projectile.rotation), 25, 50, Main.DiscoColor, 0.02f, 0.5f);
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

    }
}