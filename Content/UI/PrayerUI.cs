using DestroyerTest.Common;
using DestroyerTest.Common.Blessings;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace DestroyerTest.Content.UI
{
    public class PrayerUI : UIState
    {

        private UIPanel panel;
        private UIText herbSlotLabel;
        private UIText offeringSlotLabel;
        private UIText blessButtonText;
        private UIText Header;
        private UIImageButton blessButton;
        private UIImageButton CloseButton;
        private UIItemSlot HerbSlot;
        private UIItemSlot OfferingSlot;
        public static bool Visible = false;
        private bool dragging = false;
        private Vector2 offset;
        public Terraria.Item[] HerbItem;
        public Terraria.Item[] OfferingItem;

        public override void OnInitialize()
        {
            if (Main.dedServ)
                return;

            panel = new UIPanel();
            panel.HAlign = 0.5f;
            panel.VAlign = 0.5f;
            panel.Top.Set(-150f, 0f);
            panel.Width.Set(200f, 0f);
            panel.Height.Set(200f, 0f);
            panel.BackgroundColor = new Color(20, 20, 40, 200);

            panel.OnLeftMouseDown += StartDrag;
            panel.OnLeftMouseUp += EndDrag;
            Append(panel);

            Header = new UIText("Make an offer", 0.85f, false);
            Header.HAlign = 0.5f;
            Header.VAlign = 0.1f;
            Header.TextColor = ColorLib.Soul;
            panel.Append(Header);

            //Offer Slot

            offeringSlotLabel = new UIText("Offering", 0.75f, false);
            offeringSlotLabel.HAlign = 0.5f;
            offeringSlotLabel.VAlign = 0.35f;
            offeringSlotLabel.TextColor = Color.White;
            panel.Append(offeringSlotLabel);

            OfferingItem = new Terraria.Item[1];
            OfferingItem[0] = new Item();
            OfferingItem[0].TurnToAir();
            OfferingSlot = new UIItemSlot(OfferingItem, 0, 0);
            OfferingSlot.Width.Set(24, 0);
            OfferingSlot.Height.Set(24, 0);
            OfferingSlot.HAlign = 0.5f;
            OfferingSlot.VAlign = 0.5f;

            panel.Append(OfferingSlot);

            //Herb slot

            herbSlotLabel = new UIText("Herb", 0.75f, false);
            herbSlotLabel.HAlign = 0.8f;
            herbSlotLabel.VAlign = 0.35f;
            herbSlotLabel.TextColor = Color.White;
            panel.Append(herbSlotLabel);

            HerbItem = new Terraria.Item[1];
            HerbItem[0] = new Item();
            HerbItem[0].TurnToAir();
            HerbSlot = new UIItemSlot(HerbItem, 0, 0);
            HerbSlot.Width.Set(24, 0);
            HerbSlot.Height.Set(24, 0);
            HerbSlot.HAlign = 0.8f;
            HerbSlot.VAlign = 0.5f;
            
            panel.Append(HerbSlot);

            //Button

            blessButton = new UIImageButton(ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/BlessingButton"));
            blessButton.Width.Set(76, 0);
            blessButton.Height.Set(28, 0);
            blessButton.HAlign = 0.5f;
            blessButton.VAlign = 0.85f;
            blessButton.OnLeftClick += CheckOffer;
            panel.Append(blessButton);

            blessButtonText = new UIText("Offer", 0.75f, false);
            blessButtonText.HAlign = 0.5f;
            blessButtonText.VAlign = 0.8f;
            blessButtonText.TextColor = Color.White;
            panel.Append(blessButtonText);

            //Close functionality

            CloseButton = new UIImageButton(ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/CloseButton"));
            CloseButton.Width.Set(24f, 0f);
            CloseButton.Height.Set(24f, 0f);
            CloseButton.HAlign = 0.05f;
            CloseButton.VAlign = 0.05f;
            CloseButton.OnLeftClick += close;
            panel.Append(CloseButton);
        }

        public override void OnActivate()
        {
            
        }

        public List<int> ValidHerbTypes = new List<int>
        {
            ItemID.Daybloom,
            ItemID.Moonglow,
            ItemID.Blinkroot,
            ItemID.Deathweed,
            ItemID.Waterleaf,
            ItemID.Fireblossom,
            ItemID.Shiverthorn  
        };

        public SoundStyle Accept = new SoundStyle("DestroyerTest/Assets/Audio/Blessing/AcceptedBlessing");
        public SoundStyle Reject = new SoundStyle("DestroyerTest/Assets/Audio/Blessing/RejectedBlessing");
        public int Cooldown = 0;

        public void CheckOffer(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.LocalPlayer.TryGetModPlayer<PrayerPlayer>(out var P))
            {

                if (!HerbItem[0].active || !OfferingItem[0].active)
                {
                    return;
                }
                if (!ValidHerbTypes.Contains(HerbItem[0].type))
                {
                    return;
                }

                if (Cooldown <= 0)
                {
                    if (SlotsHaveBlessingItems(DTBlessings.RadiantHeart))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.RadiantHeart);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.Enchanted))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.Enchanted);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.Attuned))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.Attuned);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.OozingAffection))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.OozingAffection);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.Serenity))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.Serenity);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.ThrivingDarknessCorr))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.ThrivingDarknessCorr);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.ThrivingDarknessCrim))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.ThrivingDarknessCrim);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.Decadence))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.Decadence);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.Overgrown))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.Overgrown);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.RejuvenatingWarmth))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.RejuvenatingWarmth);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else if (SlotsHaveBlessingItems(DTBlessings.MilkywayStride))
                    {
                        if (Cooldown <= 0)
                        {
                            HerbItem[0].TurnToAir();
                            OfferingItem[0].TurnToAir();
                            P.ApplyBlessing(DTBlessings.MilkywayStride);

                            SoundEngine.PlaySound(Accept);
                            Cooldown = 600;
                        }
                    }
                    else
                    {
                        if (HerbItem[0].type != ItemID.None)
                        {
                            Main.LocalPlayer.QuickSpawnItem(Player.GetSource_None(), HerbItem[0]);
                            HerbItem[0].TurnToAir();
                        }
                        if (OfferingItem[0].type != ItemID.None)
                        {
                            Main.LocalPlayer.QuickSpawnItem(Player.GetSource_None(), OfferingItem[0]);
                            OfferingItem[0].TurnToAir();
                        }
                        SoundEngine.PlaySound(Reject);
                        Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BlessingParticle>(), Main.LocalPlayer.Center, Vector2.Zero, Color.Violet, 0.01f, 2f);
                        CombatText.NewText(Main.LocalPlayer.Hitbox, Color.Violet, Language.GetTextValue("Mods.DestroyerTest.Blessings.RejectedMessage"), true, false);
                        Cooldown = 600;
                    }
                }
            }
        }

        private bool SlotsHaveBlessingItems(Blessing blessing)
        {
            return HerbItem[0].type == blessing.HerbType && OfferingItem[0].type == blessing.ItemType;
        }

        private void StartDrag(UIMouseEvent evt, UIElement listeningElement)
        {
            dragging = true;
            offset = new Vector2(evt.MousePosition.X - panel.Left.Pixels, evt.MousePosition.Y - panel.Top.Pixels);
        }

        private void EndDrag(UIMouseEvent evt, UIElement listeningElement)
        {
            dragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            
            if (Visible)
            {   
                if (Cooldown > 0)
                {
                    Cooldown--;
                }
                if (Cooldown == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item25);
                }

                if (IsMouseHovering)
                {
                    Main.isMouseLeftConsumedByUI = true;
                    Main.LocalPlayer.mouseInterface = true;
                }
                else
                {
                    Main.isMouseLeftConsumedByUI = false;
                    Main.LocalPlayer.mouseInterface = false;
                }

                if (dragging)
                {
                    Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY);
                    panel.Left.Set(mouse.X - offset.X, 0f);
                    panel.Top.Set(mouse.Y - offset.Y, 0f);
                    panel.Recalculate();
                }

                base.Update(gameTime);
            }
        }

        public void close(UIMouseEvent evt, UIElement listeningElement)
        {
            if (HerbItem[0].type != ItemID.None)
            {
                Main.LocalPlayer.QuickSpawnItem(Player.GetSource_None(), HerbItem[0]);
                HerbItem[0].TurnToAir();
            }
            if (OfferingItem[0].type != ItemID.None)
            {
                Main.LocalPlayer.QuickSpawnItem(Player.GetSource_None(), OfferingItem[0]);
                OfferingItem[0].TurnToAir();
            }
            Visible = false;
        }
    }
}
