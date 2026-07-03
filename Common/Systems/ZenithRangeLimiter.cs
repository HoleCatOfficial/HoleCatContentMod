using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace DestroyerTest.Common.Systems
{
    public class ZenithRangeLimiter : ModSystem
    {
        public override void PreUpdatePlayers()
        {
            float MaxDist = 600f;

            Player p = Main.LocalPlayer;

            if (p.HeldItem.type == ItemID.Zenith)
            {

                if (Main.expertMode && !Main.masterMode)
                {
                    MaxDist = 400;
                }
                else if (Main.masterMode)
                {
                    MaxDist = 250;
                }
                else
                {
                    MaxDist = 600;
                }

                Vector2 mouseWorld = Main.MouseWorld;
                Vector2 toMouse = p.DirectionTo(mouseWorld);
                float toMouseRot = toMouse.ToRotation();

                if (mouseWorld.Distance(p.MountedCenter) > MaxDist)
                {
                    Vector2 NMouse = (p.MountedCenter - Main.screenPosition) + new Vector2(MaxDist - 20, 0).RotatedBy(toMouseRot);
                    Mouse.SetPosition((int)NMouse.X, (int)NMouse.Y);
                }

            }
        }
    }

    public class ZenithRangeLimiterPlayer : ModPlayer
    {
        float MaxDist = 600f;
        public override void PreUpdate()
        {
            

            if (Main.expertMode && !Main.masterMode)
            {
                if (MaxDist != 400)
                {
                    MaxDist = MathHelper.Lerp(MaxDist, 400, 0.01f);
                }
                else
                {
                    MaxDist = 400;
                }
            }
            else if (Main.masterMode)
            {
                if (MaxDist != 250)
                {
                    MaxDist = MathHelper.Lerp(MaxDist, 250, 0.01f);
                }
                else
                {
                    MaxDist = 250;
                }
            }
            else
            {
                if (MaxDist != 600)
                {
                    MaxDist = MathHelper.Lerp(MaxDist, 600, 0.01f);
                }
                else
                {
                    MaxDist = 600;
                }

                MaxDist = 600;
            }

            if (Player.HeldItem.type == ItemID.Zenith)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 Outer = Player.Center + Main.rand.NextVector2CircularEdge(MaxDist, MaxDist);
                    Vector2 Inwards = Player.Center - Outer;
                    Inwards.Normalize();
                    Dust d = Dust.NewDustPerfect(Outer, ModContent.DustType<ColorableNeonDust>(), Inwards * 1.6f, 100, Main.DiscoColor, 1f);
                    d.position += Player.velocity;
                }
            }
        }
        public override bool CanUseItem(Item item)
        {
            if (Player.HeldItem.type == ItemID.Zenith && !Player.WithinRange(Main.MouseWorld, 600f))
            {
                return false;
            }
            return base.CanUseItem(item);
        }
    }

    public class ZenithModGlobal : GlobalItem
    {
        public override bool InstancePerEntity => true;

        float MaxDist = 600f;
        Color LineColor = Color.White;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Main.expertMode && !Main.masterMode)
            {
                MaxDist = 400;
                LineColor = Main.DiscoColor;
            }
            else if (Main.masterMode)
            {
                MaxDist = 250;
                LineColor = Opus.Sine(Color.OrangeRed, Color.Goldenrod);
            }
            else
            {
                MaxDist = 600;
                LineColor = Color.Gray;
            }

            if (item.type == ItemID.Zenith)
            {
                TooltipLine line = new(Mod, "ZenithRangeLimit", $"Mouse range is limited to {MaxDist} pixels while holding this weapon.");
                line.OverrideColor = LineColor;

                TooltipLine line2 = new(Mod, "ZenithDamageCap", "Damage per hit is capped at 200.");
                line2.OverrideColor = LineColor;

                tooltips.Add(line);
                tooltips.Add(line2);
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (item.type == ItemID.Zenith && line.Name == "ZenithRangeLimit")
            {

            }
            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }
    }

    public class ZenithDamageCapGloal : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.type == ProjectileID.FinalFractal)
            {
                modifiers.SetMaxDamage(200);
            }
        }
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            
        }
    }
}
