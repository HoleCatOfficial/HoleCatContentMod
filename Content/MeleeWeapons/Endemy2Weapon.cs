using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
	public class Endemy2Weapon : ModItem
	{
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/MeleeWeapons/Endemy2Weapon_Highlight").Value;
			spriteBatch.Draw(glowTexture, position, frame, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
		}
		
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/MeleeWeapons/Endemy2Weapon_Highlight").Value;
			spriteBatch.Draw(glowTexture, Item.position - Main.screenPosition, null, Color.White, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}

        public override void SetStaticDefaults()
        {
            DTUtils.isSpecialSwingSword.Add(Type);
            DTUtils.TooltipScaleMult[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.width = 140;
            Item.height = 142;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetSpecialMeleeStats();
            Item.autoReuse = true;
            Item.channel = true;

            Item.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Item.damage = 600;
            Item.knockBack = 20f;
            Item.crit = 66;

            Item.rare = ModContent.RarityType<EndemyRarity>(); 
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<Endemy2Swing>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

		public override bool MeleePrefix() {
			return true; 
		}
	}
}
