using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using DestroyerTest.Common;
using SteelSeries.GameSense;
using ReLogic.Content;

public class ColorGradientOverlaySystem : ModSystem
{
    public static float ColorVisibility = 0f;

    public override void ModifyTransformMatrix(ref SpriteViewMatrix transform)
    {
        if (ColorVisibility > 0f)
        {
            /*
            ColorVisibility -= 0.05f;
            if (ColorVisibility < 0f)
                ColorVisibility = 0f;
            */
        }
    }

    public override void PreUpdatePlayers()
    {
        ManageStellarGoggleEquips();
    }

    public override void PostDrawInterface(SpriteBatch spriteBatch)
    {
        if (ColorVisibility > 0f)
        {

            Texture2D pixel = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/StellarScreenEffect").Value;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);


            spriteBatch.Draw(pixel, Main.screenPosition, Color.White * ColorVisibility);
            

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
    }


    public static void FadeInHaze(float MaxAmount)
    {
        if (ColorVisibility < MaxAmount)
        {
            ColorVisibility += 0.02f;
            if (ColorVisibility > MaxAmount)
                ColorVisibility = MaxAmount;
        }

        if (MaxAmount > 1.0f || MaxAmount < 0f)
            throw new System.Exception("MaxAmount must be between 0.0f and 1.0f");
    }

    public static void FadeOutHaze(float MinAmount)
    {
        if (ColorVisibility > MinAmount)
        {
            ColorVisibility -= 0.02f; // Adjust this value for speed
            if (ColorVisibility < MinAmount)
                ColorVisibility = MinAmount;
        }

        if (MinAmount < 0f || MinAmount > 1.0f)
            throw new System.Exception("MinAmount must be between 0.0f and 1.0f");
    }

    public void ManageStellarGoggleEquips()
    {
        if (DTFlags.StellarGogglesEquipped == true && ColorVisibility < 0.7f)
        {
            FadeInHaze(0.7f);
        }
        if (DTFlags.StellarGogglesEquipped == false && ColorVisibility > 0.0f)
        {
           FadeOutHaze(0.0f);
        }
    }
}
