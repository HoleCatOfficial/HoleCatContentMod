using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee.Quixotism;

using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class Quixotism : ModItem
    {
        public int attackType = 0;
        public int comboExpireTimer = 0;
        public int[] hitCount = new int[2];
        public bool Powered = false;
        public float PowerOpacity = 0f;

        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword[Type] = true;
            DTUtils.TooltipScaleMult[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 72;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 40;
            Item.knockBack = 8f;
            Item.crit = 26;

            Item.value = Item.buyPrice(gold: 16);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<QuixotismSwing>();
            Item.channel = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void UpdateInventory(Player player)
        {

        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Vesper>(16)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
