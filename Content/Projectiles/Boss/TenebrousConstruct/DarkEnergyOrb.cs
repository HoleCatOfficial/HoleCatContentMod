using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct
{
    public class DarkEnergyOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38; // The width of projectile hitbox
            Projectile.height = 38; // The height of projectile hitbox
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
        }

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }
        public override void AI()
        {
            AnimateProjectile();
            Vector2 FlankLeft = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            Vector2 FlankRight = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);

            if (Main.GameUpdateCount % 10 == 0 && Projectile.velocity.Length() > 2)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ChargeBreak") with { MaxInstances = 0, PitchVariance = 0.3f }, Projectile.Center);
                Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, FlankLeft, ModContent.ProjectileType<TenebrisStar>(), Projectile.damage / 2, 3, ai2: 4);
                Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, FlankRight, ModContent.ProjectileType<TenebrisStar>(), Projectile.damage / 2, 3, ai2: 4);
            }
        }
    }
}