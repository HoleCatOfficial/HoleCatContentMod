using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class RimeheartSnowflake : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 9f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.04f;

        float IHomingProjectile.HomingMaxAccel => 18f;

        float IHomingProjectile.DetectRadius => 900f;

        bool IHomingProjectile.CanHome => Timer >= 90;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            Main.projFrames[Type] = 3;
        }

        public int variant = 0;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            variant = Main.rand.Next(3);
            Projectile.frame = variant;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float Scale = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                
                Main.EntitySpriteDraw(texture, Projectile.OldCenter()[i] - Main.screenPosition, frame, lightColor * 0.1f, Projectile.oldRot[i], origin, Scale, SpriteEffects.None);
            }
            return true;
        }

        int Timer = 0;
        public override void AI()
        {
            Timer++;

            Projectile.frame = variant;

            Projectile.rotation += 0.06f * Projectile.direction;

            if (Timer < 90)
            {
                Projectile.velocity *= 0.94f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Timer >= 90 && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        }
    }
}
