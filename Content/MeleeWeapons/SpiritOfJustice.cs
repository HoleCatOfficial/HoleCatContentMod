using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using GlowmaskHelper.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    [AutoloadGlowmask]
    public class SpiritOfJustice : ModItem
    {
        public bool CanParry = true;
        public int ParryCooldown = 0;
        public const int MaxParryCooldown = 300;

        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword[Type] = true;
            DTUtils.TooltipScaleMult[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 92;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 80;
            Item.knockBack = 8f;
            Item.crit = 26;

            Item.value = Item.buyPrice(gold: 16);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<SpiritOfJusticeSwing>();
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void UpdateInventory(Player player)
        {
            if (ParryCooldown > 0)
            {
                CanParry = false;
                ParryCooldown--;

                if (ParryCooldown == 1)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath7);
                }
            }
            else
            {
                CanParry = true;
            }
        }
        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.HallowedBar, 16)
            .AddIngredient<Vesper>(16)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
