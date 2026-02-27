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
using OpusLib.Content.Helpers;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class ManifestHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2000;
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
            Main.EntitySpriteDraw(DTAssetLib.ManifestStar.Value, starPos - Main.screenPosition, null, Color.White, starRot, DTAssetLib.ManifestStar.Value.Size() / 2, starScale, SpriteEffects.None, 0f);
            return false;
        }

        public float starRot = 0f;
        public float starScale = 0f;
        public Vector2 starPos;
        public Line beamline;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            starRot += 0.06f;
            beamline = new Line(player.MountedCenter, Main.MouseWorld);

            if (player.HeldItem.type == ModContent.ItemType<Manifest>() && player.channel == true)
            {
                if(starScale < 1f)
                {
                    starScale += 0.05f;
                }
                float holdDistance = 60f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

                starPos = mountedCenter + toCursor * (holdDistance + 42f);

                Projectile.Center = desiredPos;
                Projectile.rotation = beamline.GetLineRotation + MathHelper.PiOver4;

                if (player.direction == -1)
                {
                    Projectile.spriteDirection = -1;
                }
                else
                {
                    Projectile.spriteDirection = 1;
                }
                Projectile.direction = toCursor.X > 0 ? 1 : -1;

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (MathHelper.PiOver2 + MathHelper.PiOver4));
            
                Projectile.ai[0]++;

                if (Projectile.ai[0] % 5 == 0 && starScale >= 1f)
                {
                    if (player.CheckMana(10, true, false))
                    {
                        SoundEngine.PlaySound(SoundID.Item67, starPos);
                        for (int c = 0; c < 2; c++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), starPos, (beamline.GetLineRotation.ToRotationVector2() * 2f).RotatedByRandom(0.3f), ModContent.ProjectileType<ManifestBolt>(), Projectile.damage / 2, 3, Projectile.owner);
                        }
                    }
                }
            }
            else
            {
                starScale = 0f;
                Projectile.Kill();
            }
        }

    }
}