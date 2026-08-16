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
    public class FrigidFenzimThrown : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.light = 0.5f;
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

            Projectile.rotation += 0.9f * Projectile.direction;

            if (LifeTime < 30)
            {

            }
            else
            {
                if (Main.GameUpdateCount % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1 with { }, Projectile.Center);
                }

                Projectile.velocity.Y += 0.4f;



            }
        }

        int HitCount = 0;
        int MHitCount = 20;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitCount++;
            float P_AMT = (float)HitCount / (float)MHitCount;

            float Pitch = MathHelper.Lerp(-0.4f, 0.4f, P_AMT);
            Projectile.velocity = new Vector2(-Projectile.oldVelocity.X * 0.3f, -Projectile.oldVelocity.Y * 1.1f);

            Projectile.timeLeft += 300;

            if (hit.Crit)
            {
                SoundEngine.PlaySound(DTAssetLib.FrigidFenzim.Crit with { PitchVariance = 0.1f, Pitch = Pitch }, Projectile.Center);
            }
            else
            {
                SoundEngine.PlaySound(DTAssetLib.FrigidFenzim.Hit with { PitchVariance = 0.1f, Pitch = Pitch }, Projectile.Center);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = new Vector2(-oldVelocity.X, -oldVelocity.Y);
            SoundEngine.PlaySound(DTAssetLib.FrigidFenzim.TileHit with { PitchVariance = 0.3f }, Projectile.Center);
            return HitCount > 4;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.IceImpact with { PitchVariance = 0.3f }, Projectile.Center);

            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<ExplodingIcicle>(), 7, Projectile.Center, Projectile.damage / 4, 5, 10);

        }
    }
}