using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class SoulLifeOverlay : ModResourceOverlay
    {
        // Global toggle – if true, every life UI piece will be replaced with the overlay
        public static SoulEffectPlayer soulplayer = Main.LocalPlayer.GetModPlayer<SoulEffectPlayer>();
        public static bool SoulOverlayActive = soulplayer.RoseSoul || soulplayer.WyvernSoul;

		// This field is used to cache vanilla assets used in the CompareAssets helper method further down in this file
		private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

		// These fields are used to cache the result of ModContent.Request<Texture2D>()
		private Asset<Texture2D> heartTexture, fancyPanelTexture, barsFillingTexture, barsPanelTexture;

		public override void PostDrawResource(ResourceOverlayDrawContext context) {
			// If the overlay is not active, fall back to vanilla behavior
			if (!SoulOverlayActive)
				return;

            // Overwrite all life UI textures with your overlay
            // You can pick which texture you want to force here – I’m using the heart overlay as the default
            
			context.texture = heartTexture ??= ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/SoulLifeOverlay");
			context.source = context.texture.Frame();
			context.Draw();
		}

		// --- everything below is now unused, but I left it intact in case you want the old selective behavior ---
		private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath) {
			if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
				asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

			return existingAsset == asset;
		}

		private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context) {
			context.texture = heartTexture ??= ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/SoulLifeOverlay");
			context.Draw();
		}

		private void DrawFancyPanelOverlay(ResourceOverlayDrawContext context) {
			context.texture = fancyPanelTexture ??= ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/SoulLifeOverlay_Panel");
			context.source = context.texture.Frame();
			context.Draw();
		}

		private void DrawBarsOverlay(ResourceOverlayDrawContext context) {
			context.texture = barsFillingTexture ??= ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/BarSoulLifeOverlay_Fill");
			context.Draw();
		}

		private void DrawBarsPanelOverlay(ResourceOverlayDrawContext context) {
			context.texture = barsPanelTexture ??= ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/BarSoulLifeOverlay_Panel");
			context.source = context.texture.Frame();
			context.position.Y += 6;
			context.Draw();
		}
	}
}
