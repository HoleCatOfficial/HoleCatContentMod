using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Classless;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.ClasslessItems
{
    public class TheBookofLifeAndDeath : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ModContent.RarityType<SoulRarity>();
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.autoReuse = false;
            Item.damage = 2000;
            Item.DamageType = DamageClass.Generic;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<BoLDHoldout>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new(Mod, "ThankYouMessage", Language.GetTextValue("Mods.DestroyerTest.Extras.ThankYouMessage")) { OverrideColor = Main.DiscoColor };
            tooltips.Add(line);
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            Color baseColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
            
            Vector3 HSL = Main.rgbToHsl(baseColor);


            if (line.Name == "ThankYouMessage")
            {
                //Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.DeathText.Value, line.Text, line.X, line.Y, Main.DiscoColor, Color.White, new Vector2(0.5f, 0.5f), 1f);
                Color[] Rainbow = new Color[line.Text.Length];
                for (int i = 0; i < Rainbow.Length; i++)
                {
                    float progress = (float)i / Rainbow.Length;

                    float shiftedHue = (HSL.X - progress) % 1f;

                    float Mult = MathHelper.Lerp(1f, 0f, progress);

                    Rainbow[i] = Main.hslToRgb(new Vector3(shiftedHue, HSL.Y, HSL.Z));
                }

                DTUtils.SweepColorOverString(line.Text, Rainbow, new Vector2(line.X, line.Y), 16f);
                return false;
            }
            return true;
        }
    
    }
}
