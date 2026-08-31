
using System.Collections.Generic;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Consumables;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpusLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class LegendaryItems : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return DTUtils.LegendaryWeapon[entity.type];
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            crit += 10;
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "RelicOfNepostriaTitle")
            {
                Main.EntitySpriteDraw(DTAssetLib.PointGlowPreMultiplied.Value, new Vector2(line.X, line.Y - 5) + FontAssets.MouseText.Value.MeasureString(line.Text) / 2, null, Opus.Sine(Color.GreenYellow, Color.Fuchsia, 0.2f) with { A = 0 } * 0.7f, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, new Vector2(0.08f * FontAssets.MouseText.Value.MeasureString(line.Text).X, 1f) * Main.UIScale, SpriteEffects.None);
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, Color.White, Opus.Sine(Color.GreenYellow, Color.Fuchsia, 0.2f), new Vector2(0.5f, 0.5f));
                
                return false;
            }

            if (line.Name == "RelicOfNepostriaHint")
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, Color.Gray, Opus.Sine(Color.LightSlateGray, Color.SlateBlue), new Vector2(0.5f, 0.5f));
                return false;
            }

            if (line.Name == "RelicOfNepostriaExtraLore")
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, Color.White, Opus.Sine(Color.GreenYellow, Color.Fuchsia, 0.2f), new Vector2(0.5f, 0.5f));
                return false;
            }


            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            bool ShiftKey = (Main.keyState.IsKeyDown(Keys.LeftShift) && Main.oldKeyState.IsKeyDown(Keys.LeftShift)) || (Main.keyState.IsKeyDown(Keys.RightShift) && Main.oldKeyState.IsKeyDown(Keys.RightShift));

            TooltipLine Title = new TooltipLine(Mod, "RelicOfNepostriaTitle", Language.GetTextValue("Mods.DestroyerTest.Extras.LegendaryItems.Title"));
            int TitleIndex = tooltips.IndexOf(tooltips.Find(t => t.Name == "ItemName"));
            tooltips.Insert(TitleIndex + 1, Title);

            TooltipLine Hint = new TooltipLine(Mod, "RelicOfNepostriaHint", Language.GetTextValue("Mods.DestroyerTest.Extras.LegendaryItems.Clue"));
            int HintIndex = tooltips.IndexOf(tooltips.Find(t => t.Name == Hint.Name));

            TooltipLine ExtraLore = new TooltipLine(Mod, "RelicOfNepostriaExtraLore", Language.GetTextValue($"Mods.DestroyerTest.Items.{item.ModItem.Name}.NepostriaTooltip"));
            int ExtraLoreIndex = tooltips.IndexOf(tooltips.Find(t => t.Name == ExtraLore.Name));

            TooltipLine Standard= new TooltipLine(Mod, "RelicOfNepostriaStandardTooltip", Language.GetTextValue("Mods.DestroyerTest.Extras.LegendaryItems.StandardTooltip")) { OverrideColor = Colors.CoinGold};
            int StandardIndex = tooltips.IndexOf(tooltips.Find(t => t.Name == Standard.Name));

            tooltips.Add(Hint);
            tooltips.Add(ExtraLore);
            tooltips.Add(Standard);

            if (!ShiftKey)
            {
                ExtraLore.Hide();
                Standard.Hide();
            }
            else
            {
                Hint.Hide();
            }
        }
    }
}
