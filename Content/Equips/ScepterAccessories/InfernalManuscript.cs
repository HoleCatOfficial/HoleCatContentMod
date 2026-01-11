
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
    public class InfernalManuscript : LateHardmodeScroll
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
                if (player.TryGetModPlayer<ScrollScepterUsePlayer>(out ScrollScepterUsePlayer Scptr))
                {
                    Scptr.HellfireScroll1 = true;
                    Scptr.IncendiaryScroll = true;
                }
                if (proj.TryGetGlobalProjectile<ScrollScepterProj>(out ScrollScepterProj Scptr2))
                {
                    Scptr2.DiabolicScroll = true;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<MalevolenceMantra>(), 1)
                .AddIngredient(ModContent.ItemType<IncendiaryScroll>(), 1)
                .AddIngredient(ModContent.ItemType<DiabolicScroll>(), 1)
                .AddIngredient(ItemID.SoulofNight, 30)
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}