
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class DeadlyBlossom : ModItem
    {
        public static bool CanSpawnBlossom = false;

        public override void SetDefaults()
        {
            Item.width = 26; // Width of the item
            Item.height = 26; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>(); // The rarity of the item
            Item.vanity = false;
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer<DBPlayer>(out DBPlayer Blossom))
            {
                Blossom.Active = true;
            }
        }


    }
    
    public class DBPlayer : ModPlayer
    {
        public bool Active = false;
        public int Cooldown = 0;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Cooldown > 0)
            {
                Cooldown--;
            }
            if (Cooldown == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 0.5f }, Player.Center);
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
		{
            if (DestroyerTestMod.DeadlyBlossomKeybind.JustPressed && Player.ownedProjectileCounts[ModContent.ProjectileType<MiniRose>()] < 1 && Active && Cooldown <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/MiniRoseSummon") with { PitchVariance = 0.5f }, Player.Center);
                Projectile.NewProjectile(Player.GetSource_Accessory(Player.armor.FirstOrDefault(item => item.type == ModContent.ItemType<DeadlyBlossom>() || item.type == ModContent.ItemType<WyvernSkullRose>())), Player.Center, Vector2.Zero, ModContent.ProjectileType<MiniRose>(), 0, 0, Player.whoAmI);	
                Cooldown = 1800;
            }
		}
    }
}
