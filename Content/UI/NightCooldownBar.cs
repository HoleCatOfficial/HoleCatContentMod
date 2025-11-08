using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using DestroyerTest.Common;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using DestroyerTest.Content.Equips.NightSet;

namespace DestroyerTest.Content.UI
{
    public class NightCooldownBar : UIState
    {
        private Texture2D barTexture;
        private const int totalFrames = 4;
        private UIElement area;
        private UIImage frameImage;
        private Player player;

        public override void OnInitialize()
        {
            player = Main.LocalPlayer;

            // Create root container
            area = new UIElement();
            area.Width.Set(105, 0f);
            area.Height.Set(22, 0f);
            area.Left.Set(-52, 0.5f); // Center horizontally
            area.Top.Set(200, 0f); // Vertical offset (adjust as needed)

            // Load frame texture
            frameImage = new UIImage(ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightSetCooldownBar"));
            frameImage.Width.Set(72, 0f);
            frameImage.Height.Set(22, 0f);
            frameImage.Color = Color.White * 0f;
            area.Append(frameImage);
            

            Append(area);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            player = Main.LocalPlayer;
        }

        private float opacity = 0f; // default invisible
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            NightPlayer nightPlayer = player.GetModPlayer<NightPlayer>();

            // Fade logic
            if (nightPlayer.Cooldown)
                opacity = MathHelper.Clamp(opacity + 0.05f, 0f, 1f); // fade in
            else
                opacity = MathHelper.Clamp(opacity - 0.05f, 0f, 1f); // fade out

            Color clr = Color.White * opacity;

            if (barTexture == null)
                barTexture = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightSetCooldownBar").Value;

            DTUIConfig cfg = ModContent.GetInstance<DTUIConfig>();

            // Calculate the frame based on cooldown
            float progress = 1f - (float)nightPlayer.CooldownTime / 360f;
            int frame = (int)MathHelper.Clamp(progress * totalFrames, 0, totalFrames - 1); // <-- changed

            Rectangle sourceRect = new Rectangle(
                0, frame * (barTexture.Height / totalFrames), barTexture.Width, barTexture.Height / totalFrames
            );

            Vector2 position = new Vector2(
                (Main.screenWidth / 2f - barTexture.Width / 2f) + cfg.NightBarXPos,
                (Main.screenHeight / 2f + 200) + cfg.NightBarYPos
            );

            spriteBatch.Draw(barTexture, position, sourceRect, clr);
        }

    }

    public class NightCooldownBarSystem : ModSystem
    {
        private UserInterface nightUI;
        private NightCooldownBar nightBar;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                nightBar = new NightCooldownBar();
                nightUI = new UserInterface();
                nightUI.SetState(nightBar);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            nightUI?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "DestroyerTest: NightCooldown",
                    delegate
                    {
                        nightUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
