
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

		public override void SetDefaults() {
			Item.width = 28;
			Item.height = 22;
			Item.value = Item.sellPrice(gold: 70);
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 13;
            Item.vanity = true;
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
            if (DemonSet)
            {
                Sentry = true;
                return true;
            }
            if (PaladinSet)
            {
                Scepter = true;
                return true;
            }
			return false;
		}
		public override void UpdateArmorSet(Player player) {
			if (Scepter)
            {
                PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], player.headPosition, new Vector2(0, -5), ColorLib.TenebrisGradient, 0.75f);
                if (player.TryGetModPlayer<DarkVesselScepter>(out var scepter))
                {
                    scepter.Active = true;
                }
                if (player.TryGetModPlayer<ShadePaladinHurtSounds>(out ShadePaladinHurtSounds HurtSounds))
                {
                    HurtSounds.Active = true;
                }
            }
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

    public class DarkVesselScepter : ModPlayer
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
                Player.moveSpeed *= 0.5f;
                Player.endurance += 0.2f;
                Player.GetArmorPenetration<ScepterClass>() += 15;
                Player.GetDamage<ScepterClass>() *= 1.15f;

                if (Player.statLife < Player.statLifeMax / 2)
                {
                    float RadiusSpeedModifier = 0.4f; //Typical sine speed. Goes back and forth in about 2 seconds.
                    float Radius = 150f + 50f * (float)Math.Sin(Player.miscCounter * RadiusSpeedModifier * 0.1f); //Sines between 100 and 200 back and forth. Very, very slowly. Perhaps using a float to control speed of sine.
                    int dustType = DustID.FireworksRGB;

                    if (Player.miscCounter % 60 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy);
                        NPC.HitInfo strike = new NPC.HitInfo { Crit = false, Damage = 16, DamageType = null, HideCombatText = false, HitDirection = 0, InstantKill = false, Knockback = 0};
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
    }

    public class DarkVesselSummoner : ModPlayer
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
                Player.maxMinions += 2;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.TryGetGlobalProjectile<DarkVesselSummoner_Projectile>(out var Summon))
                    {
                        Summon.Active = true;
                    }
                }
            }   
        }
    }

    public class DarkVesselSummoner_Projectile : GlobalProjectile
    {
        public bool Active = false;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.DamageType == DamageClass.Summon || entity.DamageType == DamageClass.Generic;
        }

        public float Length = 10f;
        public override void PostAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            if (Active)
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC targ = Main.npc[player.MinionAttackTargetNPC];
                    if (projectile.Center.Distance(targ.Center) > 100)
                    {
                        if (Main.GameUpdateCount % 60 == 0)
                        {
                            projectile.velocity += Vector2.Lerp(projectile.Center, targ.Center, 0.01f);
                        }
                    }
                    else
                    {
                        if (projectile.velocity.Length() > Length)
                        {
                            projectile.velocity *= 0.99f;
                        }
                    }
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
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<TenebrisStarFriendly>(), 6, Player.Center, (int)Player.GetDamage(DamageClass.Summon).Flat, 2, 8);
        }
    }
}
