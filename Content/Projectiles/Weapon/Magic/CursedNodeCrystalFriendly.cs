using System.Collections.Generic;
using DestroyerTest.Common;
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
using System;

namespace DestroyerTest.Content.Projectiles.Weapon.Magic
{
    public class CursedNodeCrystalFriendly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 30; // The width of projectile hitbox
            Projectile.height = 30; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = true;
            Projectile.penetrate = 4;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = ColorLib.CursedFlames;
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(1).Value, TrailPositions, TrailRotations, 10f, lightColor, trailOffset, 10);

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawGlowOnProj(Projectile, lightColor * GlowMult, true);

            Opus.ReturnToDefaultDrawing(spriteBatch);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }


        public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 300;
        public float GlowMult = 1f;

        public override void AI()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
            Vector2 newPos = Projectile.Center;

            float dist = Vector2.Distance(lastPos, newPos);
            float step = 1f; // how closely to sample. tweak this!

            if (dist > 0f)
            {
                int segments = (int)(dist / step);

                for (int i = 1; i <= segments; i++)
                {
                    Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
                    TrailPositions.Insert(0, pos);
                    TrailRotations.Insert(0, Projectile.rotation);
                }
            }
            else
            {
                TrailPositions.Insert(0, newPos);
                TrailRotations.Insert(0, Projectile.rotation);
            }


            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            GlowMult = MathHelper.Lerp(0.25f, 1f, (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.5f + 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 240);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/CrystalBreak") with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);
            for (int g = 0; g < 4; g++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
            }
            int Gore1 = Mod.Find<ModGore>("CursedShard1").Type;
            int Gore2 = Mod.Find<ModGore>("CursedShard2").Type;
            int Gore3 = Mod.Find<ModGore>("CursedShard3").Type;

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