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
using DestroyerTest.Content.Buffs;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed
{
    public class BlessedNodeCrystalFriendly : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/Boss/NodeBoss/Blessed/BlessedNodeCrystal";
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.SkyBlue;
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
            target.AddBuff(ModContent.BuffType<LightInferno>(), 600);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/CrystalBreak") with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);
            for (int g = 0; g < 4; g++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
            }

            int Gore1 = Mod.Find<ModGore>("BlessedShard1").Type;
            int Gore2 = Mod.Find<ModGore>("BlessedShard2").Type;
            int Gore3 = Mod.Find<ModGore>("BlessedShard3").Type;

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