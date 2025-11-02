using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles
{
    public class SoulCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 14; // The width of projectile hitbox
            Projectile.height = 14; // The height of projectile hitbox
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.Soul;
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            for (int i = 0; i < TrailPositions.Count - 1; i++)
            {
                Vector2 start = TrailPositions[i] - Main.screenPosition;
                Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
                Vector2 diff = end - start;

                float length = diff.Length();
                if (length < 0.5f)
                    continue;

                float rotation = diff.ToRotation();

                float width = MathHelper.Lerp(0.01f, 0.0007f, i / (float)TrailLength);
                float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);
                Color color = lightColor * alpha;

                Main.spriteBatch.Draw(
                    DTAssetLib.Square.Value,
                    start,
                    null,
                    color,
                    rotation,
                    new Vector2(DTAssetLib.Square.Value.Width / 2, DTAssetLib.Square.Value.Height / 2),
                    new Vector2(length, width),
                    SpriteEffects.None,
                    0f
                );
            }

            Opus.DrawGlowOnProj(Projectile, lightColor, false, 0);
            Opus.DrawTextureOnProj(DTAssetLib.Sparkle(3), Projectile, Color.White, false, 0);
            
            Opus.ReturnToDefaultDrawing(spriteBatch);
            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }

        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 40;
        public override void AI()
        {
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);
            
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/CrystalBreak") with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item14);
            for (int g = 0; g < 4; g++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Pixie, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
            }
            int Gore1 = Mod.Find<ModGore>("SoulShard1").Type;
            int Gore2 = Mod.Find<ModGore>("SoulShard2").Type;
            int Gore3 = Mod.Find<ModGore>("SoulShard3").Type;

            var entitySource = Projectile.GetSource_Death();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (optcfg.OptimizeGame == false)
            {
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore1);
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore2);
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore3);
            }
        }
    }
}