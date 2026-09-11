using System;
using System.Collections.Generic;
using System.Linq;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.RiftBiomeSpread;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;


namespace DestroyerTest.Common
{
    public class ChargeBar
    {
        public ChargeBar() { }

        public string Identifier { get; set; }

        public Asset<Texture2D> Frame;
        public Asset<Texture2D> Fill;
        public Asset<Texture2D> Back;

        public Asset<Texture2D>[] ExtraOverFrame;

        public Asset<Texture2D>[] ExtraUnderBack;


        public ChargeBar(string identifier, Asset<Texture2D> frame, Asset<Texture2D> fill, Asset<Texture2D> back, float currentValue, float maxValue)
        {
            Identifier = identifier;
            Frame = frame;
            Fill = fill;
            Back = back;
            CurrentValue = currentValue;
            MaxValue = maxValue;
        }

        public ChargeBar(string identifier, Asset<Texture2D> frame, Asset<Texture2D> fill, Asset<Texture2D> back, Asset<Texture2D>[] extraOverFrame, Asset<Texture2D>[] extraUnderBack, float currentValue, float maxValue)
        {
            Identifier = identifier;
            Frame = frame;
            Fill = fill;
            Back = back;
            ExtraOverFrame = extraOverFrame;
            ExtraUnderBack = extraUnderBack;
            CurrentValue = currentValue;
            MaxValue = maxValue;
        }

        public float CurrentValue { get; set; }
        public float MaxValue { get; set; }
        public float ProgressPercentage => CurrentValue / MaxValue;

        public Vector2 Position { get; set; } = Main.screenPosition;
        public float Opacity { get; set; } = 1f;
        public float Scale { get; set; } = 1f;

        public List<DrawData> DrawInformation()
        {
            List<DrawData> Output = new();

            Vector2 barOrigin = Back.Value.Size() * 0.5f;
            Vector2 drawPos = Position - Main.screenPosition;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(ProgressPercentage * Fill.Value.Size().X), (int)(Fill.Value.Size().Y));

            SpriteBatch spriteBatch = Main.spriteBatch;

            if (ExtraUnderBack != null && ExtraUnderBack.Length > 0)
            {
                for (int i = 0; i < ExtraUnderBack.Length; i++)
                {
                    Output.Add(new DrawData(ExtraUnderBack[i].Value, drawPos, null, Color.White * Opacity, 0f, barOrigin, Scale, SpriteEffects.None, 0));
                }
            }

            Output.Add(new DrawData(Back.Value, drawPos, null, Color.White * Opacity, 0f, barOrigin, Scale, SpriteEffects.None, 0));
            Output.Add(new DrawData(Fill.Value, drawPos, frameCrop, Color.White * Opacity, 0f, barOrigin, Scale, SpriteEffects.None, 0));
            Output.Add(new DrawData(Frame.Value, drawPos, null, Color.White * Opacity, 0f, barOrigin, Scale, SpriteEffects.None, 0));

            if (ExtraOverFrame != null && ExtraOverFrame.Length > 0)
            {
                for (int i = 0; i < ExtraOverFrame.Length; i++)
                {
                    Output.Add(new DrawData(ExtraOverFrame[i].Value, drawPos, null, Color.White * Opacity, 0f, barOrigin, Scale, SpriteEffects.None, 0));
                }
            }

            return Output;
        }
    }

    public class PlayerChargeBarManager : ModPlayer
    {
        public List<ChargeBar> ChargeBars { get; set; } = new List<ChargeBar>();
        public List<Vector2> ChargeBarPositions { get; set; } = new List<Vector2>();

        public override void ResetEffects()
        {

        }

        public override void PostUpdateMiscEffects()
        {
            if (!Main.dedServ && ChargeBars != null && ChargeBarPositions != null)
            {
                for(int i = 0; i < ChargeBars.Count; i++)
                {
                    Vector2 targetPosition = new Vector2(Player.Center.X, Player.Bottom.Y + 10 + (25 * i));

                    ChargeBar bar = ChargeBars[i];

                    if (bar.Position.Distance(targetPosition) > 4)
                    {
                        bar.Position = Vector2.Lerp(bar.Position, targetPosition, 0.5f);
                    }
                    else
                    {
                        bar.Position = targetPosition;
                    }
                }
            }
        }

        public static ChargeBar AddBar(Player player, ChargeBar bar)
        {
            var manager = player.GetModPlayer<PlayerChargeBarManager>();

            if (manager != null && !manager.ChargeBars.Any(n => n.Identifier == bar.Identifier))
            {
                manager.ChargeBars.Add(bar);
            }

            return bar;
        }

        public static void RemoveBar(Player player, ChargeBar bar)
        {
            var manager = player.GetModPlayer<PlayerChargeBarManager>();

            if (manager != null && manager.ChargeBars.Any(n => n.Identifier == bar.Identifier))
            {
                manager.ChargeBars.Remove(bar);
            }
        }

        public override void Unload()
        {
            ChargeBars.Clear();
        }

    }

    public class PlayerChargeBarDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.FrozenOrWebbedDebuff);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.DeadOrGhost)
            {
                return;
            }
            else
            {
                var Manager = drawInfo.drawPlayer.GetModPlayer<PlayerChargeBarManager>();
                if (Manager != null && Manager.ChargeBars != null)
                {
                    foreach (var Bar in Manager.ChargeBars)
                    {
                        foreach (var Data in Bar.DrawInformation())
                        {
                            drawInfo.DrawDataCache.Add(Data);
                        }
                    }
                }
            }
        }
    }
}
