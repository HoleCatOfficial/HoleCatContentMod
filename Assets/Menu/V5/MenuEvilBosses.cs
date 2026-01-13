using System;
using System.Collections.Generic;
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

namespace DestroyerTest.Assets.Menu.V5
{
    //Copied from Calamity Mod Github.
    public class MenuWyvern : ModMenu
    {
        public override string DisplayName => "Talid v3.0: The Great Wyvern";

        //public override bool IsAvailable => DTUtils.NPCDownTally[ModContent.NPCType<WyvernCorpseHead>()] > 0;

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("DestroyerTest/Assets/Menu/V5/Logo_ShadeWorld");
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>(DTUtils.NoTexture);
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>(DTUtils.NoTexture);

        public override int Music => MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/WyvernSoulAmbience");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<MenuEvilBossesBackgroundStyle>();

        // Before drawing the logo, draw the entire Calamity background. This way, the typical parallax background is skipped entirely.
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Assets/Menu/V5/MenuBackgroundWyvern").Value;

            // Calculate the draw position offset and scale in the event that someone is using a non-16:9 monitor
            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / texture.Width;
            float yScale = (float)Main.screenHeight / texture.Height;
            float scale = xScale;

            // if someone's monitor isn't in wacky dimensions, no calculations need to be performed at all
            if (xScale != yScale)
            {
                // If someone's monitor is tall, it needs to be shifted to the left so that it's still centered on screen
                // Additionally the Y scale is used so that it still covers the entire screen
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (texture.Width * scale - Main.screenWidth) * 0.5f;
                }
                else
                    // The opposite is true if someone's monitor is widescreen
                    drawOffset.Y -= (texture.Height * scale - Main.screenHeight) * 0.5f;
            }

            spriteBatch.Draw(texture, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            
            // Set the logo draw color to be white and the time to be noon
            // This is because there is not a day/night cycle in this menu, and changing colors would look bad
            drawColor = Color.White;
            Main.time = 27000;
            Main.dayTime = true;

            // Draw the logo using a different spritebatch blending setting so it doesn't have a horrible yellow glow
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f, 100f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(Logo.Value, drawPos, null, drawColor, logoRotation, Logo.Value.Size() * 0.5f, logoScale * 1.75f, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            return false;
        }
    }

    public class MenuRose : ModMenu
    {
        public override string DisplayName => "Talid v3.0: Nightmare Rose";

        //public override bool IsAvailable => DTUtils.NPCDownTally[ModContent.NPCType<NightmareRoseBoss>()] > 0;
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("DestroyerTest/Assets/Menu/V5/Logo_ShadeWorld");
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>(DTUtils.NoTexture);
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>(DTUtils.NoTexture);

        public override int Music => MusicLoader.GetMusicSlot("DestroyerTest/Assets/Music/RoseSoulAmbience");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<MenuEvilBossesBackgroundStyle>();

        // Before drawing the logo, draw the entire Calamity background. This way, the typical parallax background is skipped entirely.
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Assets/Menu/V5/MenuBackgroundRose").Value;

            // Calculate the draw position offset and scale in the event that someone is using a non-16:9 monitor
            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / texture.Width;
            float yScale = (float)Main.screenHeight / texture.Height;
            float scale = xScale;

            // if someone's monitor isn't in wacky dimensions, no calculations need to be performed at all
            if (xScale != yScale)
            {
                // If someone's monitor is tall, it needs to be shifted to the left so that it's still centered on screen
                // Additionally the Y scale is used so that it still covers the entire screen
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (texture.Width * scale - Main.screenWidth) * 0.5f;
                }
                else
                    // The opposite is true if someone's monitor is widescreen
                    drawOffset.Y -= (texture.Height * scale - Main.screenHeight) * 0.5f;
            }

            spriteBatch.Draw(texture, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            
            // Set the logo draw color to be white and the time to be noon
            // This is because there is not a day/night cycle in this menu, and changing colors would look bad
            drawColor = Color.White;
            Main.time = 27000;
            Main.dayTime = true;

            // Draw the logo using a different spritebatch blending setting so it doesn't have a horrible yellow glow
            Vector2 drawPos = new Vector2(Main.screenWidth / 2f, 100f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(Logo.Value, drawPos, null, drawColor, logoRotation, Logo.Value.Size() * 0.5f, logoScale * 1.75f, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            return false;
        }
    }
}