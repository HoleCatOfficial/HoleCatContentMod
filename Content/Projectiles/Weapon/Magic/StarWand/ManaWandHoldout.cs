using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic.StarWand
{
    public class ManaWandHoldout : HoldoutWand
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 56;
            Projectile.height = 56;
            RotationManualOffset = MathHelper.PiOver2;
            ShootSound = DTAssetLib.StellarBow.Shoot;
            Interval = 15;
            ManaCostPerShot = 20;
            //Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override Vector2 ShotPos()
        {
            Vector2 orig = Projectile.Center;
            return orig + new Vector2(0, 90).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
        }

        public override void Shoot()
        {
            
            Vector2 angle = targetAngle;
            angle.Normalize();
            Terraria.Projectile.NewProjectile(Projectile.GetSource_FromAI(), ShotPos(), angle.RotatedByRandom(0.5f) * 20, ModContent.ProjectileType<ManaCluster>(), Projectile.damage, 5, Owner.whoAmI);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            if (Projectile.spriteDirection > 0)
            {
                Draworigin = new Vector2(texture.Width / 2, texture.Height);
                effects = SpriteEffects.None;
            }
            else
            {
                Draworigin = new Vector2(texture.Width / 2, texture.Height);
                effects = SpriteEffects.None;
            }

            DrawUnder();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, (Projectile.rotation) + RotationManualOffset, Draworigin, Projectile.scale, effects, 0);
            if (Glowmask != null)
            {
                Main.EntitySpriteDraw(Glowmask.Value, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, (Projectile.rotation) + RotationManualOffset, Draworigin, Projectile.scale, effects, 0);
            }

            DrawOver();
            return false;
        }
    }
}
