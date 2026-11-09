using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Thrower
{
    public class DarkWaterAura : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            DTUtils.ThrowerProjectilesThatCantTriggerEquipEffects[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

        }

        public override void OnSpawn(IEntitySource source)
        {
            
        }

        float Scale = 0f;
        bool f1 = false;

        public override void AI()
        {
            Projectile.ai[0]++;

            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 1.5f);

            //Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(180, 180), DustID.Torch, Vector2.Zero).noGravity = true;

            if (Projectile.timeLeft > 30)
            {

                if (Scale < 1f)
                {
                    Scale += 0.08f;
                }
                else
                {

                }
            }
            else
            {
                if (Scale > 0f)
                {
                    Scale -= 0.08f;

                    if (Scale <= 0.0001f)
                    {


                    }
                }
            }

        }

        float roff = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            roff += 0.3f;

            Texture2D swirl = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/Corona_Alphablend").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            Color dark = OpusColorUtils.Darken(Color.Navy, 0.5f);
            var cap = spriteBatch.Capture();

            spriteBatch.End();
            cap.BlendState = BlendState.NonPremultiplied;
            spriteBatch.Begin(cap);

            Main.EntitySpriteDraw(DTAssetLib.Circle.Value, Projectile.Center - Main.screenPosition, null, dark, Projectile.rotation, DTAssetLib.Circle.Value.Size() / 2, (Scale * 0.3f) * Projectile.scale, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, Color.Navy, -roff, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.15f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, Color.Navy, roff, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.13f) * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, dark, roff * 0.5f, DTAssetLib.Corona.Value.Size() / 2, (Scale * 0.13f) * Projectile.scale, SpriteEffects.None, 0);

            spriteBatch.ResetToDefault();
            
            //Main.EntitySpriteDraw(DTAssetLib.BloomRing.Value, Projectile.Center - Main.screenPosition, null, Color.Blue with { A = 0 }, roff * 0.5f, DTAssetLib.BloomRing.Value.Size() / 2, (Scale * 1.3f) * Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, 90, targetHitbox);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
    }
}
