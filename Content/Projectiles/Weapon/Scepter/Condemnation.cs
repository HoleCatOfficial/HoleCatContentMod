using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class Condemnation : ModProjectile, IHomingProjectile
    {

        public float DelayTimer;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 10f;

        bool IHomingProjectile.UsesHomingAcceleration => false;

        float IHomingProjectile.HomingAccelAmount => 1f;

        float IHomingProjectile.HomingMaxAccel => 1f;

        float IHomingProjectile.DetectRadius => 1200f;

        bool IHomingProjectile.CanHome => DelayTimer >= 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = 36; // The width of projectile hitbox
            Projectile.height = 36; // The height of projectile hitbox
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>(); // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.timeLeft = 1800; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.hide = true;
            Projectile.extraUpdates = 12;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projectileTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Projectiles/Weapon/Scepter/HolyOrb").Value;
            
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = i / (float)Projectile.oldPos.Length;
                float scale = MathHelper.Lerp(0.2f, 0.0005f, progress);
                Color color = Color.Red with { A = 0 };

                Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.OldCenter()[i] - Main.screenPosition,
                    null,
                    color,
                    Projectile.rotation,
                    projectileTexture.Size() / 2,
                    scale * Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.Red,
                Projectile.rotation,
                projectileTexture.Size() / 2,
                0.2f * Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 30 && Projectile.ManualCanHitFriendly(target);
        }

        public override void AI()
        {
            DelayTimer++;

            var d = Dust.NewDustPerfect(Projectile.Center, DustID.CrimsonSpray, Vector2.Zero, 50, Color.Red, 1f);
            d.noGravity = true;
            
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
	}
}