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
        public override string Texture => "DestroyerTest/Content/MeleeWeapons/Gargantua";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(2, 49)); 
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.TerraBlade);
            Item.UseSound = SoundID.Item101;
			Item.width = 114; // The item texture's width.
			Item.height = 114; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
			Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
			Item.damage = 90; // The damage your item deals.
			Item.knockBack = 6; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 6; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

			Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<GoliathRarity>(); // Give this item our custom rarity.
			Item.shoot = ModContent.ProjectileType<GargantuaBeam>();
		}

        

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Spawn the custom Terra Beam projectile
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<GargantuaBeam>(), damage, knockback, player.whoAmI);
            return false; // Return false to prevent the default projectile from being spawned
        }


		public override void MeleeEffects(Player player, Rectangle hitbox) {
			if (Main.rand.NextBool(3)) {
				// Emit dusts when the sword is swung
				Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Blood);
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			// Inflict the OnFire debuff for 1 second onto any NPC/Monster that this hits.
			// 60 frames = 1 second
			target.AddBuff(BuffID.Bleeding, 120);
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
                .AddIngredient<Goliath>(1)
                .AddIngredient<LivingDiamond>(1)
                .AddIngredient<Item_TemperedObsidian>(10)
				.AddTile(TileID.Anvils)
				.Register();
		}
    }
}