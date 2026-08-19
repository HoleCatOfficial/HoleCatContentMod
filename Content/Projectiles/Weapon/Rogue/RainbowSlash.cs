using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class RainbowSlash : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.3f, SpriteEffects.None);
            return true;
        }


        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8; i++)
            {
                Spark spark = new();
                spark.PrepareSpark(Projectile.Center, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f)), Projectile.rotation, Main.DiscoColor, 1f, false, 60, SparkDrawMode.Additive, 2f);
                ParticleEngine.BehindProjectiles.Add(spark);
            }
        }

        public override void OnKill(int timeLeft)
        {

        }
    }
}
