
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.Localization;
using System;
using Microsoft.Xna.Framework.Graphics;
using BreadLibrary.Core.Graphics.Pixelation;
using OpusLib;

namespace DestroyerTest.Content.Equips.Cards.RiftenDeck
{
	public class Hollow : ModItem
	{
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 11));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 24;
			Item.maxStack = 1;
			Item.value = 100;
			Item.accessory = true;
            Item.rare = ModContent.RarityType<RiftRarity1>();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            if (player.TryGetModPlayer<HollowShield>(out var shield))
            {
                shield.Active = true;
            }
            player.statDefense += 10;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var shieldText = Language.GetText("Mods.DestroyerTest.ShieldPlayer.ShieldLine");
			tooltips.Add(new TooltipLine(Mod, "ShieldInfo", shieldText.Value));
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<ShineShadeDeck>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Living_Shadow>(45)
                .AddIngredient<Item_RiftClay>(25)
            .Register();
        }
    }

    public class HollowShield : ShieldPlayer
    {
        public override int MaxDurability => 90;
        private int _durability = 90;
		public override int Durability
		{
			get => _durability;
			set => _durability = Math.Clamp(value, 0, MaxDurability);
		}
        public override int Radius => 40;
        public override Color themeColor => ColorLib.Rift;
        public override SoundStyle Hit => DTAssetLib.Impacts.Deflect;
        public override SoundStyle Break => DTAssetLib.Impacts.IceImpact;
        public override NetworkText[] DeathMSGs => new NetworkText[]
        {
            NetworkText.FromLiteral($"{Player.name} felt a little hollow inside."),
            NetworkText.FromLiteral($"{Player.name} gave a little too much in return for too little."),
            NetworkText.FromLiteral($"{Player.name} fell victim to the eclipse."),
            NetworkText.FromLiteral($"{Player.name} didnt have it in them to sustain their shield.")
        };
        public override int Priority => 2;
    }

    public class HollowShieldDrawLayer : PlayerDrawLayer, IDrawPixelated
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.TryGetModPlayer<HollowShield>(out HollowShield Shield))
            {
                return Shield.Active && Shield.Absorb;
            }
            return false;
        }

        PixelLayer IDrawPixelated.PixelLayer => PixelLayer.AbovePlayer;

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.CaptureTheGem);

        private Vector2 cachedCenter;
        private bool hasCachedData;

        PlayerDrawSet D;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            D = drawInfo;
        }

        void IDrawPixelated.DrawPixelated(SpriteBatch spriteBatch)
        {

            var Shield = D.drawPlayer.GetModPlayer<HollowShield>();
            Color color = Shield.themeColor;

            var position = D.drawPlayer.Center - Main.screenPosition;

            Opus.StartSpriteBatchPixelated(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(
                DTAssetLib.BloomRingSharp.Value,
                position,
                null,
                color with { A = 0 },
                0f,
                DTAssetLib.BloomRingSharp.Value.Size() / 2,
                Shield.Radius / (DTAssetLib.BloomRingSharp.Value.Width / 2f),
                SpriteEffects.None,
                0f
            );

            Opus.ReturnToDefaultDrawing(spriteBatch);

            //hasCachedData = false; // optional: avoids stale draws
        }
    }
}