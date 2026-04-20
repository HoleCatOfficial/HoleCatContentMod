using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.MeleeWeapons
{
	public class Constitution : ModItem
	{

		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 50;

			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 80;
			Item.knockBack = 4f;
			Item.crit = 6;

			Item.value = Item.buyPrice(gold: 16);
			Item.rare = ModContent.RarityType<StellarRarity>();
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ConstitutionSwing>();
			Item.channel = true;


		}

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.altFunctionUse == 2)
			{
                damage += 100;
            }
			
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
			{
				Vector2 T = Main.MouseWorld - player.Center;
				T.Normalize();
				velocity = T * (20 + player.GetTotalAttackSpeed(DamageClass.Melee));
				type = ModContent.ProjectileType<ConstitutionThrow>();
				
			}
			else
			{
				velocity = default;
				type = Item.shoot;
				damage = 80;
			}
        }

		
        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

        public override bool CanUseItem(Player player)
        {
			if (player.altFunctionUse == 2)
			{
				Item.UseSound = SoundID.Item82;
			}
			else
			{
				Item.UseSound = null;
			}
            return player.ownedProjectileCounts[Item.shoot] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<ConstitutionThrow>()] < 1;
        }

        public override bool MeleePrefix() {
			return true;
		}
	}
}
