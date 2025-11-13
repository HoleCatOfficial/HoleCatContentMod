using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;

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
            Item.width = 70;
            Item.height = 92;
            Item.value = Item.sellPrice(gold: 25, silver: 70);
            Item.rare = ItemRarityID.Master;

            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.knockBack = 10;
            Item.autoReuse = false;
            Item.damage = 460;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 300;
            Item.crit = 16;
            Item.noMelee = false;
            Item.noUseGraphic = false;
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/Corpse/CorpseRoar2") with { PitchVariance = 1.0f, Volume = 4 };

            Item.shoot = ModContent.ProjectileType<KeeperSoulProj>();
        }
        


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback);
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (!DestroyerTestMod.EternityIsActive())
            {
                return false;
            }
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