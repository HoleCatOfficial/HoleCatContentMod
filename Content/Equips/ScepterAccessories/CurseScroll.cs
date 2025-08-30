
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
    public class CurseScroll : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.buyPrice(10);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer<CurseScrollScepterUsePlayer>(out CurseScrollScepterUsePlayer Scptr))
			{
				Scptr.Active = true;
			}
        }
    }

    public class CurseScrollScepterUsePlayer : ModPlayer
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
                        for (int t = 0; t < 3; t++)
                        {
                            Vector2 outer = Player.Center + Main.rand.NextVector2CircularEdge(10, 10);
                            Vector2 motion = outer - position;

                            Projectile.NewProjectile(
                                Player.GetSource_ItemUse(item),
                                Player.Center,
                                motion,
                                ModContent.ProjectileType<CurseProjectile>(),
                                damage / 3,
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