using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss
{
    public class MoonlordMasterShield : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < 4; i++)
            {
                float rotation = (MathHelper.TwoPi / 4) * i + (Main.GlobalTimeWrappedHourly * 0.5f);
                Vector2 offset = new Vector2(6 * Projectile.scale, 0).RotatedBy(rotation);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center + offset - Main.screenPosition, null, Color.Aquamarine with { A = 0 } * 0.5f * Projectile.Opacity, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2f, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.Aquamarine with { A = 0 } * Projectile.Opacity));

            Vector2 dustTop = Projectile.Center + (new Vector2(-31, -200).RotatedBy(Projectile.rotation) * Projectile.scale);
            Vector2 dustBottom = Projectile.Center + (new Vector2(-31, 200).RotatedBy(Projectile.rotation) * Projectile.scale);

            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, dustTop - Main.screenPosition, null, Color.Aquamarine with { A = 0 } * Projectile.Opacity, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.3f, 0.7f, 0.5f) * Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, dustBottom - Main.screenPosition, null, Color.Aquamarine with { A = 0 } * Projectile.Opacity, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.3f, 0.7f, 0.5f) * Projectile.scale, SpriteEffects.None);


            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, dustTop - Main.screenPosition, null, Color.Aquamarine with { A = 0 } * Projectile.Opacity, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.15f, 0.35f, 0.5f) * Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.ShineFlare.Value, dustBottom - Main.screenPosition, null, Color.Aquamarine with { A = 0 } * Projectile.Opacity, 0f, DTAssetLib.ShineFlare.Value.Size() / 2, Opus.Sine(0.15f, 0.35f, 0.5f) * Projectile.scale, SpriteEffects.None);
            return false;
        }

        float scroll = 0;
        bool f1 = false;
        public override void AI()
        {
            if (!Main.npc[(int)Projectile.ai[0]].active || Main.npc[(int)Projectile.ai[0]].ai[0] == -2f)
            {
                Projectile.Kill();
                return;

            }
            else
            {
           
                Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center + ( new Vector2(150, 0).RotatedBy(Projectile.rotation) * Projectile.scale);

                if (!f1)
                {
                    Projectile.rotation = Main.npc[(int)Projectile.ai[0]].Center.DirectionTo(Main.player[(int)Projectile.ai[1]].Center).ToRotation();
                    f1 = true;
                }
                
            }

            Vector2 Top = Projectile.Center + (new Vector2(0, -200).RotatedBy(Projectile.rotation) * Projectile.scale);
            Vector2 Bottom = Projectile.Center + (new Vector2(0, 200).RotatedBy(Projectile.rotation) * Projectile.scale);
            float cPoint = 0f;

            Vector2 dustTop = Projectile.Center + (new Vector2(-31, -200).RotatedBy(Projectile.rotation) * Projectile.scale);
            Vector2 dustBottom = Projectile.Center + (new Vector2(-31, 200).RotatedBy(Projectile.rotation) * Projectile.scale);

            Line topToOwner = new Line(dustTop, Main.npc[(int)Projectile.ai[0]].Center);
            Line bottomToOwner = new Line(dustBottom, Main.npc[(int)Projectile.ai[0]].Center);

            scroll += 0.3f;
            topToOwner.CycleLine(default, scroll, 12, DustID.Phantasmal);
            bottomToOwner.CycleLine(default, scroll, 12, DustID.Phantasmal);

            if (Projectile.timeLeft < 30)
            {
                Projectile.scale -= 0.01f;
                Projectile.Opacity -= 0.01f;
            }

            foreach (Projectile incoming in Main.projectile)
            {
                if (incoming.Distance(Projectile.Center) > 1000)
                {
                    continue;
                }

                if (incoming.active && incoming.friendly && incoming.damage > 0 && incoming.type != Projectile.type)
                {
                    if (Collision.CheckAABBvLineCollision(incoming.Hitbox.TopLeft(), incoming.Hitbox.Size(), Top, Bottom, 62 * Projectile.scale, ref cPoint))
                    {
                        SoundEngine.PlaySound(SoundID.Item150, incoming.Center);
                        incoming.velocity *= -1f;
                        incoming.netUpdate = true;
                    }
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}
