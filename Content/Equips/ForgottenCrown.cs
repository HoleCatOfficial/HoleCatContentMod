
﻿using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using OpusLib;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class ForgottenCrown : ModItem
	{
        public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}
		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 22;
			Item.value = Item.sellPrice(gold: 70);
			Item.rare = ModContent.RarityType<PearlRarity>();
			Item.defense = 3;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<ForgottenPlatemail>() && legs.type == ModContent.ItemType<ForgottenGreaves>();
		}

		public static readonly int SoloRangeBonus = 10;
		
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SoloRangeBonus);
		public override void UpdateArmorSet(Player player) {
			if (player.TryGetModPlayer<ForgottenCrownPlayer>(out var Crown))
            {
                Crown.Active = true;
            }
			player.setBonus = Language.GetTextValue("Mods.DestroyerTest.Items.ForgottenCrown.SetBonus");
		}

        public override void UpdateEquip(Player player)
        {
            ScepterClassStats.Range += SoloRangeBonus;
        }
	}

    public class ForgottenCrownPlayer : ModPlayer
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
                
            }
        }

        public override void OnExtraJumpStarted(ExtraJump jump, ref bool playSound)
        {
            if (Active)
            {
                playSound = true;
                jump.ShowVisuals(Player);
            }
        }

        public override bool CanStartExtraJump(ExtraJump jump)
        {
            return Active;
        }
    }

    public class ForgottenCrownOwnedProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int cooldown = 120;

        public override void PostAI(Projectile projectile)
        {
            if (cooldown > 0)
            {
                cooldown--;
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            if (player.TryGetModPlayer<ForgottenCrownPlayer>(out var Crown) && projectile.owner == player.whoAmI && projectile.DamageType == ModContent.GetInstance<ScepterClass>() && projectile.type != ModContent.ProjectileType<ExplodingIcicle>())
            {
                if (Crown.Active && cooldown <= 0)
                {
                    Opus.RadialSpreadDust(DustID.Ice, 10, target.Center, 0, Color.White, 1f, 2, true);
                    Vector2 speed = new Vector2(0, -3.5f).RotatedByRandom(1f);
                    for(int o = 0; o < Main.rand.Next(3, 6); o++)
                    {
                        Projectile.NewProjectile(player.GetSource_Misc("Crown Icicles"), target.Center, speed, ModContent.ProjectileType<ExplodingIcicle>(), projectile.damage / 2, 4, projectile.owner);
                    }
                    cooldown = 120;
                }
            }
        }
    }
}
