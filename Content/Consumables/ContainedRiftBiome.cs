using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Consumables
{
    public class ContainedRiftBiome : ModItem
    {
        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<ContainedRiftBiomeProjectile>();
            Item.shootSpeed = 10f;
            Item.width = 34;
            Item.height = 38;
            Item.UseSound = DTAssetLib.Impacts.MagicBeep;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.SetShopValues(ItemRarityColor.LightRed4, 02500);
        }
	}
}