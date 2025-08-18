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

namespace DestroyerTest.Content.Projectiles
{
    public class FungalScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.BlueViolet;
            WidthDim = 44;
            HeightDim = 44;
            DustType = DustID.GlowingMushroom;
            base.SetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 9; i++)
                {
                    float angle = MathHelper.TwoPi * i / 9f;
                    Vector2 spawnOffset = new Vector2(0, -120f).RotatedBy(angle);
                    Vector2 spawnPos = Projectile.Center + spawnOffset;
                    Vector2 toMouse = (Main.MouseWorld - spawnPos).SafeNormalize(Vector2.Zero) * 2f; // slow speed

                    Projectile.NewProjectile(
                        Entity.GetSource_FromThis(),
                        spawnPos,
                        toMouse,
                        ModContent.ProjectileType<FungalScepterMushroom>(),
                        (int)(Projectile.damage * 0.5f),
                        (int)(Projectile.knockBack * 0.5f),
                        Projectile.owner
                    );
                }
            base.OnSpawn(source);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 9; i++)
            {
                float angle = MathHelper.TwoPi * i / 9f;
                Vector2 spawnOffset = new Vector2(0, -120f).RotatedBy(angle);
                Vector2 spawnPos = Projectile.Center + spawnOffset;
                Vector2 toMouse = (Main.MouseWorld - spawnPos).SafeNormalize(Vector2.Zero) * 2f; // slow speed

                Projectile.NewProjectile(
                    Entity.GetSource_FromThis(),
                    spawnPos,
                    toMouse,
                    ModContent.ProjectileType<FungalScepterMushroom>(),
                    (int)(Projectile.damage * 0.5f),
                    (int)(Projectile.knockBack * 0.5f),
                    Projectile.owner
                );
            }
            return base.OnTileCollide(oldVelocity);
        }

        public override void AI()
        {

            
                
            
                
            
            base.AI();
        }
    }
}

