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
    public class SoulOfLightDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 1.0f;
            dust.noGravity = true;
            dust.noLight = false;
            dust.scale *= 1.11f;
        }

        public override bool PreDraw(Dust dust)
        {
            Asset<Texture2D> Dusttex = ModContent.Request<Texture2D>(Texture);
            Opus.StartSpriteBatchWithBlending(Main.spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.spriteBatch.Draw(Dusttex.Value, dust.position - Main.screenPosition, dust.frame, ColorLib.SoulOfNightColor, dust.rotation, dust.frame.Size() / 2f, dust.scale * 1.4f, SpriteEffects.None, 0f);
            Opus.ReturnToDefaultDrawing(Main.spriteBatch);

            return true;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.995f;
            dust.rotation = 0f;
            dust.scale *= 0.9f;

            float light = 0.15f * dust.scale;

            Lighting.AddLight(dust.position, ColorLib.SoulOfLightColor.ToVector3() * light);

            if (dust.scale < 0.005f)
            {
                dust.active = false;
            }

            return false;
        }

    }
}