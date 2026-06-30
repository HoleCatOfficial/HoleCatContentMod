using System.Collections.Generic;
using System.IO;
using System.Xml;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public abstract class OrganProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public int Variant;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Variant = Main.rand.Next(4);
            Projectile.frame = Variant;
        }
        
        public List<Vector2> PlayerOldPos = new List<Vector2>();
        
        public Vector2 toPlayer;

        public override void SendExtraAI(BinaryWriter writer)
        {
          
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
          
        }
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            PlayerOldPos.Add(player.Center);
            if (PlayerOldPos.Count > 35)
            {
                PlayerOldPos.RemoveAt(0);
            }
            Vector2 ToPlayer = player.Center - Projectile.Center;
            Projectile.velocity *= 0.99f;
            Projectile.rotation += Main.rand.NextFloat(-1f, 1.1f) * 0.1f;
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 0, default, 1.0f);
            }


            if (PlayerOldPos.Count > 4)
            {
                toPlayer = PlayerOldPos[4] - Projectile.Center;
            }
            else
            {
                toPlayer = player.Center - Projectile.Center;
            }

            toPlayer.SafeNormalize(Vector2.UnitY);

        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Player player = Main.player[(int)Projectile.ai[0]];
            DrawTelegraph(Projectile.Center, player.Center, DTAssetLib.Line(5).Value);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }
        


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath22, Projectile.Center);
            for (int o = 0; 0 < Main.rand.Next(3, 6); o++)
            {
                Projectile.NewProjectile(Entity.GetSource_Death(), Projectile.Center, (toPlayer.ToRotation().ToRotationVector2() * 10).RotatedBy(0.5f), ProjectileID.GoldenShowerHostile, Projectile.damage / 2, 1);
            }
        }

        public void DrawTelegraph(Vector2 start, Vector2 end, Texture2D texture)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            direction.Normalize();
            SpriteBatch spriteBatch = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            float rotation = direction.ToRotation();

            // Assuming your texture is a chain segment, like 16px long
            float segmentLength = texture.Height; // or Width, depending on the texture orientation
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            for (float i = 0; i < length; i += segmentLength)
            {
                Vector2 position = start + direction * i;

                Main.spriteBatch.Draw(
                    texture,
                    position - Main.screenPosition,
                    null,
                    ColorLib.IchorCrystalGradient,
                    rotation + MathHelper.PiOver2, // Adjust if your texture points upward
                    new Vector2(texture.Width / 2f, texture.Height / 2f), // Origin at center
                    new Vector2(0.5f, 1f),
                    SpriteEffects.None,
                    0f
                );
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }       
    }
}