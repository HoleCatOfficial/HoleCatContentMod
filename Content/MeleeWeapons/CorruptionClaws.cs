using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class CorruptionClaws : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword[Type] = true;
            DTUtils.TooltipScaleMult[Type] = 1.25f;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 3400;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.value = Item.buyPrice(gold: 70);
            Item.rare = ModContent.RarityType<WretchedRarity>();
            Item.shoot = ModContent.ProjectileType<CorruptionClawsProjectile>();
            Item.noUseGraphic = true;
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