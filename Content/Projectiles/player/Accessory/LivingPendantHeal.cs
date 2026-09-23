using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class LivingPendantHeal : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        bool IHomingProjectile.TracksNPCs => false;

        bool IHomingProjectile.TracksPlayers => true;

        float IHomingProjectile.HomingTurnSpeed => 19f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.08f;

        float IHomingProjectile.HomingMaxAccel => 20f;

        float IHomingProjectile.DetectRadius => 1200f;

        bool IHomingProjectile.CanHome => Projectile.ai[0] >= 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 90;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, Projectile.Center - Main.screenPosition, null, Color.SpringGreen with { A = 0 }, Projectile.rotation, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 2f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.TinyBloom.Value, Projectile.Center - Main.screenPosition, null, Color.SpringGreen with { A = 0 }, Projectile.rotation, DTAssetLib.TinyBloom.Value.Size() / 2, 1f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.TinyBloom.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, DTAssetLib.TinyBloom.Value.Size() / 2, 0.6f, SpriteEffects.None);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terra, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 0, default, 1f).noGravity = true;

            if (Projectile.Center.Distance(Main.player[Projectile.owner].Center) < 20)
            {
                Main.player[Projectile.owner].Heal((int)Projectile.ai[1]);
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact);
                Projectile.Kill();
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 30)
            {
                Projectile.velocity *= 0.95f;
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return Projectile.ai[0] >= 30 ? true : false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }

    }
}
