using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor
{
    public class CrimsonSpike : ModProjectile
    {
        Asset<Texture2D> Afterimage;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;

            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;

            
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.frame = Main.rand.Next(2);
            Afterimage = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/CrimsonSpikeAfterimage");
        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (Afterimage != null)
            {
                int frameHeight = TextureAssets.Projectile[Type].Value.Height / Main.projFrames[Type];
                Rectangle frame = new Rectangle(
                    0,
                    frameHeight * Projectile.frame,
                     TextureAssets.Projectile[Type].Value.Width,
                    frameHeight
                );

                Vector2 origin = new Vector2(TextureAssets.Projectile[Type].Value.Width / 2f, frameHeight / 2f);

                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    float fade = MathHelper.Lerp(1f, 0f, (float)i / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(Afterimage.Value, Projectile.OldCenter()[i] - Main.screenPosition, frame, Color.Red * Projectile.Opacity * fade, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
                }
            }


            var Tex = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/DirectionalTelegraph2");
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.Red with { A = 0 } * WarnOpacity, Projectile.rotation, new Vector2(0f, Tex.Height() / 2), new Vector2(7f, 0.7f), SpriteEffects.None);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            return false;
        }

        float StoredLength = 0f;
        public override void OnSpawn(IEntitySource source)
        {
            StoredLength = Projectile.velocity.Length();
        }

        float WarnOpacity = 1f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 60)
            {
                Projectile.velocity *= 0.9f;
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, Projectile.ai[0] / 15f);
                WarnOpacity = MathHelper.Lerp(0f, 1f, Utilities.Convert01To010(Projectile.ai[0] / 60f));
            }
            else
            {
                WarnOpacity = 0f;
                if (Projectile.velocity.Length() < StoredLength)
                {
                    Projectile.velocity *= 1.11f;
                }
            }
        }
    }
}
