using System;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
 
using DestroyerTest.Content.Projectiles.Gores;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class BlossomMine : ModProjectile, IDrawPixelated
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 500;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24; // The width of projectile hitbox
            Projectile.height = 24; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }


        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            var Cap = spriteBatch.Capture();
            spriteBatch.End();

            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.Begin(Cap);

            Opus.DrawGlowOnProj(Projectile, new Color(43, 37, 154) with { A = 0 }, false);
            TelegraphLine(spriteBatch);

            spriteBatch.ResetToDefault();
        }

        public void TelegraphLine(SpriteBatch SB)
        {
            var LineTex = DTAssetLib.Streak(13, true).Value;

            Vector2[] V = Opus.GetEquidistantVectors(8, Projectile.Center, 10, rOff);

            for (int i = 0; i < V.Length; i++)
            {
                Main.EntitySpriteDraw(LineTex, Projectile.Center - Main.screenPosition, null, new Color(43, 37, 154) with { A = 0 }, Projectile.Center.DirectionTo(V[i]).ToRotation(), new Vector2(0, LineTex.Height / 2), new Vector2(50f, 0.85f), SpriteEffects.None);
                Main.EntitySpriteDraw(LineTex, Projectile.Center - Main.screenPosition, null, new Color(43, 37, 154) with { A = 0}, Projectile.Center.DirectionTo(V[i]).ToRotation(), new Vector2(0, LineTex.Height / 2), new Vector2(50f, 0.2f), SpriteEffects.None);
            }
        }


        public Vector2 IntialPos;

        float rAmt = 0f;
        public override void OnSpawn(IEntitySource source)
        {
            for (int u = 0; u < 12; u++)
            {
                Gore.NewGore(source, Projectile.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), ModContent.GoreType<RosePetalGore1>(), 2f);
            }
            IntialPos = Projectile.Center;
            rAmt = Main.rand.NextFloat(-0.001f, 0.001f);
        }
        
        int DustAlpha = 255;
        float SoundPitch = 0f;

        float rOff = 0f;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;

        float prog;
        public override void AI()
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            Projectile.velocity *= 0.99f;
            Projectile.rotation += Main.rand.NextFloat(-1f, 1.1f) * 0.1f;

            rAmt *= 0.994f;
            rOff += rAmt;

            if (Projectile.timeLeft % 60 == 0)
            {
                Opus.RadialSpreadDust(DustID.ShadowbeamStaff, 18, Projectile.Center, DustAlpha, default, 2.3f, 7, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                DustAlpha -= 255 / 5;
                Projectile.scale *= 1.01f;
                SoundPitch += 1f / 5f;
                SoundEngine.PlaySound(SoundID.Item42 with {Volume = 0.5f, Pitch = SoundPitch, MaxInstances = 0});
                if (Projectile.timeLeft <= 60)
                {
                    Opus.RadialSpreadDust(DustID.ShadowbeamStaff, 18, Projectile.Center, DustAlpha, default, 5f, 10, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                    SoundEngine.PlaySound(SoundID.Item167 with { MaxInstances = 0});
                }
            }
            if (Main.rand.NextBool(12))
            {
                Dust Idle = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0, 0, 70, default, 1.0f);
                Idle.noGravity = true;
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int u = 0; u < 12; u++)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), ModContent.GoreType<RosePetalGore1>(), 2f);
            }
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<CorruptPetalHostile>(), 8, Projectile.Center, Projectile.damage, 8, 22, offset: rOff);
        }

      
    }
}