
using DestroyerTest.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
 
using DestroyerTest.Content.Particles;
using Terraria.Audio;
using SteelSeries.GameSense;

namespace DestroyerTest.Content.Projectiles.ParentClasses
{
	public abstract class SlowDownBomb : ModProjectile
	{
		public override void SetStaticDefaults() 
        {

		}
        
		public override void SetDefaults()
		{
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
            
            if (SlowFactor <= 0)
            {
                SlowFactor = 1f;
            }
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);


			Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

			return false;
		}

        public float SlowFactor {get; protected set;}
        public SoundStyle ExplodeSound;

		public override void AI()
		{
            Projectile.velocity *= SlowFactor;
		}


        public virtual void Explosion()
        {
            
        }

        public override void OnKill(int timeLeft)
        {
			Explosion();
			SoundEngine.PlaySound(ExplodeSound with { MaxInstances = 0 }, Projectile.Center);
        }
    }
}