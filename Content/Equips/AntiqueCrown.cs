using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using Terraria.Audio;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class AntiqueCrown : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 12;
			Item.value = Item.sellPrice(gold: 8); 
			Item.rare = ModContent.RarityType<PearlRarity>();
			Item.defense = 2;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<FleeceRobe>();
		}
		public override void UpdateArmorSet(Player player)
		{
			if (player.TryGetModPlayer<AntiqueSetPlayer>(out AntiqueSetPlayer Scptr))
			{
				Scptr.Active = true;
			}
			player.setBonus = Language.GetTextValue("Mods.DestroyerTest.Items.AntiqueCrown.SetBonus");
		}

		public int RangeBonus = 10;
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RangeBonus);
        public override void UpdateEquip(Player player)
        {
            ScepterClassStats.Range += RangeBonus;
        }
	}

	public class AntiqueSetPlayer : ModPlayer
	{
		public bool Active;
		public int Timer1 = 0;
		public bool CanTriggerSpecialFX = false;
		public bool Flag1 = false;
		public float Opacity = 0f;
		public float Timer1As0to1;
		public override void ResetEffects()
		{
			Active = false;
		}

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
			if (Active)
			{
            	DTUtils.DrawChargeBar(1.25f, (Player.Center + new Vector2(0, -40)) - Main.screenPosition, Timer1As0to1, Color.Brown * Opacity);
        	}
		}

        public override void PostUpdateEquips()
        {
			Timer1As0to1 = MathHelper.Clamp(Timer1 / 300f, 0f, 1f);
            if (Active)
			{
				Timer1++;
				if (Timer1 >= 300)
				{
					CanTriggerSpecialFX = true;
					if (!Flag1)
					{
						SoundEngine.PlaySound(SoundID.MaxMana, Player.position);
						Flag1 = true;
					}
				}

				if (Timer1 <= 0)
				{
					if (Opacity > 0f)
					{
						Opacity -= 0.02f;
					}
					CanTriggerSpecialFX = false;
				}

				if (Timer1 > 0 && Opacity < 1f)
				{
					Opacity += 0.02f;
				}
			}
			if (!Active)
			{
				Timer1 = 0;
				CanTriggerSpecialFX = false;
				Flag1 = false;
			}
        }

        public override bool CanUseItem(Item item)
        {
			if (Active && !CanTriggerSpecialFX && Player.altFunctionUse == 2)
			{
				Timer1 = 0;
			}
            return true;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (Active && CanTriggerSpecialFX && Player.altFunctionUse == 2)
			{
				for(int t = 0; t < 4; t++)
				{
					Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.5f), ModContent.ProjectileType<AncientRock>(), damage / 3, knockback * 2, Player.whoAmI);
				}
				Flag1 = false;
				Timer1 = 0;
			}
            return true;
        }
	}
}