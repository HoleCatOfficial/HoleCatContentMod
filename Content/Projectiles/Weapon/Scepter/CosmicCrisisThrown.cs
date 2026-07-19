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
using System;
using Terraria.DataStructures;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class CosmicCrisisThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.OrangeRed;
            WidthDim = 40;
            HeightDim = 40;
            DustType = DustID.OrangeTorch;
            base.SetDefaults();
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            base.AI();
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(BuffID.OnFire, 240);

            for (int i = 0; i < 3; i++)
            {
                Vector2 AboveTarget = target.Center + new Vector2(Main.rand.NextFloat(-300, 300), -900);
                Vector2 ToTarget = target.Center - AboveTarget;
                ToTarget.Normalize();

                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), AboveTarget, ToTarget * 27, ModContent.ProjectileType<MoltenStar>(), (int)(Projectile.damage * 0.75f), 2, Projectile.owner);
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {


        
            base.OnTileCollide(oldVelocity);

            return false; // Prevents the projectile from being destroyed on collision
        }

    }
}

