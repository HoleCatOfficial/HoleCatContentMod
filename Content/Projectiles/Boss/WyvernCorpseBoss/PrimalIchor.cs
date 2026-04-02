using System;
using System.Collections.Generic;
using DestroyerTest.Common;
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

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class PrimalIchor : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/Boss/WyvernCorpseBoss/PrimalBlood";
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public int Mode => (int)Projectile.ai[0];

        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils.DrawCrystalCore(spriteBatch, Projectile.Center, Color.White, ColorLib.Ichor, TrailPositions, TextureRotationOffset, Projectile, TrailLength, 0.8f);
        }
        
        
        public List<Vector2> TrailPositions = new();
		public List<float> TrailRotations = new();
        private const int TrailLength = 40;
        
        public float TextureRotationOffset = 0f;
        public override void AI()
        {
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            float turnSpeed = Projectile.ai[1];
            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = 0.03f;
            }
            if (Mode == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(-turnSpeed);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (Mode == 1)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(turnSpeed);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            TextureRotationOffset -= 0.5f;
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());

            if (Main.rand.NextBool(4) && !ModContent.GetInstance<DTOptimizationsConfig>().DisableExcessDusts)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IchorTorch, 0f, 0f, 0, default, 2f);
            }
        }


        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 300);
        }
        

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
        }
    }
}