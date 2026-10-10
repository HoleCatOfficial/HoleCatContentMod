using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class FusionBurn : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FBPlayer>().lifeRegenDebuff = true;
        }
        public override void Update(NPC target, ref int buffIndex)
        {
            if (target.TryGetGlobalNPC<FBTarget>(out var modNPC))
            {
                modNPC.lifeRegenDebuff = true;
            }
        }
    }

    public class FBTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool lifeRegenDebuff;

        public override void ResetEffects(NPC npc)
        {
            lifeRegenDebuff = false;
        }

        public Vector2[] DrawPositions;

        public bool ShouldDraw = false;
        float Radius = 120f;
        public float Opacity = 0f;

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.IsABestiaryIconDummy)
            {
                return;
            }

            if (ShouldDraw)
            {
                if (DrawPositions != null)
                {
                    for (int i = 0; i < DrawPositions.Length; i++)
                    {
                        Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, DrawPositions[i] - Main.screenPosition, null, ColorLib.CelestialGradient with { A = 0 } * Opacity, 0f, DTAssetLib.FeatheredCircle.Value.Size() / 2, 0.4f, SpriteEffects.None);
                        Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, DrawPositions[i] - Main.screenPosition, null, Color.White with { A = 0 } * Opacity, 0f, DTAssetLib.FeatheredCircle.Value.Size() / 2, 0.3f, SpriteEffects.None);
                    }
                }
            }
        }
        public override void AI(NPC npc)
        {
            DrawPositions = Opus.GetEquidistantOrbitVectors(12, npc.Center, 0.01f, Radius);

            if (lifeRegenDebuff)
            {
                
                Lighting.AddLight(npc.Center, ColorLib.CelestialGradient.ToVector3());
                ShouldDraw = true;
            }


            if (ShouldDraw)
            {
                if (lifeRegenDebuff)
                {
                    if (Radius > npc.Size.Length() * 0.6f)
                    {
                        Radius -= 0.5f;
                    }
                    else
                    {
                        Opus.Sine(npc.Size.Length() * 0.7f, npc.Size.Length() * 0.6f);
                    }

                    if (Opacity < 1)
                    {
                        Opacity += 0.05f;
                    }
                    else
                    {
                        Opacity = 1f;
                    }
                }
                else
                {
                    if (Radius < npc.Size.Length() * 1.3f)
                    {
                        Radius += 0.5f;
                    }
                    else
                    {
                        Radius = npc.Size.Length() * 1.3f;
                        ShouldDraw = false;
                    }

                    if (Opacity > 0)
                    {
                        Opacity -= 0.05f;
                    }
                    else
                    {
                        Opacity = 0f;

                    }
                }
            }
        }


        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (lifeRegenDebuff)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 90;
            }
        }
    }

    public class FBPlayer : ModPlayer
    {
        public bool lifeRegenDebuff;

        public override void ResetEffects()
        {
            lifeRegenDebuff = false;
        }

        public Vector2[] DrawPositions;

        public bool ShouldDraw = false;
        float Radius = 120f;
        public float Opacity = 0f;

        public override void PostUpdateBuffs()
        {
            DrawPositions = Opus.GetEquidistantOrbitVectors(12, Player.MountedCenter, 0.01f, Radius);

            if (lifeRegenDebuff)
            {
                
                Lighting.AddLight(Player.MountedCenter, ColorLib.CelestialGradient.ToVector3());
                ShouldDraw = true;
            }
            else
            {

            }

            if (ShouldDraw)
            {
                if (lifeRegenDebuff)
                {
                    if (Radius > 40)
                    {
                        Radius -= 0.5f;
                    }
                    else
                    {
                        Opus.Sine(30f, 40f);
                    }

                    if (Opacity < 1)
                    {
                        Opacity += 0.05f;
                    }
                    else
                    {
                        Opacity = 1f;
                    }
                }
                else
                {
                    if (Radius < 120)
                    {
                        Radius += 0.5f;
                    }
                    else
                    {
                        Radius = 120f;
                        ShouldDraw = false;
                    }

                    if (Opacity > 0 )
                    {
                        Opacity -= 0.05f;
                    }
                    else
                    {
                        Opacity = 0f;
                        
                    }
                }
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 90;
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (lifeRegenDebuff)
            {
                damageSource.CustomReason = NetworkText.FromFormattable("{0} achieved oneness with the celestial elements.", Player.name);
            }
        }
    }

    public class FusionBurnDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.BeetleBuff);
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<FBPlayer>().ShouldDraw;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            FBPlayer fb = player.GetModPlayer<FBPlayer>();
            if (drawInfo.shadow == 0)
            {
                if (fb.DrawPositions != null)
                {
                    for (int i = 0; i < fb.DrawPositions.Length; i++)
                    {
                        DrawData data = new(DTAssetLib.FeatheredCircle.Value, fb.DrawPositions[i] - Main.screenPosition, null, ColorLib.CelestialGradient with { A = 0} * fb.Opacity, 0f, DTAssetLib.FeatheredCircle.Value.Size() / 2, 0.4f, SpriteEffects.None);
                        drawInfo.DrawDataCache.Add(data);

                        DrawData data2 = new(DTAssetLib.FeatheredCircle.Value, fb.DrawPositions[i] - Main.screenPosition, null, Color.White with { A = 0 } * fb.Opacity, 0f, DTAssetLib.FeatheredCircle.Value.Size() / 2, 0.3f, SpriteEffects.None);
                        drawInfo.DrawDataCache.Add(data2);
                    }
                }
            }
        }
    }

}