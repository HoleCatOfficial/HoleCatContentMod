using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using System.Linq;

namespace DestroyerTest.Content.Magic
{
    public class Heavenbleed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 92;
            Item.value = Item.sellPrice(gold: 25, silver: 70);
            Item.rare = ModContent.RarityType<VesperRarity>();

            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 10;
            Item.autoReuse = false;
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.crit = 16;
            Item.noMelee = false;
            Item.noUseGraphic = false;
            Item.UseSound = DTAssetLib.Impacts.KCrystalConsume;

            Item.shoot = ModContent.ProjectileType<HeavenbleedProjectile>();
        }



        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 GetPosition()
            {
                Vector2 value = Vector2.Zero;

                for (int i = 0; i < 100; i++)
                {
                    Point mouse = Main.MouseWorld.ToTileCoordinates();

                    Tile Check = Framing.GetTileSafely(mouse + new Point(0, i));

                    if (Check.HasUnactuatedTile && Main.tileSolid[Check.TileType])
                    {
                        value = (mouse + new Point(0, i)).ToWorldCoordinates() + new Vector2(0, -8f);
                        break;
                    }
                }

                return value;
            }

            if (Main.projectile.Where(n => n.type == Item.shoot && n.owner == player.whoAmI).Count() > 0)
            {
                foreach (Projectile p in Main.projectile.Where(n => n.type == Item.shoot && n.owner == player.whoAmI))
                {
                    p.Kill();
                }
            }

            Projectile.NewProjectile(source, GetPosition(), Vector2.Zero, type, damage, knockback);
            return false;
        }

        public override bool CanUseItem(Player player)
        {

            return true;
        }
    }
}