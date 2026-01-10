using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using ReLogic.Content;
using Terraria.Audio;
using OpusLib;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using System.Collections.Generic;
using Microsoft.Build.Evaluation;

namespace DestroyerTest.Content.Projectiles.player.ArmorSet
{
    public class ExplodingIcicle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, 0f, 0f, 0, Color.White, 1f);
            if (LifeTime < 15)
            {
                
            }
            else
            {
                Projectile.velocity.Y += 0.2f;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return LifeTime >= 15;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.Hitbox.Inflate(100, 100);
            Opus.RadialDustRandomDir(DustID.Ice, 14, Projectile.Center, 100, default, 4f, 2.5f);
        }
    }
}
