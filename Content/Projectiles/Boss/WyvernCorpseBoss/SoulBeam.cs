using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class SoulBeam : ModProjectile
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
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 30;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(2000, 0).RotatedBy(Projectile.rotation));

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(2), ColorLib.Soul3, Main.spriteBatch, BlendState.Additive, oF, WidthScl, 3f);

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Soul2, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl), SpriteEffects.None);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(1), ColorLib.Soul, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.5f, 2f);

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.5f), SpriteEffects.None);
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

    public class SoulBeam2 : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 500;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        float WidthScl = 0f;
        Line L;
        int oF = 0;
        int oF2 = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            oF -= 15;
            oF2 -= 10;
            L = new Line(Projectile.Center, Projectile.Center + new Vector2(2000, 0).RotatedBy(Projectile.rotation));

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(8, true), ColorLib.Soul3 * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 1.4f, 4f);


            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Soul2 with { A = 0 }, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 1.2f), SpriteEffects.None);


            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(8, true), ColorLib.Soul, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 1f, 3f);

            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 1f), SpriteEffects.None);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.ai[1];
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 0.85f;

            float t = ((Projectile.ai[0] / 30));
            if (Projectile.timeLeft > 570)
            {
                WidthScl = MathHelper.Lerp(0f, MaxScl, t);
            }
            if (Projectile.timeLeft < 570 && Projectile.timeLeft > 30)
            {
                WidthScl = MaxScl;
                Projectile.ai[0] = 0;
            }
            if (Projectile.timeLeft < 30)
            {
                WidthScl = MathHelper.Lerp(MaxScl, 0, t);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 600);
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
