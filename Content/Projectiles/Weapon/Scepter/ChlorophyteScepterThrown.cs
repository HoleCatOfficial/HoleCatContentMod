using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class ChlorophyteScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Green;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.ChlorophyteWeapon;
            base.SetDefaults();
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(BuffID.Venom, 300);

            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<PoisonCloud>(), 8, Projectile.Center, Projectile.damage, 5f, 8f);

            base.OnHitNPC(target, hit, damageDone);
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {


            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<PoisonCloud>(), 8, Projectile.Center, Projectile.damage, 5f, 8f);


            base.OnTileCollide(oldVelocity);

            return false; // Prevents the projectile from being destroyed on collision
        }

    }
}

