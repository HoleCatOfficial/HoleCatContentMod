using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class Icemourne : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 76;
            Item.height = 76;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Pink;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item66;
            Item.knockBack = 5;
            Item.autoReuse = true;
            Item.damage = 15;
            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.crit = 16;
            Item.shoot = ModContent.ProjectileType<IcemourneWave>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); // Sync the changes in multiplayer.

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.AddBuff(BuffID.Chilled, 60);
        }



        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 6)
                .AddIngredient(ItemID.IceBlock, 25)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}