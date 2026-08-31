using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class Union : ModItem
    {
        public int AttackCounter = -1;

        public enum Attacks
        {
            SwingDefault,
            FullSwing,
            Throw
        }

        public Attacks CurrentAttack = Attacks.SwingDefault;

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 50;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 10;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 200;
            Item.knockBack = 4f; 
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 16);
            Item.rare = ModContent.RarityType<InfectedRarity>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<UnionSwing>();
            Item.channel = true;


        }

        public override void UpdateInventory(Player player)
        {
            
        }
    
        public override bool CanUseItem(Player player)
        {

            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Malevolence>()
                .AddIngredient<NeglectedRegards>()
                .AddIngredient<Scorn>()
                .AddIngredient<Unrest>()
                .AddIngredient<PhantasmalRemnant>(6)
                .AddCondition(Condition.DownedMoonLord)
                .Register();
        }
    }
}
