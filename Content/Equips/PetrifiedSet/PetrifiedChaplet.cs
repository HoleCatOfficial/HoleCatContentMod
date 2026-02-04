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

					Opus.RadialSpreadProjectile(ModContent.ProjectileType<FlameBurst>(), 3, Player.Center, 30, 4, 8, RandomOffset: true);
					Opus.RadialSpreadProjectile(ModContent.ProjectileType<FrostBurst>(), 3, Player.Center, 30, 4, 8, RandomOffset: true);
					Cooldown = 60 * 30;
				}
			}
        }


		/*
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (Active && Player.altFunctionUse != 2 && (Player.HeldItem.DamageType == ModContent.GetInstance<ScepterClass>() || Main.projectile[Player.heldProj].DamageType == ModContent.GetInstance<ScepterClass>()))
			{
				
				float maxSpeed = 10f;

				Vector2 Dir1 = velocity.RotatedBy(-0.5f);
				Vector2 Dir2 = velocity.RotatedBy(0.5f);

				float speed1 = Math.Min(Dir1.Length(), maxSpeed);
				float speed2 = Math.Min(Dir2.Length(), maxSpeed);

				Dir1 = Dir1.SafeNormalize(Vector2.Zero) * speed1;
				Dir2 = Dir2.SafeNormalize(Vector2.Zero) * speed2;

				Projectile.NewProjectile(source, position, Dir1, ModContent.ProjectileType<FlameBurst>(), damage / 3, 4, Player.whoAmI);
				Projectile.NewProjectile(source, position, Dir2, ModContent.ProjectileType<FrostBurst>(), damage / 3, 4, Player.whoAmI);
				return true;
            }
            return true;
        }
		*/
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
            NetworkText.FromLiteral($"{Player.name} was consumed by fire and frost."),
            NetworkText.FromLiteral($"{Player.name} didnt have it in them to sustain their shield.")
        };
        public override int RechargeHealthTax => 25;

		public override int Priority => 8;
    }

	public class PetrifiedShieldDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.TryGetModPlayer<PetrifiedShieldPlayer>(out PetrifiedShieldPlayer Shield))
            {
                return Shield.Active && Shield.Absorb;
            }
            return false;
        }

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.CaptureTheGem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var Shield = ModContent.GetInstance<PetrifiedShieldPlayer>();
            
            Color color = Shield.themeColor;
            var position = drawInfo.Center - Main.screenPosition;
			position = new Vector2((int)position.X, (int)position.Y);

            drawInfo.DrawDataCache.Add(new DrawData(
                DTAssetLib.ShieldRing.Value,
                position,
                null,
                color with {A = 0},
                0f,
                DTAssetLib.ShieldRing.Size() / 2,
                Shield.Radius / (DTAssetLib.ShieldRing.Value.Width / 2f),
                SpriteEffects.None,
                0
            ));
        }
    }
}