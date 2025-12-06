using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class RiftSpine_Thrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 180;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 FlankLeft = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 FlankRight = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);

            if (Main.GameUpdateCount % 10 == 0 && Projectile.velocity.Length() > 2)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/StarShot") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.15f }, Projectile.Center);
                Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, FlankLeft * 0.25f, ModContent.ProjectileType<RiftStarFriendly2>(), Projectile.damage / 2, 3, ai2: 1);
                Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, FlankRight * 0.25f, ModContent.ProjectileType<RiftStarFriendly2>(), Projectile.damage / 2, 3, ai2: 1);
            }

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RiftDust>(), Projectile.velocity * 0.2f, 0, default, 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
        }
		
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<DaylightOverload>(), 600);
		}
    }
}