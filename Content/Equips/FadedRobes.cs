using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.Body)]
    public class FadedRobes : ModItem
    {
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 30;
            Item.rare = ModContent.RarityType<ScepterArmorPHMRarity>();
            Item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FadedRobeLifeRegen>().Active = true;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }
	}

    public class FadedRobeLifeRegen : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void UpdateLifeRegen()
        {
            if (Active)
            {
                Player.lifeRegen += 10;
            }
        }
    }
}