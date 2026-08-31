using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.RogueItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class TrueRiftmakerClone : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 10;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 20f;

        float IHomingProjectile.DetectRadius => 1000f;

        bool IHomingProjectile.CanHome => DelayTimer >= 60;

        public float DelayTimer = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 20 && Projectile.ManualCanHitFriendly(target);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.ResetExcessTrailPoints();

                DelayTimer ++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(2, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 40f, Color.OrangeRed, 0f, 0f);

            Main.EntitySpriteDraw(TextureAssets.Item[ModContent.ItemType<TrueRiftMaker>()].Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 } * 0.5f, Projectile.rotation, TextureAssets.Item[ModContent.ItemType<TrueRiftMaker>()].Value.Size() / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Item[ModContent.ItemType<TrueRiftMaker>()].Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, Projectile.rotation, TextureAssets.Item[ModContent.ItemType<TrueRiftMaker>()].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Zombie103 with { MaxInstances = 0 }, Projectile.Center);
        }



    }
}
