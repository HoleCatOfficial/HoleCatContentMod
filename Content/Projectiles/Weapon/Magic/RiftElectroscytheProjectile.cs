using DestroyerTest.Common;
using DestroyerTest.Content.Magic;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class RiftElectroscytheProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects FX = Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D Tex = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Tex.Size() / 2, Projectile.scale, FX, 0f);
            return false;

        }

        Player Owner => Main.player[Projectile.owner];
        float MaxSpeed => 0.6f * Owner.GetTotalAttackSpeed(DamageClass.Magic);
        float Speed = 0f;

        bool AtFullSpeed = false;
        bool CanContinue = true;
        public override void AI()
        {
            Projectile.direction = Owner.direction;
            Projectile.rotation += Speed * Owner.direction;
            Projectile.rotation = MathHelper.WrapAngle(Projectile.rotation);
            Projectile.direction = Owner.direction;
            Projectile.Center = Owner.MountedCenter;

            if (Owner.controlUseItem && CanContinue)
            {
                Projectile.timeLeft = 60;

                Projectile.localAI[0] += Math.Abs(Speed);

                if (Projectile.localAI[0] >= MathHelper.TwoPi)
                {
                    Projectile.localAI[0] -= MathHelper.TwoPi;
                    SoundEngine.PlaySound(SoundID.Item71 with { MaxInstances = 0 });
                    CanContinue = Owner.CheckMana(10, true, false);
                }
                if (Speed < MaxSpeed)
                {
                    AtFullSpeed = false;
                    Speed += 0.003f;
                }
                else
                {
                    Speed = MaxSpeed;
                    if (!AtFullSpeed)
                    {
                        AtFullSpeed = true;
                        Projectile.netUpdate = true;
                    }
                }
                

                if (AtFullSpeed)
                {
                    Projectile.ai[0]++;

                    if (Projectile.ai[0] % 10 == 0)
                    {
                        Vector2 Velocity = new Vector2(10 * Owner.GetTotalAttackSpeed(DamageClass.Magic), 0).RotatedByRandom(MathHelper.TwoPi);


                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.MountedCenter, Velocity, ModContent.ProjectileType<RiftStarFriendly>(), Projectile.damage / 2, 8, Owner.whoAmI);
                    }
                }
            }
            else
            {
                if (Speed > 0)
                {
                    Speed *= 0.94f;
                }
            }
            
        }
    }
}
