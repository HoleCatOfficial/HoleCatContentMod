using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity.Scepter;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.PetrifiedSet
{
	[AutoloadEquip(EquipType.Head)]
	public class PetrifiedChaplet : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 28;
			Item.value = DTUtils.GetScepterArmorSellPricePerRarity(Item.rare);
			Item.rare = ModContent.RarityType<WineRarity>();
			Item.defense = 8;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<PetrifiedChestplate>() && legs.type == ModContent.ItemType<PetrifiedGreaves>();
		}
		public override void UpdateArmorSet(Player player)
		{
			if (player.TryGetModPlayer<PetrifiedShieldPlayer>(out PetrifiedShieldPlayer Shield))
			{
				Shield.Active = true;
			}
			if (player.TryGetModPlayer<PetrifiedScepterPlayer>(out PetrifiedScepterPlayer Scepter))
			{
				Scepter.Active = true;
			}
			ScepterClassStats.ThrowSpeedModifier = 2.5f;
			player.buffImmune[BuffID.OnFire] = true;
			player.buffImmune[BuffID.Burning] = true;
			player.buffImmune[BuffID.OnFire3] = true;
			player.buffImmune[BuffID.Frostburn] = true;
			player.buffImmune[BuffID.Frostburn2] = true;
			player.setBonus = Language.GetText("Mods.DestroyerTest.Items.PetrifiedChaplet.SetBonus").Value;
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}
	}

	public class PetrifiedScepterPlayer : ModPlayer
    {
		public bool Active = false;
		public override void ResetEffects()
		{
			Active = false;
		}

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (Active && Player.altFunctionUse != 2 && (Player.HeldItem.DamageType == ModContent.GetInstance<ScepterClass>() || Main.projectile[Player.heldProj].DamageType == ModContent.GetInstance<ScepterClass>()))
			{
				Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.5), ModContent.ProjectileType<FlameBurst>(), damage / 3, 4, Player.whoAmI);
				Projectile.NewProjectile(source, position, velocity.RotatedBy(0.5), ModContent.ProjectileType<FrostBurst>(), damage / 3, 4, Player.whoAmI);
				return true;
            }
            return true;
        }
    }
	
	public class PetrifiedShieldPlayer : ShieldPlayer
    {
        public override int MaxDurability => 400;
        private int _durability = 400;
		public override int Durability
		{
			get => _durability;
			set => _durability = Math.Clamp(value, 0, MaxDurability);
		}
        public override int Radius => 160;
        public override Color themeColor => ColorLib.JavelinEnergy;
        public override NetworkText[] DeathMSGs => new NetworkText[]
        {
            NetworkText.FromLiteral($"{Player.name} was sucked dry."),
            NetworkText.FromLiteral($"{Player.name} gave a little too much in return for too little."),
            NetworkText.FromLiteral($"{Player.name} got folded like a chair."),
            NetworkText.FromLiteral($"{Player.name} didnt have it in them to sustain their shield.")
        };
        public override int RechargeHealthTax => 5;
    }
}