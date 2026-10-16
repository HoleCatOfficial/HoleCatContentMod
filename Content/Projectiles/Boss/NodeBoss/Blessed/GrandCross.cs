using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed
{
    public class GrandCross : ModProjectile
    {

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        bool Embed = false;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3());
            if (!Embed)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            }
            else
            {
                Projectile.velocity *= 0.75f;
            }
        
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Embed)
            {
                SoundEngine.PlaySound(DTAssetLib.Charge.Anvil with { Pitch = -2.2f, PitchVariance = 0.2f, MaxInstances = 0, Volume = 1.5f }, Projectile.Center);
                BloomRingSharp Ring = new();
                Ring.Prepare(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue, 0.1f, 0.01f, 2f, BlendState.Additive);
                ParticleEngine.Particles.Add(Ring);
                Embed = true;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/Smite") with { MaxInstances = 0 }, Projectile.Center);

            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity * 0.001f, ModContent.ProjectileType<GrandCrossRay>(), Projectile.damage, 3, ai1: Projectile.rotation - MathHelper.PiOver2);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            
        }
    }

    public class GrandCrossRay : ModProjectile
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
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.rotation = Projectile.ai[1];
            Projectile.penetrate = -1;
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
            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(11), Color.MediumOrchid * 0.8f, Main.spriteBatch, BlendState.Additive, oF2, WidthScl * 0.8f, 4f);


            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.DarkGoldenrod, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.8f), SpriteEffects.None);

            DTUtils.instance.ScrollingTextureSpine(L, DTAssetLib.Streak(11), Color.PeachPuff, Main.spriteBatch, BlendState.Additive, oF, WidthScl * 0.65f, 3f);

            Main.EntitySpriteDraw(DTAssetLib.Laser.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, DTAssetLib.Laser.Value.Height / 2), new Vector2(1f, WidthScl * 0.5f), SpriteEffects.None);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.DarkGoldenrod with { A = 0 }, true, 0, WidthScl, WidthScl);
            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White with { A = 0}, true, 0, WidthScl, WidthScl);
            Main.spriteBatch.ResetToDefault();
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.ai[1];
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            float MaxScl = 0.85f;

            float t = ((Projectile.ai[0] / 30));
            if (Projectile.timeLeft > 210)
            {
                WidthScl = MathHelper.Lerp(0f, MaxScl, t);
            }
            if (Projectile.timeLeft < 270 && Projectile.timeLeft > 30)
            {
                WidthScl = MaxScl;
                Projectile.ai[0] = 0;
            }
            if (Projectile.timeLeft < 30)
            {
                WidthScl = MathHelper.Lerp(MaxScl, 0, t);
            }
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
