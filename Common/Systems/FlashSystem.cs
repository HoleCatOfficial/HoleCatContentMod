using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ModLoader;

public class ScreenFlashSystem : ModSystem
{
    public static float FlashIntensity = 0f;

    public override void ModifyTransformMatrix(ref SpriteViewMatrix transform)
    {
        if (DTConfig.instance.PhotosensitivityMode)
        {
            return;
        }

        if (FlashIntensity > 0f)
        {
            // Reduce flash intensity over time
            FlashIntensity -= 0.02f;
            if (FlashIntensity < 0f)
                FlashIntensity = 0f;
        }
    }

    public override void PostDrawInterface(SpriteBatch spriteBatch)
    {
        if (DTConfig.instance.PhotosensitivityMode)
        {
            return;
        }

        if (FlashIntensity > 0f)
        {
            Color flashColor = new Color(255, 255, 255) * FlashIntensity;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), flashColor);
        }
    }
}
