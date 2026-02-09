using System.Runtime.CompilerServices;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
	public class ConstitutionDust1 : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 1.1f;
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override bool PreDraw(Dust dust)
        {
            Asset<Texture2D> Dusttex = ModContent.Request<Texture2D>(Texture);
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.spriteBatch.Draw(Dusttex.Value, dust.position - Main.screenPosition, dust.frame, dust.color, dust.rotation, dust.frame.Size() / 2f, dust.scale, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);
            return false;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.scale *= 0.98f;
            dust.rotation += 0.1f * dust.velocity.X;

            Lighting.AddLight(dust.position,  ColorLib.StellarFireGradientLooping(3f).ToVector3() * dust.scale);
            return false;
        }

    }

}