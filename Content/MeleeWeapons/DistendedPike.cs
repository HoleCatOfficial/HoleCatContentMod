using DestroyerTest.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class DistendedPike : ModItem
        {
            SoundStyle Jab = new SoundStyle($"DestroyerTest/Assets/Audio/HellWeaponDash", 3) {
				Volume = 1.0f, 
				Pitch = 0.0f, 
				PitchVariance = 1f, 
			}; 

            public override void SetStaticDefaults() {
                ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
                ItemID.Sets.Spears[Item.type] = true;
            }

            public override void SetDefaults() {
                Item.width = 108;
                Item.height = 106;

                Item.rare = ItemRarityID.Pink;
                Item.value = Item.sellPrice(silver: 10);

                Item.useStyle = ItemUseStyleID.Shoot; 
                Item.useAnimation = 30; 
                Item.useTime = 30;
                Item.UseSound = Jab;
                Item.autoReuse = true; 

                Item.damage = 100;
                Item.knockBack = 6.5f;
                Item.noUseGraphic = true;
                Item.DamageType = DamageClass.Melee;
                Item.noMelee = true;

                Item.shootSpeed = 3.7f;
                Item.shoot = ModContent.ProjectileType<DistendedPikeProjectile>(); 
            }

            public override bool? UseItem(Player player)
            {
                SoundEngine.PlaySound(Jab, player.Center);
                return base.UseItem(player);
            }
    }
        
}

		