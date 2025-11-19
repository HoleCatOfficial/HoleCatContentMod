using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.CorpseBoss;
using DestroyerTest.Content.Projectiles.VampireBoss;
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
using InnoVault.PRT;

namespace DestroyerTest.Content.Projectiles.NightmareRose
{
    public class BlossomMine : ModProjectile
    {
      

        public override void SetDefaults()
        {
            Projectile.width = 24; // The width of projectile hitbox
            Projectile.height = 24; // The height of projectile hitbox

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false; // Can the projectile deal damage to enemies?
            Projectile.hostile = true; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }

        public override bool PreDrawExtras()
        {
            SpriteBatch sb = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Opus.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);
            Opus.DrawGlowOnProj(Projectile, Color.Purple, false);
            TelegraphLine(sb);
            Opus.ReturnToDefaultDrawing(sb);
            return false;
        }

        public void TelegraphLine(SpriteBatch SB)
        {
            var LineTex = DTAssetLib.Line(1).Value;
            Vector2 start = IntialPos;

            if (Projectile.active)
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    float angle = MathHelper.TwoPi * dir / 8f;
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                    Vector2 drawPos = start - Main.screenPosition;
                    float length = 3600f;
                    Vector2 scale = new Vector2(1f, length / LineTex.Height);

                    SB.Draw(LineTex, drawPos, null, ColorLib.CursedFlames, angle + MathHelper.PiOver2, new Vector2(LineTex.Width / 2f, 0), scale, SpriteEffects.None, 0f);
                }
            }
        }


        public Vector2 IntialPos;

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Zombie94, Projectile.Center);
            IntialPos = Projectile.Center;
        }
        
        int DustAlpha = 255;
        float SoundPitch = 0f;
        public override void AI()
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            Projectile.velocity *= 0.999f;
            Projectile.rotation += Main.rand.NextFloat(-1f, 1.1f) * 0.1f;

            if (Main.GameUpdateCount % 20 == 0)
            {
                Opus.RadialSpreadDust(DustID.ShadowbeamStaff, 18, Projectile.Center, DustAlpha, default, 2.3f, 3, true);
                DustAlpha -= (255 / 30);
                Projectile.scale *= 1.05f;
                SoundPitch += (1 / 30);
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/NodeAttackTS") with {Volume = 0.5f, Pitch = SoundPitch, MaxInstances = 0});
            }
            if (Main.rand.NextBool(12))
            {
                Dust Idle = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShadowbeamStaff, 0, 0, 70, default, 1.0f);
                Idle.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 ToPlayer = Projectile.Center - Main.LocalPlayer.Center;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            var launchVelocity = new Vector2(-12, 0);
                
            for (int i = 0; i < 8; i++)
            {
                launchVelocity = launchVelocity.RotatedBy(MathHelper.PiOver4);
                //Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, launchVelocity, ProjectileID.CursedFlameHostile, 15, 1);
                Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, launchVelocity, ModContent.ProjectileType<CorruptPetalHostile>(), 35, 1);
            }
        }
    }
}