using DestroyerTest;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;

namespace DestroyerTest.Content.MeleeWeapons
{
    public class GargantuaBeam : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/MeleeWeapons/GargantuaBeam";
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 8; // Set the number of frames in the sprite sheet
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.TerraBeam);
            Projectile.width = 80;
            Projectile.height = 136;
            Projectile.aiStyle = 27; // Terra Beam AI style
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            AnimateProjectile();

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.TwoPi;
        }
        public void AnimateProjectile() {
                // Loop through the frames, assuming each frame lasts 5 ticks
                if (++Projectile.frameCounter >= 3) {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                        Projectile.frame = 0;
                    }
                }
        }
    }

    public class Gargantua : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item101;
            Item.width = 122;
            Item.height = 122;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 100;
            Item.knockBack = 6;
            Item.crit = 10;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.shoot = ModContent.ProjectileType<GargantuaProjectile>();
            Item.noUseGraphic = true;
            Item.channel = true;
		}

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<GargantuaProjectile2>();
                damage = 380;
            }
            if (player.altFunctionUse == 1)
            {
                type = ModContent.ProjectileType<GargantuaProjectile>();
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.channel = true;
            }
            if (player.altFunctionUse == 1)
            {
                Item.channel = true;
            }
            return player.ownedProjectileCounts[ModContent.ProjectileType<GargantuaProjectile>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<GargantuaProjectile2>()] < 1;
        }



		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<Goliath>(1)
                .AddIngredient<LivingDiamond>(14)
                .AddIngredient(ItemID.SpectreBar, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
}