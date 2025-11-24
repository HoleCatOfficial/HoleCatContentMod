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

namespace DestroyerTest.Content.Projectiles
{
    public class HekatesMystiqueHoldout : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Magic/HekatesMystique";
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 60;
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

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<HekatesMystique>() && player.channel == true)
            {
                float holdDistance = 60f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

                Projectile.Center = desiredPos;
                Projectile.rotation = toCursor.ToRotation() + MathHelper.PiOver4;

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

                Dust.NewDustPerfect(Projectile.Hitbox.TopRight().RotatedBy(Projectile.rotation), DustID.FireworksRGB, null, 0, new Color(184, 45, 117), 0.75f);
            }
            else
            {
                Projectile.Kill();
            }
        }

    }
}