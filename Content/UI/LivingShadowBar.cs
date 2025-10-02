using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace DestroyerTest.Content.UI
{
	// This custom UI will show whenever the player is holding the ExampleCustomResourceWeapon item and will display the player's custom resource amounts that are tracked in ExampleResourcePlayer
	internal class LivingShadowBar : UIState
	{
		// For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
		// Once this is all set up make sure to go and do the required stuff for most UI's in the ModSystem class.
		private UIText text;
		private UIElement area;
		private UIImage barFrame;
		private Asset<Texture2D> barBack;
		private Asset<Texture2D> fillCap;
		private Color gradientA;
		private Color gradientB;

		public override void OnInitialize() {
			// Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
			// UIElement is invisible and has no padding.
			area = new UIElement();
			area.Left.Set(-area.Width.Pixels - 600, 1f); // Place the resource bar to the left of the hearts.
			area.Top.Set(50, 0f); // Placing it just a bit below the top of the screen.
			area.Width.Set(182, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
			area.Height.Set(60, 0f);

			barFrame = new UIImage(ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/LivingShadowFrame")); // Frame of our resource bar
			barFrame.Left.Set(22, 0f);
			barFrame.Top.Set(0, 0f);
			barFrame.Width.Set(138, 0f);
			barFrame.Height.Set(34, 0f);

			barBack = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/LivingShadowFrameBack");
			fillCap = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/LivingShadowCap");

			text = new UIText("0/0", 0.8f); // text to show stat
			text.Width.Set(138, 0f);
			text.Height.Set(34, 0f);
			text.Top.Set(40, 0f);
			text.Left.Set(0, 0f);

			gradientA = ColorLib.DarkRift2;
			gradientB = ColorLib.Rift; 
            
			area.Append(text);
			area.Append(barFrame);
			Append(area);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			// This prevents drawing unless we are using one of the specified items
			if (!Main.LocalPlayer.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<RiftBattery>()))
				return;

			base.Draw(spriteBatch);
		}

		// Here we draw our UI
		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			var modPlayer = Main.LocalPlayer.GetModPlayer<LivingShadowPlayer>();
			float quotient = (float)modPlayer.LivingShadowCurrent / modPlayer.LivingShadowMax2;
			quotient = Utils.Clamp(quotient, 0f, 1f);

			// Get frame hitbox to align the inside stuff
			Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
			hitbox.X += 8;
			hitbox.Width -= 16;
			hitbox.Y += 4;
			hitbox.Height -= 10;

			// --- 1. BACK TEXTURE ---
			var dims = barFrame.GetDimensions();
			Vector2 pos = dims.Position(); // Top-left of the frame in UI-scaled coords
			spriteBatch.Draw(barBack.Value, pos, Color.White);


			// --- 2. RESOURCE BAR (Gradient) ---
			int left = hitbox.Left;
			int right = hitbox.Right;
			int steps = (int)((right - left) * quotient);
			for (int i = 0; i < steps; i++) {
				float percent = (float)i / (right - left);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value,
					new Rectangle(left + i, hitbox.Y, 1, hitbox.Height),
					Color.Lerp(gradientA, gradientB, percent));
			}

			// --- 3. FILL CAP ---
			if (quotient > 0f) {
				// Where the filled bar ends
				int capX = left + steps - (fillCap.Value.Width / 2);
				int capY = hitbox.Y + (hitbox.Height / 2);
				spriteBatch.Draw(fillCap.Value, new Vector2(capX, capY), Color.White);
			}

			// --- 4. FRAME (drawn normally via UI tree) ---
			// barFrame itself draws after this since it's appended in OnInitialize.
		}


		public override void Update(GameTime gameTime) {
			// This prevents updating unless we are using one of the specified items
			if (!Main.LocalPlayer.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<RiftBattery>()))
				return;

			var modPlayer = Main.LocalPlayer.GetModPlayer<LivingShadowPlayer>();

			// Update the text to show the resource values
			float percentage = (float)modPlayer.LivingShadowCurrent / modPlayer.LivingShadowMax2 * 100;
			text.SetText(Language.GetTextValue("Mods.DestroyerTest.UI.LivingShadow", percentage.ToString("0.##"), modPlayer.LivingShadowCurrent, modPlayer.LivingShadowMax2));

			base.Update(gameTime);
		}

		
	}

	// This class will only be autoloaded/registered if we're not loading on a server
	[Autoload(Side = ModSide.Client)]
	internal class LSBarUISystem : ModSystem
	{
		private UserInterface ResourceBarUserInterface;

		internal LivingShadowBar ResourceBar;

		public static LocalizedText LivingShadowText { get; private set; }

		public override void Load() {
			ResourceBar = new();
			ResourceBarUserInterface = new();
			ResourceBarUserInterface.SetState(ResourceBar);

			string category = "UI";
			LivingShadowText ??= Mod.GetLocalization($"{category}.LivingShadow");
		}

		public override void UpdateUI(GameTime gameTime) {
			ResourceBarUserInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"DestroyerTest: Living Shadow Bar",
					delegate {
						ResourceBarUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}