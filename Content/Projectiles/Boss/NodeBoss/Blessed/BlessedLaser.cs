using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using Microsoft.Xna.Framework.Graphics;
using BreadLibrary.Core.Utilities;
using OpusLib;
using Terraria.DataStructures;
using OpusLib.Content.Helpers;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed
{
    public class BlessedLaser : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 30;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(2000, 0).RotatedBy(Projectile.rotation));

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(2), Main.DiscoColor, Main.spriteBatch, BlendState.Additive, oF, WidthScl);
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl), SpriteEffects.None);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(1), Color.White, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.5f);
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.5f), SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 1.5f;

            float t = Utilities.Convert01To010((Projectile.ai[0] / 30));
            WidthScl = MathHelper.Lerp(0f, MaxScl, t);

            Projectile.rotation += 0.03f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = 2000f; // however long your laser should be

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 30f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }

    public class BlessedLaserFriendly : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 60;
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 30;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(2000, 0).RotatedBy(Projectile.rotation));

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(2), Main.DiscoColor, Main.spriteBatch, BlendState.Additive, oF, WidthScl);
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl), SpriteEffects.None);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(1), Color.White, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.5f);
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.5f), SpriteEffects.None);

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, 0f, DTAssetLib.Star(3).Value.Size() / 2, WidthScl * 3.4f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.Star(3).Value, Projectile.Center - Main.screenPosition, null, Color.White, 0f, DTAssetLib.Star(3).Value.Size() / 2, WidthScl * 3, SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 1.5f;

            float t = Utilities.Convert01To010((Projectile.ai[0] / 30));
            WidthScl = MathHelper.Lerp(0f, MaxScl, t);

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = 2000f; // however long your laser should be

            Vector2 start = Projectile.Center;

            Vector2 S = Projectile.velocity;
            Vector2 end = start + new Vector2(length, 0).RotatedBy(Projectile.rotation);

            float collisionPoint = 0f;

            float beamWidth = 30f * WidthScl; // scale this how you want

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, beamWidth, ref collisionPoint);
        }
    }
}
