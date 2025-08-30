
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
    public class StarScroll : ModItem
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
            if (player.TryGetModPlayer<StarScrollScepterUsePlayer>(out StarScrollScepterUsePlayer Scptr))
			{
				Scptr.Active = true;
			}
        }
    }

    public class StarScrollScepterUsePlayer : ModPlayer
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
                        Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                        float ceilingLimit = target.Y;
                        if (ceilingLimit > Player.Center.Y - 200f)
                        {
                            ceilingLimit = Player.Center.Y - 200f;
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 position2 = Player.Center - new Vector2(Main.rand.NextFloat(401) * Player.direction, 600f);
                            position2.Y -= 100 * i;
                            Vector2 heading = target - position2;

                            if (heading.Y < 0f)
                            {
                                heading.Y *= -1f;
                            }

                            if (heading.Y < 20f)
                            {
                                heading.Y = 20f;
                            }

                            heading.Normalize();
                            heading *= velocity.Length();
                            heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                            Projectile Star = Projectile.NewProjectileDirect(Player.GetSource_ItemUse(item), position2, heading, ProjectileID.Starfury, damage / 2, knockback, Player.whoAmI, 0f, ceilingLimit);
                            Star.timeLeft = 240;
                        }
                    }
                }
            }
        }
	}
}