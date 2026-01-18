using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.RiftBiome.RiftSurfaceResources;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using Terraria.DataStructures;
using Terraria.Audio;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class HeliciteHeadpiece : ModItem
	{
		public override void SetStaticDefaults() {
			ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
		}

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ModContent.RarityType<RiftRarity2>(); // The rarity of the item
			Item.defense = 17; // The amount of defense the item will give when equipped
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<HeliciteRobe>() && legs.type == ModContent.ItemType<HeliciteChausses>();
		}

		public override void UpdateArmorSet(Player player) 
		{
            player.DefaultSetBonusText(player.armor[0]);
            if (player.TryGetModPlayer<HeliciteScepterPlayer>(out var Scepter))
            {
                Scepter.Active = true;
            }
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<Living_Shadow>(8)
				.AddIngredient<Item_HeliciteCrystal>(20)
				.AddIngredient<Item_RiftClay>(6)
				.AddTile<Tile_RiftConfiguratorArmory>()
				.Register();
		}
	}

    public class HeliciteScepterPlayer : ModPlayer
    {
        public bool Active = false;
        public int Cooldown = 0;
        public int ImmunityTime = 0;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (Active)
            {
                if (ImmunityTime > 0)
                {
                    drawInfo.stealth = 1f;
                }
            }
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (ImmunityTime > 0)
                {
                    Player.immune = true;
                    ImmunityTime--;
                    Cooldown = 600;
                }

                if (Cooldown > 0 && ImmunityTime <= 0)
                {
                    Cooldown--;
                }

                if (Cooldown == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 0.5f }, Player.Center);
                }
                if (DestroyerTestMod.ArmorSetBonusHotKey.JustPressed && Cooldown <= 0)
                {
                    ImmunityTime = 180;
                    Cooldown = 1200;
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HoleCatHookFreeze") with { PitchVariance = 0.5f }, Player.Center);
                    for(int o = 0; o < 7; o++)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[0]), Player.Center, Main.rand.NextVector2Circular(2, 2), ModContent.ProjectileType<SolarTrail>(), 22, 10, Player.whoAmI);
                    }
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                if (Cooldown <= 0)
                {
                    hurtInfo.Damage *= 0;
                    ImmunityTime = 180;
                    Cooldown = 1200;
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HoleCatHookFreeze") with { PitchVariance = 0.5f }, Player.Center);
                    for(int o = 0; o < 7; o++)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[0]), Player.Center, Main.rand.NextVector2Circular(2, 2), ModContent.ProjectileType<SolarTrail>(), 22, 10, Player.whoAmI);
                    }
                }
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                if (Cooldown <= 0)
                {
                    hurtInfo.Damage *= 0;
                    ImmunityTime = 180;
                    Cooldown = 1200;
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/HoleCatHookFreeze") with { PitchVariance = 0.5f }, Player.Center);
                    for(int o = 0; o < 7; o++)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor[0]), Player.Center, Main.rand.NextVector2Circular(2, 2), ModContent.ProjectileType<SolarTrail>(), 22, 10, Player.whoAmI);
                    }
                }
            }
        }
    }
}