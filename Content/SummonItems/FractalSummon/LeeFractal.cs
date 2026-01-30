using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.SummonItems.FractalSummon
{
    public class LeeFractal : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 76;
            Item.height = 86;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.buyPrice(gold: 1);
            Item.mana = 100;
            Item.knockBack = 5f;
            Item.damage = 200;
            Item.DamageType = DamageClass.Summon;
            Item.shoot = ModContent.ProjectileType<AetherAngel>();
            Item.shootSpeed = 1f;
            Item.UseSound = DTAssetLib.Impacts.AmbitionChargeBurst;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Opus.DrawItemShadowsRotating(Item, 8, Main.DiscoColor, 0.5f);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Opus.DrawItemShadowsRotating(Item, 8, Main.DiscoColor, 0.5f);
            return true;
        }

        public static List<int> Swords = new List<int>
        {
            ModContent.ProjectileType<AetherAngel>(),
            ModContent.ProjectileType<BrilliantStar>(),
            ModContent.ProjectileType<Carnage>(),
            ModContent.ProjectileType<EternalAbyss>(),
            ModContent.ProjectileType<OmegasEdge>(),
            ModContent.ProjectileType<SunsetSavior>(),
            ModContent.ProjectileType<AetherAngel>(),
        };

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            velocity = Vector2.Zero;
            type = Swords[Main.rand.Next(Swords.Count)];
            damage = Item.damage;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(ModContent.BuffType<LeeFractalBuff>(), 60);
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, 200, knockback, Main.myPlayer);
			projectile.originalDamage = 200;
			return false;
		}
    }
}