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
using ReLogic.Graphics;
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

namespace DestroyerTest.Content.UI
{
   
    public class NightmareRoseHealthBar : UIState
    {
        private UIElement area;
        private Asset<Texture2D> barFront;
        private Asset<Texture2D> barBack;
        private Asset<Texture2D> barCaps;

        private Asset<Texture2D> barFront_Fables;
        private Asset<Texture2D> barBack_Fables;

        private Asset<Texture2D> NodeLock;
        private Asset<Texture2D> NodeLock_Fables;

        public override void OnInitialize()
        {
           
            area = new UIElement();
            area.Width.Set(528f, 0f);
            area.Height.Set(168f, 0f);
            area.HAlign = 0.5f;
            area.VAlign = 0.2f;


            barBack = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBarBack");
            barFront = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBarFront");
            barCaps = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBarCaps");

            barBack_Fables = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/FablesBossBarBack");
            barFront_Fables = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/FablesBossBarFront");

            NodeLock = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBar_NodeLock");

            NodeLock_Fables = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBar_NodeLock_Fables");

            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            List<NPC> NightmareRoseInstances = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>()).ToList();

            if (NightmareRoseInstances.Count <= 0)
                return;

            var NR = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<NightmareRoseBoss>());

            if (NR.ModNPC is NightmareRoseBoss nightmareRose)
            {
                if (!nightmareRose.anyNodesAlive)
                {
                    if (NR.dontTakeDamage)
                    {
                        if (BarOpacity > 0)
                        {
                            BarOpacity -= 0.05f;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (BarOpacity < 1)
                        {
                            BarOpacity += 0.05f;
                        }
                    }
                }
                else
                {
                    if (BarOpacity < 1)
                    {
                        BarOpacity += 0.05f;
                    }
                }
            }

            base.Draw(spriteBatch);
        }

        public static Vector2[] NodeLockShake;

        float BarOpacity = 1f;

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

                Vector2 barOrigin = !DTCrossMod.FablesIsLoaded ? barBack.Size() * 0.5f : barBack_Fables.Size() * 0.5f;
                Vector2 drawPos = new Vector2(GetDimensions().Width / 2, GetDimensions().Height / 2) + new Vector2(0, -400);
                Rectangle frameCrop = new Rectangle(0, 0, (int)(quotient * barFront.Width()), barFront.Height());

                

                if (NR.ModNPC is NightmareRoseBoss nightmareRose)
                {
                    Color BarFrontColor = !DTCrossMod.FablesIsLoaded ? (!DestroyerTestMod.MasochistIsActive ? OpusColorUtils.MultiLerp(quotient.Inverse(), ColorLib.WretchedColorMap) : ColorLib.TenebrisGradient) : (!DestroyerTestMod.MasochistIsActive ? OpusColorUtils.MultiLerp(quotient.Inverse(), ColorLib.WretchedColorMap) : ColorLib.TenebrisGradient) with { A = 0 };

                    if (!DTCrossMod.FablesIsLoaded)
                    {
                        spriteBatch.Draw(barBack.Value, drawPos, null, Color.White * BarOpacity, 0f, barOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                        spriteBatch.Draw(barFront.Value, drawPos, frameCrop, BarFrontColor * BarOpacity, 0f, barOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);

                    
                        spriteBatch.Draw(barCaps.Value, drawPos, null, Color.White * BarOpacity, 0f, barCaps.Size() / 2, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                    }
                    else
                    {
                        spriteBatch.Draw(barBack_Fables.Value, drawPos, null, Color.White * BarOpacity, 0f, barOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                        spriteBatch.Draw(barFront_Fables.Value, drawPos, frameCrop, BarFrontColor * BarOpacity, 0f, barFront_Fables.Size() / 2, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                    }


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
                        NodeLockShake = newShake ;
                    }

                    if (cfn.Count > 0)
                    {
                        for (int i = 0; i < cfn.Count; i++)
                        {
                            int NodeLockFrameNumber = (int)MathHelper.Lerp(3, 0, (float)cfn[i].life / (float)cfn[i].lifeMax);
                            NodeLockFrameNumber = Utils.Clamp(NodeLockFrameNumber, 0, 3);
                            Rectangle NodeLockFrame = new Rectangle(0, NodeLockFrameNumber * NodeLockDimensions.Height, NodeLockDimensions.Width, NodeLockDimensions.Height);
                            Vector2 NodeLockOrigin = NodeLockFrame.Size() / 2f;

                            Vector2 NodeLockDrawPos =  new Vector2((drawPos.X - (186.4f * DTUIConfig.instance.CustomBossBarScaleModifier)), drawPos.Y - (0.8f * DTUIConfig.instance.CustomBossBarScaleModifier)) + new Vector2((NodeLockDimensions.Width * Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier) * i, 0);

                            if (cfn[i].life > 0)
                            {
                                if (!DTCrossMod.FablesIsLoaded)
                                {
                                    spriteBatch.Draw(NodeLock.Value, NodeLockDrawPos + ((NodeLockShake[i] / 2.5f) * DTUIConfig.instance.CustomBossBarScaleModifier), NodeLockFrame, Color.White * BarOpacity, 0f, NodeLockOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                                }
                                else
                                {
                                    spriteBatch.Draw(NodeLock_Fables.Value, NodeLockDrawPos + ((NodeLockShake[i] / 2.5f) * DTUIConfig.instance.CustomBossBarScaleModifier), NodeLockFrame, Color.White * BarOpacity, 0f, NodeLockOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                                }

                                DTUtils.DrawChargeBar(Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, NodeLockDrawPos + new Vector2(0, (16f * DTUIConfig.instance.CustomBossBarScaleModifier)), (float)cfn[i].life / (float)cfn[i].lifeMax, Color.DarkRed * BarOpacity);
                            
                            
                            }
                        }
                    }
                    



                    var TEXT = Language.GetTextValue("Mods.DestroyerTest.NPCs.NightmareRoseBoss.BossBarDisplayName");
                    spriteBatch.DrawString(DTAssetLib.Doxent.Value, TEXT, drawPos + new Vector2(0, -24f * DTUIConfig.instance.CustomBossBarScaleModifier), Color.White * BarOpacity, 0f, DTAssetLib.Doxent.Value.MeasureString(TEXT) * 0.5f, Main.UIScale * ((DTUIConfig.instance.CustomBossBarScaleModifier / 2.5f) * 0.7f ), SpriteEffects.None, 0f);

                    string DefText = $"Defense: {NR.defense}";
                    //spriteBatch.DrawString(DTAssetLib.Doxent.Value, DefText, drawPos + new Vector2(360, 40f), Color.White, 0f, DTAssetLib.Doxent.Value.MeasureString(DefText) * 0.5f, Main.UIScale * 0.2f, SpriteEffects.None, 0f);
                    float LifePercent = (float)Math.Round(((float)NR.life / (float)NR.lifeMax) * 100, 2);
                    string LifePercentText = $"{LifePercent}%";
                    spriteBatch.DrawString(DTAssetLib.Doxent.Value, LifePercentText, drawPos + new Vector2(-140 * DTUIConfig.instance.CustomBossBarScaleModifier, 40f), Color.White * BarOpacity, 0f, DTAssetLib.Doxent.Value.MeasureString(DefText) - new Vector2(0, DTAssetLib.Doxent.Value.MeasureString(DefText).Y / 2), Main.UIScale * ((DTUIConfig.instance.CustomBossBarScaleModifier / 2.5f) * 0.35f), SpriteEffects.None, 0f);
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


    public class WyvernCorpseHealthBar : UIState
    {
        private UIElement area;
        private Asset<Texture2D> barFront;
        private Asset<Texture2D> barBack;
        private Asset<Texture2D> barCaps;
        private Asset<Texture2D> barFront_Fables;
        private Asset<Texture2D> barBack_Fables;

        private Asset<Texture2D> NodeLock;
        private Asset<Texture2D> NodeLock_Fables;

        public override void OnInitialize()
        {

            area = new UIElement();
            area.Width.Set(528f, 0f);
            area.Height.Set(168f, 0f);
            area.HAlign = 0.5f;
            area.VAlign = 0.2f;


            barBack = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBarBack");
            barFront = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBarFront");
            barCaps = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/NightmareRoseBarCaps");

            barBack_Fables = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/FablesBossBarBack");
            barFront_Fables = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/FablesBossBarFront");

            NodeLock = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/WyvernCorpseBar_NodeLock");

            NodeLock_Fables = ModContent.Request<Texture2D>("DestroyerTest/Assets/Textures/WyvernCorpseBar_NodeLock_Fables");

            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            List<NPC> WyvernCorpseInstances = Main.npc.Where(n => n.active && n.type == ModContent.NPCType<WyvernCorpseHead>()).ToList();

            if (WyvernCorpseInstances.Count <= 0)
                return;

            var WC = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());

            if (WC.ModNPC is WyvernCorpseHead Head)
            {
                if (!Head.anyNodesAlive)
                {
                    if (WC.dontTakeDamage)
                    {
                        if (BarOpacity > 0)
                        {
                            BarOpacity -= 0.05f;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (BarOpacity < 1)
                        {
                            BarOpacity += 0.05f;
                        }
                    }
                }
                else
                {
                    if (BarOpacity < 1)
                    {
                        BarOpacity += 0.05f;
                    }
                }
            }

            base.Draw(spriteBatch);
        }

        public static Vector2[] NodeLockShake;

        float BarOpacity = 1f;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var WC = Main.npc.FirstOrDefault(n => n.active && n.type == ModContent.NPCType<WyvernCorpseHead>());

            var Cap = spriteBatch.Capture();

            Cap.SamplerState = SamplerState.PointWrap;

            spriteBatch.End();
            spriteBatch.Begin(Cap);

            if (WC != null)
            {
                float quotient = (float)WC.life / (float)WC.lifeMax;
                quotient = Utils.Clamp(quotient, 0f, 1f);

                Vector2 barOrigin = !DTCrossMod.FablesIsLoaded ? barBack.Size() * 0.5f : barBack_Fables.Size() * 0.5f;
                Vector2 drawPos = new Vector2(GetDimensions().Width / 2, GetDimensions().Height / 2) + new Vector2(0, -400);
                Rectangle frameCrop = new Rectangle(0, 0, (int)(quotient * barFront.Width()), barFront.Height());



                if (WC.ModNPC is WyvernCorpseHead Head)
                {
                    Color BarFrontColor = !DTCrossMod.FablesIsLoaded ? (!DestroyerTestMod.MasochistIsActive ? OpusColorUtils.MultiLerp(quotient.Inverse(), ColorLib.IchorCrystalColorMap) : ColorLib.Soul) : (!DestroyerTestMod.MasochistIsActive ? OpusColorUtils.MultiLerp(quotient.Inverse(), ColorLib.IchorCrystalColorMap) : ColorLib.Soul) with { A = 0 };
                    
                    if (!DTCrossMod.FablesIsLoaded)
                    {
                        spriteBatch.Draw(barBack.Value, drawPos, null, Color.White * BarOpacity, 0f, barOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                        spriteBatch.Draw(barFront.Value, drawPos, frameCrop, BarFrontColor * BarOpacity, 0f, barOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);


                        spriteBatch.Draw(barCaps.Value, drawPos, null, Color.White * BarOpacity, 0f, barCaps.Size() / 2, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                    }
                    else
                    {
                        spriteBatch.Draw(barBack_Fables.Value, drawPos, null, Color.White * BarOpacity, 0f, barOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                        spriteBatch.Draw(barFront_Fables.Value, drawPos, frameCrop, BarFrontColor * BarOpacity, 0f, barFront_Fables.Size() / 2, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                    }


                    Rectangle NodeLockDimensions = new Rectangle(0, 0, 176, 30);

                    var IN = Head.iNodes;

                    /*
                    if (NodeLockShake == null || NodeLockShake.Length < cfn.Count)
                    {
                        NodeLockShake = new Vector2[cfn.Count];
                    }
                    */

                    if (NodeLockShake == null || NodeLockShake.Length != IN.Count)
                    {
                        Vector2[] newShake = new Vector2[IN.Count];
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

                    if (IN.Count > 0)
                    {
                        for (int i = 0; i < IN.Count; i++)
                        {
                            int NodeLockFrameNumber = (int)MathHelper.Lerp(3, 0, (float)IN[i].life / (float)IN[i].lifeMax);
                            NodeLockFrameNumber = Utils.Clamp(NodeLockFrameNumber, 0, 3);
                            Rectangle NodeLockFrame = new Rectangle(0, NodeLockFrameNumber * NodeLockDimensions.Height, NodeLockDimensions.Width, NodeLockDimensions.Height);
                            Vector2 NodeLockOrigin = NodeLockFrame.Size() / 2f;

                            Vector2 NodeLockDrawPos = new Vector2((drawPos.X - 375), drawPos.Y - 2) + new Vector2((NodeLockDimensions.Width * Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier) * i, 0);

                            if (IN[i].life > 0)
                            {
                                if (!DTCrossMod.FablesIsLoaded)
                                {
                                    spriteBatch.Draw(NodeLock.Value, NodeLockDrawPos + NodeLockShake[i], NodeLockFrame, Color.White * BarOpacity, 0f, NodeLockOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                                }
                                else
                                {
                                    spriteBatch.Draw(NodeLock_Fables.Value, NodeLockDrawPos + NodeLockShake[i], NodeLockFrame, Color.White * BarOpacity, 0f, NodeLockOrigin, Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, 0f, 0f);
                                }

                                DTUtils.DrawChargeBar(Main.UIScale * DTUIConfig.instance.CustomBossBarScaleModifier, NodeLockDrawPos + new Vector2(0, 40), (float)IN[i].life / (float)IN[i].lifeMax, Color.DarkRed * BarOpacity);


                            }
                        }
                    }




                    var TEXT = Language.GetTextValue("Mods.DestroyerTest.NPCs.WyvernCorpseHead.BossBarDisplayName");
                    spriteBatch.DrawString(DTAssetLib.Doxent.Value, TEXT, drawPos + new Vector2(0, -60f), Color.White * BarOpacity, 0f, DTAssetLib.Doxent.Value.MeasureString(TEXT) * 0.5f, Main.UIScale * 0.7f, SpriteEffects.None, 0f);

                    string DefText = $"Defense: {WC.defense}";
                    //spriteBatch.DrawString(DTAssetLib.Doxent.Value, DefText, drawPos + new Vector2(360, 40f), Color.White, 0f, DTAssetLib.Doxent.Value.MeasureString(DefText) * 0.5f, Main.UIScale * 0.2f, SpriteEffects.None, 0f);
                    float LifePercent = (float)Math.Round(((float)WC.life / (float)WC.lifeMax) * 100, 2);
                    string LifePercentText = $"{LifePercent}%";
                    spriteBatch.DrawString(DTAssetLib.Doxent.Value, LifePercentText, drawPos + new Vector2(-350, 40f), Color.White * BarOpacity, 0f, DTAssetLib.Doxent.Value.MeasureString(DefText) - new Vector2(0, DTAssetLib.Doxent.Value.MeasureString(DefText).Y / 2), Main.UIScale * 0.35f, SpriteEffects.None, 0f);
                }
            }

            spriteBatch.ResetToDefaultUI();
            base.DrawSelf(spriteBatch);


        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class WyvernCorpseBarUISystem : ModSystem
    {
        private UserInterface ResourceBarUserInterface;

        internal WyvernCorpseHealthBar Bar;


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
                    "DestroyerTest: Wyvern Corpse Boss Bar",
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
