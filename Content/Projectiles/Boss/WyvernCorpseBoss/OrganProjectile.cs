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
using OpusLib.Content.Helpers;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class OrganProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
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
        
        
        public Vector2 toPlayer;

        public override void SendExtraAI(BinaryWriter writer)
        {
          
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
          
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            float rot = Projectile.Center.DirectionTo(player.Center).ToRotation();

            var Tex = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/DirectionalTelegraph2");
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Ichor with { A = 0 }, rot, new Vector2(0f, Tex.Height() / 2), new Vector2(4f, 1f), SpriteEffects.None);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
          
            toPlayer = player.Center - Projectile.Center;
            toPlayer.Normalize();

            Projectile.velocity *= 0.99f;
            Projectile.rotation += Main.rand.NextFloat(-1f, 1.1f) * 0.1f;
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, 0, default, 1.0f);
            }


       

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }
        


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath22, Projectile.Center);


			Projectile spit = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, toPlayer * 17, ModContent.ProjectileType<GoldenShowerNoGravity>(), (int)(Projectile.damage * 0.5f), 4);

		}
    }
}