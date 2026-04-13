using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class MeleeScaleTooltip : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void SetStaticDefaults()
        {
            
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (DTUtils.isSpecialSwingSword[item.type])
            {
                var ScaleText = Language.GetText("Mods.DestroyerTest.Tooltips.MeleeScaleTooltip").Format(DTUtils.TooltipScaleMult[item.type]);
                TooltipLine Line = new TooltipLine(Mod, "SpecialMeleeScale", ScaleText);
                tooltips.Add(Line);
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "SpecialMeleeScale" && DTUtils.isSpecialSwingSword[item.type])
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, Color.White, Main.DiscoColor, new Vector2(0.5f, 0.5f), 1f);
            }
            return true;
        }
    }
}
