using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class TestSword : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 90;
            Item.height = 90;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 300;
            Item.knockBack = 4f;
            Item.crit = 16;

            Item.value = Item.buyPrice(gold: 3);
            Item.rare = ItemRarityID.Gray;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<TestSwordProjectile>();
            Item.channel = true;


        }


        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
