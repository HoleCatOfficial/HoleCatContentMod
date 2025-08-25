using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles; // Add this line if CT3_Swing is in the Projectiles namespace
using DestroyerTest.Rarity;
using System.Linq;

namespace DestroyerTest.Content.Magic
{
	public class KeeperStaff : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, 8)); 
        }
		public override void SetDefaults()
        {
            // Common Properties
            Item.width = 70;
            Item.height = 92;
            Item.value = Item.sellPrice(gold: 25, silver: 70);
            Item.rare = ItemRarityID.Master;

            // Use Properties
            // Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
            // Each attack takes a different amount of time to execute
            // Conforming to the item useTime and useAnimation makes it much harder to design
            // It does, however, affect the item tooltip, so don't leave it out.
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;

            // Weapon Properties
            Item.knockBack = 10;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
            Item.autoReuse = false; // This determines whether the weapon has autoswing
            Item.damage = 400; // The damage of your sword, this is dynamically adjusted in the projectile code.
            Item.DamageType = DamageClass.Magic; // Deals melee damage\
            Item.mana = 300;
            Item.crit = 16; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.
            Item.noMelee = false;  // This makes sure the item does not deal damage from the swinging animation
            Item.noUseGraphic = false; // This makes sure the item does not get shown when the player swings his hand
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/CorpseRoar2") with { PitchVariance = 1.0f, Volume = 4 };

            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<KeeperSoulProj>(); // The sword as a projectile
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback);
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == Item.shoot)
                {
                    return false;
                }
            }
            return true;
        }
    }
} 