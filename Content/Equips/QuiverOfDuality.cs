using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;


namespace DestroyerTest.Content.Equips
{
    public class QuiverOfDuality : ModItem
    {
        public int AreaBuff = BuffID.CursedInferno;
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = 1400;
            Item.rare = ModContent.RarityType<InfectedRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.15f;
            player.GetModPlayer<QuiverOfDualityPlayer>().Active = true;
            if (!hideVisual)
            {
                player.GetModPlayer<QuiverOfDualityPlayer>().Aura = true;
            }
            else
            {
                player.GetModPlayer<QuiverOfDualityPlayer>().Aura = false;
            }
        }

        public override void UpdateInventory(Player player)
        {
            if (SwitchCooldown > 0)
            {
                SwitchCooldown--;
            }
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override bool CanRightClick()
        {
            bool ShiftKey = (Main.keyState.IsKeyDown(Keys.LeftShift) && Main.oldKeyState.IsKeyDown(Keys.LeftShift)) || (Main.keyState.IsKeyDown(Keys.RightShift) && Main.oldKeyState.IsKeyDown(Keys.RightShift));
            return ShiftKey;
        }

        int SwitchCooldown = 0;
        public override void RightClick(Player player)
        {
            if (SwitchCooldown <= 0)
            {
                if (AreaBuff == BuffID.CursedInferno)
                {
                    SoundEngine.PlaySound(SoundID.Item114 with { Pitch = -0.8f });
                    AreaBuff = BuffID.Ichor;
                    player.GetModPlayer<QuiverOfDualityPlayer>().CurrentBuff = BuffID.Ichor;
                    SwitchCooldown = 60;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item114 with { Pitch = -0.8f });
                    AreaBuff = BuffID.CursedInferno;
                    player.GetModPlayer<QuiverOfDualityPlayer>().CurrentBuff = BuffID.CursedInferno;
                    SwitchCooldown = 60;
                } 
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (AreaBuff == BuffID.CursedInferno)
            {
                tooltips.Add(new TooltipLine(Mod, "QuiverOfDualityCI", "Current Area Buff: Cursed Inferno"));
            }
            else if (AreaBuff == BuffID.Ichor)
            {
                tooltips.Add(new TooltipLine(Mod, "QuiverOfDualityI", "Current Area Buff: Ichor"));
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WretchedShards>(8)
                .AddIngredient<PrimalShards>(8)
                .AddIngredient(ItemID.SpectreBar, 10)
                .Register();
        }
    }

    public class QuiverOfDualityPlayer : ModPlayer
    {
        public bool Active = false;
        public bool Aura = false;
        public int CurrentBuff = -1;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override float UseSpeedMultiplier(Item item)
        {
            return Active && item.DamageType == DamageClass.Ranged && item.useAmmo == AmmoID.Arrow ? 0.84f : 1f;
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            return Active && weapon.DamageType == DamageClass.Ranged && weapon.useAmmo == AmmoID.Arrow ? Main.rand.NextBool(4) : true;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Aura && Active && CurrentBuff != -1)
            {
                foreach (NPC n in Main.npc)
                {
                    if (n.Distance(Player.MountedCenter) < 400 && n.active)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap with { MaxInstances = 10, Volume = 0.5f }, n.Center);
                        n.AddBuff(CurrentBuff, 180);
                    }
                }
            }
        }
    }

    public class QuiverOfDualityDrawLayer : IPlayerPixelatedDrawer
    {
        PixelLayer IPlayerPixelatedDrawer.PixelLayer => PixelLayer.AboveTiles;

        float R = 0f;
        void IPlayerPixelatedDrawer.DrawPixelated(Player player, SpriteBatch spriteBatch)
        {
            R += 0.18f;
            var Cap = spriteBatch.Capture();
            //Cap.TransformMatrix = PixelationSystem.PixelationMatrix;
            spriteBatch.End();
            spriteBatch.Begin(Cap);

            if (player.TryGetModPlayer(out QuiverOfDualityPlayer quiverPlayer))
            {
                if (quiverPlayer.Active)
                {
                    Color c = quiverPlayer.CurrentBuff == BuffID.CursedInferno ? ColorLib.Wretched2 : ColorLib.IchorCrystal2;
                    spriteBatch.Draw(DTAssetLib.BarrierRing.Value, player.MountedCenter - Main.screenPosition, null, c with { A = 0}, R, DTAssetLib.BarrierRing.Value.Size() / 2f, DTAssetLib.BarrierRing.Value.ScaleRingTextureToMatchRadius(400f, 1300), SpriteEffects.None, 0f);
                }
            }

            spriteBatch.ResetToDefault();
        }

        bool IPlayerPixelatedDrawer.IsActive(Player player)
        {
            return player.GetModPlayer<QuiverOfDualityPlayer>().Active;
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal sealed class QuiverOfDualityDrawLayerLoader : ModSystem
    {
        private static QuiverOfDualityDrawLayer drawer;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            drawer = new QuiverOfDualityDrawLayer();
            PlayerPixelRegistry.Register(drawer);
        }

        public override void Unload()
        {
            if (!Main.dedServ && drawer is not null)
                PlayerPixelRegistry.Unregister(drawer);

            drawer = null;
        }
    }

}
