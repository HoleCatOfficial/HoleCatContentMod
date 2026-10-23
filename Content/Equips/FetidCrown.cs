
using System.Collections.Generic;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.BloodMoon;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DestroyerTest.Content.Equips
{
    public class FetidCrown : ModItem
    {
        public bool Offensive = false;
        public int clickCooldown = 0;

        public Asset<Texture2D> ClosedEquipTexture;
        public Asset<Texture2D> OpenEquipTexture;

        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = 10;
            Item.accessory = true;
            ClosedEquipTexture = ModContent.Request<Texture2D>(Texture + "_Closed_Head");
            OpenEquipTexture = ModContent.Request<Texture2D>(Texture + "_Open_Head");

            /*
            Mod.Logger.Info($"Open asset null: {OpenEquipTexture is null}");
            Mod.Logger.Info($"Closed asset null: {ClosedEquipTexture is null}");
            Mod.Logger.Info($"Open texture null: {OpenEquipTexture?.Value is null}");
            Mod.Logger.Info($"Closed texture null: {ClosedEquipTexture?.Value is null}");
            */
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override bool CanRightClick()
        {
            return clickCooldown <= 0;
        }
        public override void RightClick(Player player)
        {
            if (!Offensive && clickCooldown <= 0)
            {
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath38);
                }

                Offensive = true;
                clickCooldown = 10;
            }
            if (Offensive && clickCooldown <= 0)
            {
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath45);
                }

                Offensive = false;
                clickCooldown = 10;
            }
        }

        public override void UpdateInventory(Player player)
        {
            GlowOpacity = Opus.Sine(0.2f, 0.7f, 0.02f);
            if (clickCooldown > 0)
            {
                clickCooldown--;
            }
        }

        float GlowOpacity = 0f;
        int frameOff = 0;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            int frameHeight = 38;

            frame.Height = frameHeight;
            frame.Y = frameOff;

            if (!Offensive)
            {
                frameOff = 0;
            }
            else
            {
                frameOff = frameHeight;
            }

            Texture2D T = ModContent.Request<Texture2D>(Texture).Value;

            if (Offensive)
            {
                spriteBatch.Draw(DTAssetLib.PointGlowPreMultiplied.Value, position, null, new Color(204, 34, 150) with { A = 0 } * GlowOpacity, 0f, DTAssetLib.PointGlowPreMultiplied.Value.Size() / 2, 3.7f, SpriteEffects.None, 0f);
            }    

            spriteBatch.Draw(T, position, frame, drawColor, 0f, new Vector2(frame.Width / 2, (frame.Height / 2)), scale * 2, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            int frameHeight = 38;

            Main.GetItemDrawFrame(Type, out var T, out var Frame);
            Vector2 orig = Frame.Size() / 2;
            Vector2 position = Item.Bottom - Main.screenPosition /* - new Vector2(0, orig.Y)*/;

            Frame.Height = frameHeight;
            Frame.Y = frameOff;

            if (!Offensive)
            {
                frameOff = 0;
            }
            else
            {
                frameOff = frameHeight;
            }

            spriteBatch.Draw(T, position, Frame, alphaColor, rotation, orig, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine OffensiveTitle = new TooltipLine(Mod, "OffensiveTitle", "--Offense Mode--");
            TooltipLine OffensiveTooltip = new TooltipLine(Mod, "OffensiveTooltip", Language.GetTextValue("Mods.DestroyerTest.Items.FetidCrown.TooltipOffensive"));

            TooltipLine DefensiveTitle = new TooltipLine(Mod, "DefensiveTitle", "--Defense Mode--");
            TooltipLine DefensiveTooltip = new TooltipLine(Mod, "DefensiveTooltip", Language.GetTextValue("Mods.DestroyerTest.Items.FetidCrown.TooltipDefensive"));

            if (Offensive)
            {
                tooltips.Add(OffensiveTitle);
                tooltips.Add(OffensiveTooltip);
            }
            else
            {
                tooltips.Add(DefensiveTitle);
                tooltips.Add(DefensiveTooltip);
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 Position = new Vector2(line.X, line.Y);
            if (line.Name == "OffensiveTitle" || line.Name == "DefensiveTitle")
            {
                Utils.DrawBorderString(spriteBatch, line.Text, Position, new Color(204, 34, 150), 1f, 0f, 0f);
                return false;
            }

            if (line.Name == "OffensiveTooltip" || line.Name == "DefensiveTooltip")
            {
                Utils.DrawBorderString(spriteBatch, line.Text, Position, new Color(130, 66, 110), 1f, 0f, 0f);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            ClosedEquipTexture = ModContent.Request<Texture2D>(Texture + "_Closed_Head");
            OpenEquipTexture = ModContent.Request<Texture2D>(Texture + "_Open_Head");
            player.GetModPlayer<FetidCrownModePlayer>().Crown = Item;
            player.GetModPlayer<FetidCrownModePlayer>().DisplayLayer = true;

            if (Offensive)
            {
                player.maxMinions += 1;
                player.GetArmorPenetration(DamageClass.Summon) += 8;
            }
            else
            {
                player.buffImmune[ModContent.BuffType<SoulErosion>()] = true;
                player.endurance += 0.11f;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("FCOffensive", Offensive);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("FCOffensive"))
            {
                Offensive = tag.GetBool("FCOffensive");
            }

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Dyrn>(32)
                .AddIngredient<LifeEcho>(6)
                .AddIngredient(ItemID.GoldCrown)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class FetidCrownModePlayer : ModPlayer
    {

        public Item Crown;

        public bool DisplayLayer = false;

        public override void ResetEffects()
        {
            DisplayLayer = false;
        }
        public override void Unload()
        {
            Crown = null;
        }
    }

    public class FetidCrownDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.FaceAcc);
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<FetidCrownModePlayer>().DisplayLayer;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            bool draw = player.GetModPlayer<FetidCrownModePlayer>().Crown != null;
            if (draw)
            {
                Item item = player.GetModPlayer<FetidCrownModePlayer>().Crown;
                FetidCrown crown = item.ModItem as FetidCrown;
                if (crown != null)
                {
                    Texture2D selectedHeadTex = crown.Offensive ? crown.OpenEquipTexture.Value : crown.ClosedEquipTexture.Value;
                    SpriteEffects effects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    DrawData data = new(selectedHeadTex, player.headPosition - Main.screenPosition, player.headFrame, Lighting.GetSubLight(player.headPosition).ToColor(), player.headRotation, Vector2.Zero, 1f, effects);
                    drawInfo.DrawDataCache.Add(data);
                }
            }
        }
    }
}