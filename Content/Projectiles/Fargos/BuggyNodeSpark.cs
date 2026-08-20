using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Fargos
{
    public class BuggyNodeSpark : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        int DelayTimer = 0;

        bool IHomingProjectile.TracksNPCs => DestroyerTestMod.MasochistIsActive;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 1.3f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.03f;

        float IHomingProjectile.HomingMaxAccel => 4f;

        float IHomingProjectile.DetectRadius => 400;

        bool IHomingProjectile.CanHome => DelayTimer >= 100;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, OpusColorUtils.Pastel(ColorLib.Wretched2, 0.75f) with { A = 0 }, Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, ColorLib.Wretched4.ToVector3() * 0.6f);
            var d = Dust.NewDustPerfect(Projectile.Center, DustID.SnowSpray, Vector2.Zero, 0, ColorLib.Wretched2, 1f);
            d.noGravity = true;


            if (DelayTimer < 100)
            {
                DelayTimer += 1;
                return;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Defilement>(), 180);

        }

        public override void OnKill(int timeLeft)
        {

        }
    }
}
