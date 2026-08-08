using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class ShimmeringSpark : ModProjectile, IHomingProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        int DelayTimer = 0;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 1.3f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.03f;

        float IHomingProjectile.HomingMaxAccel => 4f;

        float IHomingProjectile.DetectRadius => 400;

        bool IHomingProjectile.CanHome => DelayTimer >= 100;

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
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.hide = true;
            Projectile.extraUpdates = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.2f;
        }

        public override void PostDraw(Color lightColor)
        {
            Main.EntitySpriteDraw(DTAssetLib.SparkSmoothThin.Value, Projectile.Center - Main.screenPosition, null, OpusColorUtils.Pastel(ColorLib.TenebrisMagenta, 0.75f), Projectile.rotation, DTAssetLib.SparkSmoothThin.Value.Size() / 2, 4f, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, ColorLib.TenebrisMagenta.ToVector3() * 0.6f);
            var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, ColorLib.TenebrisMagenta, 1f);
            d.noGravity = true;


            if (DelayTimer < 100)
            {
                DelayTimer += 1;
                return;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                //SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShot with { PitchVariance = 0.2f });
                ShimmeringFlames.ShimmerBurn(target);
            }

            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, target.Center);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<DarkRaptureExplosion>(), Projectile.damage, 10f, Projectile.owner);
        }

        public override void OnKill(int timeLeft)
        {

        }
    }
}
