using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Scepter
{
    public class AetherflameIgnis : ScepterItem
    {
        public override int Width => 54;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            ShootDMG = 42;
            ShootCrit = 12;
            ThrowCrit = 8;
            KB = 8;
            AdditiveValue = Item.sellPrice(silver: 3);
            Rarity = ModContent.RarityType<PearlRarity>();

            ShootID = ModContent.ProjectileType<AetherflameBolt>();
            ThrowID = ModContent.ProjectileType<AetherflameIgnisThrown>();

            ShootSound = SoundID.Item60;
            ThrowSound = SoundID.Item169;

            base.SetDefaults();


        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.shootSpeed = 2f;
        }
    }

    public class AetherFlameIgnisDrop : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.DD2DarkMageT1)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AetherflameIgnis>(), 3));
            }
        }
    }
}