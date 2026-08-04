using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Dusts
{
    public class SoulErosionDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 1.0f;
            dust.noGravity = false;
            dust.noLight = false;
        }

        public override bool PreDraw(Dust dust)
        {
            Asset<Texture2D> Dusttex = ModContent.Request<Texture2D>(Texture);
            Main.spriteBatch.UseBlendState(Utilities.SubtractiveBlending);

            Main.spriteBatch.Draw(Dusttex.Value, dust.position - Main.screenPosition, dust.frame, Color.Yellow * 0.1f, dust.rotation, dust.frame.Size() / 2f, dust.scale * 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Dusttex.Value, dust.position - Main.screenPosition, dust.frame, Color.Yellow * 0.25f, dust.rotation, dust.frame.Size() / 2f, dust.scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Dusttex.Value, dust.position - Main.screenPosition, dust.frame, Color.Yellow, dust.rotation, dust.frame.Size() / 2f, dust.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.ResetToDefault();
            return false;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.15f;
            dust.scale *= 0.99f;
            dust.velocity *= 0.99f;

        

            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }

            return false;
        }

    }
}