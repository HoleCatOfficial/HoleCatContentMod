using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{

    public class Rimeheart : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {

            Item.UseSound = SoundID.Item80;
            Item.width = 84;
            Item.height = 84;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 100;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Item.GetSource_OnHit(target), player.MountedCenter, target.Center.DirectionTo(player.Center).RotatedByRandom(0.6f) * 15f, ModContent.ProjectileType<RimeheartSnowflake>(), Item.damage / 2, 8, player.whoAmI);
            }
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FrostCore, 1)
                .AddIngredient(ItemID.TitaniumBar, 2)
                .AddIngredient<Icemourne>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}