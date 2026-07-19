using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity.Scepter;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
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
        Shield PetrifiedShield = new Shield("PetrifiedShield", 400, 160, ColorLib.JavelinEnergy, DTAssetLib.ScholarShieldSounds.Activate, DTAssetLib.Impacts.Deflect, DTAssetLib.Impacts.IceImpact,
            new List<NetworkText>()
            {
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} was sucked dry."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} gave a little too much in return for too little."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} was consumed by fire and frost."),
                NetworkText.FromLiteral($"{Main.LocalPlayer.name} didnt have it in them to sustain their shield.")
            },
            25, 8
        );
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
			ShieldManager.ActivateShield(PetrifiedShield, player);

            if (player.TryGetModPlayer<PetrifiedScepterPlayer>(out PetrifiedScepterPlayer Scepter))
			{
				Scepter.Active = true;
			}
			player.ScepterClass().ThrowSpeedModifier = 2.5f;
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

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			Item body = new Item();
			body.SetDefaults(ModContent.ItemType<PetrifiedChestplate>());
			Item legs = new Item();
			legs.SetDefaults(ModContent.ItemType<PetrifiedGreaves>());
			if (IsArmorSet(Item, body, legs))
			{
				//TODO: rename this
				var pityText = Language.GetText("Mods.DestroyerTest.ShieldPlayer.ShieldLine");
				tooltips.Add(new TooltipLine(Mod, "ShieldInfo", pityText.Value));
			}
		}
	}

	public class PetrifiedScepterPlayer : ModPlayer
    {
		public bool Active = false;
		public int Cooldown = 0;
		public override void ResetEffects()
		{
			Active = false;
		}

        public override void PostUpdateEquips()
        {
            if (Active)
			{
				if (Cooldown > 0)
                {
                    Cooldown--;
                }

                if (Cooldown == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 0.5f }, Player.Center);
                }

                if (DestroyerTestMod.ArmorSetBonusHotKey.JustPressed && Cooldown <= 0 && !Player.mount.Active)
                {
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/ManaBurst") with { PitchVariance = 0.5f }, Player.Center);
					Vector2 toMouse = Main.MouseWorld - Player.Center;
					Player.velocity = toMouse.ToRotation().ToRotationVector2() * 30;

					Opus.RadialSpreadProjectile(ModContent.ProjectileType<FlameBurst>(), 3, Player.Center, 30, 4, 8, offset: Main.rand.NextFloat(MathHelper.TwoPi));
					Opus.RadialSpreadProjectile(ModContent.ProjectileType<FrostBurst>(), 3, Player.Center, 30, 4, 8, offset: Main.rand.NextFloat(MathHelper.TwoPi));
					Cooldown = 60 * 30;
				}
			}
        }
    }
}