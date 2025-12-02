using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.DataStructures;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class MoltenScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = new Color(253, 62, 3);
            WidthDim = 68;
            HeightDim = 68;
            DustType = DustID.Lava;
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
                        ModContent.ProjectileType<LavaBlob>(),
                        (int)(Projectile.damage * 0.5f),
                        (int)(Projectile.knockBack * 0.5f),
                        Projectile.owner
                    );
                }
            }
            base.AI();
        }
    }
}

