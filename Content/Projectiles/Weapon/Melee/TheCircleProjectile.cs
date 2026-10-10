using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class TheCircleProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // The following sets are only applicable to yoyo that use aiStyle 99.

            // YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player.
            // Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = 5.2f;

            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player.
            // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
            ProjectileID.Sets.YoyosMaximumRange[Type] = 165f;

            // YoyosTopSpeed is top speed of the yoyo Projectile.
            // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
            ProjectileID.Sets.YoyosTopSpeed[Type] = 10.4f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of the projectile's hitbox.
            Projectile.height = 10; // The height of the projectile's hitbox.

            Projectile.aiStyle = ProjAIStyleID.Yoyo; // The projectile's ai style. Yoyos use aiStyle 99 (ProjAIStyleID.Yoyo). A lot of yoyo code checks for this aiStyle to work properly.

            Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Benefits from melee bonuses. MeleeNoSpeed means the item will not scale with attack speed.
            Projectile.penetrate = -1; // All vanilla yoyos have infinite penetration. The number of enemies the yoyo can hit before being pulled back in is based on YoyosLifeTimeMultiplier.
                                       // Projectile.scale = 1f; // The scale of the projectile. Most yoyos are 1f, but a few are larger. The Kraken is the largest at 1.2f

            //Projectile.drawLayer = ProjectileDrawLayerID.HeldProj; // Draws over the player's body and under the player's hands [1.4.5]
        }

        // notes for aiStyle 99:
        // localAI[0] is used for timing up to YoyosLifeTimeMultiplier
        // localAI[1] can be used freely by specific types
        // ai[0] and ai[1] usually point towards the x and y world coordinate hover point
        // ai[0] is -1f once YoyosLifeTimeMultiplier is reached, when the player is stoned/frozen, when the yoyo is too far away, or the player is no longer clicking the shoot button.
        // ai[0] being negative makes the yoyo move back towards the player
        // Any AI method can be used for dust, spawning projectiles, etc specific to your yoyo.

        float radius = 0f;

  
        public override void PostAI()
        {
            if (radius < 30)
            {
                radius += 0.1f;
            }



            //Lighting.AddLight(Projectile.Center, Color.Coral.ToVector3());
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for( int i = 0; i < 4; i++ )
            {
                Main.EntitySpriteDraw(DTAssetLib.Circle.Value,  (Projectile.Center + new Vector2(10, 0).RotatedBy((Projectile.rotation * 0.1f) + (MathHelper.PiOver2 * i))) - Main.screenPosition, null, Color.DarkRed with { A = 0 } * 0.1f, Projectile.rotation, DTAssetLib.Circle.Size() / 2, DTAssetLib.Circle.Value.ScaleRingTextureToMatchRadius(radius, 300), SpriteEffects.None, 0f);
            }
          
            
            Main.EntitySpriteDraw(DTAssetLib.Circle.Value, Projectile.Center - Main.screenPosition, null, Color.DarkRed with { A = 0 } * 0.4f, Projectile.rotation, DTAssetLib.Circle.Size() / 2, DTAssetLib.Circle.Value.ScaleRingTextureToMatchRadius(radius * 0.8f, 300), SpriteEffects.None, 0f);
            return true;
        }
    }
}
