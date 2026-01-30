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
using DestroyerTest.Content.Dusts;
using System.Composition.Hosting.Core;
using DestroyerTest.Content.Projectiles.Weapon.Ranged;

namespace DestroyerTest.Content.Projectiles.Weapon.Ranged
{
    public class DreampoonProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float LifeTime => Projectile.ai[0];
        public Vector2 returnpoint;
        bool returning = false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0] += 1f;

            foreach(Projectile held in Main.projectile)
            {
                if (held.owner == Projectile.owner)
                {
                    if (held.ModProjectile is DreampoonHoldout Holdout)
                    {
                        returnpoint = Holdout.ShotPos;
                    }
                    else
                    {
                        returnpoint = player.Center;
                    }
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (LifeTime < 300)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ColorableNeonDust>(), 0, 0, 0, Color.White, 1f);
            }
            else
            {
                Projectile.velocity.Y += 0.2f;
            }

            if (LifeTime > 500)
            {
                returning = true;
            }

            if (returning)
            {
                float t = (LifeTime - 500) / 60f;
                if (returnpoint != Vector2.Zero)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.Center, returnpoint, t);
                    float.Clamp(Projectile.velocity.Length(), 0, 30);
                }

                if (Projectile.Center.Distance(returnpoint) < 10)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 300);
            SoundEngine.PlaySound(DTAssetLib.IdriGreatswordSlice(ChildSafety.Disabled));
            if (!returning)
            {
                returning = true;
            }
            if (returning)
            {
                damageDone *= 2;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity += -oldVelocity;
            returning = true;
            return false;
        }

        public override bool PreDrawExtras()
        {
            Texture2D chain = ModContent.Request<Texture2D>("DestroyerTest/Content/Projectiles/Weapon/Ranged/DreampoonChain").Value;

            Vector2 currentPos = returnpoint;
            while (Vector2.Distance(currentPos, Projectile.Center) > chain.Height)
            {
                Vector2 direction = (Projectile.Center - currentPos).SafeNormalize(Vector2.Zero);
                Main.EntitySpriteDraw(chain, currentPos - Main.screenPosition, null, Color.White, direction.ToRotation() + MathHelper.PiOver2, chain.Size() / 2, 1f, SpriteEffects.None, 0);
                currentPos += direction * chain.Height;
            }
            return true;
        }
    }
}
