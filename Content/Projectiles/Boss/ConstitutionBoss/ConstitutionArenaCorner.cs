using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
 
using DestroyerTest.Content.Particles.Stellar;

namespace DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss
{
    public class ConstitutionArenaCorner : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            spriteBatch.Draw(DTAssetLib.Sparkle(5).Value, Projectile.Center - Main.screenPosition, null, Color.White, RotOffset1, DTAssetLib.Sparkle(5).Value.Size() / 2, Scale1, SpriteEffects.None, 0f);
            spriteBatch.Draw(DTAssetLib.PointGlow.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0f, DTAssetLib.PointGlow.Value.Size() / 2, 3f, SpriteEffects.None, 0f);
            spriteBatch.Draw(DTAssetLib.FeatheredCircle.Value, Projectile.Center - Main.screenPosition, null, ColorLib.StellarFireGradientLooping() * 0.75f, 0f, DTAssetLib.FeatheredCircle.Value.Size() / 2, Scale2, SpriteEffects.None, 0f);
            spriteBatch.Draw(DTAssetLib.FeatheredCircle.Value, Projectile.Center - Main.screenPosition, null, Color.White, 0f, DTAssetLib.FeatheredCircle.Value.Size() / 2, 1f, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }

        public Entities.ConstitutionBoss Constitution;
        public bool CheckActive()
        {
            foreach(NPC Npc in Main.npc)
            {
                if (Npc.active)
                {
                    if (Npc.ModNPC is Entities.ConstitutionBoss constitution)
                    {
                        Constitution = constitution;
                        return true;
                    }
                }
            }
            return false;
        }

        public float RotOffset1 = 0f;
        public float Scale1 = 0f;
        public float Scale2 = 0f;
        public override void AI()
        {
            if (!CheckActive())
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 300;
            }

            RotOffset1 += 0.03f;
            Scale1 = Opus.Sine(1f, 0.75f);
            Scale2 = Opus.Sine(1.3f, 1.2f);
        }
    }
}