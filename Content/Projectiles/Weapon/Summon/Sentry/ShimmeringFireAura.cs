using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using InnoVault.GameSystem;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon.Sentry
{
    public class ShimmeringFireAura : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public static int Radius = 200;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            if (Main.masterMode)
            {
                Radius = 350;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            
            return false;
        }

        public int NumFrames = 12;
        public void DoAnimation()
        {
            if (++Projectile.ai[0] >= 10)
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1]++;
                if (Projectile.ai[1] >= NumFrames)
                {
                    Projectile.ai[1] = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string Path = "DestroyerTest/Content/Projectiles/Weapon/Summon/Sentry";
            Texture2D MainTex = ModContent.Request<Texture2D>(Path + "/ShimmeringFireAura1").Value;
            Texture2D BottomTex = ModContent.Request<Texture2D>(Path + "/ShimmeringFireAura2").Value;

            int MainframeHeight = MainTex.Height / NumFrames;
            Rectangle frameMain = new Rectangle(
                0,
                MainframeHeight * (int)Projectile.ai[1],
                MainTex.Width,
                MainframeHeight
            );

            Vector2 originMain = new Vector2(MainTex.Width / 2f, MainframeHeight / 2f);

            int BottomframeHeight = BottomTex.Height / NumFrames;
            Rectangle frameBottom = new Rectangle(
                0,
                BottomframeHeight * (int)Projectile.ai[1],
                BottomTex.Width,
                BottomframeHeight
            );

            Vector2 originBottom = new Vector2(BottomTex.Width / 2f, BottomframeHeight / 2f);

            Main.EntitySpriteDraw(BottomTex, Projectile.Center - Main.screenPosition, frameBottom, Color.White, 0f, originBottom, Projectile.scale, SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(MainTex, (Projectile.Center - new Vector2(0, Radius)) - Main.screenPosition, frameMain, Color.White, 0f, originMain, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public Player Owner => Main.player[Projectile.owner];

        public override void AI()
        {
            DoAnimation();
            Particles();

            Projectile.velocity.X = 0f;
            Projectile.velocity.Y += 0.2f;
           
            
            Projectile.ai[2]++;

            NPC.HitInfo Hit = new NPC.HitInfo()
            {
                DamageType = DamageClass.Summon,
                Knockback = 5f,
                Crit = false,
                SourceDamage = Projectile.damage
            };


            foreach (NPC target in Main.npc)
            {
                if (target.active && target.Center.Distance(Projectile.Center) < Radius)
                {
                    if (!target.friendly && !target.dontTakeDamage)
                    {
                        if (target.Center.Y < Projectile.Center.Y && Projectile.ai[2] % 60 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item20, target.Center);
                            target.StrikeNPC(Hit with { HitDirection = target.Center.X > Projectile.Center.X ? 1 : -1 }, false, false);
                            NetMessage.SendStrikeNPC(target, Hit with { HitDirection = target.Center.X > Projectile.Center.X ? 1 : -1 });
                            ShimmeringFlames.ShimmerBurn(target);
                        }
                    }
                }
            }


        }


        public void Particles()
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 Pos = Projectile.Center + (Main.rand.NextVector2Unit((float)MathHelper.Pi, MathHelper.Pi) * Radius);
                Vector2 D = Projectile.Center - Pos;
                D.Normalize();

                Color color()
                {
                    if (Projectile.ai[1] <= 3)
                    {
                        return ColorLib.TenebrisMagenta;
                    }
                    if (Projectile.ai[1] <= 7 && Projectile.ai[1] > 4)
                    {
                        return ColorLib.TenebrisBlue;
                    }
                    if (Projectile.ai[1] <= 11 && Projectile.ai[1] > 8)
                    {
                        return ColorLib.TenebrisBeige;
                    }
                    return Color.White;
                }

                if (!DTOptimizationsConfig.instance.DisableExcessParticles)
                {
                    Fire fire1 = new Fire();
                    fire1.PrepareFire(Pos, D * 1.25f, Projectile.direction, 0.14f, color() * 0.4f, 0.75f, 120, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
                    ParticleEngine.BehindProjectiles.Add(fire1);
                }

                Fire fire2 = new Fire();
                fire2.PrepareFire(Pos, D * 1.25f, Projectile.direction, 0.14f, color(), 0.25f, 120, FireDrawMode.Additive, PixelLayer.AboveProjectiles);
                ParticleEngine.BehindProjectiles.Add(fire2);
            }
        }
    }
}
