using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
	public class IdriPotion : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;

			ItemID.Sets.DrinkParticleColors[Type] = [
				new Color(111, 58, 95),
				new Color(130, 86, 111),
				new Color(82, 40, 69)
			];
		}

        public int BuffToGrant = ModContent.BuffType<BlackGuard>();
		public override void SetDefaults() {
			Item.UseSound = SoundID.Item3;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useTurn = true;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 20;
			Item.height = 30;
			Item.buffType = BuffToGrant;
			
			Item.value = Item.sellPrice(0, 2, 5);
			Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
		}

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.buffType = BuffToGrant;
                player.AddBuff(ModContent.BuffType<TaintedBrew>(), Item.buffTime);
            }
            else
            {
                SwitchBuff(player);
            }
            return true;
        }


        public override bool ConsumeItem(Player player)
        {
            return player.altFunctionUse != 2;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            SwitchBuff(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                Item.useStyle = ItemUseStyleID.DrinkLiquid;
                Item.buffTime = (60 * 60) * 10;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.HoldUp;
                Item.buffTime = 0;
            }
            return !player.HasBuff<TaintedBrew>();
        }

        public int Cur = 0;
        public string NameToUse = "";
        public void SwitchBuff(Player player)
        {
            Cur++;
            SoundEngine.PlaySound(SoundID.Drown);

            if (Cur < 0)
            {
                Cur = 0;
            }
            
            if (Cur > 2)
            {
                Cur = 0;
            }

            if (Cur == 0)
            {
                NameToUse = "Black Guard";
                BuffToGrant = ModContent.BuffType<BlackGuard>();
            }
            if (Cur == 1)
            {
                NameToUse = "Lightfooted";
                BuffToGrant = ModContent.BuffType<Lightfooted>();
            }
            if (Cur == 2)
            {
                NameToUse = "Bravado";
                BuffToGrant = ModContent.BuffType<Bravado>();
            }

            AdvancedPopupRequest msg = new AdvancedPopupRequest { Color = Color.HotPink, DurationInFrames = 60, Text = NameToUse, Velocity = new Vector2(0, -15)};
            PopupText.NewText(msg, player.Center);

            Item.buffType = BuffToGrant;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.EndurancePotion)
                .AddIngredient(ItemID.BattlePotion)
                .AddIngredient(ItemID.FrogLeg)
                .AddIngredient<Dyrn>(10)
                .AddTile(TileID.Bottles)
                .Register();
		}
	}
}