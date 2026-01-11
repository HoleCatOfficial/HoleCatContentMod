
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class InfectedManuscript : LateHardmodeScroll
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 32;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.1f;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.TryGetGlobalProjectile<ScrollScepterProj>(out ScrollScepterProj Scptr))
                {
                    Scptr.IchorScroll = true;
                    Scptr.CursedFlameScroll = true;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<IchorScroll>(), 1)
                .AddIngredient(ModContent.ItemType<CursedFlameScroll>(), 1)
                .AddIngredient(ModContent.ItemType<WretchedShards>(), 3)
                .AddIngredient(ModContent.ItemType<PrimalShards>(), 3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}