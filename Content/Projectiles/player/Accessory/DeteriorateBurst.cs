using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class DeteriorateBurst : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
        }

        public override void SetDefaults()
        {
            
            Projectile.width = 128;
            Projectile.height = 128;
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
            lightColor = DTColorUtils.FromHex("#601082");
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );
            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);

            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Deferred);
            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, frame, lightColor * 0.6f, Projectile.rotation, origin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(projectileTexture, Projectile.Center - Main.screenPosition, frame, DTColorUtils.Pastel(lightColor, 0.4f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ManualCanHitFriendly(target);
        }

        public SoundStyle Burst = new SoundStyle("DestroyerTest/Assets/Audio/DeteriorateBurst") { PitchVariance = 0.2f };


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

        public override void OnSpawn(IEntitySource source)
        {
            Player Owner = Main.player[Projectile.owner];
            Projectile.scale = Owner.GetAdjustedItemScale(Owner.HeldItem);
            SoundEngine.PlaySound(Burst, Projectile.Center);
        }
        public override void AI()
        {
            AnimateProjectile();
            KnockbackNPCs();
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