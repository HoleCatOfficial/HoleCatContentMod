using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Scepter
{
    public class GhoulishScepter : ScepterItem
    {
        public override int Width => 62;
        public override int Height => 62;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 70;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 4;
            AdditiveValue = Item.sellPrice(silver: 80);
            Item.rare = ModContent.RarityType<CerisePinkRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<GhoulProjectile>();
            ThrowID = ModContent.ProjectileType<GhoulishScepterThrown>();

            // Optional: change sounds
            ShootSound = new SoundStyle("DestroyerTest/Assets/Audio/GhoulishScepter/Ghost", 6) with
            {
                PitchVariance = 1.0f
            }; ;
            ThrowSound = SoundID.Item169;

            // Refresh defaults after overriding values
            base.SetDefaults();
        }
    }

    public class GhoulScep_DROP_NPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {

            if (npc.type == ModContent.NPCType<PossessedScepter>() && DownedBossSystem.downedPlanteraBoss)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GhoulishScepter>(), 5, 1, 1));
            }
        }
    }
}