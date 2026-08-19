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
using System;
using DestroyerTest.Content.Projectiles.player.ArmorSet;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue
{
    public class FangshredThrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float LifeTime => Projectile.ai[0];

        public override void AI()
        {
            Projectile.ai[0] += 1f;

            Projectile.rotation += 0.5f * Projectile.direction;

            if (Main.GameUpdateCount % 10 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { }, Projectile.Center);
            }

            if (LifeTime < 30)
            {

            }
            else
            {
                

                Projectile.velocity.Y += 0.4f;



            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            target.AddBuff(BuffID.Venom, 600);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = new Vector2(-oldVelocity.X * 0.6f, -oldVelocity.Y * 0.6f);
            SoundEngine.PlaySound(DTAssetLib.FrigidFenzim.TileHit with { PitchVariance = 0.3f }, Projectile.Center);
            Projectile.penetrate--;
            return Projectile.penetrate > 1;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RichMahogany, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1f);
            }

        }
    }
}