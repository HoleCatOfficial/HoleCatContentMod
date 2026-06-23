using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace DestroyerTest.Content.UI
{
   
    public class NightmareRoseHealthBar : UIState
    {
        private UIElement area;
        private Asset<Texture2D> barFront;
        private Asset<Texture2D> barBack;

        private Asset<Texture2D> NodeLock;

        public override void OnInitialize()
        {
           
            area = new UIElement();
            area.Width.Set(528f, 0f);
            area.Height.Set(168f, 0f);
            area.HAlign = 0.5f;
            area.VAlign = 0.2f;


            barBack = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBarBack");
            barFront = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBarFront");

            NodeLock = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBar_NodeLock");

            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            List<NPC> NightmareRoseInstances = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>()).ToList();

            if (NightmareRoseInstances.Count <= 0)
                return;

            base.Draw(spriteBatch);
        }

        public static Vector2[] NodeLockShake;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var NR = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());

            var Cap = spriteBatch.Capture();

            Cap.SamplerState = SamplerState.PointWrap;

            spriteBatch.End();
            spriteBatch.Begin(Cap);

            if (NR != null)
            {
                float quotient = (float)NR.life / (float)NR.lifeMax;
                quotient = Utils.Clamp(quotient, 0f, 1f);

                Vector2 barOrigin = barBack.Size() * 0.5f;
                Vector2 drawPos = new Vector2(GetDimensions().Width / 2, GetDimensions().Height / 2) + new Vector2(0, -400);
                Rectangle frameCrop = new Rectangle(0, 0, (int)(quotient * barFront.Width()), barFront.Height());

                

                if (NR.ModNPC is NightmareRoseBoss nightmareRose)
                {
                    spriteBatch.Draw(barBack.Value, drawPos, null, Color.White, 0f, barOrigin, Main.UIScale * 2.5f, 0f, 0f);
                    spriteBatch.Draw(barFront.Value, drawPos, frameCrop, OpusColorUtils.MultiLerp(quotient.Inverse(), ColorLib.WretchedColorMap), 0f, barOrigin, Main.UIScale * 2.5f, 0f, 0f);

                    
                    Rectangle NodeLockDimensions = new Rectangle(0, 0, 88, 30);

                    var cfn = nightmareRose.cfNodes;

                    /*
                    if (NodeLockShake == null || NodeLockShake.Length < cfn.Count)
                    {
                        NodeLockShake = new Vector2[cfn.Count];
                    }
                    */

                    if (NodeLockShake == null || NodeLockShake.Length != cfn.Count) 
                    { 
                        Vector2[] newShake = new Vector2[cfn.Count]; 
                        if (NodeLockShake != null) 
                        { 
                            int copy = Math.Min(NodeLockShake.Length, newShake.Length);
                            for (int j = 0; j < copy; j++)
                            {
                                newShake[j] = NodeLockShake[j];
                            }
                        } 
                        NodeLockShake = newShake;
                    }

                    if (cfn.Count > 0)
                    {
                        for (int i = 0; i < cfn.Count; i++)
                        {
                            int NodeLockFrameNumber = (int)MathHelper.Lerp(3, 0, (float)cfn[i].life / (float)cfn[i].lifeMax);
                            NodeLockFrameNumber = Utils.Clamp(NodeLockFrameNumber, 0, 3);
                            Rectangle NodeLockFrame = new Rectangle(0, NodeLockFrameNumber * NodeLockDimensions.Height, NodeLockDimensions.Width, NodeLockDimensions.Height);
                            Vector2 NodeLockOrigin = NodeLockFrame.Size() / 2f;

                            Vector2 NodeLockDrawPos =  new Vector2((drawPos.X - 510) + 44, drawPos.Y) + new Vector2((NodeLockDimensions.Width * Main.UIScale * 2.5f) * i, 0);

                            if (cfn[i].life > 0)
                            {
                                spriteBatch.Draw(NodeLock.Value, NodeLockDrawPos + NodeLockShake[i], NodeLockFrame, Color.White, 0f, NodeLockOrigin, Main.UIScale * 2.5f, 0f, 0f);
                            }
                        }
                    }
                    



                    var TEXT = Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.DisplayName");
                    spriteBatch.DrawString(DTAssetLib.Arial.Value, TEXT, drawPos + new Vector2(0, -100f), Color.White, 0f, DTAssetLib.Arial.Value.MeasureString(TEXT) * 0.5f, Main.UIScale * 2.5f, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.ResetToDefaultUI();
            base.DrawSelf(spriteBatch);


        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class NightmareRoseBarUISystem : ModSystem
    {
        private UserInterface ResourceBarUserInterface;

        internal NightmareRoseHealthBar Bar;


        public override void Load()
        {
            Bar = new();
            ResourceBarUserInterface = new();
            ResourceBarUserInterface.SetState(Bar);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            ResourceBarUserInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "DestroyerTest: Nightmare Rose Boss Bar",
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
