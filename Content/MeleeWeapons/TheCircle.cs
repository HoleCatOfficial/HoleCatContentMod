using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class TheCircle : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1; 

            Item.damage = 17; 
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.knockBack = 2.5f;
            Item.crit = 8;
            Item.channel = true;
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.value = Item.buyPrice(gold: 1);

            Item.shoot = ModContent.ProjectileType<TheCircleProjectile>();
            Item.shootSpeed = 16f;
        }
    }
}
