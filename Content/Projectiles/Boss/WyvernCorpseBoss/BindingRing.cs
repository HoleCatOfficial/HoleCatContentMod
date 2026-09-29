using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class BindingRing : ModProjectile, IDrawPixelated
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30 * 60;
            //Projectile.timeLeft = 70000;
            Projectile.tileCollide = false;
        }


        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AboveTiles;

        float R;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            R += 0.04f;

            var Cap = spriteBatch.Capture();
            Cap.TransformMatrix = PixelationSystem.PixelationMatrix;

            spriteBatch.End();
            spriteBatch.Begin(Cap);

            //Main.EntitySpriteDraw(DTAssetLib.Square.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0f, DTAssetLib.Square.Value.Size() / 2, 1f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(DTAssetLib.Circle.Value, Projectile.Center - Main.screenPosition, null, Color.Black * 0.5f, 0f, DTAssetLib.Circle.Value.Size() / 2, DTAssetLib.Circle.Value.ScaleRingTextureToMatchRadius(Projectile.ai[1] * 1.1f, 300), SpriteEffects.None, 0);

            Main.EntitySpriteDraw(DTAssetLib.BarrierRing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Soul with { A = 0 }, R, DTAssetLib.BarrierRing.Value.Size() / 2, DTAssetLib.BarrierRing.Value.ScaleRingTextureToMatchRadius(Projectile.ai[1], 1300), SpriteEffects.None, 0);

            spriteBatch.ResetToDefault();
        }

        bool F1 = false;
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            if (!F1)
            {
                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Activate, Projectile.Center);
                F1 = true;
            }

            if (player.Center.Distance(Projectile.Center) > Projectile.ai[1] && player.Center.Distance(Projectile.Center) > 0.1f)
            {
                player.Center = Projectile.Center + new Vector2(Projectile.ai[1] * 0.909f, 0).RotatedBy(player.DirectionFrom(Projectile.Center).ToRotation());
            }

          
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item30);
            foreach (Dust dust in Opus.RingSpreadDust(DustID.FireworksRGB, 40, Projectile.Center, 300f, 0, Color.White, 1f, 8f))
            {
                dust.noGravity = true;
            }
        }
    }
}
