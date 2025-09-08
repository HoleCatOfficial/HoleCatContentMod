using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.WyvernSoul;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
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
    public class KeeperSoulProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38; // The width of projectile hitbox
            Projectile.height = 38; // The height of projectile hitbox
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        private void AnimateProjectile()
        {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/Desperation"));
            Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, new Vector2(0.2f, 0.2f), ModContent.ProjectileType<WyvernSoulHead>(), Projectile.damage, 3, ai2: 0);
        }

        public float TextureRotationOffset = 0f;
        public float SpiralRotationOffset = 0f;
        public override void AI()
        {
            AnimateProjectile();
            TextureRotationOffset -= 0.5f;

            if (Main.GameUpdateCount % 15 == 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    var angle = SpiralRotationOffset + (i * MathHelper.TwoPi / 6f);
                    var launchVelocity = new Vector2(10, 0).RotatedBy(angle);
                    Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, launchVelocity, ModContent.ProjectileType<SoulCrystalFriendly>(), Projectile.damage / 2, 4);
                }
            }
            SpiralRotationOffset += 0.45f;
        }

        // This here is a case I'd excuse, since there can only ever be one of these. Who cares about expensive draw calls at that point?
        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DrawCrystalCore(spriteBatch, Projectile.Center);
        }
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center)
        {
            DTUtils Utility = new DTUtils();
            Utility.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            
            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                ColorLib.Soul,
                TextureRotationOffset,
                new Vector2(DTAssetLib.Cyclone(2).Value.Width / 2f, DTAssetLib.Cyclone(2).Value.Height / 2f),
                0.2f,
                SpriteEffects.None,
                1f
            );

            Main.spriteBatch.Draw(
                DTAssetLib.FeatheredCircle.Value,
                Center - Main.screenPosition,
                null,
                Color.White,
                0f,
                new Vector2(DTAssetLib.FeatheredCircle.Value.Width / 2f, DTAssetLib.FeatheredCircle.Value.Height / 2f),
                0.4f,
                SpriteEffects.None,
                1f
            );

            Utility.ReturnToDefaultDrawing(spriteBatch);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(60, 60);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 480);
        }
	}
}