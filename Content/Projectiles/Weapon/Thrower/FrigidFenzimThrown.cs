using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Thrower
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
            Projectile.penetrate = 4;
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

            Projectile.rotation += 0.5f * Projectile.direction;
            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3());

            if (LifeTime < 30)
            {

            }
            else
            {
                if (Main.GameUpdateCount % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1 with { }, Projectile.Center);
                }

                Projectile.velocity.Y += 0.4f;



            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = new Vector2(-Projectile.oldVelocity.X * 0.6f, -Projectile.oldVelocity.Y * 0.6f);

            if (hit.Crit)
            {
                SoundEngine.PlaySound(DTAssetLib.FrigidFenzim.Crit with { PitchVariance = 0.1f }, Projectile.Center);
                target.AddBuff(BuffID.Frozen, 240);
            }
            else
            {
                SoundEngine.PlaySound(DTAssetLib.FrigidFenzim.Hit with { PitchVariance = 0.1f }, Projectile.Center);
            }
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
            SoundEngine.PlaySound(DTAssetLib.Impacts.IceImpact with { PitchVariance = 0.3f }, Projectile.Center);

            int Gore1 = Mod.Find<ModGore>("FrigidFenzimGore1").Type;
            int Gore2 = Mod.Find<ModGore>("FrigidFenzimGore2").Type;
            int Gore3 = Mod.Find<ModGore>("FrigidFenzimGore3").Type;

            var entitySource = Projectile.GetSource_Death();
            if (DTOptimizationsConfig.instance.OptimizeGame == false)
            {
                Gore.NewGore(entitySource, Projectile.position, Projectile.velocity * 0.5f, Gore1);
                Gore.NewGore(entitySource, Projectile.position, Projectile.velocity * 0.5f, Gore2);
                Gore.NewGore(entitySource, Projectile.position, Projectile.velocity * 0.5f, Gore3);
            }

        }
    }
}