using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.MalakhimSet
{
    [AutoloadEquip(EquipType.Body)]
    public class MalakhimPlates : ModItem
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
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 30;
            Item.rare = ModContent.RarityType<PearlRarity>();
            Item.defense = 6;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            // By changing the equipSlot to the leg equip texture slot, the leg texture will now be drawn on the player
            // We're changing the leg slot so we set this to true
            robes = true;
            // Here we can get the equip slot by name since we referenced the item when adding the texture
            // You can also cache the equip slot in a variable when you add it so this way you don't have to call GetEquipSlot
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            /* Here is example code for supporting a female-specifig legs equip texture. See Load as well.
			if (!male) {
				equipSlot = EquipLoader.GetEquipSlot(Mod, Name + "_Female", EquipType.Legs);
			}
			*/
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer<MalakhimHurtSounds>(out MalakhimHurtSounds HurtSounds))
            {
                HurtSounds.Active = true;
            }

            
        }

        

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<Vesper>(45)
                .AddIngredient<WhiteCloth>(10)
				.AddTile(TileID.Anvils)
				.Register();
        }
    }
    
    public class MalakhimHurtSounds : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public readonly SoundStyle HurtSound = new SoundStyle("DestroyerTest/Assets/Audio/Malakhim/Hurt", 3) with { PitchVariance = 0.4f, MaxInstances = 0 };
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