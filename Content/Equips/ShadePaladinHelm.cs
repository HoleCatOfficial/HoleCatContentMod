using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Buffs;
using Terraria.Audio;

namespace DestroyerTest.Content.Equips
{
	[AutoloadEquip(EquipType.Head)]
	public class ShadePaladinHelm : ModItem
	{

		public override void SetStaticDefaults() {
			ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1); 
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.defense = 28;
		}
		
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<ShadePaladinBodyArmor>() && legs.type == ModContent.ItemType<ShadePaladinLegArmor>();
		}

		public override void UpdateArmorSet(Player player) {
			if (player.TryGetModPlayer<ShadePaladinHurtSounds>(out ShadePaladinHurtSounds HurtSounds))
            {
                HurtSounds.Active = true;
            }
        }
	}

    public class ShadePaladinHurtSounds : ModPlayer
        {
            public bool Active = false;
            public override void ResetEffects()
            {
                Active = false;
            }

            public readonly SoundStyle HurtSound = new SoundStyle("DestroyerTest/Assets/Audio/TenebrousConstruct/Hit", 5) with { PitchVariance = 0.4f, MaxInstances = 0 };
            public override void OnHurt(Player.HurtInfo info)
            {
                if (Active)
                {
                    SoundEngine.PlaySound(HurtSound, Player.Center);
                }
            }
            public override void ModifyHurt(ref Player.HurtModifiers modifiers)
            {
                if (Active)
                {
                    modifiers.DisableSound();
                }
            }
        }
}