using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Scepter;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class ThunderScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.SkyBlue;
            WidthDim = 36;
            HeightDim = 34;
            DustType = DustID.Electric;
            base.SetDefaults();
        }

        public override void DefaultBehaviour()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += 0.4f * Projectile.direction;

            if (Main.rand.NextBool(12))
            {
                for (int r = 0; r < 5; r++)
                {
                    Vector2 Offset = Projectile.Center + new Vector2(10, 0).RotatedByRandom(MathHelper.TwoPi);
                    Vector2 shootdir = Offset - Projectile.Center;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootdir * 0.2f, ProjectileID.ThunderStaffShot, (int)(Projectile.damage * 0.15), 0, Projectile.owner);
                }
            }
            if (player.controlUseTile && player.HeldItem.type == ModContent.ItemType<ThunderScepter>())
            {
                returning = false;
                Projectile.Center = Main.MouseWorld;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                returning = true;
            }

            if (returning)
            {
                ArmCatchAnimate(player);
                // InPhase: Smooth return using Lerp
                Vector2 returnDirection = player.Center - Projectile.Center;
                float speed = MathHelper.Lerp(Projectile.velocity.Length(), 15f, 0.8f); // Smooth acceleration
                Projectile.velocity = returnDirection.SafeNormalize(Vector2.Zero) * speed;

                // If close enough, remove the projectile
                if (Projectile.Distance(player.Center) < 8) // 8 pixels radius
                {
                    HitCount = 0;
                    existenceTimer = 0;
                    Projectile.Kill();
                }
            }
        }
    }
}

