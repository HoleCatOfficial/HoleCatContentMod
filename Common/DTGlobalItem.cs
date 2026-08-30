using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources.Cloths;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Rarity;
using System.Collections.Generic;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.RiftArsenal;
using Terraria.GameContent.ItemDropRules;
using System.Linq;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Equips.ScepterAccessories;
using Terraria.DataStructures;
using DestroyerTest.Content.SummonItems;
using OpusLib;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;

namespace DestroyerTest.Common
{

    public class SootFromFurnace : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            
            RecipeItemCreationContext c = context as RecipeItemCreationContext;
            if (c == null)
            {
                return;
            }
            
            if (c.Recipe.HasTile(TileID.Furnaces))
            {
                c.Recipe.AddOnCraftCallback(SootFurnaceRecipeCallback.GetSoot);
            }
        }
    }

    public static class SootFurnaceRecipeCallback
	{
		public static void GetSoot(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack) 
        {
			if (Main.rand.NextBool(24)) 
            {
				
				Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), ModContent.ItemType<Soot>(), Main.rand.Next(1, 5));
			}
		}
	}

    public class SpecifySolutions : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public static int[] SpecifysThatItCannotBeUsedByClentaminator = new int[]
        {
            ModContent.ItemType<TanninSolution>()
        };

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (SpecifysThatItCannotBeUsedByClentaminator.Contains(item.type))
            {
                TooltipLine line = new TooltipLine(Mod, "NoClentaminatorUse", "DestroyerTest.Items.Sets.CannotBeUsedByClentaminator")
                {
                    OverrideColor = Color.GreenYellow
                };
                tooltips.Add(line);
            }
        }
    }

    internal class DevTooltipFlashUpdater : ModSystem
    {
        public static DevTooltipFlashUpdater Instance = ModContent.GetInstance<DevTooltipFlashUpdater>();
        public float DevFlashOpacity = 0f;
        public override void PreUpdateItems()
        {
            if (Main.rand.NextBool(100))
            {
                DevFlashOpacity = 1f;
            }

            if (DevFlashOpacity > 0f)
            {
                DevFlashOpacity -= 0.01f;
            }
        }
    }
    public class TooltipColors : GlobalItem
    {
        public override bool InstancePerEntity => true;

        

        public override void UpdateInventory(Item item, Player player)
        {
            
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {

            if (item.rare == ModContent.RarityType<WretchedRarity>() && line.Name == "ItemName")
            {
                Color In = Opus.Sine(ColorLib.Wretched6, ColorLib.Wretched7, 0.01f);
                //line.SpecialColorInnerOuter(ColorLib.WretchedGradient(), In);

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, ColorLib.WretchedGradient(), new Vector2(0.5f, 0.5f));
                return false;
            }

            if (item.rare == ModContent.RarityType<InfectedRarity>() && line.Name == "ItemName")
            {
                Color In = Opus.Sine(ColorLib.Wretched6, Color.DarkRed, 0.01f);
                //line.SpecialColorInnerOuter(ColorLib.WretchedGradient(), In);

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, Opus.Sine(ColorLib.WretchedGradient(), ColorLib.IchorCrystalGradient, 0.01f), new Vector2(0.5f, 0.5f));
                return false;
            }

            if (item.rare == ModContent.RarityType<SoulRarity>() && line.Name == "ItemName")
            {
                Color In = Opus.Sine(ColorLib.Soul, ColorLib.Soul3, 0.01f);


                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, Color.White, new Vector2(0.5f, 0.5f));
                return false;
            }

            if (item.rare == ModContent.RarityType<PrimalRarity>() && line.Name == "ItemName")
            {
                Color In = Opus.Sine(Color.DarkRed, Color.MediumVioletRed, 0.1f);
                //line.SpecialColorInnerOuter(ColorLib.WretchedGradient(), In);

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, ColorLib.IchorCrystalGradient, new Vector2(0.5f, 0.5f));
                return false;
            }

            if (item.rare == ModContent.RarityType<ShimmeringRarity>() && line.Name == "ItemName")
            {
                //line.SpecialColorInnerOuter(ColorLib.WretchedGradient(), In);

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, Color.Black, ColorLib.TenebrisGradient, new Vector2(0.5f, 0.5f));
                return false;
            }

            if (item.rare == ModContent.RarityType<DevRarity>() && line.Name == "ItemName" || line.Name == "DevTooltip")
            {
                Color C1 = Opus.Sine(Color.Red, Color.OrangeRed, 0.01f);
                Color C2 = Opus.Sine(Color.Gold, Color.MediumOrchid, 0.01f);
                Color CA = Opus.Sine(C1, C2, 0.1f);
                Color Out = Opus.Sine(Color.Black, CA, 0.2f);
                Color In = Opus.Sine(Color.Black, Color.DarkRed, 0.4f);

                

                float scale = 1.25f;

                Vector2 textSize = FontAssets.MouseText.Value.MeasureString(line.Text);
                float xOffset = textSize.X * (scale - 1f) * 0.5f;

                float RandOff = Main.rand.NextFloat(-3f, 3f);

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, (line.X + RandOff) - xOffset, line.Y + RandOff, Color.Black * DevTooltipFlashUpdater.Instance.DevFlashOpacity, Color.NavajoWhite * DevTooltipFlashUpdater.Instance.DevFlashOpacity, new Vector2(0.5f, 0f), scale);

                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, Out, new Vector2(0.5f, 0.5f));

                return false;
            }

            if (item.rare == ModContent.RarityType<RiftRarity1>() && line.Name == "ItemName")
            {
                Color In = Opus.Sine(Color.Black, ColorLib.Rift, 0.1f);
                Color Out = Opus.Sine(ColorLib.Rift, Color.Black, 0.1f);
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, Out, new Vector2(0.5f, 0.5f));
                return false; 
            }
            if (item.rare == ModContent.RarityType<RiftRarity2>() && line.Name == "ItemName")
            {
                Color In = Opus.Sine(ColorLib.DarkRift2, ColorLib.LightRift3, 0.1f);
                Color Out = Opus.Sine(ColorLib.Rift, Color.White, 0.1f);
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, Out, new Vector2(0.5f, 0.5f));

                return false;
            }
            if (item.rare == ModContent.RarityType<ContenderRarity>() && line.Name == "ItemName")
            {
                float speed = 0.08f;
                float lerpAmount = 0.5f * (1 + (float)Math.Sin(Main.GlobalTimeWrappedHourly * speed * 2f * Math.PI));

                // Define control points in order: Black → Red → Black → White → Black
                Color[] colors = { Color.Black, Color.Red, Color.Black, Color.White, Color.Black };

                // Map lerpAmount (0 → 1) to a segment of our five-color gradient
                float scaledLerp = lerpAmount * (colors.Length - 1);
                int index = (int)scaledLerp;  // Get the segment index
                float segmentLerp = scaledLerp - index;  // Get the lerp factor within the segment

                // Ensure index stays within bounds
                index = Math.Clamp(index, 0, colors.Length - 2);

                // Lerp between the two selected colors
                Color strokeColor = Color.Lerp(colors[index], colors[index + 1], segmentLerp);



                // Main text color
                Color textColor = new Color(0, 0, 0);

                // Extract the correct font reference
                DynamicSpriteFont font = FontAssets.MouseText.Value;

                // Draw the outline first by offsetting in all directions
                Vector2 position = new Vector2(line.X, line.Y);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue; // Skip center (main text)
                        ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(i, j), strokeColor, 0f, Vector2.Zero, Vector2.One);
                    }
                }

                // Draw the actual text on top
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);



                return false; // Prevents Terraria from drawing the default text
            }
            if (item.rare == ModContent.RarityType<CrimsonSpecialRarity>() && line.Name == "ItemName")
            {
                // Define two colors to cycle between for the stroke
                Color strokeColor1 = new Color(255, 0, 0);
                Color strokeColor2 = new Color(100, 0, 0);

                Color textColor1 = new Color(40, 0, 0);
                Color textColor2 = new Color(80, 0, 0);

                Color In = Opus.Sine(textColor1, textColor2, 0.1f);
                Color Out = Opus.Sine(strokeColor1, strokeColor2, 0.1f);
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, Out, new Vector2(0.5f, 0.5f));

                return false;
            }
            if (item.rare == ModContent.RarityType<CorruptionSpecialRarity>() && line.Name == "ItemName")
            {
                // Define two colors to cycle between for the stroke
                Color strokeColor1 = new Color(120, 0, 169);
                Color strokeColor2 = new Color(68, 0, 95);

                Color textColor1 = new Color(37, 11, 48);
                Color textColor2 = new Color(61, 23, 78);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color strokeColor = Color.Lerp(strokeColor1, strokeColor2, lerpAmount);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount2 = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color textColor = Color.Lerp(textColor1, textColor2, lerpAmount2);

                // Extract the correct font reference
                DynamicSpriteFont font = FontAssets.MouseText.Value;

                // Draw the outline first by offsetting in all directions
                Vector2 position = new Vector2(line.X, line.Y);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue; // Skip center (main text)
                        ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(i, j), strokeColor, 0f, Vector2.Zero, Vector2.One);
                    }
                }

                // Draw the actual text on top
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);

                return false; // Prevents Terraria from drawing the default text
            }
            if (item.rare == ModContent.RarityType<HallowedSpecialRarity>() && line.Name == "ItemName")
            {
                // Define two colors to cycle between for the stroke
                Color strokeColor1 = new Color(0, 210, 255);
                Color strokeColor2 = new Color(0, 93, 112);

                Color textColor1 = new Color(10, 55, 65);
                Color textColor2 = new Color(65, 54, 10);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color strokeColor = Color.Lerp(strokeColor1, strokeColor2, lerpAmount);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount2 = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color textColor = Color.Lerp(textColor1, textColor2, lerpAmount2);

                // Extract the correct font reference
                DynamicSpriteFont font = FontAssets.MouseText.Value;

                // Draw the outline first by offsetting in all directions
                Vector2 position = new Vector2(line.X, line.Y);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue; // Skip center (main text)
                        ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(i, j), strokeColor, 0f, Vector2.Zero, Vector2.One);
                    }
                }

                // Draw the actual text on top
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);

                return false; // Prevents Terraria from drawing the default text
            }


            if (item.DamageType == ModContent.GetInstance<ScepterClass>() && line.Name == "Damage")
            {
                // Define two colors to cycle between for the stroke
                Color strokeColor1 = new Color(255, 255, 255);
                Color strokeColor2 = new Color(141, 242, 222);

                Color textColor1 = new Color(40, 40, 40);
                Color textColor2 = new Color(24, 48, 43);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color strokeColor = Color.Lerp(strokeColor1, strokeColor2, lerpAmount);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount2 = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color textColor = Color.Lerp(textColor1, textColor2, lerpAmount2);


                // Extract the correct font reference
                DynamicSpriteFont font = FontAssets.MouseText.Value;

                // Draw the outline first by offsetting in all directions
                Vector2 position = new Vector2(line.X, line.Y);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue; // Skip center (main text)
                        ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(i, j), strokeColor, 0f, Vector2.Zero, Vector2.One);
                    }
                }

                // Draw the actual text on top
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);

                return false; // Prevents Terraria from drawing the default text
            }
            if (item.rare == ModContent.RarityType<LifeEchoRarity>() && line.Name == "ItemName")
            {
                // Define two colors to cycle between for the stroke
                Color strokeColor1 = new Color(255, 255, 255);
                Color strokeColor2 = new Color(184, 228, 242);

                Color textColor1 = new Color(0, 0, 0);
                Color textColor2 = new Color(3, 24, 30);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color strokeColor = Color.Lerp(strokeColor1, strokeColor2, lerpAmount);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount2 = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color textColor = Color.Lerp(textColor1, textColor2, lerpAmount2);

                // Extract the correct font reference
                DynamicSpriteFont font = FontAssets.MouseText.Value;

                // Draw the outline first by offsetting in all directions
                Vector2 position = new Vector2(line.X, line.Y);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue; // Skip center (main text)
                        ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(i, j), strokeColor, 0f, Vector2.Zero, Vector2.One);
                    }
                }

                // Draw the actual text on top
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);

                return false; // Prevents Terraria from drawing the default text
            }
            if (item.rare == ModContent.RarityType<EndemyRarity>() && line.Name == "ItemName")
            {
                // Define two colors to cycle between for the stroke
                Color strokeColor1 = new Color(132, 8, 172);
                Color strokeColor2 = new Color(218, 191, 28);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color strokeColor = Color.Lerp(strokeColor1, strokeColor2, lerpAmount);

                // Main text color
                Color textColor = new Color(34, 32, 52);

                // Extract the correct font reference
                DynamicSpriteFont font = FontAssets.MouseText.Value;

                // Draw the outline first by offsetting in all directions
                Vector2 position = new Vector2(line.X, line.Y);
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue; // Skip center (main text)
                        ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(i, j), strokeColor, 0f, Vector2.Zero, Vector2.One);
                    }
                }

                // Draw the actual text on top
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);

                return false; // Prevents Terraria from drawing the default text
            }
            if (item.rare == ModContent.RarityType<StellarRarity>() && line.Name == "ItemName")
            {

                float prog1 = Opus.Sine(0f, 1f);
                float prog2 = Opus.Sine(1f, 0f);

                Color In = OpusColorUtils.MultiLerp(prog1, ColorLib.StellarFireColormap);
                Color Out = OpusColorUtils.MultiLerp(prog2, ColorLib.StellarFireColormap);
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, line.Text, line.X, line.Y, In, Out, new Vector2(0.5f, 0.5f));

                return false; // Prevents Terraria from drawing the default text
            }
            return true; // Default behavior for other rarities 
        }
    }

    public class DevGlobal : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return DTUtils.isDevItem[entity.type] == true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            string customText = "Developer Item";

            TooltipLine line = new TooltipLine(Mod, "DevTooltip", customText)
            {
                OverrideColor = Color.Black
            };

            tooltips.Add(line);
        }
    }

    public class InspiroItem : GlobalItem
    {
        // Define stroke and text colors
        static Color strokeColor1 = new Color(134, 53, 112);
        static Color strokeColor2 = new Color(108, 42, 90);

        static Color textColor1 = new Color(151, 114, 147);
        static Color textColor2 = new Color(112, 82, 109);

        // Set global behavior to affect all items
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return true; // Applies to all items
        }

        // Modify tooltips to add the custom developer line
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Define the custom tooltip text
            string customText = $"\n{CrazyText.scrambledString} \nInspiro Design \n{CrazyText.scrambledString}";

            // Create a new TooltipLine with a custom color
            TooltipLine line = new TooltipLine(Mod, "CustomTooltip", customText)
            {
                OverrideColor = Color.Purple // Optional: Base color
            };

            // Add the custom tooltip to the end of the list
            if (item.TryGetGlobalItem(out InspiroItemList globalItem) && globalItem.isInspiro)
            {
                tooltips.Add(line);
            }
        }

        // PreDrawTooltipLine - Draw the text and stroke manually
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            // Check if it's our custom tooltip
            if (line.Name == "CustomTooltip" && line.Mod == Mod.Name && item.TryGetGlobalItem(out InspiroItemList globalItem) && globalItem.isInspiro)
            {
                // Smoothly interpolate between stroke and text colors using sine wave
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color strokeColor = Color.Lerp(strokeColor1, strokeColor2, lerpAmount);
                Color textColor = Color.Lerp(textColor1, textColor2, lerpAmount);

                // Define the font and position
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                Vector2 position = new Vector2(line.X, line.Y);

                // Draw the stroke by offsetting text in all directions
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue; // Skip center (main text)
                        Vector2 offsetPosition = position + new Vector2(i, j);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, line.Text, offsetPosition, strokeColor, 0f, Vector2.Zero, Vector2.One);
                    }
                }

                // Draw the actual text on top with the smooth color transition
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);

                // Return false to prevent default drawing since we manually drew it
                return false;
            }

            // Allow other tooltips to draw normally
            return true;
        }
    }


    public class ScepterClassFamily : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.DamageType == ModContent.GetInstance<ScepterClass>();
        }

        public override void SetDefaults(Item item)
        {
            if (item.DamageType == ModContent.GetInstance<ScepterClass>())
            {
                item.GetGlobalItem<ScepterClassFamily>().isScepter = true;
            }
        }

        public bool isScepter = false;
    }

    public class InspiroItemList : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            if (ModLoader.TryGetMod("FranciumCalamityWeapons", out Mod FCW))
            {
                return entity.type == FCW.Find<ModItem>("Overlord").Type || entity.type == ModContent.ItemType<SparkFrostCleaver>();
            }
            else
            {
                // Apply this GlobalItem to specific items based on criteria
                return entity.type == ModContent.ItemType<SparkFrostCleaver>();
            }
        }

        public override void SetDefaults(Item item)
        {
            if (ModLoader.TryGetMod("FranciumCalamityWeapons", out Mod FCW))
            {
                if (item.type == ModContent.ItemType<SparkFrostCleaver>() || item.type == FCW.Find<ModItem>("Overlord").Type)
                {
                    item.GetGlobalItem<InspiroItemList>().isInspiro = true;
                }
            }
            else if (item.type == ModContent.ItemType<SparkFrostCleaver>())
            {
                item.GetGlobalItem<InspiroItemList>().isInspiro = true;
            }
        }
        public bool isInspiro = false;
    }


    public class NonWhiteCloth : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            // Apply this GlobalItem to specific items based on criteria
            return entity.type == ModContent.ItemType<RedCloth>() ||
            entity.type == ModContent.ItemType<OrangeCloth>() ||
            entity.type == ModContent.ItemType<YellowCloth>() ||
            entity.type == ModContent.ItemType<LimeCloth>() ||
            entity.type == ModContent.ItemType<GreenCloth>() ||
            entity.type == ModContent.ItemType<TealCloth>() ||
            entity.type == ModContent.ItemType<CyanCloth>() ||
            entity.type == ModContent.ItemType<SkyBlueCloth>() ||
            entity.type == ModContent.ItemType<BlueCloth>() ||
            entity.type == ModContent.ItemType<PurpleCloth>() ||
            entity.type == ModContent.ItemType<VioletCloth>() ||
            entity.type == ModContent.ItemType<PinkCloth>() ||
            entity.type == ModContent.ItemType<BlackCloth>() ||
            entity.type == ModContent.ItemType<BrownCloth>();
        }

        public override void SetDefaults(Item item)
        {
            if (item.type == ModContent.ItemType<OrangeCloth>() ||
            item.type == ModContent.ItemType<YellowCloth>() ||
            item.type == ModContent.ItemType<LimeCloth>() ||
            item.type == ModContent.ItemType<GreenCloth>() ||
            item.type == ModContent.ItemType<TealCloth>() ||
            item.type == ModContent.ItemType<CyanCloth>() ||
            item.type == ModContent.ItemType<SkyBlueCloth>() ||
            item.type == ModContent.ItemType<BlueCloth>() ||
            item.type == ModContent.ItemType<PurpleCloth>() ||
            item.type == ModContent.ItemType<VioletCloth>() ||
            item.type == ModContent.ItemType<PinkCloth>() ||
            item.type == ModContent.ItemType<BlackCloth>() ||
            item.type == ModContent.ItemType<BrownCloth>())
            {
                item.GetGlobalItem<NonWhiteCloth>().isNonWhiteCloth = true;
            }
        }
        public bool isNonWhiteCloth = false;
    }

    public class AllCloth : GlobalItem
    {
        public override bool InstancePerEntity => true;


        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            // Apply this GlobalItem to specific items based on criteria
            return entity.type == ModContent.ItemType<RedCloth>() ||
            entity.type == ModContent.ItemType<OrangeCloth>() ||
            entity.type == ModContent.ItemType<YellowCloth>() ||
            entity.type == ModContent.ItemType<LimeCloth>() ||
            entity.type == ModContent.ItemType<GreenCloth>() ||
            entity.type == ModContent.ItemType<TealCloth>() ||
            entity.type == ModContent.ItemType<CyanCloth>() ||
            entity.type == ModContent.ItemType<SkyBlueCloth>() ||
            entity.type == ModContent.ItemType<BlueCloth>() ||
            entity.type == ModContent.ItemType<PurpleCloth>() ||
            entity.type == ModContent.ItemType<VioletCloth>() ||
            entity.type == ModContent.ItemType<PinkCloth>() ||
            entity.type == ModContent.ItemType<BlackCloth>() ||
            entity.type == ModContent.ItemType<BrownCloth>() ||
            entity.type == ModContent.ItemType<WhiteCloth>();
        }
    }

    public static class StaticCloths
    {
        public static void StaticDefaultToCloth(this Item item)
        {
            item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[item.type] = 22;
        }

        public static void DefaultToCloth(this Item item)
        {
            item.width = 26;
			item.height = 28;
			item.value = 20;
			item.maxStack = 9999;
            item.rare = ItemRarityID.White;
        }

        public static void DefaultRecipe(this Item item, int DyeItemID1, int DyeItemID2 = -1)
        {
            Recipe recipeTatteredCloth = Recipe.Create(item.type, 1);
            Recipe recipeSilk = Recipe.Create(item.type, 1);
            Recipe recipeWhiteCloth = Recipe.Create(item.type, 1);

            recipeTatteredCloth.AddIngredient(ItemID.TatteredCloth, 1);
            recipeTatteredCloth.AddIngredient(DyeItemID1, 1);
            if (DyeItemID2 != -1)
            {
                recipeTatteredCloth.AddIngredient(DyeItemID2, 1);
            }
            recipeTatteredCloth.AddCondition(Condition.NearWater);

            recipeSilk.AddIngredient(ItemID.Silk, 1);
            recipeSilk.AddIngredient(DyeItemID1, 1);
            if (DyeItemID2 != -1)
            {
                recipeSilk.AddIngredient(DyeItemID2, 1);
            }
            recipeSilk.AddCondition(Condition.NearWater);

            recipeWhiteCloth.AddIngredient<WhiteCloth>(1);
            recipeWhiteCloth.AddIngredient(DyeItemID1, 1);
            if (DyeItemID2 != -1)
            {
                recipeWhiteCloth.AddIngredient(DyeItemID2, 1);
            }
            recipeWhiteCloth.AddCondition(Condition.NearWater);

            recipeTatteredCloth.Register();
            recipeSilk.Register();
            recipeWhiteCloth.Register();
        }
    }

    public class LockBoxLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.ObsidianLockbox)
            {
                foreach (IItemDropRule item4 in itemLoot.Get(false))
                {
                    OneFromRulesRule val = (OneFromRulesRule)(object)(item4 is OneFromRulesRule ? item4 : null);
                    if (val != null && CheckIfAtleastOneWithin(val.options, 274, 683, 220, 218, 3019))
                    {
                        HashSet<IItemDropRule> hashSet = new HashSet<IItemDropRule>(val.options);
                        hashSet.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ShadowScepter>(), 3, 1, 1));
                        val.options = hashSet.ToArray();
                    }
                }
            }

            if (item.type == ItemID.OasisCrate || item.type == ItemID.OasisCrateHard)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThunderScepter>(), 8, 1, 1));
            }
            if (item.type == ItemID.FloatingIslandFishingCrate || item.type == ItemID.FloatingIslandFishingCrateHard)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<StarScroll>(), 4, 1, 1));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TurbulenceScroll>(), 6, 1, 1));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TurbulenceScroll>(), 6, 1, 1));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TurbulenceScroll>(), 6, 1, 1));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TurbulenceScroll>(), 6, 1, 1));
            }
            if (item.type == ItemID.LavaCrate || item.type == ItemID.LavaCrateHard)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MalevolenceMantra>(), 4, 1, 1));
            }
        }

        private static bool CheckIfAtleastOneWithin(IItemDropRule[] rules, params int[] items)
        {
            foreach (IItemDropRule val in rules)
            {
                CommonDropNotScalingWithLuck val2 = (CommonDropNotScalingWithLuck)(object)(val is CommonDropNotScalingWithLuck ? val : null);
                if (val2 != null && items.Contains(val2.itemId))
                {
                    return true;
                }
                ItemDropWithConditionRule val3 = (ItemDropWithConditionRule)(object)(val is ItemDropWithConditionRule ? val : null);
                if (val3 != null && items.Contains(val3.itemId))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class CraftingModification : ModSystem
    {

        public override void AddRecipes()
        {

        }

    }

    public class BossBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.QueenBeeBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScepterOfVespae>(), 3, 1, 1));
            }
            if (item.type == ItemID.QueenSlimeBossBag)
            {
                itemLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<GelCane>()));
            }
            if (item.type == ItemID.FairyQueenBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrismaticScepter>(), 5, 1, 1));
            }
            if (item.type == ItemID.MoonLordBossBag)
            {
                itemLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ElementalAlkahest>()));
            }

            if (item.type == ItemID.WallOfFleshBossBag)
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScepterEmblem>(), 5, 1, 1));

                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConstantineMask>(), 1, 1, 1));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CoatStantine>(), 1, 1, 1));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ConstanJeans>(), 1, 1, 1));
            }
        }

    }

    public class ModifyVanillaItems : GlobalItem
    {
        public override void UpdateEquip(Item item, Player player)
        {
            if (item.type == ItemID.CobaltShield || item.type == ItemID.ObsidianShield)
            {
                player.noKnockback = false;
                if (player.TryGetModPlayer<BroochKnockbackPlayer>(out var knockbackPlayer))
                {
                    knockbackPlayer.Active = true;
                    knockbackPlayer.CobaltShieldKnockback = true;
                }
            }
        }
    }
}