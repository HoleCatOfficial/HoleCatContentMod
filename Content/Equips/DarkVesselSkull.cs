
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
using InnoVault.PRT;
using System;
using OpusLib;
using Terraria.Audio;
using DestroyerTest.Content.Projectiles;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class DarkVesselSkull : ModItem
	{

        public override void SetStaticDefaults() {
			ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; 
		}

		public override void SetDefaults() 
        {
			Item.width = 28;
			Item.height = 22;
			Item.value = Item.sellPrice(gold: 70);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 13;
		}

        public bool Scepter = false;
        public bool Summoner = false;
        public bool Sentry = false;
		public override bool IsArmorSet(Item head, Item body, Item legs) 
        {
            bool ArchmageSet = (body.type == ModContent.ItemType<TenebrousArchmageCoat>() && legs.type == ModContent.ItemType<TenebrousArchmagePants>());
            bool DemonSet = (body.type == ModContent.ItemType<TenebrousDemonChestplate>() && legs.type == ModContent.ItemType<TenebrousDemonChausses>());
            bool PaladinSet = (body.type == ModContent.ItemType<ShadePaladinBodyArmor>() && legs.type == ModContent.ItemType<ShadePaladinLegArmor>());
            if (ArchmageSet)
            {
                Summoner = true;
                return true;
            }
            if (PaladinSet)
            {
                Sentry = true;
                return true;
            }
			return false;
		}
		public override void UpdateArmorSet(Player player) 
        {
            if (Summoner)
            {
                if (player.TryGetModPlayer<DarkVesselSummoner>(out var summoner))
                {
                    summoner.Active = true;
                }
            }
            if (Sentry)
            {
                if (player.TryGetModPlayer<DarkVesselSentry>(out var sentry))
                {
                    sentry.Active = true;
                }
            }
		}

		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<Tenebris>(8)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}

    public class DarkVesselSummoner : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public bool Flag1;
        public override void PostUpdateEquips()
        {
            if (Active)
            {
                Player.maxMinions += 2;
                
                Player.AddBuff(ModContent.BuffType<ShadeThrasherBuff>(), 10);

                if (!Flag1)
				{
					//Projectile.NewProjectile(Player.GetSource_None(), Player.Center, Vector2.One, ModContent.ProjectileType<ShadeThrasherFriendlyHead>(), 120, 7);
					Flag1 = true;
				}
            }   
        }
    }

    public class DarkVesselSentry : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                Player.maxTurrets += 4;

                if (Player.statLife < Player.statLifeMax / 2)
                {
                    float RadiusSpeedModifier = 0.4f; //Typical sine speed. Goes back and forth in about 2 seconds.
                    float Radius = 150f + 50f * (float)Math.Sin(Player.miscCounter * RadiusSpeedModifier * 0.1f); //Sines between 100 and 200 back and forth. Very, very slowly. Perhaps using a float to control speed of sine.
                    int dustType = DustID.FireworksRGB;

                    if (Player.miscCounter % 60 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy);
                        NPC.HitInfo strike = new NPC.HitInfo { Crit = false, Damage = 30, DamageType = null, HideCombatText = false, HitDirection = 0, InstantKill = false, Knockback = 0};
                        Opus.RingDustOutward(dustType, 30, Player.Center, Radius, 0, ColorLib.TenebrisGradient, 2f, 8, true);

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

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarFriendly>(), 6, Player.Center, (int)Player.GetDamage(DamageClass.Summon).Flat, 2, 8, RandomOffset: true);
            }
        }
    }
}
