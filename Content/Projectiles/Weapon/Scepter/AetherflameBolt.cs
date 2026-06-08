using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class AetherflameBolt : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = true;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.extraUpdates = 70;
            Projectile.scale = 0.05f;
        }

        Color MainColor = new Color(255, 116, 75);
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Projectile.DrawAfterimages(spriteBatch, DTAssetLib.SparkSmooth.Value, MainColor with { A = 0}, 1f, true, true, true);
            
            //Main.EntitySpriteDraw(DTAssetLib.SparkSmooth.Value, Projectile.Center - Main.screenPosition, null, MainColor with { A = 0}, Projectile.rotation, DTAssetLib.SparkSmooth.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();


            Spark Fx = new();
            Fx.PrepareSpark(Projectile.Center, Projectile.velocity * 0.0005f, Projectile.rotation + MathHelper.PiOver2, MainColor, 0.25f, false, 30, SparkDrawMode.Additive, 1f);
            ParticleEngine.BehindProjectiles.Add(Fx);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(DTAssetLib.Charge.MetalTinkLight);
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X + Main.rand.NextFloat(-4f, 4f);
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y + Main.rand.NextFloat(-4f, 4f); ;
            }
            Projectile.penetrate--;
            return Projectile.penetrate == 1;
        }
       
    }
}
