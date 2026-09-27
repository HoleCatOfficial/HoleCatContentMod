using System;
using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Asset.Menu.V6
{
    //Copied from Calamity Mod Github.
    public class DreamMenu : ModMenu, IDrawPixelated
    {
        public override string DisplayName => "Talid v2.3: The Heavens";

        //public override bool IsAvailable => DTUtils.NPCDownTally[ModContent.NPCType<WyvernCorpseHead>()] > 0;

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("DestroyerTest/Assets/Menu/V6/TalidMenuIconAnim");
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>(DTUtils.NoTexture);
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>(DTUtils.NoTexture);

        public override int Music => MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/MainMenu");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<DreamMenuBackgroundStyle>();

        int LogoFrame = 0;
        int LogoFrameCounter = 0;

        float BGScroll = 0;

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.BehindTiles;
        bool IDrawPixelated.ShouldDrawPixelated => true;
        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {
            
        }


        // Before drawing the logo, draw the entire Calamity background. This way, the typical parallax background is skipped entirely.
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            Texture2D FarTex = ModContent.Request<Texture2D>("DestroyerTest/Assets/Menu/V6/TalidMenuBackground").Value;

            BGScroll += 0.1f;

            // Calculate the draw position offset and scale in the event that someone is using a non-16:9 monitor
            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / FarTex.Width;
            float yScale = (float)Main.screenHeight / FarTex.Height;
            float scale = xScale;

            // if someone's monitor isn't in wacky dimensions, no calculations need to be performed at all
            if (xScale != yScale)
            {
                // If someone's monitor is tall, it needs to be shifted to the left so that it's still centered on screen
                // Additionally the Y scale is used so that it still covers the entire screen
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (FarTex.Width * scale - Main.screenWidth) * 0.5f;
                }
                else
                    // The opposite is true if someone's monitor is widescreen
                    drawOffset.Y -= (FarTex.Height * scale - Main.screenHeight) * 0.5f;
            }

            var Cap = spriteBatch.Capture();

            spriteBatch.End();
            Cap.SamplerState = SamplerState.PointWrap;
            Cap.TransformMatrix = Main.UIScaleMatrix;

            spriteBatch.Begin(Cap);

            spriteBatch.Draw(FarTex, drawOffset, new Rectangle(((int)BGScroll).WrapToTwo(), 0, FarTex.Width, FarTex.Height), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            spriteBatch.ResetToDefaultUI();

            // Set the logo draw color to be white and the time to be noon
            // This is because there is not a day/night cycle in this menu, and changing colors would look bad
            drawColor = Color.White;
            Main.time = 27000;
            Main.dayTime = true;

            LogoFrameCounter++;

            if (LogoFrameCounter % 10 == 0)
            {
                LogoFrame++;

                if (LogoFrame >= 11)
                {
                    LogoFrame = 0;
                }
            }

            // Draw the logo using a different spritebatch blending setting so it doesn't have a horrible yellow glow
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f, 100f);

            var Cap2 = spriteBatch.Capture();


            spriteBatch.End();

            Cap2.TransformMatrix = Main.UIScaleMatrix;
            Cap2.SamplerState = SamplerState.PointClamp;
            Cap2.RasterizerState = Main.Rasterizer;

            spriteBatch.Begin(Cap2);

            spriteBatch.Draw(Logo.Value, drawPos, Logo.Value.Frame(1, 12, 0, LogoFrame), drawColor, 0f, new Vector2(Logo.Value.Width * 0.5f, 144 * 0.5f), 2f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(DTAssetLib.Doxent.Value, "T A L I D", drawPos + new Vector2(0, 90f), Color.White, 0f, DTAssetLib.Doxent.Value.MeasureString("T A L I D") * 0.5f, 0.5f, SpriteEffects.None, 0f);

            spriteBatch.ResetToDefaultUI();

            return false;
        }
    }
}