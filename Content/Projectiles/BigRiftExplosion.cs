using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public override void AI()
        {
            AnimateProjectile();

            if (Projectile.frame == 2 && Projectile.frameCounter == 0)
            {
                SoundEngine.PlaySound(Burst, Projectile.position);
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