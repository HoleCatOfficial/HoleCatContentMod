
﻿using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using Steamworks;
using DestroyerTest.Common;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Content.Projectiles.ShadeThrasherFriendly;
using System;
using Terraria.Audio;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class TenebrousArchmageHat : ModItem
	{

		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 20;
			Item.value = Item.sellPrice(gold: 70);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 29;
		}

        public override void UpdateEquip(Player player)
        {
			player.ScepterClass().ThrowSpeedModifier += 0.1f;
        }

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<TenebrousArchmageCoat>() && legs.type == ModContent.ItemType<TenebrousArchmagePants>();
		}

		public override void UpdateArmorSet(Player player)
		{
			player.DefaultSetBonusText(player.armor[0]);
			if (player.TryGetModPlayer<TenebrisScepterPlayer>(out TenebrisScepterPlayer scptr))
			{
				scptr.Active = true;
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Tenebris>(8)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
	
	public class TenebrisScepterPlayer : ModPlayer
    {
		public bool Active = false;
		public override void ResetEffects()
		{
			Active = false;
		}

		const float MinDist = 900f;
		const float MinDistSq = MinDist * MinDist;

		public static bool IsValidClusterPos(Vector2 pos)
		{
			int type = ModContent.ProjectileType<ShimmeringShardCluster>();

			foreach (Projectile p in Main.projectile)
			{
				if (!p.active || p.type != type)
					continue;

				if (Vector2.DistanceSquared(p.Center, pos) < 900f * 900f)
					return false;
			}

			return true;
		}

		public float Rot = 0;
		public bool Flag1 = false;
		public int Cooldown = 0;
		public override void PostUpdateEquips()
		{
			Rot += 0.05f * Player.direction;
			if (Active)
			{
				Player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.15f;
				
				Player.moveSpeed *= 1.3f;
                Player.endurance += 0.06f;
                Player.GetArmorPenetration<ScepterClass>() += 15;
                Player.GetDamage<ScepterClass>() *= 1.15f;

				if (Player.ArmorSetBonusKey() && Cooldown <= 0)
				{
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/TenebrisImpact") with { PitchVariance = 0.5f }, Player.Center);
					for (int t = 0; t < 3; t++)
					{
						const int MaxAttempts = 20;
						Vector2 pos = Vector2.Zero;
						bool found = false;

						for (int i = 0; i < MaxAttempts; i++)
						{
							pos = Player.Center + Main.rand.NextVector2Circular(1200f, 1200f);

							if (IsValidClusterPos(pos))
							{
								found = true;
								break;
							}
						}

						if (found)
						{
							Projectile.NewProjectile(
								Player.GetSource_Misc("TenebrisScepterBonus"),
								pos,
								Vector2.Zero,
								ModContent.ProjectileType<ShimmeringShardCluster>(),
								0,
								0,
								Player.whoAmI
							);
						}
					}

					Opus.RingSpreadDust(DustID.TintableDustLighted, 16, Player.Center, 10, 0, ColorLib.TenebrisGradient, 1, 2, offset: Main.rand.NextFloat(MathHelper.TwoPi));
					Cooldown = 1800;
				}

				if (Cooldown > 0)
				{
					Cooldown--;
				}
				if (Cooldown == 1)
				{
					SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 0.5f }, Player.Center);
				}

                if (Player.statLife < Player.statLifeMax / 2)
                {
                    float RadiusSpeedModifier = 0.4f; //Typical sine speed. Goes back and forth in about 2 seconds.
                    float Radius = 150f + 50f * (float)Math.Sin(Player.miscCounter * RadiusSpeedModifier * 0.1f); //Sines between 100 and 200 back and forth. Very, very slowly. Perhaps using a float to control speed of sine.
                    int dustType = DustID.FireworksRGB;

                    if (Player.miscCounter % 60 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy);
                        NPC.HitInfo strike = new NPC.HitInfo { Crit = false, Damage = 16, DamageType = null, HideCombatText = false, HitDirection = 0, InstantKill = false, Knockback = 0};
                        Opus.RingSpreadDust(dustType, 30, Player.Center, Radius, 0, ColorLib.TenebrisGradient, 2f, 8, offset: Main.rand.NextFloat(MathHelper.TwoPi));

                        foreach (NPC enemy in Main.npc)
                        {
                            if (!enemy.friendly && enemy.Center.Distance(Player.Center) < Radius)
                            {
                                enemy.StrikeNPC(strike, false, true);    
                                enemy.AddBuff(ModContent.BuffType<ShimmeringFlames>(), 600);
                            }
                        }
                    }
                }  
			}
		}
    }

	public class TenebrousArchmageDrawLayer : PlayerDrawLayer
    {

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			Player player = drawInfo.drawPlayer;
			bool scepterActive = player.TryGetModPlayer<TenebrisScepterPlayer>(out var Scepter) && Scepter.Active;
			bool magicActive = player.TryGetModPlayer<TenebrisMagicPlayer>(out var Magic) && Magic.Active;
			return scepterActive || magicActive;
		}
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.FrozenOrWebbedDebuff);

        protected override void Draw(ref PlayerDrawSet drawInfo) 
        {
            Player player = drawInfo.drawPlayer;
			bool scepterActive = player.TryGetModPlayer<TenebrisScepterPlayer>(out var Scepter) && Scepter.Active;
			bool magicActive = player.TryGetModPlayer<TenebrisMagicPlayer>(out var Magic) && Magic.Active;
			if (scepterActive && drawInfo.shadow == 0)
            {
                DrawRuneRing(ref drawInfo, 1f, 0.25f, Scepter.Rot);
            }
			if (magicActive && drawInfo.shadow == 0)
            {
                DrawRuneRing(ref drawInfo, 1f, 0.25f, Magic.Rot);
            }
		}

        private void DrawRuneRing(ref PlayerDrawSet drawInfo, float Opacity = 1f, float Scale = 1f, float Rotation = 0f)
        {
            var Tex = DTAssetLib.RuneCircle.Value;

			var position = drawInfo.Center - Main.screenPosition;
			position = new Vector2((int)position.X, (int)position.Y);

            drawInfo.DrawDataCache.Add(new DrawData(
				Tex,
				position,
				null,
				Color.White with {A = 0} * Opacity,
				Rotation,
				Tex.Size() * 0.5f,
				Scale,
				SpriteEffects.None,
				0
			));
        }

    }
}
