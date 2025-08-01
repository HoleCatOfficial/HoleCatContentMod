using Terraria.ModLoader;

namespace DetroyerTest.Content.SHADEMANAGEMENT
{
	public class ShadeStyle : ModSurfaceBackgroundStyle
	{
		// Use this to keep far Backgrounds like the mountains.
		public override void ModifyFarFades(float[] fades, float transitionSpeed) {
			for (int i = 0; i < fades.Length; i++) {
				if (i == Slot) {
					fades[i] += transitionSpeed;
					if (fades[i] > 1f) {
						fades[i] = 1f;
					}
				}
				else {
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f) {
						fades[i] = 0f;
					}
				}
			}
		}

        

		

		public override int ChooseFarTexture() {
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/ShadeMountains");
		}

		public override int ChooseMiddleTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/ParadiseBack");
        }



		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) {
			scale = 1f; // Ensures the background remains at its original scale
			parallax = 0.5; // Adjust this to control how much it moves with the player (0 locks it in place)
			return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Textures/Backgrounds/ParadiseFront");
		}
	}
}