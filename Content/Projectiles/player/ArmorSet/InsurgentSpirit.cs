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
            DTUtils.DrawCrystalCore(spriteBatch, Projectile.Center, Color.White, new Color(184, 45, 117), TrailPositions, TextureRotationOffset, Projectile, TrailLength, 0.8f);
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