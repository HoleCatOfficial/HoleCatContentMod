using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.CompilerServices;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.AmmoProjectiles
{
    public class TenebrisBulletProjectile : ModProjectile, IHomingProjectile
    {

        public int Variant = Main.rand.Next(3);

        int DelayTimer = 0;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 0.8f;

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
            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
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

        public void ColorAffectedFX(Color color)
        {
            Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.6f);

            var d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, color, 1f);
            d.noGravity = true;
        }

        public override void AI()
        {
            if (Variant == 0)
            {
                ColorAffectedFX(ColorLib.TenebrisBlue);
            }
            if (Variant == 1)
            {
                ColorAffectedFX(ColorLib.TenebrisMagenta);
            }
            if (Variant == 2)
            {
                ColorAffectedFX(ColorLib.TenebrisBeige);
            }

            // Spawn dust along the trail (tweak DustSpawnStep for performance)
            Color color = Variant switch
            {
                0 => ColorLib.TenebrisBlue,
                1 => ColorLib.TenebrisMagenta,
                _ => ColorLib.TenebrisBeige
            };

            
           

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

                SmallShine shine = new SmallShine();
                shine.Prepare(target.Center, Vector2.Zero, Color.White, 1f);
                ParticleEngine.ShaderParticles.Add(shine);
            }
        }

        public override void OnKill(int timeLeft)
        {
            
        }
	}
}