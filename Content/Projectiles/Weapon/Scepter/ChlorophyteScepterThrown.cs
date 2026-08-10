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
using OpusLib;

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

            Opus.RadialSpreadProjectileRandom(ProjectileID.SporeGas, 8, Projectile.Center, Projectile.damage, 5f, 8f);

            base.OnHitNPC(target, hit, damageDone);
        }

        

        public override bool OnTileCollide(Vector2 oldVelocity) {


            Opus.RadialSpreadProjectileRandom(ProjectileID.SporeGas, 8, Projectile.Center, Projectile.damage, 5f, 8f);


            base.OnTileCollide(oldVelocity);

            return false; // Prevents the projectile from being destroyed on collision
        }

    }
}

