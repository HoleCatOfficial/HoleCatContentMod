using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class BigRiftExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 320;
            Projectile.height = 320;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Projectile.hide = true; //Ugly ass sprite
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );
            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);

            Main.EntitySpriteDraw(
                    projectileTexture,
                    Projectile.Center - Main.screenPosition,
                    frame,
                    lightColor,
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Bursting && Projectile.ManualCanHitFriendly(target);
        }

        public SoundStyle Burst = DTAssetLib.Impacts.FlameImpact;
        public bool Bursting = false;

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.Kill();
                }
            }
        }
        bool f1 = false;
        public override void AI()
        {
            AnimateProjectile();

            if (Projectile.frame < 2)
            {
               // Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 10), DustID.)
            }
            else
            {
                if (!f1)
                {
                    SoundEngine.PlaySound(Burst, Projectile.position);
                    LerpingBloomRingSharp Ring = new();
                    Ring.Prepare(Projectile.Center, Vector2.Zero, [Color.White, ColorLib.LightRift4, ColorLib.LightRift1, ColorLib.Rift], 0.1f, 0.03f, 1f);
                    ParticleEngine.BehindProjectiles.Add(Ring);

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(60, 60);
                        ElectricArc Arc = new();
                        Arc.Create(pos, ColorLib.Rift);
                        ParticleEngine.BehindProjectiles.Add(Arc);
                    }

                    f1 = true;
                }
            }

            if (Projectile.frame >= 6)
            {
                Bursting = true;
                KnockbackNPCs();
            }
        }

        private void KnockbackNPCs()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.noGravity && npc.Distance(Projectile.Center) < 150f)
                {
                    if (!npc.knockBackResist.Equals(0f))
                    {
                        Vector2 direction = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        npc.velocity += direction * 15f * npc.knockBackResist;
                    }
                }
            }
        }
    }
}