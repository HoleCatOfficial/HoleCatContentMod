using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class GhoulProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 70;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;

            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();
            Asset<Texture2D> strip = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/GhoulStreak");

            Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            DTTrail.DrawTrail(spriteBatch, BlendState.AlphaBlend, strip.Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 17f, Color.White, 0);
            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.ResetExcessTrailPoints();

            Projectile.velocity *= 0.98f;


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulErosion>(), 600);
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioPath + "/DAHit"), Projectile.Center);
                SoundEngine.PlaySound(DTAssetLib.Impacts.DarkMagicImpact with { Pitch = -0.8f }, Projectile.Center);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<SplitGhoulProjectile>(), 4, Projectile.Center, Projectile.damage / 2, 2f, 2f, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }
    }

}