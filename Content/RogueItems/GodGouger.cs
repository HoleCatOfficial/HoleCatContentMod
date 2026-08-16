using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.RogueItems
{
    public class GodGouger : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<GodGougerThrown>();
            Item.width = 18;
            Item.height = 102;
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/SwiftSwing1") with { PitchVariance = 0.2f };
            Item.useAnimation = 15;
            Item.useTime = 30;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ModContent.RarityType<CorruptionSpecialRarity>();
            Item.damage = 16;
            Item.knockBack = 5;
            Item.autoReuse = false;
            Item.DamageType = DamageClass.Throwing;
        }
	}
}