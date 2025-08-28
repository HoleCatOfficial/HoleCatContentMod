
using DestroyerTest.Common;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class DetritizedPendant : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<CorruptPendantScepterUsePlayer>(out CorruptPendantScepterUsePlayer Scptr))
			{
				Scptr.Active = true;
			}
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.04f;
            player.GetArmorPenetration(DamageClass.Generic) += 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EbonstoneBlock, 12)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class CorruptPendantScepterUsePlayer : ModPlayer
	{
		public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }
		public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Active)
            {
                if (item.DamageType == ModContent.GetInstance<ScepterClass>() && Player.altFunctionUse == 2)
                {
                    if (Main.rand.NextBool(3))
                    {
                        for (int t = 0; t < 5; t++)
                        {
                            Vector2 outer = Player.Center + Main.rand.NextVector2CircularEdge(20, 20);
                            Vector2 motion = outer - position;

                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                motion,
                                ProjectileID.TinyEater,
                                damage / 2,
                                knockback,
                                Player.whoAmI
                            );
                        }
                    }
                }
            }

        }
	}
}