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
using System.Collections.Generic;
using Terraria.UI;

namespace DestroyerTest.Common.Systems
{
    /*
    public class BugMessageSystem : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            DTConfig config = ModContent.GetInstance<DTConfig>();
            if (!Main.dedServ)
            {
                int interfaceLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

                if (interfaceLayerIndex != -1)
                {
                    layers.Insert(interfaceLayerIndex, new LegacyGameInterfaceLayer(
                        "DestroyerTest: Bug Command Message",
                        delegate
                        {
                            if (config.ShowBugCommandMessage)
                            {
                                DrawAdvert(Main.spriteBatch);
                            }
                            return true;
                        },
                        InterfaceScaleType.UI)
                    );
                }
            }
        }

        private void DrawAdvert(SpriteBatch spriteBatch)
        {
            Texture2D Advert = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/BugCommandMessage", AssetRequestMode.ImmediateLoad).Value;  
            spriteBatch.Draw(Advert, new Vector2((Main.screenWidth / 2) - Advert.Width / 2, (Main.screenHeight / 2) - Advert.Height / 2), Color.White);
        }
    }
    */
}