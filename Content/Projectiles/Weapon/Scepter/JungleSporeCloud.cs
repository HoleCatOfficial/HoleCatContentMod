
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
using OpusLib.Content.Particles;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class JungleSporeCloud : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {

         
            return false;
        }

        public override void AI()
        {
            TintableSmoke Smoke = new();
            Smoke.CreateWithBlending(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(1.3f, 1.3f), Color.GreenYellow, 0.5f, 1.4f, 60, PixelLayer.AboveTiles, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Smoke);


            Projectile.velocity *= 0.93f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 480);
        }

        public override void OnKill(int timeLeft)
        {
            
        }


    }

}