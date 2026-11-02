using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class FrostScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.SkyBlue;
            WidthDim = 58;
            HeightDim = 48;
            DustType = DustID.Frost;
            base.SetDefaults();
        }

        public override void AI()
        {
            if (Main.rand.NextBool(18))
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 Direction = Main.rand.NextVector2CircularEdge(1f, 1f); // Random unit vector on circle edge
                    Vector2 velocity = Direction * 3f; // 6f = desired projectile speed

                    Projectile.NewProjectile(
                        Entity.GetSource_FromThis(),
                        Projectile.Center,
                        velocity,
                        ProjectileID.IceBolt,
                        (int)(Projectile.damage * 0.15f),
                        (int)(Projectile.knockBack * 0.5f),
                        Projectile.owner
                    );
                }
            }
            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 600);
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}

