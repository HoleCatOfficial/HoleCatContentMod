using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.BorderBiome
{
    public class BorderHeatPlayer : ModPlayer
    {
        public int CurrentHeat = 0;
        public int MaxHeat = 1000;

        public float Normalized => (float)CurrentHeat / (float)MaxHeat;

        public int ExhaustionThreshold = 200;
        public int[] DamageThreshold = [400, 600, 800];

        string DeathMessage => Language.GetTextValueWith("Mods.DestroyerTest.CustomDeath.BorderCooked", Player.name);

        public bool IsInCoolingState = false;

        public override void ResetEffects()
        {

        }

        public override void PostUpdateMiscEffects()
        {
            if (IsInCoolingState)
            {
                CurrentHeat -= 2;
            }
            else
            {


                if (Player.InModBiome<Border>() || Player.ZoneUnderworldHeight)
                {
                    if (CurrentHeat < MaxHeat)
                    {
                        if (Player.miscCounter % 20 == 0)
                        {
                            int amount = (int)MathHelper.Lerp(1, 3, Normalized);
                            CurrentHeat += amount;
                        }
                    }
                    else
                    {
                        CurrentHeat = MaxHeat;
                    }

                    if (CurrentHeat >= MaxHeat)
                    {
                        Player.KillMe(PlayerDeathReason.ByCustomReason(NetworkText.FromLiteral(DeathMessage)), 72000, 0);
                    }
                    else
                    {
                        if (CurrentHeat > ExhaustionThreshold)
                        {

                        }

                        for (int i = 0; i < 2; i++)
                        {
                            if (CurrentHeat > DamageThreshold[i])
                            {
                                Player.AddBuff(BuffID.OnFire, 300);
                            }
                        }
                    }
                }
                else
                {
                   
                    CurrentHeat--;
                }
            }

            if (CurrentHeat < 0)
            {
                CurrentHeat = 0;
            }
            
            
        }

        public override void UpdateDead()
        {
            CurrentHeat = 0;
        }
    }

    public class BorderHeatDraw : ModSystem
    {
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.GetModPlayer<BorderHeatPlayer>().CurrentHeat > 0)
            {
                float Opacity = MathHelper.Lerp(0f, 0.8f, Main.LocalPlayer.GetModPlayer<BorderHeatPlayer>().Normalized);
                float sin = Opus.Sine(1f, 0.9f, 0.2f);
                Texture2D tex = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/TenebrousConstructBGGlow").Value;

                Main.EntitySpriteDraw(tex, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.OrangeRed with { A = 0 } * Opacity * sin, 0f, tex.Size() / 2, new Vector2(220f, 1.5f), SpriteEffects.None);
            }
        }
    }
}
