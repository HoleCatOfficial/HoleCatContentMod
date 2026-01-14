using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using ReLogic.Utilities;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Humanizer;

namespace DestroyerTest.Content.Projectiles.player.ArmorSet
{
    public class InsurgentSpirit : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DrawCrystalCore(spriteBatch, Projectile.Center);
        }
        
        public void DrawCrystalCore(SpriteBatch spriteBatch, Vector2 Center)
        {
            DTUtils Utility = new DTUtils();
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            for (int i = 0; i < TrailPositions.Count; i++)
            {
                float progress = i / (float)TrailLength;
                float scale = MathHelper.Lerp(0.1f, 0.0005f, progress);
                Color color = new Color(184, 45, 117);

                Main.EntitySpriteDraw(
                    DTAssetLib.Cyclone(2).Value,
                    TrailPositions[i] - Main.screenPosition,
                    null,
                    color,
                    TextureRotationOffset,
                    DTAssetLib.Cyclone(2).Value.Size() / 2f,
                    scale * TextureScale,
                    SpriteEffects.None,
                    0
                );
            }
            
            for (int i = 0; i < TrailPositions.Count; i++)
			{
				float progress = i / (float)TrailLength;
				float scale = MathHelper.Lerp(0.8f, 0.001f, progress);
				Color color = Color.White;

				Main.EntitySpriteDraw(
					DTAssetLib.FeatheredCircle.Value,
					TrailPositions[i] - Main.screenPosition,
					null,
					color,
					Projectile.rotation,
					DTAssetLib.FeatheredCircle.Value.Size() / 2f,
					scale * TextureScale,
					SpriteEffects.None,
					0
				);
			}

            Main.spriteBatch.Draw(
                DTAssetLib.Cyclone(2).Value,
                Center - Main.screenPosition,
                null,
                new Color(184, 45, 117),
                TextureRotationOffset,
                new Vector2(DTAssetLib.Cyclone(2).Value.Width / 2f, DTAssetLib.Cyclone(2).Value.Height / 2f),
                0.1f * TextureScale,
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
                0.8f * TextureScale,
                SpriteEffects.None,
                1f
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
        
        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
        private const int TrailLength = 40;
        public float TextureRotationOffset = 0f;
        public float TextureScale = 1f;
        public Vector2 GoalPos;

        public override void AI()
        {
            GoalPos = Vector2.Zero;
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            TextureRotationOffset -= 0.5f;
            Lighting.AddLight(Projectile.Center, new Color(242, 209, 255).ToVector3() * 0.3f);
            PRTLoader.NewParticle(Projectile.Center, Projectile.velocity * 0.5f, PRTLoader.GetParticleID<SimpleParticle>(), new Color(242, 209, 255) * 0.5f, 2f);

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, 0f, 0f, 0, new Color(242, 209, 255), 0.5f);
            }

            //Get an array of all projectiles of this type owned by the player.
            Player player = Main.player[Projectile.owner];
            var Indx = Opus.GetEquidistantOrbitVectors(player.ownedProjectileCounts[Type], player.Center, 2, 50);

            // Get all active projectiles of this type owned by the player
            List<Projectile> owned = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == player.whoAmI && p.type == Projectile.type)
                {
                    owned.Add(p);
                }
            }

            // Find this projectile’s index in that list
            int index = owned.IndexOf(Projectile);

            // Snap to orbit vector
            if (index >= 0 && index < Indx.Length && Projectile.ai[0] < 1)
            {
                Projectile.timeLeft = 180;
                GoalPos = Indx[index];
            }

            if (Projectile.ai[0] >= 1)
            {
                Projectile.Kill();
            }

            if (GoalPos != Vector2.Zero)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, GoalPos, 0.04f);
            }
        }
        

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.DD2_KoboldIgnite, Projectile.Center);
            for (int u = 0; u < 15; u++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2CircularEdge(10, 10), 0, new Color(242, 209, 255), 2);
            }
            player.AddBuff(ModContent.BuffType<InsurgentBoost>(), 600);
        }
    }
}