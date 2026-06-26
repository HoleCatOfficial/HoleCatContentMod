using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class NightmareRoseCursedCrystal : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/Boss/NodeBoss/CursedFlame/CursedNodeCrystal";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 300;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			trailOffset += 0.04f;


			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();


            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(12, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 20, ColorLib.CursedFlames, trailOffset, 3);
         
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

			Opus.DrawGlowOnProj(Projectile, lightColor * GlowMult, true);

			Opus.ReturnToDefaultDrawing(spriteBatch);

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

        public override void OnSpawn(IEntitySource source)
        {

        }


       
        public float GlowMult = 1f;
        public int Dir => (int)Projectile.ai[1];
        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            if (Dir == 0 || Dir > 1 || Dir < -1)
            {
                return;
            }
            if (Dir == 1)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.01f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            if (Dir == -1)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(-0.01f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Defilement>(), 300);
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