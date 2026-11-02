using Microsoft.Xna.Framework.Graphics;

namespace DestroyerTest.Common
{
    public class Invert
    {
        public static BlendState InvertBlendState = new BlendState
        {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract,

            AlphaSourceBlend = Blend.Zero,
            AlphaDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.Add
        };


    }
}
