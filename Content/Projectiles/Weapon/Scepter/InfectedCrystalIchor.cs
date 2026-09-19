using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class InfectedCrystalIchor : ModProjectile, IHomingProjectile
    {
        public float DelayTimer;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 5f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.03f;

        float IHomingProjectile.HomingMaxAccel => 45f;

        float IHomingProjectile.DetectRadius => 1200;

        bool IHomingProjectile.CanHome => DelayTimer > 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; 
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 30 && Projectile.ManualCanHitFriendly(target);
        }

        public void DustSpawn1()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, -8).RotatedBy(Projectile.rotation);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, 8).RotatedBy(Projectile.rotation);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.5f);

            Dust.NewDustPerfect(DustPos, DustID.IchorTorch, Vector2.Zero, 0, default, 0.75f).noGravity = true;
        }

        public void DustSpawn2()
        {
            Vector2 Pos1 = Projectile.Center + new Vector2(0, 8).RotatedBy(Projectile.rotation);
            Vector2 Pos2 = Projectile.Center + new Vector2(0, -8).RotatedBy(Projectile.rotation);

            Vector2 DustPos = Opus.Sine(Pos1, Pos2, 0.5f);

            Dust.NewDustPerfect(DustPos, DustID.IchorTorch, Vector2.Zero, 0, default, 0.75f).noGravity = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, ColorLib.Ichor.ToVector3() * 0.5f);
            DustSpawn1();
            DustSpawn2();

            DelayTimer++;

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 600);
        }
	}
}