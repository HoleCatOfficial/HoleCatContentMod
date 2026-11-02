using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Fargos;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Fargos.Melee
{
    public class BrutalAbsolvency : ModItem
    {

        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword[Type] = true;
            DTUtils.TooltipScaleMult[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.width = 140;
            Item.height = 142;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;
            Item.channel = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 600;
            Item.knockBack = 20f;
            Item.crit = 66;

            Item.rare = ModContent.RarityType<HallowedSpecialRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<BrutalAbsolvencySwing>();
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
